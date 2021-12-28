using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3Log
{
    [Serializable]
    public class Remotes
    {
        public string Station { get; set; }
        public string Grid { get; set; }
        public string ITUZone { get; set; }
        public string CQZone { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        
        public Remotes()
        {
            
        }
        

    }

    
}
