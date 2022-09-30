using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;


namespace K3Log
{
    public class QSO : IDisposable
    {
        public string qsoid
        {
            get { return getColValue("qsoid"); }
            set { setColValue("qsoid", value); }
        }
        public string callsign
        {
            get { return getColValue("callsign"); }
            set { setColValue("callsign", value); }
        }
        public string ownercallsign
        {
            get { return getColValue("ownercallsign"); }
            set { setColValue("ownercallsign", value); }
        }
        public string date
        {
            get { return getColValue("qsoenddate"); }
            set { setColValue("qsoenddate", value); }
        }
        public string start
        {
            get { return getColValue("qsodate"); }
            set { setColValue("qsodate", value); }
        }
        public string end;
        public string mode
        {
            get { return getColValue("mode"); }
            set { setColValue("mode", value); }
        }
        public string power
        {
            get { return getColValue("txpwr"); }
            set { setColValue("txpwr", value); }
        }
        public string band
        {
            get { return getColValue("band"); }
            set { setColValue("band", value); }
        }
        public string freqrx
        {
            get { return getColValue("freqrx"); }
            set { setColValue("freqrx", value); }
        }
        public string bandrx
        {
            get { return getColValue("bandrx"); }
            set { setColValue("bandrx", value); }
        }
        public string freq
        {
            get { return getColValue("freq"); }
            set { setColValue("freq", value); }
        }
        public string sent
        {
            get { return getColValue("rstsent"); }
            set { setColValue("rstsent", value); }
        }
        public string rcvd
        {
            get { return getColValue("rstrcvd"); }
            set { setColValue("rstrcvd", value); }
        }
        public string country
        {
            get { return getColValue("country"); }
            set { setColValue("country", value); }
        }
        public string qth
        {
            get { return getColValue("qth"); }
            set { setColValue("qth", value); }
        }

        public string ituzone
        {
            get { return getColValue("ituzone"); }
            set { setColValue("ituzone", value); }
        }
        public string cqzone
        {
            get { return getColValue("cqzone"); }
            set { setColValue("cqzone", value); }
        }
        public string dxcc
        {
            get { return getColValue("dxcc"); }
            set { setColValue("dxcc", value); }
        }

        public string grid
        {
            get { return getColValue("gridsquare"); }
            set { setColValue("gridsquare", value); }
        }

        public string continent
        {
            get { return getColValue("cont"); }
            set { setColValue("cont", value); }
        }
        public string name
        {
            get { return getColValue("name"); }
            set { setColValue("name", value); }
        }
        public string cnty
        {
            get { return getColValue("cnty"); }
            set { setColValue("cnty", value); }
        }
        public string mycnty
        {
            get { return getColValue("mycnty"); }
            set { setColValue("mycnty", value); }
        }
        public string mystate
        {
            get { return getColValue("mystate"); }
            set { setColValue("mystate", value); }
        }
        public string mygridsquare
        {
            get { return getColValue("mygridsquare"); }
            set { setColValue("mygridsquare", value); }
        }
        public int mycqzone
        {
            get { return Convert.ToInt32(getColValue("mycqzone")); }
            set { setColValue("mycqzone", value.ToString()); }
        }
        public int myituzone
        {
            get { return Convert.ToInt32(getColValue("myituzone")); }
            set { setColValue("myituzone", value.ToString()); }
        }
        public int mydxcc   
        {
            get { return Convert.ToInt32(getColValue("mydxcc")); }
            set { setColValue("mydxcc", value.ToString()); }
        }
        public string mysiginfo   // using this field for Remote LoTW definition  (callsign of remote station)
        {
            get { return getColValue("mysiginfo"); }
            set { setColValue("mysiginfo", value); }
        }
        public bool lotw_sent;
        /*
        {
            get { return Convert.ToBoolean(getColValue("lotw_sent")); }
            set { setColValue("lotw_sent", value.ToString()); }
        }*/
        public bool lotw_rcvd
        {
            get { return parseQsoConfirmations("LoTW",qsoconfirmations); }
            set {  }
        }
        public string qsoconfirmations
        {
            get { return getColValue("qsoconfirmations"); }
            set { setColValue("qsoconfirmations", value.ToString()); }
        }
        public string lotw_sent_date;
        public string lotw_rcvd_date;
        public bool qsl_sent;
        public bool qsl_rcvd;
        public string qsl_sent_date;
        public string qsl_rcvd_date;
        public string op;
        /*
        {
            get { return getColValue("op"); }
            set { setColValue("op", value); }
        }*/
        public string propmode
        {
            get { return getColValue("propmode"); }
            set { setColValue("propmode", value); }
        }
        public string station;
        public string state
        {
            get { return getColValue("state"); }
            set { setColValue("state", value); }
        }
        public string comment
        {
            get { return getColValue("comment"); }
            set { setColValue("comment", value); }
        }
        public string theoperator
        {
            get { return getColValue("operator"); }
            set { setColValue("operator", value); }
        }
        public string satelliteqso
        {
            get { return getColValue("satelliteqso"); }
            set { setColValue("satelliteqso", value); }
        }
        public string satname
        {
            get { return getColValue("satname"); }
            set { setColValue("satname", value); }
        }
        public string satmode
        {
            get { return getColValue("satmode"); }
            set { setColValue("satmode", value); }
        }

