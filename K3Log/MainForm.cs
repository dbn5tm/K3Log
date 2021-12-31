using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Reflection;



namespace K3Log
{

    public partial class MainForm : Form
    {
        string callsign = "x1xx";
        string operatorName = "xxxx";
        string grid = "xx01xx";
        List<String> myWorkedGrids;
        List<String> myWorkedCalls;
        List<String> spottedCalls = new List<string>();
        int autoCondxIndex;
        Boolean contestMode = false;
        Boolean wsjtMode = false;
        string searchstring = "";
        string rig; //= Properties.Settings.Default.Rig;
        string rig2; // = Properties.Settings.Default.Rig2;
        private string outBuffer = "";
        private int lastTextOut;
        private List<K3Log.RadioCAT.TXList> words = new List<K3Log.RadioCAT.TXList>();
        private RadioCAT K3;
        private RadioCAT K3_Remote;
        private K3Log.QRZ QRZLogbook = new K3Log.QRZ("n5tm", "dange123");
        public K3Log.Cluster DXCluster;
        //private short ActiveRadioMD;
        private string[] comport = { "com2", "com4", "com9", "com11" };
        string[] mode = { "", "LSB", "USB", "CW", "FM", "AM", "DATA", "CW-R", "CW-R", "DATA-R" };
        private short radioport = 0;
        private string myDB = "";
        TextBox thisRadioBox;
        TextBox thisRadioMode;
        Thread backgroundK3;
        // This is the real dB, be careful
        String sqliteConn; //= "Data Source=" +
                           //"E:\\Documents\\N5TM_DB\\N5TM_Full_NextGen.SQLite;Version=3;";
                           //String sqliteConn = "Data Source=" +
                           //"E:\\Documents\\N5TM_DB\\N5TM_VHF_Jun_2020.SQLite;Version=3;";
        SQLiteConnection my_db; // = new SQLiteConnection("Data Source=" +
                                //"E:\\Documents\\N5TM_DB\\N5TM_Full_NextGen.SQLite;Version=3;");
                                //SQLiteConnection my_db = new SQLiteConnection("Data Source=" +
                                //"E:\\Documents\\N5TM_DB\\N5TM_VHF_Jun_2020.SQLite;Version=3;");
        SQLiteDB logbook;
        GridForm DecodedGrids;
        //XDocument xmlDoc;
        List<Button> MacroButtons = new List<Button>();
        // Use this one for testing.  It is a backup
        //SQLiteConnection my_db = new SQLiteConnection("Data Source=" +
        //"C:\\Users\\Dan\\Documents\\N5TM_DB\\N5TM_Full.SQLite;Version=3;");
        List<String> colNames = new List<string>();  // use this to rebuild the Insert sql
        List<String> colValue = new List<string>();
        List<K3Log.macro> macroList;
        countryXML countrylist;  // = new countryXML("E:\\Documents\\N5TM_DB\\country.xml");
        BandplanXML bandlist;  // = new BandplanXML("E:\\Documents\\N5TM_DB\\bandplan_r2.xml");

        //StationCollection LoTWLocations = new StationCollection();
        List<Remotes> LoTWLocations = new List<Remotes>();
        Remotes selectedLoTW = new Remotes();

        SatelliteCollection mySats = new SatelliteCollection();
        Satellites selectedSatellite = new Satellites();
        double linkFactor = 0;
        double oldFA = 0;
        double deltaHz = 0;

        QSO q;
        String newQsoId = "";
        //Boolean editing = false;
        Boolean BlockX = false;
        Boolean calling = false;
        String[] nocallGrids = { "XX00", "XX00" };
        Winkey keyer;
        UDPSocket WsjtX;
        OmniClient MyOmniRadio1;  // = new OmniClient();
        OmniClient MyOmniRadio2;
        SatTrack satTrack;
        
        short TXRadio = 1;
        short[] RadioBand = { 0, 0 };
        short[] RadioMode = { 0, 0 };
        double[] RadioFreq = { 0, 0 };
        private delegate void newFillView(DataGridView dgv, String lookup);

        private void getLotWLocations()
        {

            string serializeList = Properties.Settings.Default.LoTW;
            List<Remotes> myRemotes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Remotes>>(serializeList);
            if(myRemotes != null)
            {
                foreach (Remotes lotw in myRemotes)
                {
                    Remotes newStation = new Remotes();
                    newStation.Station = lotw.Station;
                    newStation.Grid = lotw.Grid;
                    newStation.ITUZone = lotw.ITUZone;
                    newStation.CQZone = lotw.CQZone;
                    newStation.County = lotw.County;
                    newStation.State = lotw.State;
                    LoTWLocations.Add(lotw);
                    cboLoTWStation.Items.Add(lotw.Station);
                }
            }                
            
            
        }

        private void loadSatellites()
        {
            if (Properties.Settings.Default.Satellites != null)
            {
                
                foreach (Satellites sat in Properties.Settings.Default.Satellites.mySatellites)
                {
                    Satellites newSatellite = new Satellites();
                    newSatellite.Satellite = sat.Satellite;
                    newSatellite.Uplink = sat.Uplink;
                    newSatellite.Mode = sat.Mode;
                    newSatellite.Downlink = sat.Downlink;
                    newSatellite.UpMode = sat.UpMode;
                    newSatellite.DownMode = sat.DownMode;
                    newSatellite.PLTone = sat.PLTone;
                    newSatellite.UpDoppler = sat.UpDoppler;
                    newSatellite.DownDoppler = sat.DownDoppler;
                    newSatellite.Invert = sat.Invert;
                    
                    mySats.mySatellites.Add(newSatellite);
                    cbosatname.Items.Add(newSatellite.Satellite);
                                        
                }
            }
        }

        private void FillView(DataGridView dgv, String lookup)
        {
            if (myDB != "No Database")
            {
                this.InvokeAndClose((MethodInvoker)delegate
                {
                    dgv.Rows.Clear();
                    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;

                    SQLiteCommand comm;
                    if (my_db.State == ConnectionState.Closed) my_db.Open();
                    if (lookup == "")
                    {
                    //comm = new SQLiteCommand("Select * From Log ORDER BY datetime(QsoDate, 'unixepoch') DESC", my_db);
                    comm = new SQLiteCommand("Select * From Log ORDER BY qsoid DESC LIMIT 1000", my_db);
                    }
                    else
                    {
                        comm = new SQLiteCommand("Select * From Log WHERE LIKE('" + lookup + "%',callsign)=1 ORDER BY datetime(qsodate, 'unixepoch') DESC", my_db);
                    }

                    using (SQLiteDataReader read = comm.ExecuteReader())
                    {


                        while (read.Read())
                        {
                            string d = read.GetString(read.GetOrdinal("qsodate")).ToString();
                            string ed = "";
                            string lotwLoc = "";
                            try
                            {
                                ed = read.GetString(read.GetOrdinal("qsoenddate")).ToString();
                                lotwLoc = read.GetValue(read.GetOrdinal("mysiginfo")).ToString();
                                if (lotwLoc == "0" || lotwLoc == "")  // test for lagacy when operator was used for Remote definitions
                                {
                                    lotwLoc = read.GetValue(read.GetOrdinal("operator")).ToString();
                                }
                                
                            }
                            catch (Exception e)
                            {
                                Console.Write(e.ToString());
                                ed = d;
                            //throw;
                            }

                            dgv.Rows.Add(new object[] {
                            d,ed,
                            //read.GetDateTime(read.GetOrdinal("qsodate")),  // U can use column index
                            //read.GetValue(read.GetOrdinal("qsoenddate")),  // Or column name like this
                            //read.GetValue(read.GetOrdinal("mysiginfo")),
                            lotwLoc,
                            read.GetValue(read.GetOrdinal("callsign")),
                            read.GetValue(read.GetOrdinal("mode")),
                            read.GetValue(read.GetOrdinal("band")),
                            read.GetValue(read.GetOrdinal("freq")),
                            read.GetValue(read.GetOrdinal("rstrcvd")),
                            read.GetValue(read.GetOrdinal("rstsent")),
                            read.GetValue(read.GetOrdinal("country")),
                            read.GetValue(read.GetOrdinal("gridsquare")),
                            read.GetValue(read.GetOrdinal("name")),
                            read.GetValue(read.GetOrdinal("comment")),
                            read.GetValue(read.GetOrdinal("propmode")),
                            read.GetValue(read.GetOrdinal("qsoId"))
                            });
                        }
                    }
                });
            }
        }

        public MainForm()
        {
            InitializeComponent();
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            
        }
                       
