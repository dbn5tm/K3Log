using System;
using System.Collections.Generic;
using System.Data.SQLite;


namespace K3Log
{

    class SQLiteDB : IDisposable
    {

        public class SQLiteRowDataFields
        {
            public String QsoId { get; set; }
            public String Address { get; set; } = "";
            public String Age { get; set; } = "";
            public Int32 AIndex { get; set; } = 0;
            public Int32 AntAz { get; set; } = 0;
            public Int32 AntEl { get; set; } = 0;
            public String Band { get; set; } = "40m";
            public String BandRx { get; set; } = "40m";
            public Double FB { get; set; }
            public Int16 BN { get; set; }
            public Int16 BNsub { get; set; }
            public String TB { get; set; }
            public Int16 MD { get; set; }
            public Int16 MDsub { get; set; }
            public String msg { get; set; }
            public int txBufcnt { get; set; }
            public int rxBufcnt { get; set; }
            public int wordindex { get; set; }
            public List<string> pendingTX { get; set; }
        }



        public SQLiteConnection conn;  //"Data Source=" +
                                       //"C:\\Users\\Dan\\Documents\\N5TM_DB\\N5TM_Full.SQLite;Version=3;"

        public SQLiteDB(String connection)
        {

            try
            {
                conn = new SQLiteConnection(connection);
                conn.Open();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<string> RetrieveDistinct(String tbl, String fld, String test, String val, String cond, String[] cols)
        {

            String sql;
            if (cond.Contains("LIKE"))
            {
                sql = "SELECT DISTINCT substr(gridsquare, 1,4) FROM " + tbl + " Where " + test + " " + cond + " " + "'" + val + "%'";
            }
            else
            {
                sql = "SELECT DISTINCT " + fld + " FROM " + tbl + " Where " + test + " " + cond + " " + "'" + val + "' ORDER BY gridsquare";
            }
            List<string> inLog = new List<string>();
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            using (SQLiteDataReader reader = command.ExecuteReader())
            { 
            
            //int i = 0;
                while (reader.Read())
                {
                    String mylog = "";
                    //foreach (String col in cols)
                    //{
                    mylog += reader[0].ToString(); //+ ",";
                    //}
                    inLog.Add(mylog);
                }
            }
            return inLog;

        }

        public List<string> Retrieve(String tbl, String fld, String val, String cond, String[] cols)
        {

            String sql;
            if (cond.Contains("LIKE"))
            {
                sql = "SELECT * FROM " + tbl + " WHERE " + fld + " " + cond + " " + "'" + val + "%'";
            }
            else
            {
                sql = "SELECT * FROM " + tbl + " WHERE " + fld + " " + cond + " " + "'" + val + "'";
            }
            List<string> inLog = new List<string>();

            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {

                    //int i = 0;

                    while (reader.Read())
                    {
                        String mylog = "";
                        foreach (String col in cols)
                        {
                            mylog += reader[col].ToString() + ",";
                        }
                        inLog.Add(mylog);
                    }
                }
            }
            return inLog;

        }

        public void Delete(string id)
        {
            try
            {
                string sql = "DELETE FROM LOG WHERE QsoId = '" + id + "'";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))

                    command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                //throw;
            }
        }

        public List<string> sqliteFind(string lookup)
        {
            List<string> inLog = new List<string>();
            string sql = "SELECT * FROM Log WHERE LIKE('" + lookup + "%',callsign)=1 ORDER BY band DESC;";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            using (SQLiteDataReader reader = command.ExecuteReader())
            { 
                //int i = 0;
                while (reader.Read())
                {
                    string mylog = reader["Call"] + "," + reader["qsodate"] + "," + reader["band"] + "," + reader["mode"];
                    inLog.Add(mylog);
                }
            }
            return inLog;
        }

        public void Dispose()
        {
            conn.Dispose();
        }
    }


}