        List<String> colNames = new List<string>();  // use this to rebuild the Insert sql
        List<String> colValue = new List<string>();
        private SQLiteConnection my_db;
        private bool parseQsoConfirmations(string field, string qsocfm)
        {
            if (qsocfm.Contains("LOTW"))
            {
                string q = qsocfm.Split('W')[1];
                switch (field)
                {
                    case "LoTW":
                        string[] flds = q.Split(':');
                        if (flds[2].Contains("Yes")) return true;

                        break;

                }
            }
            else
            {
                string q = "False";
            }
            

            return false;
        }
        public QSO(SQLiteConnection conn)
        {
            my_db = conn;
            initColumnLists();
            
            callsign = "callsign";
            //date = DateTime.Today.ToShortDateString();
            start = DateTime.Today.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ");
            date = DateTime.Today.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ");
            mode = "cw";
            power = "500";
            band = "40m";
            freq = "7010";
            sent = "599";
            rcvd = "599";
            country = "US";
            qth = "";
            cnty = "";
            dxcc = "0000";
            grid = "XXxxnn";
            continent = "NA";
            name = "name";
            lotw_sent = false;
            lotw_rcvd = false;
            qsl_sent = false;
            qsl_rcvd = false;
            lotw_sent_date = "0000-00-00";
            lotw_rcvd_date = "0000-00-00";
            qsl_sent_date = "0000-00-00";
            qsl_rcvd_date = "0000-00-00";
            op = "dan";
            station = "home";
            comment = "";
            propmode = "";
            theoperator = Properties.Settings.Default.MyCall;
            satelliteqso = "0";
            satname = "";
            satmode = "";
        }
        ~QSO()
        {
            this.Dispose();
        }

        private void initColumnLists()
        {
            SQLiteCommand comm = new SQLiteCommand("Select * From Log", my_db);

            if (my_db.State == ConnectionState.Closed) my_db.Open();

            using (SQLiteDataReader read = comm.ExecuteReader())
            {

                for (int i = 0; i < read.FieldCount; i++)
                {
                    colNames.Add(read.GetName(i));
                    String dt = read.GetDataTypeName(i);
                    if (dt.Contains("nvarchar"))
                    {
                        colValue.Add("");
                    }
                    else
                    {
                        colValue.Add("0");
                    }

                }
            }
        }

        public String getColValue(String coln)
        {
            string ret = colValue[colNames.FindIndex(a => a.Equals(coln))];
            return ret; // colValue[colNames.FindIndex(a => a.Equals(coln))];
        }

        public void setColValue(String coln, String colv)
        {
            colValue[colNames.FindIndex(a => a.Equals(coln))] = colv;
        }

        public void logQso()
        {

            if (this.qsoid == "")
            {
                this.qsoid = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff");
            }
            if(this.qsoid.Contains("import"))
            {
                string milli = DateTime.Now.Millisecond.ToString();
                this.qsoid = this.date.Replace("-", "").Replace(":","").Replace(" ", "").Replace("Z", "") + this.qsoid.Split(' ')[1] + milli; 
            }
       
            

            // look up the index which matches the column name and set that column's value from the q passed.

            colValue[colNames.FindIndex(a => a.Equals("qsoid"))] = qsoid;

            SQLiteCommand cmd = my_db.CreateCommand();
            cmd.CommandText = "INSERT INTO Log (";
            
            String columnNames = "";
            String columnValues = "";

            foreach (String c in colNames)
            {
                columnNames += c + ", ";
                columnValues += "'" + colValue[colNames.FindIndex(a => a.Equals(c))] + "', ";
            }
            cmd.CommandText += columnNames.Substring(0, columnNames.Length - 2) + ") VALUES (" +
                columnValues.Substring(0, columnValues.Length - 2) + ")";

            cmd.ExecuteNonQuery();
        }

        
       
