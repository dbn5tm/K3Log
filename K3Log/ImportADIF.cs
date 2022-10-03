using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K3Log
{
    public partial class ImportADIF : Form
    {
        SQLiteConnection my_db;
        QSO q;
        public List<Remotes> LoTWLocations;
        public ImportADIF(SQLiteConnection db)
        {
            my_db = db;
            q = new QSO(my_db);
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string adifFile = BrowseFiles();
            txtAdifPath.Text = adifFile;
        }
        private string BrowseFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"c:\Users\dan\Documents\Radio Logs",
                Title = "Browse ADIF Files",

                CheckFileExists = false,
                CheckPathExists = true,

                DefaultExt = "adi",
                Filter = "adi files (*.adi)|*.adi",
                FilterIndex = 2,
                RestoreDirectory = true,

                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return "";
        }
        private string readADIF(string fspec)
        {
            if (fspec != "")
            {
                StreamReader sfilereader = File.OpenText(fspec);
                string sinputline = "";
                string adif = "";
                // read the ADIF file                
                while ((sinputline = sfilereader.ReadLine()) != null)
                {
                    adif += sinputline.Replace("<eoh>", "<EOH>");
                }

                return adif;
            }
            return "";
        }
        private void AdifToLog(AdifRow r, int adi_index, bool lotwUpdate)
        {
            SQLiteCommand comm;
            string date_mask = "####-##-##";
            string time_mask = "##:##:##Z";
            bool foundmatch = false;

            if (my_db.State == ConnectionState.Closed) my_db.Open();
            // fisrt use mask to normalize date and time to ##-##-#### ##:##:##Z  
            q.start = r.QSO_DATE.WithMask(date_mask) + " " + r.TIME_ON.WithMask(time_mask);
            if (r.QSO_DATE_OFF == null)
            {
                q.date = q.start;
            }
            else
            {
                q.date = r.QSO_DATE_OFF.WithMask(date_mask) + " " + r.TIME_OFF.WithMask(time_mask);
            }
            q.callsign = r.CALL;
            q.ownercallsign = Properties.Settings.Default.MyCall;
            q.band = r.BAND;
            q.bandrx = r.BAND_RX;
            q.country = r.COUNTRY;
            q.mode = r.MODE;
            q.propmode = r.PROP_MODE;
            q.satmode = r.SAT_MODE;
            q.satname = r.SAT_NAME;
            if (r.SAT_NAME != "") q.satelliteqso = "True";
            q.name = r.NAME;
            q.rcvd = r.RST_RCVD;
            q.sent = r.RST_SENT;
            q.freq = r.FREQ;
            q.freqrx = r.FREQ_RX;
            q.comment = r.COMMENT;
            q.power = r.TX_PWR;
            q.qth = r.QTH;
            q.grid = r.GRIDSQUARE;

            q.theoperator = Properties.Settings.Default.MyCall;

            q.op = r.OPERATOR;
            q.mycnty = r.MY_COUNTRY;
            q.mycqzone = Convert.ToInt32(r.MY_CQ_ZONE);
            q.mydxcc = Convert.ToInt32(r.MY_DXCC);
            q.myituzone = Convert.ToInt32(r.MY_ITU_ZONE);
            q.mystate = r.MY_STATE;
            q.mysiginfo = r.MY_SIG_INFO;
            q.dxcc = r.DXCC;

            // does qso exist?
            comm = new SQLiteCommand("Select * From Log WHERE '" + q.callsign + "' = callsign ORDER BY datetime(qsodate, 'unixepoch') DESC", my_db);
            using (SQLiteDataReader read = comm.ExecuteReader())
            {
                while (read.Read())
                {
                    string d = read.GetString(read.GetOrdinal("qsodate")).ToString();
                    string ed = "";
                    string lotwLoc = "";
                    try
                    {
                        ed = read.GetString(read.GetOrdinal("qsodate")).ToString();
                        lotwLoc = read.GetValue(read.GetOrdinal("mysiginfo")).ToString();
                        if (lotwLoc == "0" || lotwLoc == "")  // test for lagacy when operator was used for Remote definitions
                        {
                            lotwLoc = read.GetValue(read.GetOrdinal("operator")).ToString();
                        }
                        var c = read.GetValue(read.GetOrdinal("callsign"));
                        var m = read.GetValue(read.GetOrdinal("mode"));
                        var b = read.GetValue(read.GetOrdinal("band"));
                        var qd = read.GetValue(read.GetOrdinal("qsoid")).ToString();
                        if (ed == q.date)
                        {
                            foundmatch = true;
                            q.qsoid = qd;
                            break;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.Write(e.ToString());
                        ed = d;
                        //throw;
                    }
                }
                if (!foundmatch)
                {
                    if (!lotwUpdate)
                    {
                        q.qsoid = "import " + adi_index.ToString();
                        q.logQso();
                        bool remoteExists = false;
                        foreach (Remotes lotw in LoTWLocations)
                        {
                            if (lotw.Station == q.mysiginfo)
                            {
                                remoteExists = true;
                                break;
                            }
                        }
                        if (!remoteExists)
                        {
                            Remotes newRemote = new Remotes();
                            newRemote.Station = q.mysiginfo;
                            newRemote.State = q.mystate;
                            newRemote.County = q.mycnty;
                            newRemote.CQZone = q.mycqzone.ToString();
                            newRemote.ITUZone = q.myituzone.ToString();
                            newRemote.Grid = q.mygridsquare;
                            LoTWLocations.Add(newRemote);
                            // add this new LoTW location to Remotes List
                            string serializedList = Newtonsoft.Json.JsonConvert.SerializeObject(LoTWLocations);
                            Properties.Settings.Default.LoTW = serializedList;
                            Properties.Settings.Default.Save();
                            //cboLoTWStation.Items.Clear();
                            // now update the dropdown
                            foreach (Remotes rmt in LoTWLocations)
                            {
                                //cboLoTWStation.Items.Add(rmt.Station);
                            }
                        }
                    }
                    else
                    {
                        Debug.Print("QSO Not Found");
                    }
                }
                else
                {
                    // found QSO in Log  Check for LoTW R
                    if (lotwUpdate && rdoLoTW.Checked)
                    {
                        if (r.QSL_RCVD == "Y")
                        {
                            //q.qsoid = "import " + adi_index.ToString();
                            q.qsoconfirmations = "[{\"CT\":\"LOTW\",\"S\":\"Yes\",\"R\":\"Yes\",\"SV\":\"Electronic\",\"RV\":\"Electronic\",\"SD\":\"" + r.QSLRDATE + "\"}]";
                            //q.qsoconfirmations = "Yes";
                            txtImport.Text += q.callsign + " " + q.date + q.lotw_rcvd + "\r\n";
                            q.EditQso(q.qsoid);
                        }
                    }

                }

            }


           
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            txtImport.Text = "";
            string adifFile = txtAdifPath.Text;
            String adifIn = readADIF(adifFile);
            bool LotwUpdate = false;
            if (adifIn.Contains("ARRL Logbook of the World Status Report")) LotwUpdate = true;

            AdifReader adifRead = new AdifReader(adifIn);
            // get all ADIF Rows from the read file
            List<AdifRow> rows = adifRead.GetAdifRows();
            int i = 0;
            foreach (AdifRow r in rows)
            {
                if (LotwUpdate)
                {
                    AdifToLog(r, i, true);
                    i++;
                }
                else
                {
                    AdifToLog(r, i, false);
                    i++;
                }

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