        void OnApplicationExit(object sender, EventArgs e)
        {
            // When the application is exiting, Close the comport and threads.

            try
            {
                // Ignore any errors that might occur while closing the file handle.
                //K3.ClosePort();
                this.InvokeAndClose((MethodInvoker)delegate
                {
                    K3.ClosePort();
                    K3 = null;
                    Environment.Exit(Environment.ExitCode);
                });
            }
            catch { }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            // create macro button events
            BtnMacro1.Click += BtnMacro0_Click;
            BtnMacro2.Click += BtnMacro0_Click;
            BtnMacro3.Click += BtnMacro0_Click;
            BtnMacro4.Click += BtnMacro0_Click;
            BtnMacro5.Click += BtnMacro0_Click;
            BtnMacro6.Click += BtnMacro0_Click;
            BtnMacro7.Click += BtnMacro0_Click;
            BtnMacro8.Click += BtnMacro0_Click;
            BtnMacro9.Click += BtnMacro0_Click;
            BtnMacro10.Click += BtnMacro0_Click;
            BtnMacro11.Click += BtnMacro0_Click;
            BtnMacro1.MouseDown += BtnMacro0_MouseDown;
            BtnMacro2.MouseDown += BtnMacro0_MouseDown;
            BtnMacro3.MouseDown += BtnMacro0_MouseDown;
            BtnMacro4.MouseDown += BtnMacro0_MouseDown;
            BtnMacro5.MouseDown += BtnMacro0_MouseDown;
            BtnMacro6.MouseDown += BtnMacro0_MouseDown;
            BtnMacro7.MouseDown += BtnMacro0_MouseDown;
            BtnMacro8.MouseDown += BtnMacro0_MouseDown;
            BtnMacro9.MouseDown += BtnMacro0_MouseDown;
            BtnMacro10.MouseDown += BtnMacro0_MouseDown;
            BtnMacro11.MouseDown += BtnMacro0_MouseDown;
            // create mousewheel events
            VFOA_Radio1.MouseWheel += new System.Windows.Forms.MouseEventHandler(VFOA_MouseWheel);
            VFOB_Radio1.MouseWheel += new System.Windows.Forms.MouseEventHandler(VFOB_MouseWheel);
            radioButtonRadio1.MouseHover += buttonMouse_Hover;
            radioButtonRadio2.MouseHover += buttonMouse_Hover;
            radioButtonRadio3.MouseHover += buttonMouse_Hover;
            radioButtonRadio4.MouseHover += buttonMouse_Hover;
            radioButtonRadio1.CheckedChanged += button_CheckChanged;
            radioButtonRadio2.CheckedChanged += button_CheckChanged;
            radioButtonRadio3.CheckedChanged += button_CheckChanged;
            radioButtonRadio4.CheckedChanged += button_CheckChanged;

            MacroButtons.Add(BtnMacro0);
            MacroButtons.Add(BtnMacro1);
            MacroButtons.Add(BtnMacro2);
            MacroButtons.Add(BtnMacro3);
            MacroButtons.Add(BtnMacro4);
            MacroButtons.Add(BtnMacro5);
            MacroButtons.Add(BtnMacro6);
            MacroButtons.Add(BtnMacro7);
            MacroButtons.Add(BtnMacro8);
            MacroButtons.Add(BtnMacro9);
            MacroButtons.Add(BtnMacro10);
            MacroButtons.Add(BtnMacro11);
            macroList = loadMacros("macros.xml");

            grid = Properties.Settings.Default.MyGrid;
            callsign = Properties.Settings.Default.MyCall;
            operatorName = Properties.Settings.Default.MyName;

            countrylist = new countryXML(Properties.Settings.Default.CountryList);
            bandlist = new BandplanXML(Properties.Settings.Default.BandPlan);
            getLotWLocations();
            loadSatellites();
            // open SQLite DB
            myDB = Properties.Settings.Default.SQLiteFile;
            try
            {
                my_db = new SQLiteConnection("Data Source=" + myDB + ";Version=3;");
                sqliteConn = "Data Source=" + myDB + ";Version=3;";
                logbook = new SQLiteDB(sqliteConn);
                this.Text = myDB;
            }
            catch (Exception)
            {
                myDB = "No Database";
                MessageBox.Show("Unable to open SQLite Database");
            }
            
            // open local K3
            rig = Properties.Settings.Default.Rig;
            rig2 = Properties.Settings.Default.Rig2;
            try
            {
                nocallGrids = Properties.Settings.Default.NoCallGrids.Split(';');
            }
            catch (Exception)
            {
                MessageBox.Show("No Call Grids not correct format");
                
            }
            
            if(rig == "K3")
            {
                
                string port = Properties.Settings.Default.Port;
                int baud = Properties.Settings.Default.Baud;
                K3 = new K3Log.RadioCAT(0, port, baud);
                
                K3.VPort = Properties.Settings.Default.VirtualPort;
                if (K3.InitSerialPort())
                {
                    K3.K3Rcvd += new EventHandler<K3Log.RadioCAT.DatarcvdEventArgs>(newK3Data);
                    lblComconnection.Text = "K3 on " + K3.PrimaryPort;
                    lblComconnection.BackColor = Color.LightGreen;
                }
                else
                {
                    //MessageBox.Show("Could not open K3 Port");
                    lblComconnection.Text = "Radio not found on " + K3.PrimaryPort;
                    lblComconnection.BackColor = Color.Red;
                    //System.Windows.Forms.Application.Exit();
                }
            }
            else
            {
                if (rig == "OmniRig1")
                {
                    MyOmniRadio1 = new OmniClient(1);
                    MyOmniRadio1.OmniRigData += new EventHandler<K3Log.OmniClient.DatarcvdEventArgs>(newOmniData);
                    lblComconnection.Text = "OminRig1";
                    lblComconnection.BackColor = Color.LightGreen;
                }
                else 
                {
                    if(rig == "OmniRig2")
                    {
                        MyOmniRadio2 = new OmniClient(2);
                        MyOmniRadio2.OmniRigData += new EventHandler<K3Log.OmniClient.DatarcvdEventArgs>(newOmniData);
                        lblComconnection.Text = "OminRig2";
                        lblComconnection.BackColor = Color.LightGreen;
                    }
                }

            }
            if(rig2 == "OmniRig2")
            {
                MyOmniRadio2 = new OmniClient(2);
                MyOmniRadio2.OmniRigData += new EventHandler<K3Log.OmniClient.DatarcvdEventArgs>(newOmniData);
            }
            //MyOmniRadio1 = new OmniClient(1);
            //MyOmniRadio2 = new OmniClient(2);
            //MyOmniRadio1.OmniRigData += new EventHandler<K3Log.OmniClient.DatarcvdEventArgs>(newOmniData);
           // MyOmniRadio2.OmniRigData += new EventHandler<K3Log.OmniClient.DatarcvdEventArgs>(newOmniData);
            
            // open remote K3
            K3_Remote = new K3Log.RadioCAT(1, "COM14", 38400);
            K3_Remote.Baud = 38400;
            //K3_Remote.Port = "COM9";   // RHR
            //K3_Remote.Port = "COM4";  // dxmate
            K3_Remote.PrimaryPort = "COM14";   // W7DXX
            //K3_Remote.Port = "COM6";   // test remote

            K3_Remote.InitSerialPort();
            K3_Remote.K3Rcvd += new EventHandler<K3Log.RadioCAT.DatarcvdEventArgs>(newK3_RemoteData);
            
            btnAll.BackColor = Color.LightGray;
            FillView(LogdataGridView, "");
            //myWorkedGrids = GetWorkedGrids();
            //cboAutoCondx.SelectedIndex = 1;
            q = new QSO(my_db);
            //c.Client("127.0.0.1", 2237);
            //s.Server("127.0.0.1", 2233);
            //c.Send("TEST!");
            WsjtX = new UDPSocket();
            WsjtX.UDPRcvd += new EventHandler<UDPSocket.DatarcvdEventArgs>(newWSJTXdata);

            WsjtX.UDPServer();

            if (Properties.Settings.Default.WKeyer)
            {
                if (WKConnect())  // Connect to WK
                {
                    lblWKConnected.BackColor = Color.LightGreen;
                    lblWKConnected.Text = "WinKey on " + Properties.Settings.Default.WKComport;
                    if (Properties.Settings.Default.WKOutport == 0) rdoWK1.Checked = true;
                    else rdoWK2.Checked = true;
                }
                else
                {
                    lblWKConnected.BackColor = Color.Pink;
                    lblWKConnected.Text = "WK Not Connected";
                }
               
            }

            satTrack = new SatTrack();
            satTrack.SatTrackEvent += new EventHandler<SatTrackEventArgs>(newSatPosition);
            if (QRZLogbook.loggedIn)
            {
                lblQRZKeyStatus.Text = "QRZ Logbook Connected";
                lblQRZKeyStatus.BackColor = Color.LightGreen;
            }
            else
            {
                lblQRZKeyStatus.Text = "QRZ Logbook Not Avaliable";
                lblQRZKeyStatus.BackColor = Color.Red;
            }

        }

        private List<K3Log.macro> loadMacros(string fspec)
        {
            List<K3Log.macro> mlist = new List<macro>();
            XDocument xmlDoc = XDocument.Load(fspec);
            XName itemName = XName.Get("number", xmlDoc.Root.Name.NamespaceName);
            foreach (XElement e in xmlDoc.Descendants(itemName))
            {
                Console.WriteLine(e);
                macro newMacro = new macro();
                newMacro.btnLabel = e.Attribute("btnLabel").Value;
                newMacro.macroShortCut = Convert.ToInt16(e.Attribute("shortcut").Value);
                newMacro.macroName = e.Attribute("macroName").Value;
                newMacro.macroAction = e.Attribute("macroAction").Value;
                newMacro.number = Convert.ToInt16(e.Value);
                mlist.Add(newMacro);
                Button thisButton = MacroButtons.Find(x => Convert.ToInt16(x.Tag) == newMacro.number);
                thisButton.Text = newMacro.btnLabel;
            }

            return mlist;
        }
        private void startK3()
        {
            string port = Properties.Settings.Default.Port;
            int baud = Properties.Settings.Default.Baud;
            
            K3 = new K3Log.RadioCAT(0, port, baud);
            K3.K3Rcvd += new EventHandler<K3Log.RadioCAT.DatarcvdEventArgs>(newK3Data);
            
            
        }
        private List<String> GetWorked(String[] cols, String ret)
        {
            //SQLiteDB logbook = new SQLiteDB(sqliteConn);
            //String[] cols = { "GridSquare" };
            List<String> WorkedGrids = logbook.RetrieveDistinct("Log", ret, "band", cboBand.Text, "=", cols);
            return WorkedGrids;
        }

