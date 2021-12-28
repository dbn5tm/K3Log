using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace K3Log
{
    class Winkey
    {
        public String Port { get; set; }
        public byte Output = 0;
        SerialPort Keyer;
        bool isOpen = false;
        private String buffin;
        public bool disposed { get; set; }
        private bool paused = false;
        byte[] buff = new byte[32];
        public int bufptr = 0;
        //private System.Timers.Timer cmdtimer = new System.Timers.Timer(500);
        public WinkeyEventArgs args = new WinkeyEventArgs();
        public event EventHandler<WinkeyEventArgs> WinkeyRcvd;
        public class WinkeyEventArgs : EventArgs
        {
            public string text;
            public string wpm;
        }
        protected virtual void OnWinkeyRcvd(object sender, WinkeyEventArgs e)
        {
            if (WinkeyRcvd != null)
            {
                if (!disposed) WinkeyRcvd(this, e);
            }
        }
        public Winkey(String port)
        {
            Port = port;
            //cmdtimer.Elapsed += new System.Timers.ElapsedEventHandler(timerTick);
            //cmdtimer.Enabled = false;
        }

        public bool InitSerialPort()
        {
            try
            {
                Keyer = new SerialPort(Port, 1200, Parity.None, 8, StopBits.One);
                
                Keyer.DtrEnable = false;
                Keyer.RtsEnable = false;
                Keyer.ReadTimeout = 500;
                Keyer.WriteTimeout = 500;
                Keyer.Open();
                isOpen = true;
                byte[] buff = { 0x13, 0x13, 0x13 };
                Keyer.Write(buff, 0, 3);
                System.Threading.Thread.Sleep(200);
                buff[0] = 0x00;
                buff[1] = 4;
                buff[2] = 0x55;
                Keyer.Write(buff, 0, 3);
                System.Threading.Thread.Sleep(200);
                buff[0] = 0x00;     // WK admin command
                buff[1] = 2;        // Host open, WK will now receive commands and Morse characters
                Keyer.Write(buff, 0, 2);
                System.Threading.Thread.Sleep(200);
                receiveInit();
                Keyer.DataReceived += new SerialDataReceivedEventHandler(SerialDataHandler);
                //cmdtimer.Enabled = true;
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return false;
            }

        }

        void receiveInit()
        {
            if (Keyer.IsOpen)
            {
                int dataLength = Keyer.BytesToRead;
                byte[] data = new byte[dataLength];
                int nbrDataRead = Keyer.Read(data, 0, dataLength);
                if (nbrDataRead == 0)
                    return;
                System.Threading.Thread.Sleep(200);
                buffin += BytesToString(data);

                Console.Write(buffin.ToString());
                openWinkey2();
                
            }
        }

        void openWinkey2()
        {
            byte[] buff = { 0, 0, 0, 0 };
            
            //  set some saved user settings
            // set sidetone config
            buff[0] = 0x01;     // Sidetone control command, next byte sets sidetone parameters
            buff[1] = 0;

            // Paddle sidetone only?  Set bit 7 (msb) of buff[1]
            if (Properties.Settings.Default.WKSidetone)
            {
                buff[1] += 128;
                
                // Set sidetone frequency (chosen in GUI)
                buff[1] += Properties.Settings.Default.WKSToneFreq;
            }
            
            Keyer.Write(buff, 0, 2);
            System.Threading.Thread.Sleep(100);
            
            // set other winkey features
            buff[0] = 0x0e;     // Set WK options command, next byte sets WK options
            buff[1] = 4;        // enables serial echoback

            // CT spacing?  Set bit 0 (lsb) of buff[1]
            
            if(Properties.Settings.Default.WKCTSpace) buff[1] += 1;
           
            // Paddle swap?  Set bit 3 of buff[1]
            if(Properties.Settings.Default.WKRevPdl) buff[1] += 8;
            
            // Paddle mode, set bits 5,4 to bit mask, 00 = iambic B, 01 = iambic A,
            // 10 = ultimatic, 11 = bug
            
            buff[1] += (byte)(Properties.Settings.Default.WKPdlMode << 4);
            
            Keyer.Write(buff, 0, 2);
            System.Threading.Thread.Sleep(200);
           
            // Pot min/max
            // must set this up or paddle speed screwed up.
            buff[0] = 0x05;     // Setup speed pot command, next three bytes setup the speed pot
            buff[1] = 0x12;       // min wpm
            buff[2] = 0x18;       // wpm range (min wpm + wpm range = wpm max)
            buff[3] = 0;        // Used only on WK1 keyers (does 0 cause a problem on WK1?)
            Keyer.Write(buff, 0, 4);
            System.Threading.Thread.Sleep(200);
            // set speed wpm to 0 for initial pot value
            buff[0] = 0x02;
            buff[1] = 0;
            Keyer.Write(buff, 0, 2);
            System.Threading.Thread.Sleep(200);

            // set output port 0 or 1
            buff[0] = 0x1d;
            buff[1] = Output;
            Keyer.Write(buff, 0, 2);
            System.Threading.Thread.Sleep(200);

        }
        public void setOutput(byte i)
        {
            if (Keyer.IsOpen)
            {
                byte[] buff = { 0, 0, 0, 0 };
                buff[0] = 0x1d;
                buff[1] = i;
                Keyer.Write(buff, 0, 2);
                System.Threading.Thread.Sleep(200);
            }
            
        }
        private void SerialDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int dataLength = 0;
            int nbrDataRead = 0;
            args.text = "";
            while (Keyer.BytesToRead > 0)
            {
                dataLength = Keyer.BytesToRead;
                byte[] data = new byte[dataLength];
                nbrDataRead = Keyer.Read(data, 0, dataLength);
                if (IsBitSet(data[0], 7))
                {
                    if (!IsBitSet(data[0], 6))
                    {
                        // wpm pot returned
                        args.wpm = (data[0] - 128 + 0x12).ToString();
                    }
                }
                args.text += BitConverter.ToString(data);
            }
            
            
            OnWinkeyRcvd(this, args);
        }

        private string BytesToString(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response += (Char)b;

            return response;
        }

        public void tune(byte value)
        {
            byte[] buff = { 0x0B, 0x00 };
            buff[1] = value;
            Keyer.Write(buff, 0, buff.Length);
            System.Threading.Thread.Sleep(200);
        }

        public void requestWPM()
        {
            // read speed
            byte[] buff = { 0x0B, 0x00 };
            buff[0] = 0x07;
            buff[1] = 0;
            Keyer.Write(buff, 0, 2);
            
        }
        public void sendcw(String msg)
        {
            buff = ASCIIEncoding.ASCII.GetBytes(msg);
            bufptr = buff.Length;
            Keyer.Write(buff, 0, buff.Length);
            //System.Threading.Thread.Sleep(500);
        }
        public void stop()
        {
            /*byte[] buff = { 0, 0 };

            buff[0] = 0x06;  // Pause
            if (paused)
            {
                buff[1] = 0;
                paused = false;
            }
            else
            {
                buff[1] = 1;
                paused = true;
            }
            
            Keyer.Write(buff, 0, 2);*/

            buff[0] = 0x0a;
            buff[1] = 0;
            Keyer.Write(buff, 0, 2);  // clear buffer.
        }
        public void setspeed(byte bb)  // set speed wpm
        {
            byte[] buff = { 0, 0 };
            
            buff[0] = 0x02;  // speed command
            buff[1] = bb;
            Keyer.Write(buff, 0, 2);
        }
        public void send(String msg)
        {
            if (Keyer.IsOpen)
            {
                Keyer.Write(msg);
            }
            else
            {
                //this.InitSerialPort();
            }

        }

        
        bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

    }
    

}

