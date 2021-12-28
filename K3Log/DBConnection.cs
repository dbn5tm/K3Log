using MySql.Data.MySqlClient;
using System;
using System.Data;


namespace K3Log
{
    public class DBConnection
    {
        public DBConnection()
        {
        }
        public DataTable tblQso;

        private string databaseName = string.Empty;
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            bool result = true;
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(databaseName))
                    result = false;
                string connstring = string.Format("Server=localhost; database={0}; UID=dan; password=dange123; convert zero datetime=True", databaseName);
                connection = new MySqlConnection(connstring);
                connection.Open();
                MySqlDataAdapter myAdapter = new MySqlDataAdapter("SELECT * FROM qso", connection);
                DataSet myDataSet = new DataSet("qso");

                myAdapter.FillSchema(myDataSet, SchemaType.Source, "qso");
                myAdapter.Fill(myDataSet, "qso");

                tblQso = myDataSet.Tables["qso"];
                result = true;
            }

            return result;
        }

        public string[] mysqlQuery(string query, DBConnection dbCon)
        {
            string[] ret = new string[2];
            var cmd = new MySqlCommand(query, dbCon.Connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ret[0] = reader.GetString(0);
                ret[1] = reader.GetString(1);
                //Console.WriteLine(someStringFromColumnZero + "," + someStringFromColumnOne);
            }
            return ret;
        }

        public bool addRecord(QSO qso, DBConnection dbCon)
        {

            //string connString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            //MySqlConnection conn = new MySqlConnection(dbCon.Connection.ConnectionString);
            //conn.Open();
            string connstring = string.Format("Server=localhost; database={0}; UID=dan; password=dange123", databaseName);
            connection = new MySqlConnection(connstring);
            connection.Open();
            MySqlCommand comm = connection.CreateCommand();
            comm.CommandText = "INSERT INTO qso(Callsign,QSO_Date,Start_Time,End_Time,Mode,TX_Power,Band," +
                "Freq,RST_Sent,RST_Rcvd,Country,DXCC,Grid,Continent,County,State,Name,Lotw_Sent,Lotw_Rcvd," +
                "Lotw_Sent_Date,Lotw_Rcvd_Date," +
                "QSL_Sent,QSL_Rcvd,QSL_Sent_Date,QSL_Rcvd_Date,Operator,Station,Comment) VALUES(" +
                "?Callsign, ?QSO_Date,?Start_Time,?End_Time,?Mode,?TX_Power,?Band," +
                "?Freq,?RST_Sent,?RST_Rcvd,?Country,?DXCC,?Grid,?Continent,?County,?State,?Name," +
                "?Lotw_Sent,?Lotw_Rcvd,?Lotw_Sent_Date,?Lotw_Rcvd_Date," +
                "?QSL_Sent,?QSL_Rcvd,?QSL_Sent_Date,?QSL_Rcvd_Date,?Operator,?Station,?Comment)";
            comm.Parameters.AddWithValue("?Callsign", qso.callsign);
            comm.Parameters.AddWithValue("?QSO_Date", qso.date);
            //comm.Parameters.AddWithValue("?Start_Time", qso.start);
            comm.Parameters.AddWithValue("?End_Time", qso.end);
            comm.Parameters.AddWithValue("?Mode", qso.mode);
            comm.Parameters.AddWithValue("?TX_Power", qso.power);
            comm.Parameters.AddWithValue("?Band", qso.band);
            comm.Parameters.AddWithValue("?Freq", qso.freq);
            comm.Parameters.AddWithValue("?RST_Sent", qso.sent);
            comm.Parameters.AddWithValue("?RST_Rcvd", qso.rcvd);
            comm.Parameters.AddWithValue("?Country", qso.country);
            comm.Parameters.AddWithValue("?DXCC", qso.dxcc);
            comm.Parameters.AddWithValue("?Grid", qso.grid);
            comm.Parameters.AddWithValue("?Continent", qso.continent);
            comm.Parameters.AddWithValue("?County", qso.cnty);
            comm.Parameters.AddWithValue("?State", qso.state);
            comm.Parameters.AddWithValue("?Name", qso.name);
            comm.Parameters.AddWithValue("?Lotw_Sent", qso.lotw_sent);
            comm.Parameters.AddWithValue("?Lotw_Rcvd", qso.lotw_rcvd);
            comm.Parameters.AddWithValue("?Lotw_Sent_Date", qso.lotw_sent_date);
            comm.Parameters.AddWithValue("?Lotw_Rcvd_Date", qso.lotw_rcvd_date);
            comm.Parameters.AddWithValue("?QSL_Sent", qso.qsl_sent);
            comm.Parameters.AddWithValue("?QSL_Rcvd", qso.qsl_rcvd);
            comm.Parameters.AddWithValue("?QSL_Sent_Date", qso.qsl_sent_date);
            comm.Parameters.AddWithValue("?QSL_Rcvd_Date", qso.qsl_rcvd_date);
            comm.Parameters.AddWithValue("?Operator", qso.op);
            comm.Parameters.AddWithValue("?Station", qso.station);
            comm.Parameters.AddWithValue("?Comment", qso.comment);
            comm.ExecuteNonQuery();
            connection.Close();

            return true;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