        private void postspots(String msg)
        {
            String urlWithAccessToken = "https://hooks.slack.com/services/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";
            SlackClient client = new SlackClient(urlWithAccessToken);

            client.PostMessage(username: operatorName + " " + callsign,
                       text: msg,
                       channel: "#ft8-ft4");
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
        // returns (0 = worked band and mode), (1 = need band), (2 = need mode), (3 = need mode and band)
        private Int32 checkworked(String DXCC, String freq, string mode)
        {
            Int32 ret = 4;
            String[] wrkdcols = { "band", "mode" };  //, "qsoconfirmations" };
            List<String> wrkd = logbook.Retrieve("Log", "country", DXCC, "=", wrkdcols);
            if (wrkd.Count != 0) // if no ATNO then check for worked mode on bands.
            {
                if (mode == "USB") mode = "SSB";
                if (mode == "LSB") mode = "SSB";
                if (mode == "PHONE") mode = "SSB";
                
                ret = 3;  // assume need on both band and mode
                String[] list = bandlist.Band(Convert.ToDouble(freq));
                // do we need it on this band?
                foreach (String w in wrkd)
                {
                    String b = w.Split(',')[0];
                    String m = w.Split(',')[1];
                    if (b == list[0])
                    {
                        ret = ret & 0x2;  // found band so exit for
                        if(mode == "DATA")
                        {
                            foreach (string digmode in cboSubmode.Items)
                            {
                                if(m == digmode)
                                {
                                    ret = ret & 0x01;  // found mode so exit for
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (m == mode)
                            {
                                ret = ret & 0x01;  // found mode so exit for
                                break;
                            }
                        }
                        
                        //break;
                    }
                }

                // do we need it on this mode?

                //String mode = list[1];

                
                
                /*
                foreach (String w in wrkd)
                {
                    String m = w.Split(',')[1];
                   
                    if (m == mode)
                    {
                        ret = ret & 0x01;  // found mode so exit for
                        break;
                    }

                }*/
            }


            return ret;
        }
        private string[] getcountry(string DX, String rawFreq, string mode)
        {
         
                // returns (0 = worked band and mode), (1 = need band), (2 = need mode), (3 = need mode and band), (4 = ATNO)
            string retCtry = "";
            
            String dxPre = "";
            Int32 wrkd = 4;
            String[] cols = { "Entity", "EntityCode" };
            //'try match on full callsign
            string ctry = countrylist.CountryName(DX.Split('/')[0])[0];
            string dxcc = countrylist.CountryName(DX.Split('/')[0])[1];
            string[] ret = { ctry, dxcc };
            if (DX != "")
            {
                //todo below looking up this station in log database  If worked, check band mode and skip the rest below
                List<String[]> stationworked = foundInLog((DX.Split('/')[0]));
                //logbookDB.Retrieve("Log", "Prefix", DX.Split('/')[0], "=", cols);
                string Freq = "";
                //Freq = ((Convert.ToDouble(rawFreq)) / 1000).ToString();  // why are we doing this?  maybe only needed for Cluster spot???

                Freq = rawFreq;
                if (ctry != "")
                {
                    wrkd = checkworked(ctry, Freq, mode);
                    retCtry = ctry; //[0].ToString();  // { DX, ctry.Item[0].item[0], Freq, Comment, spotTime, Spotter};

                }
                else
                {
                    dxPre = DX;
                    //'try exact match on first four letters
                    if (DX.Length >= 4) dxPre = DX.Substring(0, 4);
                    //ctry = logbookDB.sqliteFind(dxPre); //logbookDB.Retrieve("Country", "Prefix", dxPre, "=", cols);
                    ctry = countrylist.CountryName(dxPre)[0];
                    if (ctry != "")
                    {
                        wrkd = checkworked(ctry, Freq, mode);
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
                            wrkd = checkworked(ctry, Freq, mode);
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
                                wrkd = checkworked(ctry, Freq, mode);
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
                                    wrkd = checkworked(ctry, Freq, mode);
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
            }

            return ret;
        }

        private bool WKConnect()
        {
            if(keyer == null)
            {
                keyer = new Winkey(Properties.Settings.Default.WKComport);
                keyer.Output = Properties.Settings.Default.WKOutport;
                if (keyer.InitSerialPort())
                {
                    keyer.WinkeyRcvd += new EventHandler<Winkey.WinkeyEventArgs>(newWinkeydata);
                    keyer.requestWPM();
                    //keyer.wkthread();
                    keyer.sendcw("V");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            
        }
        private void newWinkeydata(object sender, K3Log.Winkey.WinkeyEventArgs e)
        {
            //SetTextBox(textIn, e.text, true, Color.White);
            if (e.wpm != "") SetTextBox(txtCWSpeed, e.wpm, false, Color.White);
        }

        private void newWSJTXdata(object sender, K3Log.UDPSocket.DatarcvdEventArgs e)
        {

            if (q.qsoid == "" && !BlockX && chkwsjtx.Checked)
            {
                switch (e.xType)
                {
                    case 1:   // status
                        SetTextBox(txtCallsign, e.xDxCall, false, Color.White);
                        SetTextBox(txtRSTs, e.xReport, false, Color.White);
                        if (myWorkedGrids.Contains(e.xDxGrid))
                        {
                            SetTextBox(txtGrid, e.xDxGrid, false, Color.LightBlue);
                        }
                        else
                        {
                            SetTextBox(txtGrid, e.xDxGrid, false, Color.LightYellow);
                        }
                        int x = 0;
                        foreach (string smode in cboSubmode.Items)
                        {

                            if (smode == e.xMode)
                            {
                                SetComboBox(cboSubmode, (short)x);

                                break;
                            }
                            x++;


                        }
                        //FillView(LogdataGridView, txtCallsign.Text);
                        QRZFillIn();
                        
                        break;

                    case 2:  // decode
                        wsjtXmsg msg = new wsjtXmsg(e.xMessage);
                        
                        Color markcolor = Color.Black;
                        bool autocall = false;
                        
                        if (!nocallGrids.Contains(msg.grid))
                        {
                            if (msg.called == callsign)
                            {
                                if (chkautocall.Checked && !calling)
                                {
                                    calling = true;
                                    SetButton(btnTX, Color.Red);
                                    SetButton(btnLog, Color.Yellow);
                                    WsjtX.SendToWsjt();

                                }
                            }
                            else
                            {
                                if (msg.called == "CQ" || msg.grid != "")
                                {
                                    // we spot only once per callsign, only spot cq
                                    if (!spottedCalls.Contains(msg.callsign) && !nocallGrids.Contains(msg.grid))
                                    {
                                        spottedCalls.Add(msg.callsign);
                                        if (chkSpot.Checked)
                                        {
                                            postspots("de " + callsign + " " + grid + " > " + e.xMessage);

                                        }
                                    }
                                    if (myWorkedCalls.Contains(msg.callsign))
                                    {
                                        markcolor = Color.Blue; // call worked before on this band
                                        SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "");
                                        SendGrid(msg.grid, "WorkedSpot", msg.callsign);
                                    }
                                    else
                                    {
                                        string spottype = "InitialSpot";
                                        string postmsg = "";
                                        if (chkAutoInit.Checked)  // if we got here, this is an initial
                                        {
                                            if (!calling && chkAutoInit.Checked) autocall = true;
                                            postmsg += "Init";
                                            markcolor = Color.Green;
                                            //SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "Init");
                                            
                                        }
                                        
                                        if (chkAutoGrid.Checked)  // new grid
                                        {
                                            string qualmsg = "";
                                            //markcolor = Color.Black;
                                            if (msg.grid != "")
                                            {
                                                //  myWorkedGrids is updated with band change.
                                                if (!myWorkedGrids.Contains(msg.grid) || msg.grid == "nil")
                                                {
                                                    markcolor = Color.Red;   // new grid this band detected
                                                    qualmsg = "Grid";
                                                    if (!calling && chkAutoGrid.Checked) autocall = true;
                                                    spottype = "NewGrid";
                                                }
                                            }
                                            postmsg += qualmsg;
                                            //SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, qualmsg);
                                        }

                                        // new DXCC
                                        string ctry = getcountry(msg.callsign, e.xRxDF, e.xMode)[0];
                                        int worked = checkworked(ctry, e.xRxDF, e.xMode);
                                        // returns (0 = worked band and mode), (1 = need band), (2 = need mode), (3 = need mode and band), (4 = ATNO)

                                        switch (worked)
                                        {
                                            case 0:     // worked band and mode
                                                if ((chkAutoBand.Checked || chkAutoMode.Checked) && !calling)
                                                {
                                                    //markcolor = Color.Black;
                                                    //SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "");
                                                }
                                                
                                                break;
                                            case 1:     // need band
                                                if (chkAutoBand.Checked && !calling)
                                                {
                                                    autocall = true;
                                                    markcolor = Color.Orange;
                                                    postmsg += "Band";
                                                    //SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "Band");
                                                }
                                                break;
                                            case 2:     // need mode
                                                if (chkAutoMode.Checked && !calling)
                                                {
                                                    autocall = true;
                                                    markcolor = Color.DarkMagenta;
                                                    postmsg += "Mode";
                                                    //SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "Mode");
                                                }
                                                break;
                                            case 3:     // need band and mode
                                                if (!calling)
                                                {
                                                    if (chkAutoBand.Checked || chkAutoMode.Checked)
                                                    {
                                                        autocall = true;
                                                    }

                                                    markcolor = Color.Green;
                                                    postmsg += "BndMd";
                                                    //SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "BndMd");
                                                }
                                                break;
                                            case 4:     // ATNO
                                                if (!calling && chkAutoATNO.Checked)
                                                {
                                                    autocall = true;
                                                    markcolor = Color.Violet;
                                                    postmsg += "ATNO";
                                                    //SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "ATNO");
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                                                                
                                        SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, postmsg);
                                        if (msg.grid != "") SendGrid(msg.grid, spottype, msg.callsign);
                                        if (chkautocall.Checked && !calling)
                                        {
                                            if (autocall)
                                            {
                                                
                                                if (msg.extra == "")
                                                {
                                                    
                                                    if (msg.called == "CQ")
                                                    {
                                                        
                                                        calling = true;
                                                        SetButton(btnTX, Color.Red);
                                                        SetButton(btnLog, Color.Yellow);
                                                        if (q.qsoid == "")
                                                        {
                                                            SetTextBox(txtStartDate, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), false, Color.White);
                                                            SetTextBox(txtEndDate, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), false, Color.White);
                                                            newQsoId = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff");
                                                        }
                                                        WsjtX.SendToWsjt();
                                                    }
                                                    if (chkSMS.Checked)  // do we send an SMS?
                                                    {
                                                        string smsNum = Properties.Settings.Default.SMSNumber;
                                                        sendSMS("de " + callsign + " New Q > " + e.xMessage, smsNum);

                                                    }
                                                }
                                            }
                                            
                                        }
                                    
                                    }

                                }
                            }
                        }
                        else
                        {
                            markcolor = Color.Gray;
                            SetRichTextBox(WorkedBox, e.xMessage, true, markcolor, "Local");
                        }


                        break;

                    case 5:   // log out
                        SetTextBox(txtCallsign, e.xDxCall, false, Color.White);
                        if (contestMode)
                        {
                            SetTextBox(txtRSTs, "00", false, Color.White);
                            SetTextBox(txtRSTr, "00", false, Color.White);
                        }
                        else
                        {
                            SetTextBox(txtRSTs, e.xRptSent, false, Color.White);
                            SetTextBox(txtRSTr, e.xReport, false, Color.White);
                        }

                        if (txtStartDate.Text == "")
                        {
                            SetTextBox(txtStartDate, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), false, Color.White);
                            SetTextBox(txtEndDate, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), false, Color.White);
                            newQsoId = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff");
                        }
                        if (myWorkedGrids.Contains(e.xDxGrid))
                        {
                            SetTextBox(txtGrid, e.xDxGrid, false, Color.LightBlue);
                        }
                        else
                        {
                            SetTextBox(txtGrid, e.xDxGrid, false, Color.LightYellow);
                        }
                        x = 0;
                        foreach (string smode in cboSubmode.Items)
                        {

                            if (smode == e.xMode)
                            {
                                SetComboBox(cboSubmode, (short)x);

                                break;
                            }
                            x++;


                        }
                        //FillView(LogdataGridView, txtCallsign.Text);
                        QRZFillIn();
                        //logThisQ();  this caused cross thread 

                        break;
                    default:
                        break;

                }


                /*// type 1 message Status
                    // type 5 message Log Out
                    SetTextBox(txtCallsign, e.xDxCall, false, Color.White);
                    if (e.xType == 1) SetTextBox(txtRSTs, e.xReport, false, Color.White);
                    if (e.xType == 5)
                    {
                        if (contestMode)
                        {
                            SetTextBox(txtRSTs, "00", false, Color.White);
                            SetTextBox(txtRSTr, "00", false, Color.White);
                        }
                        else
                        {
                            SetTextBox(txtRSTs, e.xRptSent, false, Color.White);
                            SetTextBox(txtRSTr, e.xReport, false, Color.White);
                        }
                        
                    }


                    int x = 0;
                    foreach (string smode in cboSubmode.Items)
                    {

                        if (smode == e.xMode)
                        {
                            SetComboBox(cboSubmode, (short)x);
                            
                            break;
                        }
                        x++;


                    }
                    if (myWorkedGrids.Contains(e.xDxGrid))
                    {
                        SetTextBox(txtGrid, e.xDxGrid, false, Color.LightBlue);
                    }
                    else
                    {
                        SetTextBox(txtGrid, e.xDxGrid, false, Color.LightYellow);
                    }
                    if (txtDate.Text == "")
                    {
                        SetTextBox(txtDate, DateTime.Now.ToUniversalTime().ToString("yyyyMMdd"), false, Color.White);
                        SetTextBox(txtTime, DateTime.Now.ToUniversalTime().ToString("HHmmss"), false, Color.White);
                        newQsoId = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff");
                    }
                    FillView(LogdataGridView, txtCallsign.Text);
                    QRZFillIn();*/

            }

        }

        private void newOmniData(object sender, K3Log.OmniClient.DatarcvdEventArgs e)
        {
            try
            {
                this.InvokeAndClose((MethodInvoker)delegate
                {
                    

                    if (e.Radio_Num == 1)
                    {
                        thisRadioBox = VFOA_Radio1;
                        thisRadioMode = txtRadio1Mode;
                    }
                    else
                    {
                        thisRadioBox = VFOA_Radio2;
                        thisRadioMode = txtRadio2Mode;
                    }
                    double dFA = e.FA / 1000;
                    if (chkLink.Checked)
                    {
                        if (e.Radio_Num == 1 && radiobtnRadio2.Checked)
                        {
                            //linkedRadio(1);
                        }
                        else
                        {
                            //if (e.Radio_Num == 2 && radiobtnRadio1.Checked) linkedRadio(2);
                        }

                    }

                    if (e.Radio_Num == 1)
                    {
                        //SetComboBox(cboMode, e.MD);
                        cboMode.SelectedIndex = e.MD;
                        //SetTextBox(txtFreq, (e.FA / 1000).ToString("#.00"), false, Color.White);
                        txtFreq.Text = (e.FA / 1000).ToString("###.00");
                        //SetComboBox(cboBand, getBand(dFA));
                        cboBand.SelectedIndex = getBand(dFA);
                    }
                    else
                    {
                        RadioBand[1] = getBand(dFA);
                        // fix omnirig inversion modes
                        short realmode = e.MD;
                        if (e.MD == 7) realmode = 3;
                        if (e.MD == 8) realmode = 6;

                        RadioMode[1] = realmode;
                        RadioFreq[1] = e.FA;

                    }

                    //SetTextBox(thisRadioBox, dFA.ToString("#.00"), false, Color.White);
                    thisRadioBox.Text = dFA.ToString("###.00");
                    thisRadioMode.Text = mode[e.MD];
                    /*switch (e.MD)
                    {
                        case 1:
                            SetTextBox(thisRadioMode, "LSB", false, Color.White);
                            break;
                        case 2:
                            SetTextBox(thisRadioMode, "USB", false, Color.White);
                            break;
                        case 3:
                            SetTextBox(thisRadioMode, "CW", false, Color.White);
                            break;
                        case 4:
                            SetTextBox(thisRadioMode, "FM", false, Color.White);
                            break;
                        case 5:
                            SetTextBox(thisRadioMode, "AM", false, Color.White);
                            break;
                        case 6:
                            SetTextBox(thisRadioMode, "DATA", false, Color.White);
                            break;
                        case 7:
                            SetTextBox(thisRadioMode, "CW-R", false, Color.White);
                            break;
                        case 9:
                            SetTextBox(thisRadioMode, "DATA-R", false, Color.White);
                            break;
                        default:
                            break;

                    }*/
                });
            }
                
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                //throw;
            }
        }

        private short getBand(double freq)
        {
            short band = 0;
            switch (freq)
            {
                case double n when (n <= 150 && n >= 120):
                    band = 0;
                    break;
                case double n when (n <= 478 && n >= 473):
                    band = 1;
                    break;
                case double n when (n <= 2000 && n >= 1800):
                    band = 2;
                    break;
                case double n when (n <= 4000 && n >= 3500):
                    band = 3;
                    break;
                case double n when (n <= 5500 && n >= 5000):
                    band = 4;
                    break;
                case double n when (n <= 7350 && n >= 7000):
                    band = 5;
                    break;
                case double n when (n < 11000 && n >= 10000):
                    band = 6;
                    break;
                case double n when (n < 14400 && n >= 14000):
                    band = 7;
                    break;
                case double n when (n < 18800 && n >= 18000):
                    band = 8;
                    break;
                case double n when (n < 21450 && n >= 21000):
                    band = 9;
                    break;
                case double n when (n < 25000 && n >= 24000):
                    band = 10;
                    break;
                case double n when (n < 30000 && n >= 28000):
                    band = 11;
                    break;
                case double n when (n < 54000 && n >= 50000):
                    band = 12;
                    break;
                case double n when (n < 148000 && n >= 144000):
                    band = 13;
                    break;
                case double n when (n < 450000 && n >= 420000):
                    band = 14;
                    break;

            }

            return band;
        }

        private void linkedRadio(int radionum)
        {

            
            if (oldFA != 0)
            {
                // get Radio1 freq
                double controlfreq = 0; 

                try
                {
                    
                        
                    if (radionum == 1)
                    {
                        controlfreq = Convert.ToDouble(VFOA_Radio2.Text);
                        double delta = controlfreq - oldFA;  // delta of the control radio
                        if (delta != 0)
                        {
                            oldFA = Convert.ToDouble(VFOA_Radio2.Text) * 1000;
                            MyOmniRadio1.DialFreq(Convert.ToInt32(delta), deltaHz);
                        }
                    }
                    else
                    {
                        controlfreq = Convert.ToDouble(VFOA_Radio1.Text) * 1000;
                        double delta = controlfreq - oldFA;
                        if (delta != 0)
                        {
                            oldFA = Convert.ToDouble(VFOA_Radio1.Text);
                            MyOmniRadio2.DialFreq(Convert.ToInt32(delta), deltaHz);
                        }
                    }

                    
                }
                catch (Exception)
                {
                    //oldFA = dFA;
                }


            }
            

        }
        
        private void newK3Data(object sender, K3Log.RadioCAT.DatarcvdEventArgs e)
        {

            if (K3 != null && !this.IsDisposed)
            {

                this.InvokeAndClose((MethodInvoker)delegate
                {
                                         
                    VFOA_Radio1.Text = (e.FA * 1000).ToString("###.00");
                    VFOB_Radio1.Text = (e.FB * 1000).ToString("###.00");
                    // K3 decoced CW PSK or FSK
                    textIn.Text += e.TB;
                    //WorkedBox.Text += e.VirtualCmd +" ";
                    if(textIn.Text.Length > 220)
                    {
                        string trimt = textIn.Text.Substring(textIn.Text.Length - 160);
                        textIn.Text = trimt;
                    }
                    textIn.SelectionStart = textIn.Text.Length;
                    textIn.ScrollToCaret();
                    //SetTextBox(VFOA_Radio1, K3.FA, false, Color.White);
                    if (chkLink.Checked)
                    {
                        double dFA = e.FA;
                        if (oldFA != 0)
                        {
                            double deltaHz = dFA - oldFA;

                            if (deltaHz != 0)
                            {
                                oldFA = dFA;
                                MyOmniRadio2.DialFreq((int)deltaHz, linkFactor);
                            }

                        }
                        else
                        {
                            oldFA = dFA;
                        }


                    }

                    //SetTextBox(VFOB_Radio1, K3.FB, false, Color.White);
                    //SetTextBox(textIn, e.TB, true, Color.White);
                    
                    txtRadio1Mode.Text = mode[e.MD];
                    
                    //SetComboBox(txtMode, e.MDsub);
                    txtRadio1SubMode.Text = mode[e.MDsub];
                    
                    if (q != null) {
                        if (q.qsoid == "")
                        {
                            if (!chkSatellite.Checked)
                            {
                                txtFreq.Text = (e.FA * 1000).ToString("###.00");
                                txtRXFreq.Text = (e.FA * 1000).ToString("###.00");
                                //SetTextBox(txtFreq, K3.FA, false, Color.White);
                                //SetTextBox(txtRXFreq, K3.FA, false, Color.White);
                                cboMode.SelectedIndex = e.MD;

                                //SetComboBox(cboMode, e.MD);

                                //ActiveRadioMD = e.MD;


                                switch (e.BN)
                                {
                                    case 16:  // 2m
                                              //SetComboBox(cboBand, 13);
                                              //SetComboBox(cboRXBand, 13);
                                        cboBand.SelectedIndex = 13;
                                        cboRXBand.SelectedIndex = 13;
                                        break;
                                    case 17:   //23cm
                                               //SetComboBox(cboBand, 15);
                                               //SetComboBox(cboRXBand, 15);
                                        cboBand.SelectedIndex = 15;
                                        cboRXBand.SelectedIndex = 15;
                                        break;
                                    default:
                                        
                                        if (e.BN < 3)  // handle 630m and 2200m as they show up as band = 160
                                        {
                                            if (e.FA < .200)
                                            {
                                                //SetComboBox(cboBand, 0);
                                                //SetComboBox(cboRXBand, 0);
                                                cboBand.SelectedIndex = 0;
                                                cboRXBand.SelectedIndex = 0;
                                            }
                                            else
                                            {
                                                if (e.FA > .300 && e.FA < .500)
                                                {
                                                    //SetComboBox(cboBand, 1);
                                                    //SetComboBox(cboRXBand, 1);
                                                    cboBand.SelectedIndex = 1;
                                                    cboRXBand.SelectedIndex = 1;
                                                }
                                                else
                                                {
                                                    //SetComboBox(cboBand, (short)(e.BN + 2));
                                                    //SetComboBox(cboRXBand, (short)(e.BN + 2));
                                                    cboBand.SelectedIndex = (short)(e.BN + 2);
                                                    cboRXBand.SelectedIndex = (short)(e.BN + 2);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //SetComboBox(cboBand, (short)(e.BN + 2));
                                            //SetComboBox(cboRXBand, (short)(e.BN + 2));
                                            cboBand.SelectedIndex = (short)(e.BN + 2);
                                            cboRXBand.SelectedIndex = (short)(e.BN + 2);
                                        }
                                        break;

                                }

                            }
                            else
                            {
                                
                                if (TXRadio == 2)
                                {
                                    //SetTextBox(txtFreq, (RadioFreq[TXRadio-1]/1000).ToString(), false, Color.White);
                                    //SetTextBox(txtRXFreq, (e.FA*1000).ToString(), false, Color.White);
                                    txtFreq.Text = (RadioFreq[TXRadio - 1] / 1000).ToString();
                                    txtRXFreq.Text = (e.FA * 1000).ToString();
                                    //SetComboBox(cboBand, RadioBand[TXRadio - 1]);
                                    //SetComboBox(cboRXBand, getBand(e.FA * 1000));
                                    cboBand.SelectedIndex = RadioBand[TXRadio - 1];
                                    cboRXBand.SelectedIndex = getBand(e.FA * 1000);
                                    //SetComboBox(cboMode, RadioMode[TXRadio-1]);
                                    cboMode.SelectedIndex = RadioMode[TXRadio - 1];
                                }
                                else
                                {

                                    //SetTextBox(txtFreq, K3.FA, false, Color.White);
                                    //SetTextBox(txtRXFreq, VFOA_Radio2.Text, false, Color.White);
                                    txtFreq.Text = (e.FA * 1000).ToString("###.00");
                                    txtRXFreq.Text = VFOA_Radio2.Text;
                                    //SetComboBox(cboRXBand, 14);
                                    //SetComboBox(cboMode, e.MD);
                                    cboRXBand.SelectedIndex = 14;
                                    cboMode.SelectedIndex = e.MD;
                                    //ActiveRadioMD = e.MD;


                                    switch (e.BN)
                                    {
                                        case 16:  // 2m
                                                  //SetComboBox(cboBand, 13);
                                            cboBand.SelectedIndex = 13;
                                            ////SetComboBox(cboRXBand, 13);
                                            break;
                                        case 17:   //23cm
                                                   //SetComboBox(cboBand, 15);
                                            cboBand.SelectedIndex = 15;
                                            ////SetComboBox(cboRXBand, 15);
                                            break;
                                        default:
                                            
                                            if (e.BN < 3)  // handle 630m and 2200m as they show up as band = 160
                                            {
                                                if (e.FA < .200)
                                                {
                                                    //SetComboBox(cboBand, 0);
                                                    cboBand.SelectedIndex = 0;
                                                    ////SetComboBox(cboRXBand, 0);
                                                }
                                                else
                                                {
                                                    if (e.FA > .300 && e.FA < .500)
                                                    {
                                                        //SetComboBox(cboBand, 1);
                                                        cboBand.SelectedIndex = 1;
                                                        ////SetComboBox(cboRXBand, 1);
                                                    }
                                                    else
                                                    {
                                                        //SetComboBox(cboBand, (short)(e.BN + 2));
                                                        cboBand.SelectedIndex = (short)(e.BN + 2);
                                                        ////SetComboBox(cboRXBand, (short)(e.BN + 2));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //SetComboBox(cboBand, (short)(e.BN + 2));
                                                //SetComboBox(cboRXBand, (short)(e.BN + 2));
                                                cboBand.SelectedIndex = (short)(e.BN + 2);
                                                cboRXBand.SelectedIndex = (short)(e.BN + 2);
                                            }
                                            break;

                                    }

                                }
                            }
                        }
                    }
                    if (e.wordindex > 0)
                    {
                        StrikeTextBox(textOut, e.wordindex);
                        /*if (words.Count > 0)
                        {
                            textOut.SelectionStart = 0;
                            textOut.SelectionLength = e.wordindex; // words[0].index;
                            textOut.SelectionColor = Color.Red;
                            textOut.SelectionStart = textOut.Text.Length;
                            
                            //textbox.Refresh();
                            //textbox.Text += " ";

                        }*/

                    }
                });
            }

            //SetTextBox(DebugBox, e.msg + "\n", true);
        }

        private void newK3_RemoteData(object sender, K3Log.RadioCAT.DatarcvdEventArgs e)
        {
            if (radioButtonRadio2.Checked)
            {
                if (K3_Remote != null && !this.IsDisposed)
                {


                    SetTextBox(VFOA_Radio2, K3_Remote.FA, false, Color.White);
                    SetTextBox(VFOB_Radio2, K3_Remote.FB, false, Color.White);
                    SetTextBox(textIn, e.TB, true, Color.White);
                    if (q.qsoid == "")
                    {
                        SetTextBox(txtFreq, K3_Remote.FA, false, Color.White);
                        SetComboBox(cboMode, e.MD);
                        //ActiveRadioMD = e.MD;
                        if (e.BN == 16) SetComboBox(cboBand, 13);
                        else
                        {
                            if (e.BN < 3)  // handle 630m and 2200m as they show up as band = 160
                            {
                                if (e.FA < .200)
                                    SetComboBox(cboBand, 0);
                                else
                                    if (e.FA > .300 && e.FA < .500)
                                    SetComboBox(cboBand, 1);
                                else
                                    SetComboBox(cboBand, (short)(e.BN + 2));
                            }
                            else
                            {
                                SetComboBox(cboBand, (short)(e.BN + 2));
                            }

                        }
                    }


                    switch (e.MD)
                    {
                        case 1:
                            SetTextBox(txtRadio2Mode, "LSB", false, Color.White);
                            break;
                        case 2:
                            SetTextBox(txtRadio2Mode, "USB", false, Color.White);
                            break;
                        case 3:
                            SetTextBox(txtRadio2Mode, "CW", false, Color.White);
                            break;
                        case 4:
                            SetTextBox(txtRadio2Mode, "FM", false, Color.White);
                            break;
                        case 5:
                            SetTextBox(txtRadio2Mode, "AM", false, Color.White);
                            break;
                        case 6:
                            SetTextBox(txtRadio2Mode, "DATA", false, Color.White);
                            break;
                        case 7:
                            SetTextBox(txtRadio2Mode, "CW-R", false, Color.White);
                            break;
                        case 9:
                            SetTextBox(txtRadio2Mode, "DATA-R", false, Color.White);
                            break;
                        default:
                            break;

                    }
                    //SetComboBox(txtMode, e.MDsub);
                    switch (e.MDsub)
                    {
                        case 1:
                            SetTextBox(txtRadio2SubMode, "LSB", false, Color.White);
                            break;
                        case 2:
                            SetTextBox(txtRadio2SubMode, "USB", false, Color.White);
                            break;
                        case 3:
                            SetTextBox(txtRadio2SubMode, "CW", false, Color.White);
                            break;
                        case 4:
                            SetTextBox(txtRadio2SubMode, "FM", false, Color.White);
                            break;
                        case 5:
                            SetTextBox(txtRadio2SubMode, "AM", false, Color.White);
                            break;
                        case 6:
                            SetTextBox(txtRadio2SubMode, "DATA", false, Color.White);
                            break;
                        case 7:
                            SetTextBox(txtRadio2SubMode, "CW-R", false, Color.White);
                            break;
                        case 9:
                            SetTextBox(txtRadio2SubMode, "DATA-R", false, Color.White);
                            break;
                        default:
                            break;

                    }
                    if (e.wordindex > 0)
                    {
                        StrikeTextBox(textOut, e.wordindex);

                    }
                }
            }

        }

        private void sendSMS(String msg, String phone)
        {

            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "mail.katytx.net";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("n5tm@katytx.net", "dange$123");

                MailMessage mm = new MailMessage("n5tm@katytx.net", phone, "Spot", msg);
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //private delegate void newSetComboBox(ComboBox cbo, Int16 value);
        public void SetComboBox(ComboBox cbo, Int16 value)
        {

            /*if (cbo.InvokeRequired)
            {
                newSetComboBox c = new newSetComboBox(SetComboBox);
                System.IAsyncResult r = this.BeginInvoke(c, new object[] { cbo, value });
                this.EndInvoke(r);
                //this.Dispose();
                //this.Invoke(new Action<ComboBox, Int16>(SetComboBox), new object[] { cbo, value});
                return;
            }*/

            try
            {
                this.InvokeAndClose((MethodInvoker)delegate
                {
                    cbo.SelectedIndex = value;
                });

            }
            catch (Exception)
            {

                //throw;
            }


        }


        //private delegate void newSetRichTextCallback(RichTextBox textbox, string value, bool concat, Color color);
        public void SetRichTextBox(RichTextBox textbox, string value, bool concat, Color color, string modifier)
        {

            this.InvokeAndClose((MethodInvoker)delegate
            {
                if (concat)
                {
                    textbox.AppendText("[" + DateTime.Now.ToShortTimeString() + "]  ", Color.Blue);
                    textbox.AppendText(" ");
                    //if (color == Color.Red)
                    //{
                        if(modifier != "") textbox.AppendText(modifier + cboBand.Text + ": ", Color.DarkRed);
                        if (chkSound.Checked)
                        {
                            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\chimes.wav"))
                            {
                                soundPlayer.Play();
                            }
                        }

                    //}
                    textbox.AppendText(value, color);
                    textbox.AppendText(Environment.NewLine);

                    textbox.ScrollToCaret();

                    if (textbox.Lines.Length > 100) textbox.Text = "";

                }

                else
                {
                    textbox.Text = value;

                }
            });


        }
        //private delegate void newSetButtonCallback(Button btn, Color color);
        public void SetButton(Button btn, Color color)
        {
            /*if (btn.InvokeRequired)
            {
                newSetButtonCallback b = new newSetButtonCallback(SetButton);
                this.Invoke(b, new object[] { btn, color });
                b = null;
                return;
            }*/
            this.InvokeAndClose((MethodInvoker)delegate
            {
                btn.BackColor = color;
                if (color == Color.Red)
                {
                    if (chkSound.Checked)
                    {
                        using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\chimes.wav"))
                        {
                            soundPlayer.Play();
                        }
                    }
                }
            });

        }

        private void SendGrid(string grid, string highlite, string info)
        {
            try
            {
                this.InvokeAndClose((MethodInvoker)delegate
                {
                    if(DecodedGrids != null)
                    {
                        DecodedGrids.highlitegrid(grid, highlite, info);
                    }
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SetTextBox(TextBox textbox, string value, bool concat, Color color)
        {
            
            try
            {

                
                if (concat)
                {
                    this.InvokeAndClose((MethodInvoker)delegate
                    {

                        textbox.Text += value; /// + "\r\n";
                        textbox.SelectionStart = textbox.Text.Length;
                        textbox.ScrollToCaret();
                    });
                    

                }
                else
                {
                    this.InvokeAndClose((MethodInvoker)delegate
                    {
                        if (color != Color.Yellow) textbox.Text = value; // if yellow don't overwrite
                    });
                }
                this.InvokeAndClose((MethodInvoker)delegate
                {
                    if (color != Color.White && color != Color.Yellow) textbox.BackColor = color;
                });
                
                //Application.DoEvents();
            }
            catch (Exception e)
            {

                Console.Write(e.ToString());
            }

        }
        
        public void StrikeTextBox(RichTextBox textbox, int lastindex)
        {
            
            this.InvokeAndClose((MethodInvoker)delegate
            {
                if (words.Count > 0)
                {
                    textbox.SelectionStart = 0;
                    textbox.SelectionLength = lastindex; // words[0].index;
                    textbox.SelectionColor = Color.Red;
                    textbox.SelectionStart = textbox.Text.Length;
                    //textbox.Refresh();
                    //textbox.Text += " ";

                }
            });

        }

        private void sendKYOut(string text, int index, bool echo)
        {
            string[] wordarray = text.Split(' ');
            /*
            if (!chkUseKY.Checked)
            {

                // uSe WinKey for cw
                SendBtn.BackColor = Color.Red;
                foreach (string s in wordarray)
                {
                    
                    keyer.sendcw(s.ToUpper() + " ");
                    if (echo)
                    {
                        textOut.AppendText(s + " ", Color.Red);
                    }
                    else
                    {
                        textOut.SelectionStart = index;
                        string t = textOut.Text.Substring(0, index);
                        textOut.Text = t;
                        textOut.AppendText(text, Color.Red);

                    }
                    if (textOut.Text.Length > 220)
                    {
                        string trimt = textOut.Text.Substring(textOut.Text.Length - 160);
                        textOut.Text = trimt;
                    }
 
                }
                SendBtn.BackColor = Color.Green;
            }
            else
            {*/
                //  use KY commands if WinKeyer not connected.
                
            if (lastTextOut > textOut.Text.Length)
            {
                lastTextOut = textOut.Text.Length;
            }
            outBuffer = textOut.Text.Substring(lastTextOut, textOut.Text.Length - lastTextOut);

            //string[] wordarray = outBuffer.Split(' ');
            //int offset = 0;
            foreach (string s in wordarray)
            {
                SendBtn.BackColor = Color.Red;
                if (echo)
                {
                    textOut.AppendText(s + " ", Color.Red);
                }
                if (chkUseKY.Checked)
                {
                    K3.sendKYWord(s + " ");
                }
                else
                {
                    keyer.sendcw(s.ToUpper() + " ");
                }
                    
                /*
                offset += s.Length + 1;
                K3Log.RadioCAT.TXList tx = new K3Log.RadioCAT.TXList();
                tx.index = lastTextOut + offset;
                tx.word = s + " ";
                words.Add(tx);*/

                    
            }
            SendBtn.BackColor = Color.Green;
            lastTextOut = textOut.Text.Length;
                
            //K3.sendKYList(words);
            if (textOut.Text.Length > 220)
            {
                string trimt = textOut.Text.Substring(textOut.Text.Length - 160);
                textOut.Text = trimt;
            }
                
                
            textOut.Focus();
            //}

        }

        public void SendRXMode()
        {
            K3.send("RX;");
        }

        private void SendBtn_Click(object sender, EventArgs e)
        {

            if (SendBtn.BackColor != Color.Green)
            {
                SendBtn.BackColor = Color.Green;
                SendRXMode();
            }
            else
            {
                SendBtn.BackColor = Color.Red;
                sendKYOut(textOut.Text, 0, false);
            }

        }

        private void textOut_KeyDown(object sender, KeyEventArgs e)
        {
            // if the send checkbox is checked, send on space, comma, question mark, and period.
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.OemPeriod
                || e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemQuestion
                || e.KeyCode == Keys.Return)
            {
                if (outBuffer.TrimEnd() != textOut.Text.TrimEnd())
                {
                    if (lastTextOut > textOut.Text.Length)
                    {
                        lastTextOut = textOut.Text.Length;
                    }
                    outBuffer = textOut.Text.Substring(lastTextOut, textOut.Text.Length - lastTextOut);
                    
                    if (chkSendWord.Checked) sendKYOut(outBuffer, lastTextOut, false);
                    lastTextOut = textOut.Text.Length + 1 ;
                }
                if (e.KeyCode == Keys.Return)
                {
                    //sendKYOut(textOut.Text += " EOL ");
                }

            }
            else
            {
                // set text to color to black
                textOut.SelectionStart = textOut.Text.Length;
                textOut.SelectionLength = textOut.Text.Length;
                textOut.SelectionColor = Color.Black;

            }


        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            textOut.Text = "";
            outBuffer = "";
            lastTextOut = 0;
        }

        private void ClearRxBtn_Click(object sender, EventArgs e)
        {
            textIn.Text = "";
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            if(myDB != "No Database")
            {
                logThisQ();
                SendGrid(q.grid, "WorkedGrid", q.callsign);
            }
            
        }

        private void logThisQ()
        {
            if (txtCallsign.Text != "")  // do not enter a black callsign
            {
                if (txtStartDate.Text == "")
                {
                    txtStartDate.Text = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ");
                    txtEndDate.Text = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ");

                }
                /*if (cboSubmode.Text.Contains("FT"))
                {
                    if (cboProp.Text == "") cboProp.Text = "FT8";
                }*/
                q.callsign = this.txtCallsign.Text;
                q.ownercallsign = Properties.Settings.Default.MyCall;
                q.band = cboBand.Text;
                q.bandrx = cboRXBand.Text;
                q.country = txtCountry.Text.Replace("'","''");
                if (cboMode.Text.Contains("DATA"))
                {
                    if (cboSubmode.SelectedIndex == -1)
                    {
                        MessageBox.Show("Digital Mode Selection Required");
                        return;
                    }
                    q.mode = cboSubmode.Text;

                }
                else
                {
                    q.mode = cboMode.Text;
                }
                //q.mode = txtK3SubMode.Text;
                q.name = txtName.Text.Replace("'", "''");
                q.start = txtStartDate.Text;
                if (chkContest.Checked && chkwsjtx.Checked)
                {
                    q.rcvd = "R";
                    q.sent = "R";
                }
                else
                {
                    q.rcvd = txtRSTr.Text;
                    q.sent = txtRSTs.Text;
                }

                q.date = txtEndDate.Text;
                q.freq = txtFreq.Text;
                q.freqrx = txtRXFreq.Text;
                q.comment = txtComment.Text.Replace("'", "''");
                q.power = txtPower.Text;
                q.qth = txtQTH.Text.Replace("'", "''");
                q.grid = txtGrid.Text;
                
                q.theoperator = Properties.Settings.Default.MyCall;
                q.propmode = cboProp.Text;
                q.op = callsign;
                if(selectedLoTW.County != null)
                {
                    q.mycnty = selectedLoTW.County.Replace("'", "''");
                }
                else
                {
                    q.mycnty = "";
                }
                q.mycqzone = Convert.ToInt32(selectedLoTW.CQZone);
                q.mydxcc = Properties.Settings.Default.MyDXCC;
                q.myituzone = Convert.ToInt32(selectedLoTW.ITUZone);
                q.mygridsquare = selectedLoTW.Grid;
                q.mystate = selectedLoTW.State;
                
                q.mysiginfo = selectedLoTW.Station;
                //if (cboLoTWStation.Text == "") cboLoTWStation.Text = callsign;
                
                q.dxcc = txtDxcc.Text;
                
                if (chkSatellite.Checked)
                {
                    if (q.band != "2m")
                    {
                        if (q.band != "70cm")
                        {
                            DialogResult result = MessageBox.Show("Satellite Mode is Checked, But you are on HF", "Wrong Prop Mode", MessageBoxButtons.OKCancel);
                            if (result == DialogResult.Cancel) return;
                        }
                        
                    }
                    q.satelliteqso = "1";
                    q.satname = cbosatname.Text;
                    q.satmode = cbosatmode.Text;
                }
                else
                {
                    q.satelliteqso = "0";
                    q.satname = "";
                    q.satmode = "";
                }
                if (q.qsoid == "")
                {
                    q.logQso();
                }
                else
                {
                    q.EditQso(q.qsoid);
                }
                q.qsoid = "";

                clearEntry();

                //-----------------------------
                FillView(LogdataGridView, "");
                String[] tofind = { "gridSquare" };
                myWorkedGrids = GetWorked(tofind, "substr(gridsquare, 1, 4)");
                tofind[0] = "Call";
                myWorkedCalls = GetWorked(tofind, "callsign");
                calling = false;
                btnTX.BackColor = Color.Green;
                btnLog.BackColor = Color.LightGray;
                txtCallsign.Focus();
            }
        }
        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log myQSOs = new Log();
            myQSOs.my_db = my_db;
            myQSOs.Show();
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
        
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportADIF Export = new ExportADIF(my_db);
            Export.Show();

            

        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string adifFile = BrowseFiles();
            
            String adifIn = readADIF(adifFile);
            AdifReader adifRead = new AdifReader(adifIn);
            // get all ADIF Rows from the read file
            List<AdifRow> rows =  adifRead.GetAdifRows();
            int i = 0;
            foreach (AdifRow r in rows)
            {
                AdifToLog(r, i);
                i++;
            }
        }
        
        private void AdifToLog(AdifRow r, int adi_index)
        {
            SQLiteCommand comm;
            string date_mask = "####-##-##";
            string time_mask = "##:##:##Z";
            bool foundmatch = false;

            if (my_db.State == ConnectionState.Closed) my_db.Open();
              
            q.start = r.QSO_DATE.WithMask(date_mask) + " " + r.TIME_ON.WithMask(time_mask);
            if(r.QSO_DATE_OFF == null)
            {
                q.date = q.start;
            }
            else
            {
                q.date = r.QSO_DATE_OFF.WithMask(date_mask) +" "+ r.TIME_OFF.WithMask(time_mask);
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
                        if (ed == q.date)
                        {
                            foundmatch = true;
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
                    q.qsoid = "import " + adi_index.ToString();
                    q.logQso();
                    bool remoteExists = false;
                    foreach (Remotes lotw in LoTWLocations)
                    {
                        if(lotw.Station == q.mysiginfo)
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
                        cboLoTWStation.Items.Clear();
                        // now update the dropdown
                        foreach (Remotes rmt in LoTWLocations)
                        {
                            cboLoTWStation.Items.Add(rmt.Station);
                        }
                    }
                    
                }
                
            }

            
            //q.EditQso(q.qsoid);
            /*if (q.qsoid == "")
            {
                q.logQso();
            }
            else
            {
                q.EditQso(q.qsoid);
            }
            q.qsoid = "";*/
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

        private void writeADIF(List<AdifRow> rows, string filespec)
        {
            ADIFHelper helper = new ADIFHelper();

            string adifstr = helper.ExportAsADIF(rows, filespec);

            
        }

        private void clusterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DXCluster == null)
            {
                DXCluster = new Cluster(sqliteConn);
                DXCluster.QRZLogbook = QRZLogbook;
                DXCluster.SpotClicked += new EventHandler<Cluster.SpotClickedEventArgs>(SpotClicked);
                DXCluster.my_db = my_db;
                DXCluster.callsign = callsign;
                DXCluster.Show();
            }

        }

        private void SpotClicked(object sender, Cluster.SpotClickedEventArgs e)
        {
            SetTextBox(txtCallsign, e.dx, false, Color.White);

            q.qsoid = "";
            string frq = e.FA.ToString("FA00000000.000").Replace(".", "");
            //if (radioButtonRadio1.Checked)
            //{
            if(rig == "K3")
            {
                K3.timerEnable(false);
                K3.send(frq + ";");
                Thread.Sleep(200);
            
                switch (e.MD)
                {
                    case 3:
                        K3.send("MD3;");
                        break;
                    case 1:
                        K3.send("MD1;");
                        break;
                    case 2:
                        K3.send("MD2;");
                        break;
                    case 6:
                        K3.send("MD6;");
                        break;
                    default:
                        K3.send("MD3;");
                        break;
                }
                K3.timerEnable(true);
            }
            else
            {
                // OmniRig would go here
                
                /*K3_Remote.timerEnable(false);
                K3_Remote.send(frq + ";");
                Thread.Sleep(200);
                switch (e.MD)
                {
                    case 3:
                        K3_Remote.send("MD3;");
                        break;
                    case 1:
                        K3_Remote.send("MD1;");
                        break;
                    case 2:
                        K3_Remote.send("MD2;");
                        break;
                    case 6:
                        K3_Remote.send("MD6;");
                        break;
                    default:
                        K3_Remote.send("MD3;");
                        break;
                }
                K3_Remote.timerEnable(true);*/
            }


            QRZFillIn();
        }

        private void QRZFillIn()
        {
            if (!BlockX) FillView(LogdataGridView, txtCallsign.Text);// don't fill if we are set for Recent QSOs
            this.InvokeAndClose((MethodInvoker)delegate
            {
                if (QRZLogbook.loggedIn)
                {
                    lblQRZKeyStatus.Text = "QRZ Logbook Connected";
                    lblQRZKeyStatus.BackColor = Color.LightGreen;

                    if (QRZLogbook.Lookup(txtCallsign.Text))
                    {
                        //SetTextBox(txtName, QRZLogbook.fname, false, Color.White);
                        txtName.Text = QRZLogbook.fname;
                        Color postcolor = Color.White;
                        if (wsjtMode) postcolor = Color.Yellow;
                        //SetTextBox(txtGrid, QRZLogbook.grid, false, postcolor);
                        txtGrid.Text = QRZLogbook.grid;
                        //SetTextBox(txtCountry, QRZLogbook.country, false, Color.White);
                        txtCountry.Text = QRZLogbook.country;
                        if (QRZLogbook.addr2.Contains(","))
                        {
                            if (QRZLogbook.addr1 == "")
                            {
                                String[] qth = QRZLogbook.addr2.Split(',');
                                if (QRZLogbook.state == "")
                                {
                                    //SetTextBox(txtQTH, qth[1], false, Color.White);
                                    txtQTH.Text = qth[1];
                                }
                                else
                                {
                                    //SetTextBox(txtQTH, qth[1] + ", " + QRZLogbook.state, false, Color.White);
                                    txtQTH.Text = qth[1] + ", " + QRZLogbook.state;
                                }

                            }

                        }
                        else
                        {
                            if (QRZLogbook.state == "")
                            {
                                //SetTextBox(txtQTH, QRZLogbook.addr2, false, Color.White);
                                txtQTH.Text = QRZLogbook.addr2;
                            }
                            else
                            {
                                //SetTextBox(txtQTH, QRZLogbook.addr2 + ", " + QRZLogbook.state, false, Color.White);
                                txtQTH.Text = QRZLogbook.addr2 + ", " + QRZLogbook.state;
                            }

                        }
                        q.dxcc = QRZLogbook.dxcc;
                        //SetTextBox(txtDxcc, q.dxcc, false, Color.White);
                        txtDxcc.Text = q.dxcc;
                        //q.ituzone = QRZLogbook.ituzone;
                        //q.cqzone = QRZLogbook.cqzone;
                        SetRST();

                    }
                }
                else
                {
                    lblQRZKeyStatus.Text = "QRZ Logbook Not Avaliable";
                    lblQRZKeyStatus.BackColor = Color.Red;
                    // try to get key agn
                    int i = 0;
                    do
                    {
                        i++;
                        if (QRZLogbook.getkey()) QRZFillIn();
                    } while (i < 1);
                    
                }
                
            });
        }
        private void SetRST()
        {
            switch (cboMode.Text)
            {
                case "AM":
                case "FM":
                case "LSB":
                case "USB":
                    txtRSTr.Text = "59";
                    txtRSTs.Text = "59";
                    break;
                case "CW":
                case "CW-R":
                    txtRSTr.Text = "599";
                    txtRSTs.Text = "599";
                    break;
                case "DATA":
                case "DATA-R":
                    if (!chkwsjtx.Checked)
                    {
                        txtRSTr.Text = "-00";
                        txtRSTs.Text = "-00";
                    }
                    break;

            }
        }

        private void txtCallsign_Leave(object sender, EventArgs e)
        {
            string[] thiscall = getcountry(txtCallsign.Text, txtFreq.Text, txtRadio1Mode.Text);
            string ctry = thiscall[0];
            string dxcc = thiscall[1];
            txtCountry.Text = ctry;
            txtDxcc.Text = dxcc;
            int worked = checkworked(ctry, txtFreq.Text, txtRadio1Mode.Text); 
            // returns (0 = worked band and mode), (1 = need band), (2 = need mode), (3 = need mode and band), (4 = ATNO)
            switch (worked)
            {
                case 0:
                    lblATNO.Text = "Worked";
                    lblATNO.BackColor = Color.AliceBlue;
                    break;
                case 1:
                    lblATNO.Text = "New Band";
                    lblATNO.BackColor = Color.Beige;
                    break;
                case 2:
                    lblATNO.Text = "New Mode";
                    lblATNO.BackColor = Color.Aqua;
                    break;
                case 3:
                    lblATNO.Text = "New Band";
                    lblATNO.BackColor = Color.Aquamarine;
                    break;
                case 4:
                    lblATNO.Text = "ANTO";
                    lblATNO.BackColor = Color.Red;
                    break;
            }
            if (DXCluster != null)
            {
                DXCluster.spotcall = txtCallsign.Text;
                double sfreq = Convert.ToDouble(txtFreq.Text) / 1000.0;
                DXCluster.spotfreq = sfreq.ToString();
            }
            QRZFillIn();
        }

        private void txtDate_Enter(object sender, EventArgs e)
        {
            if (txtStartDate.Text == "")
            {
                txtStartDate.Text = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ");
                txtEndDate.Text = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ");
                newQsoId = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff");
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearEntry();
            btnLog.BackColor = Color.LightGray;
            if (!BlockX) FillView(LogdataGridView, txtCallsign.Text);// don't fill if we are set for Recent QSOs
        }
        private void clearEntry()
        {
            txtCallsign.Text = "";
            txtComment.Text = "";
            txtGrid.Text = "";
            txtName.Text = "";
            txtQTH.Text = "";
            txtCountry.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtRSTr.Text = "";
            txtRSTs.Text = "";
            switch (cboBand.Text)
            {
                case "70cm":
                    txtPower.Text = "10";
                    break;
                case "2m":
                    txtPower.Text = "1000";
                    if (chkSatellite.Checked) txtPower.Text = "50";
                    break;
                case "30m":
                    txtPower.Text = "200";
                    break;
                default:
                    txtPower.Text = "500";
                    if (cboLoTWStation.Text == "N5TM/W7DXX") txtPower.Text = "1500";
                    break;
            }
            //editing = false;
            q.qsoid = "";
            //cboOperator.Text = "";
            cboProp.Text = "";
            cboLoTWStation.Text = callsign;
            lblATNO.Text = "";
            lblATNO.BackColor = Color.White;
            if (chkSatellite.Checked)
            {
                cboProp.Text = "SAT";
            }
            else
            {
                cboProp.Text = "";
                cbosatmode.Text = "";
                cbosatname.Text = "";
            }
        }

        private void LogdataGridView_SelectionChanged(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in LogdataGridView.SelectedRows)
            {
                String QsoId = row.Cells["qsoId"].Value.ToString();
                q.qsoid = QsoId;
                if (q.ReadQso(QsoId))
                {
                    txtCallsign.Text = q.callsign;
                    txtStartDate.Text = q.start;
                    txtComment.Text = q.comment;
                    txtCountry.Text = q.country;
                    txtFreq.Text = q.freq;
                    txtRXFreq.Text = q.freqrx;
                    txtEndDate.Text = q.date;
                    txtRSTr.Text = q.rcvd;
                    txtRSTs.Text = q.sent;
                    txtName.Text = q.name;
                    txtGrid.Text = q.grid;
                    txtPower.Text = q.power;
                    txtQTH.Text = q.qth;
                    cboBand.Text = q.band;
                    cboRXBand.Text = q.bandrx;
                    cboMode.Text = q.mode;
                    // if this is DATA mode, fill submode
                    foreach (string m in cboSubmode.Items)
                    {
                        if (m == q.mode)
                        {
                            cboSubmode.Text = q.mode;
                            cboMode.Text = "DATA";
                            break;
                        }
                    }
                    string lotwRemote = "";
                    if (q.theoperator.Contains("/")) // legacy was using operator to define remotes
                    {                                // now using mysiginfo in database
                        lotwRemote = q.theoperator.Split('/')[1];
                    }
                    else
                    {   // if mysiginfo is not set, assume home station Lotw[0]
                        if (q.mysiginfo == "" || q.mysiginfo == "0") lotwRemote = LoTWLocations[0].Station;
                        else lotwRemote = q.mysiginfo;
                    }
                    cboLoTWStation.Text = lotwRemote;
                    txtRadio1SubMode.Text = q.mode;
                    txtPower.Text = q.power;
                    cboProp.Text = q.propmode;
                    txtDxcc.Text = q.dxcc;
                    if (q.satelliteqso == "1") chkSatellite.Checked = true;
                    else chkSatellite.Checked = false;
                    cbosatmode.Text = q.satmode;
                    cbosatname.Text = q.satname;
                    //editing = true;
                }
            }

        }

        private void btnQRZ_Click(object sender, EventArgs e)
        {
            QRZFillIn();
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            if (btnAll.BackColor != Color.LightGray)
            {
                btnAll.BackColor = Color.LightGray;
                BlockX = false;
            }
            else
            {
                btnAll.BackColor = Color.Red;
                BlockX = true;
                FillView(LogdataGridView, "");
            }

        }

        private void txtCallsign_TextChanged(object sender, EventArgs e)
        {
            //if (txtCallsign.Text.Length > 2) FillView(LogdataGridView, txtCallsign.Text);// don't fill if we are set for Recent QSOs
        }



        private void VFOA_MouseHover(object sender, EventArgs e)
        {
            VFOA_Radio1.Focus();
        }

        private void VFOA_MouseWheel(object sender, MouseEventArgs e)
        {

            if (rig == "OmniRig1")
            {
                double fa = MyOmniRadio1.args.FA;
                if (e.Delta > 0)
                {
                    fa += 10;
                }
                else
                {
                    fa -= 10;
                }
                //string frq = fa.ToString("FA00000000.000").Replace(".", "");
                MyOmniRadio1.SetFreq(Convert.ToInt32(fa));
            }
            else
            {
                
                if (e.Delta > 0)
                {
                    K3.send("UP2;");
                    
                }
                else
                {
                    K3.send("DN2;");
                    
                }
               
            }
                


        }
        private void VFOB_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                K3.send("UPB2;");
                
            }
            else
            {
                K3.send("DNB2;");
                
            }


        }
        private double ConvertToDouble(string Value)
        {
            if (Value == null)
            {
                return 0;
            }
            else
            {
                double OutVal;
                double.TryParse(Value, out OutVal);

                if (double.IsNaN(OutVal) || double.IsInfinity(OutVal))
                {
                    return 0;
                }
                return OutVal;
            }
        }

        private void VFOB_MouseHover(object sender, EventArgs e)
        {
            VFOB_Radio1.Focus();
        }

        private void btnAtoB_Click(object sender, EventArgs e)
        {
            Double fa = ConvertToDouble(K3.FA);
            string frq = fa.ToString("FB00000000.000").Replace(".", "");
            K3.send(frq + ";");
        }

        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (q.qsoid == "")
            {
                switch (cboMode.SelectedIndex)
                {
                    case 1:  //LSB
                    case 2:  //USB
                    case 4:  // FM
                    case 5:  // AM
                        SetTextBox(txtPower, "500", false, Color.White);
                        SetTextBox(txtRSTr, "59", false, Color.White);
                        SetTextBox(txtRSTs, "59", false, Color.White);
                        SetComboBox(cboSubmode, 0);
                        break;
                    case 3:  // CW
                    case 7:  // CW-R
                        SetTextBox(txtPower, "500", false, Color.White);
                        SetTextBox(txtRSTr, "599", false, Color.White);
                        SetTextBox(txtRSTs, "599", false, Color.White);
                        SetComboBox(cboSubmode, 0);
                        break;

                    case 6:  //DATA
                    case 9: // DATA-R
                        if (cboBand.Text == "2m")
                        {
                            SetTextBox(txtPower, "1000", false, Color.White);
                        }
                        else
                        {
                            SetTextBox(txtPower, "500", false, Color.White);
                        }
                        SetTextBox(txtRSTr, "-20", false, Color.White);
                        SetTextBox(txtRSTs, "-20", false, Color.White);
                        break;

                    default:
                        break;
                }
            }

        }

        private void LogdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cboBand_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.Rig == "None")
            {
                string[] freqreslolve = { "137", "472", "1800", "3500", "5200", "7000", "10100", "14000", "18700", "21000", "24500", "28000", "50000", "144000", "222000", "432000", "1296000" };
                txtFreq.Text = freqreslolve[cboBand.SelectedIndex];
            }

            switch (cboBand.Text)
            {
                case "630m":
                    SetTextBox(txtPower, "150", false, Color.White);
                    
                    break;
                case "70cm":
                    SetTextBox(txtPower, "10", false, Color.White);
                    break;
                case "2m":
                    if(cboProp.Text == "SAT")
                    {
                        SetTextBox(txtPower, "50", false, Color.White);
                    }
                    else
                    {
                        SetTextBox(txtPower, "1000", false, Color.White);
                    }
                    
                    break;
                default:
                    SetTextBox(txtPower, "500", false, Color.White);
                    
                    break;

            }
            
            WorkedBox.Text = "";
            String[] tofind = { "gridsquare" };
            myWorkedGrids = GetWorked(tofind, "substr(gridsquare, 1, 4)");
            tofind[0] = "callsign";
            myWorkedCalls = GetWorked(tofind, "callsign");
            // no spots to Slack VHF-Chat if not on VHF Band
            if (cboBand.Text != "6m" || cboBand.Text != "2m") chkSpot.Checked = false;
        }

        private void radioButtonRemote2_CheckedChanged(object sender, EventArgs e)
        {
            /*if (!radioButtonLocal.Checked)
            {
                if (radioButtonRemote2.Checked)
                {
                    K3_Remote.Port = "COM4";  // dxmate
                }
                else
                {
                    K3_Remote.Port = "COM9";   // RHR
                }
            }*/

        }

        private void button_CheckChanged(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;
            if (r.Checked)
            {
                radioport = Convert.ToInt16(r.Tag);
                K3.PrimaryPort = comport[radioport];
                if (K3.InitSerialPort())
                {
                    K3.K3Rcvd += new EventHandler<K3Log.RadioCAT.DatarcvdEventArgs>(newK3Data);
                }
                else
                {
                    MessageBox.Show("Could not open K3 Port");
                    //System.Windows.Forms.Application.Exit();
                }
                if (!K3.InitSerialPort()) r.ForeColor = Color.Red;
            }
            else
            {
                K3.ClosePort();
            }
        }

        private void buttonMouse_Hover(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(r, comport[Convert.ToInt16(r.Tag)]);
        }



        private void txtCalling_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

            }
        }

        private void btnTX_Click(object sender, EventArgs e)
        {
            calling = false;
            btnTX.BackColor = Color.Green;

        }

        private void cboSubmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBand.Text == "2m" || cboBand.Text == "6m")
            {
                if (cboSubmode.Text == "FT8") cboProp.Text = "ES";
                if (cboSubmode.Text == "MSK144" || cboSubmode.Text == "PSK2K") cboProp.Text = "MS";
                if (cboSubmode.Text == "JT65B") cboProp.Text = "EME";
            }
        }

