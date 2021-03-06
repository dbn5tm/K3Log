using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;


namespace K3Log
{
    public class AdifReader  : IDisposable
    {
        public bool IsDisposed { get; protected set; }
        public bool isInitialized;
        private List<AdifRow> adifRows;
        //private ILoggerFactory loggerFactory = new LoggerFactory();
        private string adif;

        public AdifReader(string adifContent)
        {
            adif = adifContent;
            isInitialized = true;
        }

        public List<AdifRow> GetAdifRows()
        {
            string[] rawrecords = AdifRecords();  // spilts <EOH>
            adifRows = new List<AdifRow>();

            foreach (string record in rawrecords)
            {
                AdifRow AdifRow = new AdifRow();
                
                string[] x = Regex.Split(record.Replace("\n", "").Replace("\r", ""), @"<([^:]+):\d+[^>]*>").ToArray();
                List<string> l = new List<string>(x);
                l.RemoveAt(0);
                x = l.ToArray();

                var dic = new Dictionary<string, string>();
                if (x.Length % 2 == 0)
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        dic.Add(x[i].ToUpper(), x[i + 1]);
                        i++;
                    }

                    var props = typeof(AdifRow).GetRuntimeProperties();

                    foreach (PropertyInfo prp in props)
                    {
                        if (dic.ContainsKey(prp.Name))
                        {
                            PropertyInfo pi = typeof(AdifRow).GetRuntimeProperty(prp.Name);
                            pi.SetValue(AdifRow, dic[prp.Name]?.Trim(), null);
                        }
                    }
                }
                
                if (!string.IsNullOrEmpty(AdifRow.CALL))
                {
                    adifRows.Add(ADIFHelper.FixRecord(AdifRow));
                }
                
            }

            return adifRows;
        }

        private string[] AdifRecords()
        {
            
            if (adif.Contains("<EOH>"))
            {
                if (adif.Contains("<EOR>"))
                {
                    string[] RawRecords = adif?.Split(new string[] { "<EOH>" }, StringSplitOptions.RemoveEmptyEntries)[1]?.Split(new string[] { "<EOR>" }, StringSplitOptions.RemoveEmptyEntries);
                    return RawRecords;
                }
                else
                {
                    if (adif.Contains("<eor>"))
                    {
                        string[] RawRecords = adif?.Split(new string[] { "<EOH>" }, StringSplitOptions.RemoveEmptyEntries)[1]?.Split(new string[] { "<eor>" }, StringSplitOptions.RemoveEmptyEntries);
                        return RawRecords;
                    }
                    else
                    {
                        return new string[0];
                    }
                }
                
            }
            else
            {
                return new string[0];
            }
             
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    adifRows = null;
                    adif = null;
                }
                isInitialized = false;
                IsDisposed = true;
            }
        }
    }
}
