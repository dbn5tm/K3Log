using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace K3Log
{
    public partial class Cluster : Form
    {
        public string spotcall
        {
            get { return DXTextBox.Text; }
            set { DXTextBox.Text = value; }
        }
        public string spotfreq
        {
            get { return FreqTextBox.Text; }
            set { FreqTextBox.Text = value; }
        }
        public K3Log.QRZ QRZLogbook;
        private System.Timers.Timer cmdtimer = new System.Timers.Timer(600000);

        public SQLiteConnection my_db;
        public static string logbookConn;
        SQLiteDB logbookDB;  // = new SQLiteDB(logbookConn);
        public String callsign;
        TelnetThread DXCluster = new TelnetThread();

        //SQLiteDB logbookDB = new SQLiteDB("Data Source=" +
        //"C:\\Users\\Dan\\Documents\\N5TM_DB\\N5TM_Full.SQLite;Version=3;");
        //SQLiteDB logbookDB = new SQLiteDB("Data Source=" +
        //"E:\\Documents\\N5TM_DB\\N5TM_Full.SQLite;Version=3;");
        countryXML countrylist = new countryXML(Properties.Settings.Default.CountryList);
        BandplanXML bandlist = new BandplanXML(Properties.Settings.Default.BandPlan);

        private bool[] filter = { false, false, false, false, false, false, false };
        private string[] filterstr = { "", "", "", "", "", "", "", "" };

        public class SpotClickedEventArgs : EventArgs
        {
            public Double FA { get; set; }
            public Double FB { get; set; }
            public Int16 MD { get; set; }
            public Int16 MDsub { get; set; }
            public String ctry { get; set; }
            public String dx { get; set; }
        }
        SpotClickedEventArgs args = new SpotClickedEventArgs();
        public event EventHandler<SpotClickedEventArgs> SpotClicked;
        protected virtual void OnSpotClicked(object sender, SpotClickedEventArgs e)
        {
            SpotClicked?.Invoke(this, e);
        }

        public Cluster(string logbookConn)
        {
            InitializeComponent();
            logbookDB = new SQLiteDB(logbookConn);
        }

        private void Cluster_Load(object sender, EventArgs e)
        {

            DXCluster.IPDataRcvd += new EventHandler<TelnetThread.RcvdDataEventArgs>(NewTelnetData);
            cmdtimer.Elapsed += new System.Timers.ElapsedEventHandler(timerTick);
            cmdtimer.Enabled = true;
            initDXGrid();
            filterstr[0] = "reject/spots 1 not by_itu 1,2,3,4,6,7,8,9,10,11";  // NA spots only
            filterstr[1] = "accept/spots 1 call_itu 1,2,3,4,6,7,8,9,10,11";  // NA DX only
            filterstr[2] = "reject/spots 2 not on hf/cw "; // cw only no data
            filterstr[3] = "reject/spots 3 not on hf/cw or not on hf/data or not on hf/rtty";  // cw and data
            filterstr[4] = "reject/spots 4 not on hf/data or not on hf/rtty";// data only
            filterstr[5] = "reject/spots 5 not on hf/phone";// data only
            //logbookDB.FillDataSet("Country", ctryDSDA, "", False)
            //logbookDB.FillDataSet("QSO", qsoDSDA, "id", True)
            //sSettings = New XMLcls(appPath + "\settings.xml", "settings")
            //initGridColumns()
        }
        private void timerTick(object sender, EventArgs e)
        {
            // keep alive every 10min
            if (DXCluster.connected)
            {
                DXCluster.sendCommand("");
            }
        }

        private List<String[]> foundInLog(String lookup)
        {
            List<String[]> logRet = new List<String[]>();

            SQLiteCommand comm;
            if (my_db.State == ConnectionState.Closed) my_db.Open();
            if (lookup == "")
            {
                comm = new SQLiteCommand("Select * From Log", my_db);
            }
            else
            {
                comm = new SQLiteCommand("Select * From Log WHERE LIKE('" + lookup + "%',callsign)=1", my_db);
            }

            using (SQLiteDataReader read = comm.ExecuteReader())
            {

                while (read.Read())
                {
                    String[] logRow = { "", "", "", "" };
                    logRow[0] = read.GetValue(read.GetOrdinal("callsign")).ToString();
                    logRow[1] = read.GetValue(read.GetOrdinal("qsodate")).ToString();
                    logRow[2] = read.GetValue(read.GetOrdinal("band")).ToString();
                    logRow[3] = read.GetValue(read.GetOrdinal("mode")).ToString();
                    logRet.Add(logRow);
                }
            }
            return logRet;
        }

        private void AddToGrid(String text)
        {
            Regex utc = new Regex("[0-9][0-9][0-9][0-9]Z");
            Regex f = new Regex(@"\d{2,10}\.\d");
            Regex de = new Regex("DX de ");
            Regex DXde = new Regex(@"DX de \S{3,15}:");
            Regex dxCall = new Regex(@"\S{3,15}");

            String Spotter = "";
            String DX = "";
            String Freq = "";
            String spotTime = "";
            String Comment = "";
            Int32 wrkd = 0;

            try
            {
                if (DXde.Matches(text).Count > 0)
                {


                    String[] spots = de.Split(text);

                    foreach (String s in spots)
                    {
                        if (s.Length > 0)
                        {

                            if (utc.IsMatch(s))
                            {
                                String[] newspot = { "DX", "country", "freq", "comment", "time", "spotter" };
                                spotTime = utc.Match(s).ToString();
                                Spotter = s.Split(':')[0];
                                Freq = f.Match(s).ToString();
                                DX = dxCall.Match(f.Split(s)[1].ToString()).ToString();

                                Regex c = new Regex(DX);
                                Comment = c.Split(utc.Split(s)[0])[1];
                                newspot[0] = DX;
                                newspot[1] = "?";
                                newspot[2] = Freq;
                                newspot[3] = Comment;
                                newspot[4] = spotTime;
                                newspot[5] = Spotter;

                                String dxPre = "";

                                String[] cols = { "Entity", "EntityCode" };
                                //'try match on full callsign
                                String ctry = countrylist.CountryName(DX.Split('/')[0])[0];

                                //todo below looking up this station in log database  If worked, check band mode and skip the rest below
                                List<String[]> stationworked = foundInLog((DX.Split('/')[0]));
                                //logbookDB.Retrieve("Log", "Prefix", DX.Split('/')[0], "=", cols);
                                if (QRZLogbook.Lookup(DX))
                                {
                                    ctry = QRZLogbook.country;
                                    wrkd = checkworked(ctry, Freq);
                                }

                                if (ctry != "")
                                {
                                    wrkd = checkworked(ctry, Freq);
                                    newspot[1] = ctry; //[0].ToString();  // { DX, ctry.Item[0].item[0], Freq, Comment, spotTime, Spotter};

                                }
                                else
                                {
                                    
                                    //'try exact match on first four letters
                                    if(DX.Length > 3)
                                    {
                                        dxPre = DX.Substring(0, 4);
                                        ctry = countrylist.CountryName(dxPre)[0];
                                    }
                                    
                                    //ctry = logbookDB.sqliteFind(dxPre); //logbookDB.Retrieve("Country", "Prefix", dxPre, "=", cols);
                                    
                                    if (ctry != "")
                                    {
                                        wrkd = checkworked(ctry, Freq);
                                        //newspot[1] = ctry[0].ToString();  // newSpotRow( DX, ctry[0], Freq, Comment, spotTime, Spotter);
                                    }
                                    else
                                    {

                                        //' try exact match on first three letters
                                        dxPre = DX.Substring(0, 3);
                                        //ctry = logbookDB.sqliteFind(dxPre); //logbookDB.Retrieve("Country", "Prefix", dxPre, "=", cols);
                                        ctry = countrylist.CountryName(dxPre)[0];
                                        if (ctry != "")
                                        {
                                            wrkd = checkworked(ctry, Freq);
                                            //newspot[1] = ctry[0].ToString();   //newspot = { DX, ctry.Item(0).item(0), Freq, Comment, spotTime, Spotter};
                                        }
                                        else
                                        {
                                            //' try exact match on first two letters
                                            dxPre = DX.Substring(0, 2);
                                            //ctry = logbookDB.sqliteFind(dxPre); //.Retrieve("Country", "Prefix", dxPre, "=", cols);
                                            ctry = countrylist.CountryName(dxPre)[0];
                                            if (ctry != "")
                                            {
                                                wrkd = checkworked(ctry, Freq);
                                                //newspot[1] = ctry[0].ToString();   //newspot = { DX, ctry.Item[0].item[0], Freq, Comment, spotTime, Spotter};
                                            }
                                            else
                                            {
                                                //' try exact match first letter only
                                                dxPre = DX.Substring(0, 1);
                                                //ctry = logbookDB.sqliteFind(dxPre); //.Retrieve("Country", "Prefix", dxPre, "=", cols);
                                                ctry = countrylist.CountryName(dxPre)[0];
                                                if (ctry != "")
                                                {
                                                    wrkd = checkworked(ctry, Freq);
                                                    //newspot[1] = ctry[0].ToString();   //newspot = { DX, ctry.Item(0).item(0), Freq, Comment, spotTime, Spotter};
                                                }
                                                else
                                                {
                                                    //newspot[1] = ctry[0].ToString();   //newspot = { DX, "???", Freq, Comment, spotTime, Spotter };
                                                }
                                            }
                                        }
                                    }
                                }
                                newspot[1] = ctry;
                                newSpotRow(newspot, wrkd);

                            }
                        }
                    }
                }
                else  // maybe from SH/DX command
                {


                    foreach (Match frq in Regex.Matches(text, @"\d{2,10}\.\d "))
                    {
                        Freq = frq.Value;
                        foreach (Match dx in Regex.Matches(text, @"\S{" + frq.Length + ",15}"))
                        {
                            DX = dx.Value;
                        }
                    }

                }
            }
            catch (Exception e)
            {

                String err = e.ToString();
            }

        }

        // returns (0 = worked band and mode), (1 = need band), (2 = need mode), (3 = need mode and band)
        private Int32 checkworked(String DXCC, String freq)
        {
            Int32 ret = 4;
            String[] wrkdcols = { "band", "mode", "qsoconfirmations" };
            List<String> wrkd = logbookDB.Retrieve("Log", "country", DXCC, "=", wrkdcols);
            if (wrkd.Count != 0) // if no ATNO then check for worked mode on bands.
            {

                ret = 3;  // assume need on both band and mode
                String[] list = bandlist.Band(Convert.ToDouble(freq));
                // do we need it on this band?
                foreach (String w in wrkd)
                {
                    String b = w.Split(',')[0];
                    if (b == list[0])
                    {
                        ret = ret & 0x2;  // found band so exit for
                        break;
                    }
                }

                // do we need it on this mode?

                String mode = list[1];

                if (mode == "USB") mode = "SSB";
                if (mode == "LSB") mode = "SSB";
                if (mode == "PHONE") mode = "SSB";


                foreach (String w in wrkd)
                {
                    String m = w.Split(',')[1];
                    if (m == list[1])
                    {
                        ret = ret & 0x01;  // found mode so exit for
                        break;
                    }

                }
            }


            return ret;
        }


        private void initDXGrid()
        {
            this.DXDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            this.DXDataGridView.ColumnCount = 6;
            this.DXDataGridView.Columns[0].Name = "DX Spot";
            this.DXDataGridView.Columns[1].Name = "Country";
            this.DXDataGridView.Columns[2].Name = "Freq";
            this.DXDataGridView.Columns[3].Name = "Comment";
            this.DXDataGridView.Columns[4].Name = "Time";
            this.DXDataGridView.Columns[5].Name = "Spotter";

        }
        private void NewTelnetData(object sender, TelnetThread.RcvdDataEventArgs e)
        {
            SetText(this.txtRecv, e.rcvdMsg);
            AddToGrid(e.rcvdMsg);
        }

        //private delegate void newSpotRowCallback(String[] newrow, Int32 wrkd);
        private void newSpotRow(String[] newrow, Int32 wrkd)
        {
            this.InvokeAndClose((MethodInvoker)delegate
            {
                /*if (DXDataGridView.InvokeRequired)
                {
                    //newSpotRowCallback d = new newSpotRowCallback(newSpotRow);
                    try
                    {
                        this.BeginInvoke(d, new object[] { newrow, wrkd });
                    }
                    catch (Exception e)
                    {

                        String err = e.ToString();  //throw;
                    }
                }
                else
                {*/
                    DXDataGridView.Rows.Insert(0, newrow);
                    int r = DXDataGridView.NewRowIndex;
                    r = 1;
                    switch (wrkd)
                    {
                        case 0:  // worked band and mode
                            DXDataGridView.Rows[r - 1].DefaultCellStyle.BackColor = Color.Cyan;
                            break;
                        case 1:  // need band
                            DXDataGridView.Rows[r - 1].DefaultCellStyle.BackColor = Color.LimeGreen;
                            break;
                        case 2:  // need mode
                            DXDataGridView.Rows[r - 1].DefaultCellStyle.BackColor = Color.LightYellow;
                            break;
                        case 4:  // ATNO
                            DXDataGridView.Rows[r - 1].DefaultCellStyle.BackColor = Color.Red;
                            break;

                    }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                //}
            });
            
        }

        //private delegate void SetTextCallBackDelegate(Control c, String text);
        private void SetText(Control c, String text)
        {
            this.InvokeAndClose((MethodInvoker)delegate
            {
                /*if (c.InvokeRequired)
                {
                    SetTextCallBackDelegate d = new SetTextCallBackDelegate(SetText);
                    try
                    {
                        this.BeginInvoke(d, new object[] { c, text });
                    }
                    catch
                    {
                        //Console. ("disposed Object");
                    }
                }
                else
                {*/
                    c.Text += text;

                //}
            });
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {

            if (TelNetURLComboBox.Text != "")
            {

                String[] thisCluster = TelNetURLComboBox.Text.Split(':');
                DXCluster.url = thisCluster[0];

                if (thisCluster.Length > 1)
                {

                    if (thisCluster[1] == "")
                    {
                        DXCluster.port = "7300";
                    }
                    else
                    {
                        DXCluster.port = thisCluster[1];
                    }
                }
                else
                {
                    DXCluster.port = "7300";
                }

                DXCluster.callsign = callsign;


                DXCluster.Logon();
                //ThreadStart threadDelegate = new ThreadStart(DXCluster.TelnetDataRcvd);
                //Thread dxc = new Thread(threadDelegate);
                //dxc.Start();
            }
            else
            {
                MessageBox.Show("Select or enter URL for DX Cluster");
            }

        }

        private void SHDXButton_Click(object sender, EventArgs e)
        {
            DXCluster.sendCommand("show/dx 20");
        }

        private void btntest_Click(object sender, EventArgs e)
        {
            String test = "DX de N5TM:      14018.0  3D2CR test post                   0244Z";
            AddToGrid(test);
        }

        private void DXDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            args.FA = Convert.ToDouble(DXDataGridView.CurrentRow.Cells["Freq"].Value);
            args.dx = DXDataGridView.CurrentRow.Cells["DX Spot"].Value.ToString();
            args.ctry = DXDataGridView.CurrentRow.Cells["Country"].Value.ToString();
            String[] frqmode = bandlist.Band(args.FA);
            String mode = frqmode[1];
            if (mode == "CW") args.MD = 3;
            if (mode == "PHONE")
            {
                String spotBand = "";
                if (frqmode[0].Contains('m')) spotBand = frqmode[0].Replace("m", "");
                if (frqmode[0].Contains("c")) spotBand = frqmode[0].Replace("c", "");
                if (frqmode[0].Contains("cm")) spotBand = frqmode[0].Replace("cm", "");

                Int16 band = Convert.ToInt16(spotBand);
                if (band > 30)
                {
                    args.MD = 1;
                }
                else
                {
                    args.MD = 2;
                }
            }
            SpotClicked(this, args);
            if (mode == "DIGITAL") args.MD = 6;
        }


        private void Cluster_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {

            List<string> checkedItems = new List<string>();
            foreach (var item in checkedListBox1.CheckedItems)
                checkedItems.Add(item.ToString());

            if (e.NewValue == CheckState.Checked)
            {
                filter[e.Index] = true;
                checkedItems.Add(checkedListBox1.Items[e.Index].ToString());
            }
            else
            {
                filter[e.Index] = false;
                checkedItems.Remove(checkedListBox1.Items[e.Index].ToString());
            }
            for (int x = 0; x < 6; x++)
            {
                if (filter[x]) DXCluster.sendCommand(filterstr[x]);
                else DXCluster.sendCommand("clear/spots " + (x + 1).ToString());

            }

            foreach (string item in checkedItems)
            {

            }
        }

        private void btnShowFilters_Click(object sender, EventArgs e)
        {
            DXCluster.sendCommand("show/filter");
        }

        private void btnclr_Click(object sender, EventArgs e)
        {
            DXCluster.sendCommand("clear/spots all");
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            if (DXTextBox.Text.Length > 2 && IsNumber(FreqTextBox.Text))
            {
                DXCluster.sendCommand("DX " + FreqTextBox.Text + " " + DXTextBox.Text + " " + CommentTextBox.Text);
            }
        }
        public Boolean IsNumber(String value)
        {
            return value.Replace(".", "").All(Char.IsDigit);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DXCluster = null;
            this.Close();
        }
    }

}
