using OmniRig;
using System;
using System.Collections.Generic;

namespace K3Log
{
    class OmniClient
    {
        OmniRig.OmniRigX OmniRigEngine = new OmniRigX();
        RigX Rig;// = new OmniRig.RigX();
        int OurRigNo;

        public bool disposed { get; set; }
        // Constants for enum RigParamX
        const int PM_UNKNOWN = 0x00000001;
        const int PM_FREQ = 0x00000002;
        const int PM_FREQA = 0x00000004;
        const int PM_FREQB = 0x00000008;
        const int PM_PITCH = 0x00000010;
        const int PM_RITOFFSET = 0x00000020;
        const int PM_RIT0 = 0x00000040;
        const int PM_VFOAA = 0x00000080;
        const int PM_VFOAB = 0x00000100;
        const int PM_VFOBA = 0x00000200;
        const int PM_VFOBB = 0x00000400;
        const int PM_VFOA = 0x00000800;
        const int PM_VFOB = 0x00001000;
        const int PM_VFOEQUAL = 0x00002000;
        const int PM_VFOSWAP = 0x00004000;
        const int PM_SPLITON = 0x00008000;
        const int PM_SPLITOFF = 0x00010000;
        const int PM_RITON = 0x00020000;
        const int PM_RITOFF = 0x00040000;
        const int PM_XITON = 0x00080000;
        const int PM_XITOFF = 0x00100000;
        const int PM_RX = 0x00200000;
        const int PM_TX = 0x00400000;
        const int PM_CW_U = 0x00800000;
        const int PM_CW_L = 0x01000000;
        const int PM_SSB_U = 0x02000000;
        const int PM_SSB_L = 0x04000000;
        const int PM_DIG_U = 0x08000000;
        const int PM_DIG_L = 0x10000000;
        const int PM_AM = 0x20000000;
        const int PM_FM = 0x40000000;

        // Constants for enum RigStatusX
        const int ST_NOTCONFIGURED = 0x00000000;
        const int ST_DISABLED = 0x00000001;
        const int ST_PORTBUSY = 0x00000002;
        const int ST_NOTRESPONDING = 0x00000003;
        const int ST_ONLINE = 0x00000004;
        private System.Timers.Timer cmdtimer = new System.Timers.Timer(300);
        
        public DatarcvdEventArgs args = new DatarcvdEventArgs();
        public int Doppler { get; set; }
        public bool enableDoppler { get; set; }
        public bool invertRXDoppler { get; set; }
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
        }

        public event EventHandler<DatarcvdEventArgs> OmniRigData;
        protected virtual void OnOmniRigData(object sender, DatarcvdEventArgs e)
        {
            if (OmniRigData != null)
            {
                if (!disposed) OmniRigData(this, e);
            }
        }

        public OmniClient(int radioNo)
        {
            cmdtimer.Elapsed += new System.Timers.ElapsedEventHandler(timerTick);
            cmdtimer.Enabled = false;
            args.Radio_Num = radioNo;
            enableDoppler = false;
            timerEnable(true);
            SelectRig(radioNo);
        }
        public void timerEnable(bool enabled)
        {
            cmdtimer.Enabled = enabled;
        }
        private void StartOmniRig()
        {
            SelectRig(2);

            RigParamX rmode = GetRigStatus();
        }

        private void SelectRig(int NewRigNo)
        {
            OurRigNo = NewRigNo;
            switch (NewRigNo)
            {
                case 1:
                    Rig = OmniRigEngine.Rig1;
                    break;
                case 2:
                    Rig = OmniRigEngine.Rig2;
                    break;
                default:
                    break;
            }

        }

        public void SetFreq(int freq)
        {
            Rig.Freq = freq;
            Rig.FreqA = freq;
        }
        public void SetMode(string mode)
        {
            OmniRig.RigParamX thismode = RigParamX.PM_CW_U;
            switch (mode)
            {
                case "USB":
                    thismode = RigParamX.PM_SSB_U;
                    break;
                case "LSB":
                    thismode = RigParamX.PM_SSB_L;
                    break;
                case "CW":
                    thismode = RigParamX.PM_CW_U;
                    break;
                case "FM":
                    thismode = RigParamX.PM_FM;
                    break;
            }

            Rig.Mode = thismode;
        }

        public void DialFreq(int hertz, double factor)
        {
            int RigFreq;
            int invert = 1;
            if (this.invertRXDoppler) invert = -1;
            if (Rig.FreqA == 0)  // for radios with only one VFO to read
            {
                RigFreq = Rig.Freq;
                Rig.Freq = RigFreq - Convert.ToInt16(((double)hertz * factor * invert));
            }
            else
            {
                RigFreq = Rig.FreqA;
                Rig.FreqA = RigFreq - Convert.ToInt16(((double)hertz * factor * invert));
            }

        }

        public RigParamX GetRigStatus()
        {
            RigParamX RigMode = Rig.Mode;
            int RigFreq = Rig.Freq;
            return RigMode;
        }
        private void timerTick(object sender, EventArgs e)
        {
            int rigfreq = Rig.Freq;
            args.FA = Convert.ToDouble(rigfreq);
            if(enableDoppler) DialFreq(Doppler, 1);

            switch (Rig.Mode)
            {
                case (OmniRig.RigParamX)PM_SSB_L:
                    args.MD = 1;
                    break;
                case (OmniRig.RigParamX)PM_SSB_U:
                    args.MD = 2;
                    break;
                case (OmniRig.RigParamX)PM_CW_U:
                    args.MD = 7;
                    break;
                case (OmniRig.RigParamX)PM_CW_L:
                    args.MD = 3;
                    break;
                case (OmniRig.RigParamX)PM_DIG_U:
                    args.MD = 8;
                    break;
                case (OmniRig.RigParamX)PM_DIG_L:
                    args.MD = 6;
                    break;
                case (OmniRig.RigParamX)PM_AM:
                    args.MD = 5;
                    break;
                case (OmniRig.RigParamX)PM_FM:
                    args.MD = 4;
                    break;
                default:
                    args.MD = 0;
                    break;

            }
            OnOmniRigData(this, args);  // event returned to calling form
            //Rig.Freq += 10;
        }
    }
}
