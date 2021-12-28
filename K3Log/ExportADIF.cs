using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Reflection;

namespace K3Log
{
    public partial class ExportADIF : Form
    {
        string myDB = "";
        SQLiteConnection my_db;
        private string sqlfilter;
        public ExportADIF(SQLiteConnection db)
        {
            my_db = db;
            InitializeComponent();
            FillDropdowns();
            FillView(this.LogdataGridView, "");
        }

        private void FillDropdowns()
        {

            SQLiteCommand comm = new SQLiteCommand("Select * From Log ORDER BY qsoid DESC LIMIT 10", my_db);
            using (SQLiteDataReader reader = comm.ExecuteReader())
            {
                List<string> columns = reader.GetSchemaTable().Rows
                                    .Cast<DataRow>()
                                    .Select(r => (string)r["ColumnName"])
                                    .ToList();
                var bindingsource = new BindingSource();
                bindingsource.DataSource = columns;
                cboField1.DataSource = bindingsource.DataSource;
                cboField1.DisplayMember = "Name";
                
                cboField2.DataSource = bindingsource.DataSource;
                cboField2.DisplayMember = "Name";
            }
            

        }
        private void FillView(DataGridView dgv, String lookup)
        {
            if (myDB != "No Database")
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
                    comm = new SQLiteCommand("Select * From Log WHERE " + lookup + " ORDER BY datetime(qsodate, 'unixepoch') DESC", my_db);
                }

