using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K3Log
{
    public partial class LogSettings : Form
    {
        public RadioCAT K3;
        public LogSettingsEventArgs args = new LogSettingsEventArgs();
        public bool disposed { get; set; }

        public LogSettings()
        {
            InitializeComponent();
        }

        public class LogSettingsEventArgs : EventArgs
        {
            public string Radio1 { get; set; }
            public string Radio2 { get; set; }
            public string Comport1 { get; set; }
            public string Comport2 { get; set; }
            public int Baud1 { get; set; }
            public int Baud2 { get; set; }
            public string MirrorPort { get; set; }
        }
        public event EventHandler<LogSettingsEventArgs> LogSettingsChanged;
        protected virtual void OnK3Rcvd(object sender, LogSettingsEventArgs e)
        {
            if (LogSettingsChanged != null)
            {
                if (!disposed) LogSettingsChanged(this, e);
            }
        }
        private void Settings_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            
            //var sortedPorts = ports.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty)));
            foreach (string port in ports)
            {
                cboPorts.Items.Add(port);
                cboWKComport.Items.Add(port);
                cboVirtPort.Items.Add(port);
            }
            txtMyGrid.Text = Properties.Settings.Default.MyGrid;
            txtMyCall.Text = Properties.Settings.Default.MyCall;
            cboPorts.Text = Properties.Settings.Default.Port;
            cboVirtPort.Text = Properties.Settings.Default.VirtualPort;
            cboRig.Text = Properties.Settings.Default.Rig;
            cboRig2.Text = Properties.Settings.Default.Rig2;
            txtBaud.Text = Properties.Settings.Default.Baud.ToString();
            txtVirtBaud.Text = Properties.Settings.Default.VirtualBaud.ToString();
            txtSMSNo.Text = Properties.Settings.Default.SMSNumber;
            txtNoCallGrids.Text = Properties.Settings.Default.NoCallGrids;
            txtBandPlan.Text = Properties.Settings.Default.BandPlan;
            txtCountryList.Text = Properties.Settings.Default.CountryList;
            txtSQLiteFile.Text = Properties.Settings.Default.SQLiteFile;
            txtQRZPwd.Text = Properties.Settings.Default.QRZPwd;
            txtQRZLogin.Text = Properties.Settings.Default.QRZLogin;
            chkWKEnable.Checked = Properties.Settings.Default.WKeyer;
            chkAutoSpace.Checked = Properties.Settings.Default.WKAutoSpace;
            chkCTSpace.Checked = Properties.Settings.Default.WKCTSpace;
            chkWKSwapPdl.Checked = Properties.Settings.Default.WKRevPdl;
            chkWKSideTone.Checked = Properties.Settings.Default.WKSidetone;
            cboPdlMode.SelectedIndex = Properties.Settings.Default.WKPdlMode;
            cboWKComport.Text = Properties.Settings.Default.WKComport;
            cboSideTone.SelectedIndex = Properties.Settings.Default.WKSToneFreq;
            txtMyCnty.Text = Properties.Settings.Default.MyCnty;
            txtMyName.Text = Properties.Settings.Default.MyName;
            txtMyDXCC.Text = Properties.Settings.Default.MyDXCC.ToString();
            txtMyContinent.Text = Properties.Settings.Default.MyContinent;
            txtMyCQZone.Text = Properties.Settings.Default.MyCQZone.ToString();
            txtMyITUZone.Text = Properties.Settings.Default.MyITUZone.ToString();
            if (Properties.Settings.Default.WKOutport == 0)
            {
                rdoWKOutput1.Checked = true;
            }
            else
            {
                rdoWKOutput2.Checked = true;
            }

            loadRemoteGrid();
            loadSatellitesGrid();
            loadBandPwrGrid();
        }
        
        private void loadRemoteGrid()
        {
            string serializeList = Properties.Settings.Default.LoTW;
            List<Remotes> myRemotes = new List<Remotes>();
            
            myRemotes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Remotes>>(serializeList);
            if(myRemotes != null)
            {
                foreach (Remotes lotw in myRemotes)
                {
                    RemotesdataGridView.Rows.Add(lotw.Station, lotw.Grid,
                        lotw.ITUZone, lotw.CQZone, lotw.County, lotw.State);
                    
                }
            }
            
        }

        private void loadSatellitesGrid()
        {
            try
            {
                foreach (Satellites sat in Properties.Settings.Default.Satellites.mySatellites)
                {
                    SatellitesDataGridView.Rows.Add(sat.Satellite, sat.Uplink,
                        sat.Downlink, sat.Mode, sat.UpMode, sat.DownMode, sat.PLTone,
                        sat.UpDoppler, sat.DownDoppler, sat.Invert);

                }
            }
            catch (Exception)
            {

                //throw;
            }
                
            
        }

        private void loadBandPwrGrid()
        {
            try
            {
                foreach (BandPower band in Properties.Settings.Default.BandPower.myBandPower)
                {
                    BandPwrDataGridView.Rows.Add(band.Band, band.Power, band.Antenna, band.PropMode);
                }
            }
            catch (Exception)
            {

                //throw;
            }
                
            
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.MyCall = txtMyCall.Text;
            Properties.Settings.Default.MyGrid = txtMyGrid.Text;
            Properties.Settings.Default.MyName = txtMyName.Text;
            Properties.Settings.Default.Rig = cboRig.Text;
            Properties.Settings.Default.Rig2 = cboRig2.Text;
            Properties.Settings.Default.Port = cboPorts.Text;
            Properties.Settings.Default.VirtualPort = cboVirtPort.Text;
            Properties.Settings.Default.Baud = Convert.ToInt32(txtBaud.Text);
            Properties.Settings.Default.VirtualBaud = Convert.ToInt32(txtVirtBaud.Text);
            Properties.Settings.Default.SMSNumber = txtSMSNo.Text;
            Properties.Settings.Default.NoCallGrids = txtNoCallGrids.Text;
            Properties.Settings.Default.BandPlan = txtBandPlan.Text;
            Properties.Settings.Default.CountryList = txtCountryList.Text;
            Properties.Settings.Default.SQLiteFile = txtSQLiteFile.Text;
            Properties.Settings.Default.QRZLogin = txtQRZLogin.Text;
            Properties.Settings.Default.QRZPwd = txtQRZPwd.Text;
            Properties.Settings.Default.WKAutoSpace = chkAutoSpace.Checked;
            Properties.Settings.Default.WKComport = cboWKComport.Text;
            Properties.Settings.Default.WKCTSpace = chkCTSpace.Checked;
            Properties.Settings.Default.WKeyer = chkWKEnable.Checked;
            Properties.Settings.Default.WKPdlMode = (byte)cboPdlMode.SelectedIndex;
            Properties.Settings.Default.WKRevPdl = chkWKSwapPdl.Checked;
            Properties.Settings.Default.WKSidetone = chkWKSideTone.Checked;
            Properties.Settings.Default.WKSToneFreq = (byte)cboSideTone.SelectedIndex;
            Properties.Settings.Default.MyCnty = txtMyCnty.Text;
            Properties.Settings.Default.MyContinent = txtMyContinent.Text;
            Properties.Settings.Default.MyCQZone = Convert.ToInt32(txtMyCQZone.Text);
            Properties.Settings.Default.MyITUZone = Convert.ToInt32(txtMyITUZone.Text);
            Properties.Settings.Default.MyDXCC = Convert.ToInt32(txtMyDXCC.Text);
            if (rdoWKOutput1.Checked) Properties.Settings.Default.WKOutport = 0;
            else Properties.Settings.Default.WKOutport = 1;

            Properties.Settings.Default.Save();
            args.Baud1 = Convert.ToInt32(txtBaud.Text);
            args.Baud2 = Convert.ToInt32(txtBaud.Text);
            args.Comport1 = cboPorts.Text;
            args.Radio1 = cboRig.Text;
            args.MirrorPort = cboVirtPort.Text;
            LogSettingsChanged(this, args);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowseSQLiteDB_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbrowser = new OpenFileDialog();
            
            if (fbrowser.ShowDialog() == DialogResult.OK)
            {
                txtSQLiteFile.Text = fbrowser.FileName;
            }



        }

        private void btnCountryListBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbrowser = new OpenFileDialog();

            if (fbrowser.ShowDialog() == DialogResult.OK)
            {
                txtCountryList.Text = fbrowser.FileName;
            }
        }

        private void btnBandPlanBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbrowser = new OpenFileDialog();

            if (fbrowser.ShowDialog() == DialogResult.OK)
            {
                txtBandPlan.Text = fbrowser.FileName;
            }
        }

        private void cboPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboRig.Text == "K3")
            {
                //K3.ClosePort();
                //K3.InitSerialPort();
            }
        }

        private void btnAddLoTWLoc_Click(object sender, EventArgs e)
        {
            //K3Log.StationsCollection sc = new StationsCollection();            
            //if(Properties.Settings.Default.Remotes == null)
            //{
            //Properties.Settings.Default.Remotes = new StationCollection();
            //}
            //Properties.Settings.Default.Remotes.myStations.Clear();
            //StationCollection myRemotes = new StationCollection();
            List<Remotes> myRemotes = new List<Remotes>();
            foreach (DataGridViewRow r in RemotesdataGridView.Rows)
            {
                if (r.Cells["StationID"].Value != null)
                {
                    Remotes thisStation = new Remotes();
                    thisStation.Station = r.Cells["StationID"].Value.ToString();
                    thisStation.Grid = r.Cells["grid"].Value.ToString();
                    thisStation.ITUZone = r.Cells["ITUZone"].Value.ToString();
                    thisStation.CQZone = r.Cells["CQZone"].Value.ToString();
                    thisStation.County = r.Cells["County"].Value.ToString();
                    thisStation.State = r.Cells["State"].Value.ToString();
                    myRemotes.Add(thisStation);
                    //Properties.Settings.Default.Remotes.myStations.Add(thisStation);
                }
            }
            string serializedList = Newtonsoft.Json.JsonConvert.SerializeObject(myRemotes);
            Properties.Settings.Default.LoTW = serializedList;
            Properties.Settings.Default.Save();
        }

        private void btnAddSatellite_Click(object sender, EventArgs e)
        {
            //K3Log.SatelliteCollection sat = new SatelliteCollection();
            if (Properties.Settings.Default.Satellites == null)
            {
                Properties.Settings.Default.Satellites = new SatelliteCollection();
            }
            Properties.Settings.Default.Satellites.mySatellites.Clear();
            foreach (DataGridViewRow r in SatellitesDataGridView.Rows)
            {
                if (r.Cells["Satellite"].Value != null)
                {
                    K3Log.Satellites thisSatellite = new Satellites();
                    thisSatellite.Satellite = r.Cells["Satellite"].Value.ToString();
                    thisSatellite.Uplink = Convert.ToDouble(r.Cells["Uplink"].Value);
                    thisSatellite.Downlink = Convert.ToDouble(r.Cells["Downlink"].Value);
                    thisSatellite.Mode = r.Cells["Mode"].Value.ToString();
                    thisSatellite.UpMode = r.Cells["UpMode"].Value.ToString();
                    thisSatellite.DownMode = r.Cells["DownMode"].Value.ToString();
                    thisSatellite.PLTone = r.Cells["PLTone"].Value.ToString();
                    thisSatellite.UpDoppler = Convert.ToDouble(r.Cells["UpDoppler"].Value);
                    thisSatellite.DownDoppler = Convert.ToDouble(r.Cells["DownDoppler"].Value);
                    if (r.Cells["Invert"].Value != null )
                    {
                        if (r.Cells["Invert"].Value == "" || r.Cells["Invert"].Value == "false" || (bool) r.Cells["Invert"].Value == false)
                        {
                            thisSatellite.Invert = false;
                            r.Cells["Invert"].Value = "false";
                        }
                        else
                        {
                            thisSatellite.Invert = true;
                            r.Cells["Invert"].Value = "true";
                        }
                    }
                    else
                    {
                        thisSatellite.Invert = false;
                        r.Cells["Invert"].Value = "false";
                    }
                        
                    Properties.Settings.Default.Satellites.mySatellites.Add(thisSatellite);
                }
            }
            Properties.Settings.Default.Save();
        }

        private void btnAddBandPower_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.BandPower == null)
            {
                Properties.Settings.Default.BandPower = new BandPowerCollection();
            }
            Properties.Settings.Default.BandPower.myBandPower.Clear();
            foreach (DataGridViewRow r in BandPwrDataGridView.Rows)
            {
                if (r.Cells["Band"].Value != null)
                {
                    K3Log.BandPower thisBand = new BandPower();
                    thisBand.Band = r.Cells["Band"].Value.ToString();
                    thisBand.Power = r.Cells["Power"].Value.ToString();
                    thisBand.Antenna = r.Cells["ANtenna"].Value.ToString();
                    thisBand.PropMode = r.Cells["PropMode"].Value.ToString();
                    
                    Properties.Settings.Default.BandPower.myBandPower.Add(thisBand);
                }
            }
            Properties.Settings.Default.Save();
        }

        private void SatellitesDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // this is necessary to stop null error when the box is not manually checked.
            //int nr = SatellitesDataGridView.NewRowIndex;
            //SatellitesDataGridView.Rows[nr].Cells["Invert"].Value = false;
        }
    }
    
}