        private void dXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Countries ctry = new Countries();
            ctry.Show();
        }

        private void chkContest_CheckedChanged(object sender, EventArgs e)
        {
            if (chkContest.Checked)
            {
                contestMode = true;
            }
            else
            {
                contestMode = false;
            }
        }

        private void chkwsjtx_CheckedChanged(object sender, EventArgs e)
        {
            if (chkwsjtx.Checked)
            {
                wsjtMode = true;
                q.qsoid = "";
            }
            else
            {
                wsjtMode = false;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            searchstring = txtSearch.Text;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (q.qsoid != "")
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show("Delete this QSO?", "", buttons);
                if (result == DialogResult.Yes)
                {
                    logbook.Delete(q.qsoid);
                }
                FillView(LogdataGridView, txtCallsign.Text);
            }
        }

        private void BtnMacro0_Click(object sender, EventArgs e)
        {
            BtnMacroEventHandler(Convert.ToInt32((sender as Button).Tag));
                        
        }

        private void BtnMacroEventHandler(int btn)
        {
            macro myMacro;
            if (btn >= 112)  // is this a button TAG or a shortcut function key?
            {
                myMacro = macroList.Find(x => x.macroShortCut == btn);
            }
            else
            {
                myMacro = macroList.Find(x => x.number == btn);
            }

            //SendBtn.BackColor = Color.Red;
            string tout = myMacro.macroAction.Replace("!", txtCallsign.Text).Replace("*", callsign);
            if (chkSendF2OnRTN.Checked && btn == 1) tout = txtCallsign.Text + " " + tout;
            //textOut.Text += tout;
            sendKYOut(tout, 0, true);
            //lastTextOut += tout.Length;
            //textOut.Text += myMacro.macroAction.Replace("!", txtCallsign.Text).Replace("*", callsign);
            //sendKYOut(textOut.Text);
            //K3.send("RX;");
            
            //SendBtn.BackColor = Color.Green;
        }

        private void BtnMacro0_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                MacroMsgs mac = new MacroMsgs();
                mac.MacUpdate += new EventHandler<MacroMsgs.MacroButtonEventArgs>(MacroUpdated);
                macro myMacro = macroList.Find(x => x.number == Convert.ToInt32((sender as Button).Tag));
                if (myMacro == null)
                {
                    myMacro = new macro();
                    myMacro.macroName = (sender as Button).Text;
                    myMacro.btnLabel = (sender as Button).Text;
                    myMacro.number = Convert.ToInt16((sender as Button).Tag);
                    myMacro.macroShortCut = myMacro.number + 112;
                }
                mac.thisMacro = myMacro;

                mac.Show();
            }
        }

        private void MacroUpdated(object sender, MacroMsgs.MacroButtonEventArgs e)
        {
            macro myMacro = macroList.Find(x => x.number == e.mac.number);
            if (myMacro != null)
            {
                myMacro.macroAction = e.mac.macroAction;
                myMacro.macroName = e.mac.macroName;
                myMacro.btnLabel = e.mac.btnLabel;
                myMacro.macroShortCut = e.mac.macroShortCut;

            }
            else
            {   // add new macro definition

                macro newMacro = new macro();
                newMacro.number = e.mac.number;
                newMacro.btnLabel = e.mac.btnLabel;
                newMacro.macroAction = e.mac.macroAction;
                newMacro.macroName = e.mac.macroName;
                newMacro.macroShortCut = e.mac.macroShortCut;
                macroList.Add(newMacro);
            }
            WriteXML("macros.xml");
            foreach (macro m in macroList)
            {
                Button btn = MacroButtons.Find(x => Convert.ToInt16(x.Tag) == m.number);
                if (btn != null) btn.Text = m.btnLabel;
            }
        }

        private void WriteXML(string fspec)
        {
            XDocument xmlDoc = new XDocument(new XElement("macros"));

            foreach (K3Log.macro m in macroList)
            {
                xmlDoc.Root.Add(new XElement("number", m.number.ToString(),
                    new XAttribute("shortcut", m.macroShortCut),
                    new XAttribute("macroName", m.macroName),
                    new XAttribute("btnLabel", m.btnLabel),
                    new XAttribute("macroAction", m.macroAction)));

            }
            xmlDoc.Save(fspec);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode >= 112 && (int)e.KeyCode < 120) BtnMacroEventHandler((int)e.KeyCode - 112);
            if ((int)e.KeyCode == 27)  // abort KY TX
            {
                if (chkUseKY.Checked)
                {
                    sendKYOut("@", 0, true);
                    K3.send("RX;");
                    SendBtn.BackColor = Color.Green;
                }
                else
                {
                    keyer.stop();
                    SendBtn.BackColor = Color.Green;
                }
            }
            if((int)e.KeyCode == 13)
            {
                if (txtCallsign.ContainsFocus)
                {
                    if (chkSendF2OnRTN.Checked) BtnMacroEventHandler(1);
                }
                else
                {
                    if(chkSendF2OnRTN.Checked) BtnMacroEventHandler(2);
                    logThisQ();
                }
                
            }
        }

        private void btnUp2_Click(object sender, EventArgs e)
        {
            K3.send("SWT13;SWT13;FT1;UPB5;RTO;XTO;");
        }

        private void chkSatellite_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSatellite.Checked)
            {
                cboProp.SelectedIndex = 0;
            }
            else
            {
                cboProp.SelectedIndex = -1;
                cbosatmode.Text = "";
                cbosatname.Text = "";
                chkDoppler.Checked = false;
                chkDopplerDown.Checked = false;
            }
        }

        private void chkLink_CheckedChanged(object sender, EventArgs e)
        {
            if (cboFactor.SelectedIndex == -1) cboFactor.SelectedIndex = 5;
        }

        private void cboFactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFactor.SelectedIndex == -1) cboFactor.SelectedIndex = 3;
            deltaHz = Convert.ToDouble(cboFactor.Text);
        }

        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Properties.Settings.Default.SQLiteFile; //@"E:Documents\N5TM_DB\";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Select SQLite Database";
            openFileDialog1.DefaultExt = "SQLite";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                myDB = openFileDialog1.FileName;
                Properties.Settings.Default.SQLiteFile = myDB;
                Properties.Settings.Default.Save();
            }

        }

        private void macrosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void communicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogSettings MySettings = new LogSettings();
            MySettings.LogSettingsChanged += new EventHandler<LogSettings.LogSettingsEventArgs>(newSetting);
            MySettings.K3 = K3;
            MySettings.Show();


        }
        private void newSetting(object sender, LogSettings.LogSettingsEventArgs e)
        {
            if(e.Comport1 != K3.PrimaryPort)
            {
                K3.ClosePort();
                K3 = new RadioCAT(1, e.Comport1, e.Baud1);
                K3.PrimaryPort = e.Comport1;
                K3.VPort = e.MirrorPort;
                K3.InitSerialPort();
            }
        }

        private void gridsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DecodedGrids = new GridForm();
                if (myWorkedGrids == null) 
                {
                    String[] tofind = { "gridSquare" };
                    myWorkedGrids = GetWorked(tofind, "substr(gridsquare, 1, 4)");
                }

                DecodedGrids.myWorkedGrids = myWorkedGrids;
                DecodedGrids.Show();
            }
            catch (Exception)
            {

                //throw;
            }
            {
                
                
            }
            
        }

        private void txtCWSpeed_TextChanged(object sender, EventArgs e)
        {
            if (txtCWSpeed.Text.IsNumeric())
            {
                byte[] bb = BitConverter.GetBytes(Convert.ToInt32(txtCWSpeed.Text));
                //keyer.setspeed(bb[0]);
            }
        }

        private void txtCWSpeed_Enter(object sender, EventArgs e)
        {
            keyer.requestWPM();
        }

        private void WKConnected_Click(object sender, EventArgs e)
        {
            
            if (WKConnect())  // Connect to WK
            {
                lblWKConnected.BackColor = Color.LightGreen;
                lblWKConnected.Text = "WK Connected";
            }
            else
            {
                lblWKConnected.BackColor = Color.Pink;
                lblWKConnected.Text = "WK Not Connected";
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            clearEntry();
            btnLog.BackColor = Color.LightGray;
            if (!BlockX) FillView(LogdataGridView, txtCallsign.Text);// don't fill if we are set for Recent QSOs
        }

        private void cbosatmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbosatmode.Text != "VU") TXRadio = 2;
            else TXRadio = 1;
        }

        private void cboOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLoTW = LoTWLocations[cboLoTWStation.SelectedIndex];
            this.Text = "K3Log  ( Using Station: " + selectedLoTW.Station + ", " +
                selectedLoTW.Grid + ", " + selectedLoTW.County + " County, " + selectedLoTW.State + " )";
            switch (cboLoTWStation.Text)
            {
                case "N5TM":
                    txtPower.Text = "500";
                    if (cboBand.Text == "2m") txtPower.Text = "1000";
                    if (cboBand.Text == "30m") txtPower.Text = "200";
                    if (cboBand.Text == "630m") txtPower.Text = "150";
                    break;
                case "W7DXX":
                    txtPower.Text = "1500";
                    if (cboBand.Text == "30m") txtPower.Text = "200";
                    break;
                default:
                    break;
            }
            if (cboLoTWStation.SelectedIndex > 0) cboLoTWStation.BackColor = Color.Red;
            else cboLoTWStation.BackColor = Color.White;
        }

        private int getsatOpMode(string MD)
        {
            int ret = 0;
            switch (MD)
            {
                case "CW":
                    ret = 3;
                    break;
                case "USB":
                    ret = 2;
                    break;
                case "LSB":
                    ret = 1;
                    break;
                case "FM":
                    ret = 4;
                    break;
                case "AM":
                    ret = 5;
                    break;
                case "DATA":
                    ret = 6;
                    break;
                case "CW-R":
                    ret = 7;
                    break;
                case "DATA-R":
                    ret = 9;
                    break;
                default:
                    ret = 3;
                    break;

            }
            return ret;
        }

        private void cbosatname_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSatellite = mySats.mySatellites[cbosatname.SelectedIndex];
            cbosatmode.Text = selectedSatellite.Mode;
            chkDoppler.Checked = false;
            chkDopplerDown.Checked = false;
            chkInvertRXDplr.Checked = selectedSatellite.Invert;
            lblPL.Text = "PL:" + selectedSatellite.PLTone;

            if (selectedSatellite.Mode != "VU")
            {
                string frq = selectedSatellite.Downlink.ToString("FA00000000.000").Replace(".", "");
                K3.send(frq + ";");
                Thread.Sleep(200);
                int md = getsatOpMode(selectedSatellite.DownMode);
                K3.send("MD" + md.ToString() + ";");
                MyOmniRadio2.SetFreq(Convert.ToInt32(selectedSatellite.Uplink*1000));
                MyOmniRadio2.SetMode(selectedSatellite.UpMode);
                txtPower.Text = "10";
            }
            else
            {
                string frq = selectedSatellite.Uplink.ToString("FA00000000.000").Replace(".", "");
                K3.send(frq + ";");
                Thread.Sleep(200);
                int md = getsatOpMode(selectedSatellite.UpMode);
                K3.send("MD" + md.ToString() + ";");
                MyOmniRadio2.SetFreq(Convert.ToInt32(selectedSatellite.Downlink * 1000));
                MyOmniRadio2.SetMode(selectedSatellite.DownMode);
                txtPower.Text = "50";
            }
            
            Thread.Sleep(200);
            satTrack.selectSatellite(cbosatname.Text);
        }
        
        private void newSatPosition(object sender, SatTrackEventArgs e)
        {
            this.InvokeAndClose((MethodInvoker)delegate
            {
                Color satvisible = Color.Green;
                if (e.position.Contains("-")) satvisible = Color.Red;
                //SetTextBox(txtSatPosition, e.satellite + ": " + e.position, false, satvisible);
                txtSatPosition.Text = e.satellite + ": " + e.position;
                txtSatPosition.BackColor = satvisible;
                //SetTextBox(txtRangeRate, e.rangerate.ToString(), false, Color.White);
                txtRangeRate.Text = e.rangerate.ToString();
                //SetTextBox(txtdopper, e.doppler.ToString(), false, Color.White);
                txtdopper.Text = e.doppler.ToString();
                if (chkDoppler.Checked)
                {
                
                    if (TXRadio == 1)
                    {
                        K3.Doppler = Convert.ToInt32(e.doppler  * selectedSatellite.UpDoppler);
                        if (chkDopplerDown.Checked)
                        {
                            MyOmniRadio2.Doppler = Convert.ToInt32(e.doppler * selectedSatellite.DownDoppler);
                        }
                    }
                    else
                    {
                        MyOmniRadio2.Doppler = Convert.ToInt32(e.doppler * selectedSatellite.UpDoppler);
                        if (chkDopplerDown.Checked)
                        {
                            K3.Doppler = Convert.ToInt32(e.doppler * selectedSatellite.DownDoppler);
                        }
                    
                    }
                
                }
            });
        }
        private void chkDoppler_CheckedChanged(object sender, EventArgs e)
        {
            if(chkDoppler.Checked == true)
            {
                if(cbosatmode.Text == "VU")
                {
                    if (chkDoppler.Checked) K3.enableDoppler = true;
                    else K3.enableDoppler = false;
                    //K3.Doppler =  1;
                }
                else
                {
                    if (chkDoppler.Checked) MyOmniRadio2.enableDoppler = true;
                    else MyOmniRadio2.enableDoppler = false;
                }
            }
            else
            {
                K3.enableDoppler = false;
                MyOmniRadio2.enableDoppler = false;
            }
            
            
        }

        private void cboFactor_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            linkFactor = Convert.ToDouble(cboFactor.Text);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.InvokeAndClose((MethodInvoker)delegate
            {
                K3.ClosePort(); 
                K3 = null;
                Environment.Exit(Environment.ExitCode);
            });
        }

        private void chkDopplerDown_CheckedChanged(object sender, EventArgs e)
        {
            if (cbosatmode.Text != "VU")
            {
                if (chkDopplerDown.Checked && chkDoppler.Checked)
                {
                    K3.enableDoppler = true;
                    if (chkInvertRXDplr.Checked) K3.invertRXDoppler = true;
                    else K3.invertRXDoppler = false;
                }
                else K3.enableDoppler = false;
                //K3.Doppler =  1;
            }
            else
            {

                if (chkDopplerDown.Checked && chkDoppler.Checked)
                {
                    MyOmniRadio2.enableDoppler = true;
                    if (chkInvertRXDplr.Checked) MyOmniRadio2.invertRXDoppler = true;
                    else MyOmniRadio2.invertRXDoppler = false;
                }
                else MyOmniRadio2.enableDoppler = false;
            }

        }

        private void rdoWK1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoWK1.Checked) keyer.setOutput(0);
            else keyer.setOutput(1);
        }
    }

    public class wsjtXmsg
    {

        public string grid = "";
        public string callsign = "";
        public string extra = "";
        public string called = "";
        Regex gridReg = new Regex("[A-Z][A-Z][0-9][0-9]");
        public wsjtXmsg(string Xmsg)
        {
            string[] s = Xmsg.Split(' ');
            switch (s.Length)
            {
                case 2:
                    this.callsign = s[1];
                    this.called = s[0];
                    this.grid = "nil";
                    break;
                case 3:
                    this.callsign = s[1];
                    this.called = s[0];
                    if (s[2].Length == 4 && s[2] != "RR73" && !s[2].Contains("-") && !s[2].Contains("+")) this.grid = s[2];
                    if (this.grid != "" && gridReg.Matches(s[2]).Count == 0)
                    {
                        this.grid = "";
                    }
                    break;
                case 4:
                    if (s[3].Length == 4) this.grid = s[3];
                    if (this.grid != "" && gridReg.Matches(s[3]).Count == 0)
                    {
                        this.grid = "";
                    }
                    this.callsign = s[2];
                    this.extra = s[1];
                    this.called = s[0];
                    break;
                default:
                    break;
            }
        }


    }
    
        
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
    public static class ExtensionMethods
    {
        public static void InvokeAndClose(this Control self, MethodInvoker func)
        {
            IAsyncResult result = self.BeginInvoke(func);
            self.EndInvoke(result);
            result.AsyncWaitHandle.Close();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }
        public static string WithMask(this string s, string mask)
        {
            var slen = Math.Min(s.Length, mask.Length);
            var charArray = new char[mask.Length];
            var sPos = s.Length - 1;
            for (var i = mask.Length - 1; i >= 0 && sPos >= 0;)
                if (mask[i] == '#') charArray[i--] = s[sPos--];
                else
                    charArray[i] = mask[i--];
            return new string(charArray);
        }
        
    }
    public static class StringExt
    {
        public static bool IsNumeric(this string text) => double.TryParse(text, out _);

    }
    
}