        public void EditQso(String QsoId)
        {
            
            try
            {

                {
                    SQLiteCommand cmd = my_db.CreateCommand();
                    cmd = new SQLiteCommand("UPDATE Log Set qsodate = '" + this.date +
                        "', qsodate = '" + this.start +
                        "', qsoenddate = '" + this.date +
                        "', callsign = '" + this.callsign +
                        "', band = '" + this.band +
                        "', bandrx = '" + this.bandrx +
                        "', mode = '" + this.mode +
                        "', freq = '" + this.freq +
                        "', freqrx = '" + this.freqrx +
                        "', country = '" + this.country +
                        "', dxcc = '" + this.dxcc +
                        "', satelliteqso = '" + this.satelliteqso +
                        "', satmode = '" + this.satmode +
                        "', satname = '" + this.satname +
                        "', mydxcc = '" + this.mydxcc +
                        "', mycqzone = '" + this.mycqzone +
                        "', myituzone = '" + this.myituzone +
                        "', mycnty = '" + this.mycnty +
                        "', mystate = '" + this.mystate +
                        "', mysiginfo = '" + this.mysiginfo +
                        "', mygridsquare = '" + this.mygridsquare +
                        "', qth = '" + this.qth +
                        "', cnty = '" + this.cnty +
                        "', cont = '" + this.continent +
                        "', rstrcvd = '" + this.rcvd +
                        "', rstsent = '" + this.sent +
                        "', comment = '" + this.comment +
                        "', txpwr = '" + this.power +
                        "', gridsquare = '" + this.grid +
                        "', name = '" + this.name +
                        "', operator = '" + this.theoperator +
                        "', ownercallsign = '" + this.ownercallsign +
                        "', propmode = '" + this.propmode + 
                        "', qsoconfirmations = '" + this.qsoconfirmations + "' " +
                        "WHERE qsoid = '" + QsoId + "'", my_db);


                    cmd.ExecuteNonQuery();

                    //rows number of record got inserted

                }
            }
            catch (SQLiteException ex)
            {
                Console.Write(ex.ToString());
                //Log exception
                //Display Error message
            }
        }
        public bool ReadQso(String QsoId)
        {

            bool ret = false;
            SQLiteCommand cmd = my_db.CreateCommand();
            cmd.CommandText = "Select * From Log WHERE QsoId = '" + QsoId + "'";
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    this.qsoid = reader.GetValue(reader.GetOrdinal("qsoid")).ToString();
                    this.date = reader.GetString(reader.GetOrdinal("qsodate")).ToString();
                    this.start = reader.GetString(reader.GetOrdinal("qsodate")).ToString();
                    this.callsign = reader.GetValue(reader.GetOrdinal("callsign")).ToString();
                    this.mode = reader.GetValue(reader.GetOrdinal("mode")).ToString();
                    this.band = reader.GetValue(reader.GetOrdinal("band")).ToString();
                    this.freq = reader.GetValue(reader.GetOrdinal("freq")).ToString();
                    this.bandrx = reader.GetValue(reader.GetOrdinal("bandrx")).ToString();
                    this.freqrx = reader.GetValue(reader.GetOrdinal("freqrx")).ToString();
                    this.sent = reader.GetValue(reader.GetOrdinal("rstsent")).ToString();
                    this.rcvd = reader.GetValue(reader.GetOrdinal("rstrcvd")).ToString();
                    this.country = reader.GetValue(reader.GetOrdinal("country")).ToString();
                    this.name = reader.GetValue(reader.GetOrdinal("name")).ToString();
                    this.qth = reader.GetValue(reader.GetOrdinal("qth")).ToString();
                    this.grid = reader.GetValue(reader.GetOrdinal("gridsquare")).ToString();
                    this.power = reader.GetValue(reader.GetOrdinal("txpwr")).ToString();
                    this.comment = reader.GetValue(reader.GetOrdinal("comment")).ToString();
                    this.propmode = reader.GetValue(reader.GetOrdinal("propmode")).ToString();
                    this.theoperator = reader.GetValue(reader.GetOrdinal("operator")).ToString();
                    this.dxcc = reader.GetValue(reader.GetOrdinal("dxcc")).ToString();
                    this.satelliteqso = reader.GetValue(reader.GetOrdinal("satelliteqso")).ToString();
                    this.satmode = reader.GetValue(reader.GetOrdinal("satmode")).ToString();
                    this.satname = reader.GetValue(reader.GetOrdinal("satname")).ToString();
                    this.mystate = reader.GetValue(reader.GetOrdinal("mystate")).ToString();
                    this.mycnty = reader.GetValue(reader.GetOrdinal("mycnty")).ToString();
                    this.mygridsquare = reader.GetValue(reader.GetOrdinal("mygridsquare")).ToString();
                    this.ituzone = reader.GetValue(reader.GetOrdinal("ituzone")).ToString();
                    this.cqzone = reader.GetValue(reader.GetOrdinal("cqzone")).ToString();
                    this.myituzone = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("myituzone")));
                    this.mycqzone = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("mycqzone")));
                    this.mysiginfo = reader.GetValue(reader.GetOrdinal("mysiginfo")).ToString();
                    this.qsoconfirmations = reader.GetValue(reader.GetOrdinal("qsoconfirmations")).ToString();
                    ret = true;
                }

                return ret;
            }

        }
        public void Dispose()
        {
            my_db.Dispose();
        }
    }

}
