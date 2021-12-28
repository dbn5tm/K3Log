using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace K3Log
{
    class UDPSocket
    {

        public bool disposed { get; set; }

        DatarcvdEventArgs args = new DatarcvdEventArgs();
        UdpClient newsock;
        IPEndPoint sender;
        byte[] data = new byte[1024];
        byte[] reply = new byte[100];
        IPAddress rcvIP;
        int rcvPort;

        public class DatarcvdEventArgs : EventArgs
        {
            public int xType { get; set; }
            public String xFreq { get; set; }
            public String xMode { get; set; }
            public String xBand { get; set; }
            public String xDeCall { get; set; }
            public String xDxCall { get; set; }
            public String xDxGrid { get; set; }
            public String xDeGrid { get; set; }
            public String xReport { get; set; }
            public String xRptSent { get; set; }
            public String xTXMode { get; set; }
            public String xTxPwr { get; set; }
            public Boolean xTxEnbl { get; set; }
            public Boolean xTxing { get; set; }
            public Boolean xDecoding { get; set; }
            public String xRxDF { get; set; }
            public String xTxDF { get; set; }
            public String xSubMode { get; set; }
            public String xComments { get; set; }
            public String xName { get; set; }
            public Boolean xWatchDog { get; set; }
            public Boolean xFastMode { get; set; }
            public String xMessage { get; set; }
        }

        public event EventHandler<DatarcvdEventArgs> UDPRcvd;
        protected virtual void OnUDPRcvd(object sender, DatarcvdEventArgs e)
        {
            if (UDPRcvd != null)
            {
                if (!disposed) UDPRcvd(this, e);
            }
        }

        private Int16 bufptr = 0;
        private Int16 ulen = 0;
        private String nextField(Byte[] data, Int16 p)
        {
            // String fields:  \0 \0 len {value} \0 
            // Unint fields: \0 {value} \0
            // bool fields {byte} 1 = true 0 = false
            Encoding ascii = Encoding.ASCII;

            //p += 2;  // skip leading nulls
            // increment pointer to find beginning of next field
            while (BitConverter.ToString(data, p, 1).ToString() == "00")
            {
                p++;

            }

            // now get the length of the new field.
            while (data[p] == 255) p++;
            byte[] c = { 0, 0 };
            c[0] = data[p];
            ulen = BitConverter.ToInt16(c, 0);
            //ulen = Convert.ToInt16(BitConverter.ToString(data, p, 1));
            bufptr = p;
            try
            {
                return ascii.GetString(data, p + 1, ulen);
            }
            catch (Exception e)
            {

                Console.Write(e.ToString());
            }


            return "";

        }
        private String getString(byte[] data, int p)
        {
            // now get the length of the new field.
            Encoding ascii = Encoding.ASCII;
            String ulen = BitConverter.ToString(data, p, 1);
            if (ulen != "FF")
            {
                return ascii.GetString(data, p + 1, Convert.ToInt16(ulen));
            }

            return "";
        }
        private bool getBool(byte b)
        {

            if (b == 0) return false;
            if (b == 1) return true;
            return false;
        }

        private void copybuff(byte[] data, int dstart, int rstart, int length)
        {
            int r = rstart;

            for (int i = dstart; i < dstart + length; i++)
            {
                reply[r] = data[i];
                r++;
            }

        }

        private void parseUDPdata(Byte[] data)
        {
            List<String> ret = new List<string>();
            UnicodeEncoding unicode = new UnicodeEncoding();
            Encoding ascii = Encoding.ASCII;
            Int16 n;
            args.xType = data[11];
            switch (data[11])   // type of message
            {
                case 1:
                    /**Status        Out       1                      quint32
                    * Id(unique key)        utf8
                    * Dial Frequency(Hz)    quint64
                    * Mode                   utf8
                    * DX call utf8
                     *Report                 utf8
                    * Tx Mode utf8
                     *Tx Enabled             bool
                    * Transmitting           bool
                    * Decoding               bool
                    * Rx DF qint32
                     *Tx DF qint32
                     *DE call utf8
                     *DE grid utf8
                     *DX grid utf8
                     *Tx Watchdog            bool
                    * Sub-mode               utf8
                    * Fast mode              bool*/
                    // ----------  status message parse below -------------
                    bufptr = 30;

                    byte[] freq = { data[29], data[28], data[27], data[26] };
                    args.xFreq = BitConverter.ToInt32(freq, 0).ToString();
                    args.xMode = nextField(data, 31);
                    args.xDxCall = nextField(data, (Int16)(bufptr + ulen + 2));
                    args.xReport = nextField(data, (Int16)(bufptr + ulen + 2));
                    args.xTXMode = nextField(data, (Int16)(bufptr + ulen + 2));

                    args.xTxEnbl = getBool(data[(Int16)(bufptr + ulen + 2)]);
                    args.xTxing = getBool(data[(Int16)(bufptr + ulen + 3)]);
                    args.xDecoding = getBool(data[(Int16)(bufptr + ulen + 4)]);
                    if (args.xDecoding)
                    {
                        Console.Write("decoding");
                    }
                    n = (Int16)(bufptr + ulen + 5);
                    byte[] rxDf = { data[n], data[n + 1], data[n + 2], data[n + 3] };
                    args.xRxDF = BitConverter.ToInt32(freq, 0).ToString();
                    n += 4;
                    byte[] txDf = { data[n], data[n + 1], data[n + 2], data[n + 3] };
                    args.xTxDF = BitConverter.ToInt32(freq, 0).ToString();
                    n += 4;
                    args.xDeCall = nextField(data, n);
                    args.xDeGrid = nextField(data, (Int16)(bufptr + ulen + 2));
                    args.xDxGrid = nextField(data, (Int16)(bufptr + ulen + 2));
                    OnUDPRcvd(this, args);
                    break;
                case 2:
                    /**Decode        Out       2                      quint32
                    * Id(unique key)        utf8
                    * New                    bool
                    * Time                   QTime
                    * snr                    qint32
                    * Delta time(S)         float(serialized as double)
                    * Delta frequency(Hz)   quint32
                    * Mode                   utf8
                    * Message                utf8
                    * Low confidence         bool
                    * Off air                bool*/
                    ulen = 0;
                    bufptr = 0;
                    string s_unicode2 = System.Text.Encoding.UTF8.GetString(data);
                    string id = ascii.GetString(data, 0, 5);
                    bufptr = 4;
                    bool newQ = getBool(data[(Int16)(bufptr + ulen)]);
                    bufptr = 40;
                    byte[] qfreq = { data[bufptr + 2], data[bufptr + 1], data[bufptr], data[bufptr - 1] };

                    args.xFreq = BitConverter.ToInt32(qfreq, 0).ToString();
                    //bufptr = 44;
                    //args.xMode = nextField(data, (Int16)(bufptr));
                    bufptr = 49;

                    args.xMessage = nextField(data, (Int16)(bufptr + ulen + 2));

                    /*args.xDxCall = nextField(data, (Int16)(bufptr + ulen + 2));
                    args.xDxGrid = nextField(data, (Int16)(bufptr + ulen + 2));
                    bufptr = (Int16)(bufptr + ulen + 2);
                    while (data[bufptr] == 0) bufptr++;
                    byte[] qfreq = { data[bufptr + 2], data[bufptr + 1], data[bufptr], data[bufptr - 1] };
                    args.xFreq = BitConverter.ToInt32(qfreq, 0).ToString();
                    //bufptr +=4;
                    args.xMode = nextField(data, (Int16)(bufptr + ulen));

                    args.xRptSent = nextField(data, (Int16)(bufptr + ulen + 2));
                    args.xReport = nextField(data, (Int16)(bufptr + ulen + 2));
                    //args.xTxPwr = nextField(data, (Int16)(bufptr + ulen + 2));*/
                    /*for (int i = 0; i < 100; i++)
                    {
                        reply[i] = 0;
                    }
                    copybuff(data, 0, 0, 5);  // copy id to reply

                    copybuff(data, 51, 45, args.xMessage.Length);*/

                    OnUDPRcvd(this, args);
                    break;
                case 3:
                    /**Clear         Out       3                      quint32
                    * Id(unique key)        utf8*/
                    break;
                case 4:
                    break;
                case 5:
                    /**QSO Logged Out       5                      quint32
                    * Id(unique key)        utf8
                    * Date &Time Off QDateTime
                    * DX call utf8
                    * DX grid utf8
                    * Tx frequency(Hz)      quint64
                    * Mode                   utf8
                    * Report sent utf8
                    * Report received utf8
                    * Tx power utf8
                    * Comments               utf8
                    * Name                   utf8
                    * Date &Time On QDateTime
                    * Operator call utf8
                    * My call utf8
                    * My grid utf8
                    */
                    // ----------  status message parse below -------------
                    ulen = 0;
                    bufptr = 34;
                    args.xDxCall = nextField(data, (Int16)(bufptr + ulen + 2));
                    args.xDxGrid = nextField(data, (Int16)(bufptr + ulen + 2));
                    bufptr = (Int16)(bufptr + ulen + 2);
                    while (data[bufptr] == 0) bufptr++;
                    byte[] qsofreq = { data[bufptr + 2], data[bufptr + 1], data[bufptr], data[bufptr - 1] };
                    args.xFreq = BitConverter.ToInt32(qsofreq, 0).ToString();
                    //bufptr +=4;
                    args.xMode = nextField(data, (Int16)(bufptr + ulen));

                    args.xRptSent = nextField(data, (Int16)(bufptr + ulen + 2));
                    args.xReport = nextField(data, (Int16)(bufptr + ulen + 2));
                    //args.xTxPwr = nextField(data, (Int16)(bufptr + ulen + 2));


                    OnUDPRcvd(this, args);
                    break;
                default:
                    break;

            }


        }


        public void UDPServer()
        {

            ThreadStart listen = new ThreadStart(Listen);
            Thread listenthread = new Thread(listen);
            listenthread.IsBackground = true;
            listenthread.Start();
            /*byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 2237);
            UdpClient newsock = new UdpClient(ipep);

            Console.WriteLine("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            data = newsock.Receive(ref sender);

            Console.WriteLine("Message received from {0}:", sender.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = "";
            data = Encoding.UTF8.GetBytes(welcome);
            //newsock.Send(data, data.Length, sender);

            
            while (true)
            {
                
                data = newsock.Receive(ref sender);
                if(data.Length > 100)
                {
                    parseUDPdata(data);
                    
                }


                
                newsock.Send(data, data.Length, sender);
            }*/
        }

        public void SendToWsjt()
        {
            List<byte> byteArray = data.ToList();
            byteArray[11] = 4;
            byteArray.RemoveRange(22, 1);
            Byte[] bytes = byteArray.ToArray();

            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                rcvPort = sender.Port;
                rcvIP = sender.Address;
                IPEndPoint endPoint = new IPEndPoint(rcvIP, rcvPort);
                sock.SendTo(bytes, endPoint);
            }
        }


        private void Listen()
        {

            //byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 2237);
            newsock = new UdpClient(ipep);

            Console.WriteLine("Waiting for a client...");

            sender = new IPEndPoint(IPAddress.Any, 0);
            rcvPort = sender.Port;
            rcvIP = sender.Address;

            data = newsock.Receive(ref sender);

            Console.WriteLine("Message received from {0}:", sender.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = "";
            data = Encoding.UTF8.GetBytes(welcome);
            //newsock.Send(data, data.Length, sender);

            while (true)
            {
                try
                {
                    data = newsock.Receive(ref sender);
                    if (data.Length >= 40)
                    {
                        parseUDPdata(data);

                    }



                    //newsock.Send(data, data.Length, sender);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    //throw;
                }

            }
        }

    }
}