                using (SQLiteDataReader read = comm.ExecuteReader())
                {


                    while (read.Read())
                    {
                        string d = read.GetString(read.GetOrdinal("qsodate")).ToString();
                        string ed = "";
                        try
                        {
                            ed = read.GetString(read.GetOrdinal("qsoenddate")).ToString();
                        }
                        catch (Exception)
                        {
                            ed = d;
                            //throw;
                        }

                        dgv.Rows.Add(new object[] {
                    d,ed,
                    //read.GetDateTime(read.GetOrdinal("qsodate")),  // U can use column index
                    //read.GetValue(read.GetOrdinal("qsoenddate")),  // Or column name like this
                    read.GetValue(read.GetOrdinal("operator")),
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
                
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            string lookup = sqlfilter;
            double freq = 0;
            string adifFile = BrowseFiles();
            string[][] mapnames = new string[][]
                {
                new string[] {"BANDRX","BAND_RX"},
                new string[] {"CALLSIGN","CALL"},
                new string[] {"ARRLSECT","ARRL_SECT"},
                new string[] {"CQZONE","CQZ"},
                new string[] {"FREQRX","FREQ_RX"},
                new string[] {"ITUZONE","ITUZ"},
                new string[] {"MYCNTY","MY_CTNY"},
                new string[] {"MYCITY","MY_CITY"},
                new string[] {"MYGRIDSQUARE", "MY_GRIDSQUARE"},
                new string[] {"MYCQZONE","MY_CQ_ZONE"},
                new string[] {"MYITUZONE","MY_ITU_ZONE"},
                new string[] {"MYSTATE","MY_STATE"},
                new string[] {"PROPMODE","PROP_MODE"},
                new string[] {"RSTSENT","RST_SENT"},
                new string[] {"RSTRCVD","RST_RCVD"},
                new string[] {"SATMODE","SAT_MODE"},
                new string[] {"SATNAME","SAT_NAME"},
                new string[] {"TXPOWER","TX_POWER"},
                };
            if (myDB != "No Database")
            {

                SQLiteCommand comm;
                if (my_db.State == ConnectionState.Closed) my_db.Open();
                if (lookup == "")
                {
                    //comm = new SQLiteCommand("Select * From Log ORDER BY datetime(QsoDate, 'unixepoch') DESC", my_db);
                    comm = new SQLiteCommand("Select * From Log ORDER BY qsoid DESC LIMIT 1000", my_db);
                }
                else
                {
                    comm = new SQLiteCommand("Select * From Log WHERE " + lookup + " ORDER BY datetime(qsodate, 'unixepoch') DESC", my_db);
                    //comm = new SQLiteCommand("Select * From Log WHERE qsodate > date('2020-11-01') ORDER BY qsoid DESC LIMIT 1000", my_db);
                    //comm = new SQLiteCommand("Select * From Log WHERE LIKE('" + lookup + "%',callsign)=1 ORDER BY datetime(qsodate, 'unixepoch') DESC", my_db);
                }

                using (SQLiteDataReader reader = comm.ExecuteReader())
                {

                    List<AdifRow> rows = new List<AdifRow>();
                    while (reader.Read())
                    {
                        // get all column names
                        AdifRow newRow = new AdifRow();
                        var columns = reader.GetSchemaTable().Rows
                                    .Cast<DataRow>()
                                    .Select(r => (string)r["ColumnName"])
                                    .ToList();
                        for (int x = 0; x < columns.Count; x++)
                        {
                            columns[x] = columns[x].ToUpper();
                        }

                        var props = typeof(AdifRow).GetRuntimeProperties();

                        string colVal = "";
                        
                        foreach (string col in columns)
                        {
                            int index = columns.IndexOf(col);
                            switch (col)
                            {
                                case "QSODATE":
                                    DateTime dt = reader.GetDateTime(reader.GetOrdinal("qsodate")).ToUniversalTime();
                                    colVal = dt.ToString("yyyyMMdd");
                                    PropertyInfo pi = typeof(AdifRow).GetRuntimeProperty("QSO_DATE");
                                    pi.SetValue(newRow, colVal, null);
                                    pi = typeof(AdifRow).GetRuntimeProperty("TIME_ON");
                                    colVal = dt.TimeOfDay.ToString("hhmmss");
                                    pi.SetValue(newRow, colVal, null);
                                    try
                                    {
                                        dt = reader.GetDateTime(reader.GetOrdinal("qsoenddate")).ToUniversalTime();
                                    }
                                    catch (Exception)
                                    {
                                        Console.Write("no end date");
                                        //throw;
                                    }
                                    //dt = reader.GetDateTime(reader.GetOrdinal("qsoenddate")).ToUniversalTime();
                                    colVal = dt.ToString("yyyyMMdd");
                                    pi = typeof(AdifRow).GetRuntimeProperty("QSO_DATE_OFF");
                                    pi.SetValue(newRow, colVal, null);
                                    pi = typeof(AdifRow).GetRuntimeProperty("TIME_OFF");
                                    colVal = dt.TimeOfDay.ToString("hhmmss");
                                    pi.SetValue(newRow, colVal, null);
                                    break;
                                case "FREQRX":
                                    try
                                    {
                                        freq = reader.GetDouble(index);
                                        colVal = (freq / 1000).ToString();
                                        pi = typeof(AdifRow).GetRuntimeProperty("FREQ_RX");
                                        pi.SetValue(newRow, colVal, null);
                                    }
                                    catch (Exception)
                                    {

                                        //throw;
                                    }
                                    
                                    break;
                                case "FREQ":
                                    freq = reader.GetDouble(index);
                                    colVal = (freq/1000).ToString();
                                    pi = typeof(AdifRow).GetRuntimeProperty("FREQ");
                                    pi.SetValue(newRow, colVal, null);
                                    break;
                                
                                default:
                                    string colName = col;
                                    for(int x = 0; x < mapnames.Length; x++)
                                    {
                                        if (mapnames[x][0] == col)
                                        {
                                            colName = mapnames[x][1];
                                        }
                                    }                                  
                                    pi = typeof(AdifRow).GetRuntimeProperty(colName);
                                    if(pi != null) 
                                    {
                                        colVal = reader.GetValue(index).ToString();
                                        if(col == "SATNAME")
                                        {
                                            if (colVal == "AO-07") colVal = "AO-7";
                                        }
                                        pi.SetValue(newRow, colVal, null);
                                    }
                                    break;

                            }

                        }
                        rows.Add(newRow);
                        lblExportProgress.Text = newRow.QSO_DATE.ToString() + "  " + newRow.CALL.ToString();
                        Application.DoEvents();
                    }
                    writeADIF(rows, adifFile);
                }

            }

        }
        private void writeADIF(List<AdifRow> rows, string filespec)
        {
            ADIFHelper helper = new ADIFHelper();

            string adifstr = helper.ExportAsADIF(rows, filespec);


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

        private void btnFillGrid_Click(object sender, EventArgs e)
        {
            
            if (chkFromDate.Checked && !chkFilterBy.Checked)
            {
                sqlfilter = "qsodate >= date('" + dateTimePickerFrom.Value.ToString("yyyy-MM-dd") + "') AND qsodate <= date('" + dateTimePickerTo.Value.ToString("yyyy-MM-dd") + "')";
            }
            if (chkFromDate.Checked && chkFilterBy.Checked)
            {
                sqlfilter = "qsodate >= date('" + dateTimePickerFrom.Value.ToString("yyyy-MM-dd") + "') AND qsodate <= date('" + dateTimePickerTo.Value.ToString("yyyy-MM-dd") + "')" +
                    " AND " + cboField1.Text + " = '" + txtMatch1.Text + "' ";
            }

                FillView(this.LogdataGridView, sqlfilter);
        }
    }
}
