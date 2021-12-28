using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace K3Log
{
    public partial class Log : Form
    {
        //public SQLiteConnection conn;
        //public DataTable tblQSO;
        //SQLiteConnection my_db = new SQLiteConnection("Data Source=" +
        //"E:\\Documents\\N5TM_DB\\N5TM_Full.SQLite;Version=3;");
        public SQLiteConnection my_db;


        public Log()
        {
            InitializeComponent();

        }

        private void Log_Load(object sender, EventArgs e)
        {
            LogdataGridView.Columns[0].Width = 70;
            FillView(this.LogdataGridView, "");
        }

        private void FillView(DataGridView dgv, String lookup)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<DataGridView, string>(FillView), new object[] { dgv, lookup });
                return;
            }
            else
            {

                dgv.Rows.Clear();
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;

                SQLiteCommand comm;
                if (my_db.State == ConnectionState.Closed) my_db.Open();
                if (lookup == "")
                {
                    comm = new SQLiteCommand("Select * From Log ORDER BY datetime(QsoDate, 'unixepoch') DESC", my_db);
                }
                else
                {
                    comm = new SQLiteCommand("Select * From Log WHERE LIKE('" + lookup + "%',Call)=1 ORDER BY datetime(QsoDate, 'unixepoch') DESC", my_db);
                }

                using (SQLiteDataReader read = comm.ExecuteReader())
                {


                    while (read.Read())
                    {
                        dgv.Rows.Add(new object[] {
                            read.GetValue(read.GetOrdinal("QsoDate")),  // U can use column index
                            read.GetValue(read.GetOrdinal("TimeOn")),  // Or column name like this
                            read.GetValue(read.GetOrdinal("TheOperator")),
                            read.GetValue(read.GetOrdinal("Call")),
                            read.GetValue(read.GetOrdinal("Mode")),
                            read.GetValue(read.GetOrdinal("Band")),
                            read.GetValue(read.GetOrdinal("Freq")),
                            read.GetValue(read.GetOrdinal("RstRcvd")),
                            read.GetValue(read.GetOrdinal("RstSent")),
                            read.GetValue(read.GetOrdinal("Country")),
                            read.GetValue(read.GetOrdinal("Name")),
                            read.GetValue(read.GetOrdinal("Comment")),
                            read.GetValue(read.GetOrdinal("PropMode")),
                            read.GetValue(read.GetOrdinal("QsoId"))
                            });
                    }
                }
            }
        }
        /*private void FillView()
        {

            conn.Open();
            SQLiteCommand comm = new SQLiteCommand("Select * From Log", conn);
            using (SQLiteDataReader read = comm.ExecuteReader())
            {
                while (read.Read())
                {
                    LogdataGridView.Rows.Add(new object[] {
                        read.GetValue(read.GetOrdinal("QsoDate")),  // U can use column index
                        read.GetValue(read.GetOrdinal("TimeOn")),  // Or column name like this
                        read.GetValue(read.GetOrdinal("Call")),
                        read.GetValue(read.GetOrdinal("Band")),
                        read.GetValue(read.GetOrdinal("Mode")),
                        read.GetValue(read.GetOrdinal("RstSent")),
                        read.GetValue(read.GetOrdinal("RstRcvd"))
                        });
                }
            }

        }*/
    }
}
