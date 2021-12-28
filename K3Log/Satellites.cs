using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace K3Log
{
    [Serializable]
    public class Satellites : ISerializable
    {
        public string Satellite { get; set; }
        public double Uplink { get; set; }
        public double Downlink { get; set; }
        public string Mode { get; set; }
        public string UpMode { get; set; }
        public string DownMode { get; set; }
        public string PLTone { get; set; }
        public double UpDoppler { get; set; }
        public double DownDoppler { get; set; }
        public bool Invert { get; set; }

        public Satellites()
        {

        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Satellite", Satellite);
            info.AddValue("Uplink", Uplink);
            info.AddValue("Downlink", Downlink);
            info.AddValue("UpMode", UpMode);
            info.AddValue("DownMode", DownMode);
            info.AddValue("Mode", Mode);
            info.AddValue("PLTone", PLTone);
            info.AddValue("UpDoppler", UpDoppler);
            info.AddValue("DownDoppler", DownDoppler);
            info.AddValue("Invert", Invert);  // causes doppler RX correction to move same as TX
 
        }

    }
    [Serializable]
    public class SatelliteCollection : ISerializable
    {
        public List<Satellites> mySatellites { get; set; }
        public SatelliteCollection()
        {
            mySatellites = new List<Satellites>();
        }
        public SatelliteCollection(SerializationInfo info, StreamingContext ctxt)
        {
            mySatellites = (List<Satellites>)info.GetValue("Satellites", typeof(List<Satellites>));
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Satellites", mySatellites);
        }
    }
}
