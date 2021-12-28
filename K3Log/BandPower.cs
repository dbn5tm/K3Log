using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace K3Log
{
    [Serializable]
    public class BandPower : ISerializable
    {
        public string Band { get; set; }
        public string Power { get; set; }
        public string Antenna { get; set; }
        public string PropMode { get; set; }

        public BandPower()
        {

        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Band", Band);
            info.AddValue("Power", Power);
            info.AddValue("Antenna", Antenna);
            info.AddValue("PropMode", PropMode);
        }
       
    }
    [Serializable]
    public class BandPowerCollection : ISerializable
    {
        public List<BandPower> myBandPower { get; set; }
        public BandPowerCollection()
        {
            myBandPower = new List<BandPower>();
        }
        public BandPowerCollection(SerializationInfo info, StreamingContext ctxt)
        {
            myBandPower = (List<BandPower>)info.GetValue("Band", typeof(List<BandPower>));
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BandPower", myBandPower);
        }
    }
}
