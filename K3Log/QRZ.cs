using System;
using System.Xml;

namespace K3Log
{
    public class QRZ : IDisposable
    {
        public String state { get; set; }
        public String fname { get; set; }
        public String name { get; set; }
        public String country { get; set; }
        public String dxcc { get; set; }
        public String email { get; set; }
        public String addr1 { get; set; }
        public String addr2 { get; set; }
        public String grid { get; set; }
        public String cqzone { get; set; }
        public String ituzone { get; set; }
        public String city { get; set; }
        public Boolean loggedIn = false;

        private String QRZKey = "";
        private String QRZLogin;
        private String QRZPwd;

        public QRZ(String login, String pwd)
        {
            QRZLogin = Properties.Settings.Default.QRZLogin;  //login;
            QRZPwd = Properties.Settings.Default.QRZPwd;  //pwd;
            getkey();
        }
        ~QRZ()
        {
            this.Dispose();
        }
        public bool getkey()
        {
            QRZKey = GetQRZSessionKey();
            if (QRZKey.Contains("Error")) return false;
            return true;
        }

        public Boolean Lookup(String callsign)
        {
            if (QRZKey != "")
            {
                XmlDocument xQRZ = new XmlDocument();
                String queryStr = "https://xmldata.qrz.com/xml/current/?s=" + QRZKey + ";callsign=" + callsign;
                xQRZ.Load(queryStr);
                if (!xQRZ.InnerText.Contains("Not found:"))
                {
                    fname = getQRZElement(xQRZ, "fname");
                    name = getQRZElement(xQRZ, "name");
                    country = getQRZElement(xQRZ, "land");
                    dxcc = getQRZElement(xQRZ, "dxcc");
                    email = getQRZElement(xQRZ, "email");
                    addr1 = getQRZElement(xQRZ, "addr1");
                    addr2 = getQRZElement(xQRZ, "addr2");
                    grid = getQRZElement(xQRZ, "grid");
                    state = getQRZElement(xQRZ, "state");
                    //cqzone = getQRZElement(xQRZ, "cqzone");
                    //ituzone = getQRZElement(xQRZ, "ituzone");
                    city = addr2;// getQRZElement(xQRZ, "city");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }           
        

        private String getQRZElement(XmlDocument xQ, String tagName)
        {
            try
            {
                return xQ.GetElementsByTagName(tagName)[0].InnerText;
            }
            catch (Exception)
            {
                return "";
                //throw;
            }

        }

        private String GetQRZSessionKey()
        {
            try
            {
                XmlDocument xDocKey = new XmlDocument();
                xDocKey.Load("https://xmldata.qrz.com/xml/current/?username=" + QRZLogin + ";password=" + QRZPwd + ";agent=q5.0");
                XmlNodeList keyElement = xDocKey.GetElementsByTagName("Key");
                String key;
                if (keyElement.Count > 0)
                {
                    key = keyElement[0].InnerText;
                    loggedIn = true;
                }
                else
                {
                    key = "";
                }
                XmlNodeList alert = xDocKey.GetElementsByTagName("Alert");
                if (alert.Count > 0) loggedIn = false;  //key = "alert:" + alert[0].InnerText;
                XmlNodeList errmsg = xDocKey.GetElementsByTagName("Error");
                if (errmsg.Count > 0) loggedIn = false;  //key = "Error: " + errmsg[0].InnerText;
                return key;
            }
            catch (Exception e)
            {

                Console.Write("QRZ Unreachable " + e.ToString());
                return "Error";
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~QRZ() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
