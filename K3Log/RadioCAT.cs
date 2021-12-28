using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace K3Log
{
    public class RadioCAT
    {
        public RadioCAT Instance { get; private set; }
        public bool disposed { get; set; }
        public struct TXList
        {
            public int index;
            public string word;
        }

        public String PrimaryPort { get; set; }
        
        public string VPort { get; set; }
        public int Baud { get; set; }
        public bool isOpen { get; set; }
        public bool VirtisOpen { get; set; }
        public String FA { get; set; }
        public String FB { get; set; }
        public Int16 MD { get; set; }
        public Int16 MDsub { get; set; }
        public int Doppler { get; set; }
        public bool enableDoppler { get; set; }
        public bool invertRXDoppler { get; set; }

        private string Port;
        private double vfoA;
        private double vfoB;

        private String buffin;
        private String Vbuffin;
        private String VsendBuf = "nil";

        public List<TXList> words = new List<TXList>();
        public DatarcvdEventArgs args = new DatarcvdEventArgs();
        
        private int radio_num;
        SerialPort K3Com;
        SerialPort VirtCom;
        Thread rts;
        public bool killthread;

        private System.Timers.Timer cmdtimer = new System.Timers.Timer(100);
        private DateTime startTime = DateTime.Now;
        private string[] sendStr = new string[] { "TQ", "BG" , "FA", "FB", "TB", "MD", "MD$", "BN", "BN$" };
        private int sendStrPointer = 0;
        public void timerEnable(bool enabled)
        {
            cmdtimer.Enabled = enabled;
        }
        public class DatarcvdEventArgs : EventArgs
        {
            public int Radio_Num { get; set; }
            public Double FA { get; set; }
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
            public String VirtualCmd { get; set; }

        }

        public event EventHandler<DatarcvdEventArgs> K3Rcvd;
        protected virtual void OnK3Rcvd(object sender, DatarcvdEventArgs e)
        {
            if (K3Rcvd != null)
            {
                if (!disposed) K3Rcvd(this, e);
            }
        }

        // constructor
        public RadioCAT(Int16 num, String port, int baud)
        {
            radio_num = num;
            args.Radio_Num = num;
            Baud = baud;
            PrimaryPort = port;
            Port = PrimaryPort;
            cmdtimer.Elapsed += new System.Timers.ElapsedEventHandler(timerTick);
            cmdtimer.Enabled = false;
            enableDoppler = false;
        }
        
        ~RadioCAT()
        {
            this.Dispose(false); 
        }
        /// <summary>
        /// The dispose method that implements IDisposable.
        /// </summary>
        public void Dispose()
        {
            //rts.Abort();
            cmdtimer.Enabled = false;
            this.ClosePort();
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The virtual dispose method that allows
        /// classes inherithed from this one to dispose their resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here.
                    cmdtimer.Enabled = false;
                    //this.ClosePort();
                    //VirtCom.Close();
                    //rts.Abort();
                }
                //this.timerEnable(false);
                //this.ClosePort();
                // Dispose unmanaged resources here.
            }

            disposed = true;
        }
        
        public bool changePort(string port, int baud)
        {
            if (K3Com.IsOpen) K3Com.Close();
            PrimaryPort = port;
            Baud = baud;
            return InitSerialPort();
            
        }

        public bool InitSerialPort()
        {
            try
            {
                K3Com = new SerialPort(Port, Baud, Parity.None, 8, StopBits.One);
                K3Com.DataReceived += new SerialDataReceivedEventHandler(SerialDataHandler);
                K3Com.DtrEnable = false;
                K3Com.RtsEnable = false;
                K3Com.ReadTimeout = 500;
                K3Com.WriteTimeout = 500;
                K3Com.Open();
                isOpen = true;
                
                //cmdtimer.Elapsed += new System.Timers.ElapsedEventHandler(timerTick);
                cmdtimer.Enabled = true;
                send("TT1");
                if (radio_num == 0)
                {
                    if (InitMirrorPort()) 
                    { 
                        // don't do if this is remote thread
                    // set up a thread to watch for RTS/DTR and respond
                    ThreadStart childref = new ThreadStart(CalltoRTSThread);
                    rts = new Thread(childref);
                    rts.Start();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                K3Com = null;
                Thread.Sleep(200);
                

                if (isOpen) return true;
                else return false;
            }

        }

        public void CalltoRTSThread()
        {
            while (!killthread)
            {
                if (VirtCom.CtsHolding)
                {
                    K3Com.RtsEnable = true;
                    
                }
                else
                {
                    K3Com.RtsEnable = false;
                }
                
                if (VirtCom.DsrHolding)
                {
                    K3Com.DtrEnable = true;
                }
                else
                {
                    K3Com.DtrEnable = false;
                }
                Thread.Sleep(200);
            }
        
        }

        public bool InitMirrorPort()
        {
            try
            {
                //VPort = "Com15";
                VirtCom = new SerialPort(VPort, Baud, Parity.None, 8, StopBits.One);
                VirtCom.DataReceived += new SerialDataReceivedEventHandler(VirtSerialDataHandler);
                VirtCom.DtrEnable = false;
                VirtCom.RtsEnable = false;
                VirtCom.ReadTimeout = 500;
                VirtCom.WriteTimeout = 500;
                VirtCom.Open();
                VirtisOpen = true;
                VirtCom.Handshake = Handshake.None;
                //cmdtimer.Elapsed += new System.Timers.ElapsedEventHandler(timerTick);
                //cmdtimer.Enabled = true;
                //send("TT1");
                return true;
            }
            catch (Exception e)
            {
                VirtCom = null;

                Console.Write(e.ToString());
                return false;
            }
        }
        public void ClosePort()
        {
            try
            {
                //K3Com.Close();
                //K3Com.Dispose();
                killthread = true;
                cmdtimer.Enabled = false;
                //VirtCom.Close();
            }
            catch (Exception)
            {

                //throw;
            }

        }

        private string BytesToString(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response += (Char)b;

            return response;
        }

        private void SerialDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (K3Com.IsOpen)
            {
                int dataLength = K3Com.BytesToRead;
                byte[] data = new byte[dataLength];
                int nbrDataRead = K3Com.Read(data, 0, dataLength);
                if (nbrDataRead == 0)
                    return;

                buffin += BytesToString(data);

                if (buffin.Contains(';'))
                {
                    Vsend(buffin);
                    //Console.Write(buffin);
                    string[] p = buffin.Split(';');
                    for (int i = 0; i < p.Length; i++)
                    {
                        if (i == p.Length - 1)
                        {
                            if (buffin.Substring(buffin.Length - 1).Contains(';'))
                            {
                                if (p[i] != "" && !p[i].Contains("PS1")) parsebuf(p[i]);
                                buffin = "";
                            }
                            else
                            {
                                buffin = p[i];
                            }
                        }
                        else
                        {
                            if (p[i] != "" && !p[i].Contains("PS1")) parsebuf(p[i]);
                        }
                    }

                }
            }

        }

        private void VirtSerialDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (VirtCom.IsOpen)
            {
                int dataLength = VirtCom.BytesToRead;
                byte[] data = new byte[dataLength];
                int nbrDataRead = VirtCom.Read(data, 0, dataLength);
                if (nbrDataRead == 0)
                    return;

                Vbuffin += BytesToString(data);

                if (Vbuffin.Contains(';'))
                {
                    //Console.Write(buffin);
                    string[] p = Vbuffin.Split(';');
                    for (int i = 0; i < p.Length; i++)
                    {
                        if (i == p.Length - 1)
                        {
                            if (Vbuffin.Substring(Vbuffin.Length - 1).Contains(';'))
                            {
                                if (p[i] != "" && !p[i].Contains("PS1")) Virtparsebuf(p[i]);
                                Vbuffin = "";
                            }
                            else
                            {
                                buffin = p[i];
                            }
                        }
                        else
                        {
                            if (p[i] != "" && !p[i].Contains("PS1")) Virtparsebuf(p[i]);
                        }
                    }

                }
            }
        
        }

        private void Virtparsebuf(string buf)
        {
            args.VirtualCmd = buf;
            VsendBuf = buf;
            if(buf == "U")  // check for tuning knob up down command
            {
                send(buf + "P2;");
                VsendBuf = "nil";
            }
            else
            {
                if(buf == "D")
                {
                    send(buf + "N2;");
                    VsendBuf = "nil";
                }
                else
                {
                    send(buf + ";");
                }
            }
            
            
        }
        private string formatFreq(Double freq)
        {
            /*
            string wholepart = freq.ToString("###.00000");
            int len = wholepart.Length;
            string leftpart = wholepart.Substring(0, len -2);
            string rightpart = wholepart.Substring(len - 2, 2);
            return leftpart + "." + rightpart;
            */
            return (freq * 1000).ToString("###.00");
        }

        private void parsebuf(string buf)
        {
            try
            {
                // if the recieved message matches what as requested by the Virtual Comm then send response to Vsend
                if (buf.Contains(VsendBuf.Substring(0, 2)))
                {
                    Vsend(buf + ";");
                    Thread.Sleep(200);
                    args.VirtualCmd = buf;
                    VsendBuf = "nil";
                    //K3Rcvd(this, args);
                    
                }
                else
                {
                
                    args.TB = "";
                    args.msg = buf + ";\r\n";
                    string K3Cmd = buf.Substring(0, 2);
                    switch (K3Cmd)
                    {
                        case "ID":
                        case "OM":
                        case "IF":
                        case "K2":
                        case "K3":
                        case "RV":
                        case "AI":
                        case "DT":
                        case "BW":
                     
                        case "FA":
                            vfoA = Convert.ToDouble(buf.Split('A')[1]) / 1000000.0;
                            this.FA = formatFreq(vfoA);
                            args.FA = vfoA;
                            Vsend((vfoA * 1000000).ToString("FA00000000000") + ";");
                            break;

                        case "FB":
                            vfoB = Convert.ToDouble(buf.Split('B')[1]) / 1000000.0;
                            this.FB = formatFreq(vfoB);
                            args.FB = vfoB;
                            Vsend((vfoB * 1000000).ToString("FB00000000000") + ";");
                            break;

                        case "MD":
                            String ModeNum;
                            Int16 mode;

                            if (buf.Contains('$'))
                            {
                                ModeNum = buf.Substring(3);
                                mode = Convert.ToInt16(ModeNum);
                                this.MD = mode;
                                args.MDsub = mode;
                                Vsend((MD).ToString("MD0") + ";");
                            }
                            else
                            {
                                ModeNum = buf.Substring(2);
                                mode = Convert.ToInt16(ModeNum);
                                this.MDsub = mode;
                                args.MD = mode;
                            }
                            break;

                        case string i when i.Substring(0, 2) == "BN":
                            String BandNum;
                            Int16 band;

                            if (buf.Contains('$'))
                            {
                                BandNum = buf.Substring(3);
                                band = Convert.ToInt16(BandNum);
                                args.BNsub = band;
                            }
                            else
                            {
                                BandNum = buf.Substring(2);
                                band = Convert.ToInt16(BandNum);
                                args.BN = band;
                            }
                            break;

                        case string i when i.Substring(0, 2) == "TB":
                            Int16 bufcnt = Convert.ToInt16(buf.Substring(2, 3));

                            int len = Convert.ToInt16(buf.Substring(3, 2));
                            if (len > 0) args.TB = buf.Substring(5, len);
                            args.rxBufcnt = len;
                            args.txBufcnt = Convert.ToInt16(buf.Substring(2, 1));
                            break;

                        default:
                            //Vsend(buf + ";");
                            //args.VirtualCmd = buf;
                            break;
                    }


                    // using events to notify MainForm works, but causes a large number of CLR heap calls. so for now

                    K3Rcvd(this, args);
                }
            }
            catch (Exception e)
            {

                Console.Write(e.ToString());
            }
        }
        public void DialFreq(int hertz, double factor)
        {
            int newFreq;
            if (this.invertRXDoppler)
            {
                newFreq = Convert.ToInt32((vfoA * 1000000) + ((double)hertz * factor));
            }
            else
            {
                newFreq = Convert.ToInt32((vfoA * 1000000) - ((double)hertz * factor));
            }
            
            string frq = newFreq.ToString("FA00000000000");
            this.send(frq + ";");
        }
        private void timerTick(object sender, EventArgs e)
        {
            if (enableDoppler)
            {
                //Double elapsedMillisecs = ((TimeSpan)(DateTime.Now - startTime)).TotalMilliseconds;
                //if (elapsedMillisecs > 300)
                //{
                DialFreq(Doppler, 1);
                startTime = DateTime.Now;
                //}
            }


            if (words.Count > 0)
            {
                args.wordindex = words[0].index;

                send("KY " + words[0].word + ";");
                words.RemoveAt(0);

                if (words.Count > 0)
                {
                    if (words[0].word.Contains("EOL"))
                    {
                        send("RX;");
                    }
                }


            }

            System.Timers.ElapsedEventArgs tick = (System.Timers.ElapsedEventArgs)e;
            if(VsendBuf == "nil")
            {
                // send the next request to the k3
                
                send(sendStr[sendStrPointer] + ";");
                sendStrPointer += 1;
                if (sendStrPointer > 8) sendStrPointer = 0;
                //send("TQ;BG;FA;FB;TB;MD;MD$;BN;BN$;");
                args.VirtualCmd = "";
            }
            //else
            //{
                //args.VirtualCmd = VsendBuf;
                //send(VsendBuf);
                //VsendBuf = "";
                //Thread.Sleep(500);
            //}
            
            

        }    

        public void sendKYList(List<TXList> wordList)
        {
            words = wordList;
        }

        public void sendKYWord(string word)
        {
            send("KY " + word + ";");
            
        }
        //public void sendKYmsg(String KYmsg)
        //{

        //    string[] wordarray = KYmsg.Split(' ');
        //    foreach(string s in wordarray)
        //    {
        //        words.Add(s + " ");
        //    }
        //}

        public void send(String msg)
        {
            try
            {
                if (K3Com.IsOpen)
                    {
                        K3Com.Write(msg);
                    }
                    else
                    {
                        //this.InitSerialPort();
                    }
                    }
            catch (Exception)
            {

                //throw;
            }
            

        }
        public void Vsend(String msg)
        {
            try
            {
                if(VirtCom != null)
                {
                    if (VirtCom.IsOpen)
                    {
                        VirtCom.Write(msg);
                    }
                    else
                    {
                        //this.InitSerialPort();
                    }
                }
                
            }
            catch (Exception)
            {

                //throw;
            }
            

        }


    }
}
