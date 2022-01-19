using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace K3Log
{
    class SatTrack
    {
        public bool disposed { get; set; }
        public SatTrackEventArgs args = new SatTrackEventArgs();
        private System.Timers.Timer posTimer = new System.Timers.Timer(1000);
        private double oldrangerate = 0;
        private const double c = 299792458;
        private string[] tleargs1 = { "https://www.amsat.org/amsat/ftp/keps/current/nasabare.txt", "" };
        private string[] tleargs2 = { "https://www.amsat.org/tle/current/nasa.all", "" };
        private string[] tleargs3 = { "http://www.celestrak.com/NORAD/elements/amateur.txt", "(" };
        //private string[] tleargs = { "https://www.amsat.org/tle/current/nasabare.txt", "" };
        private List<TLEelement> SatTLEs = new List<TLEelement>();
        TLEelement thisSat = new TLEelement();
        bool noTLE;
        public void timerEnable(bool enabled)
        {
            posTimer.Enabled = enabled;
        }

        public SatTrack()
        {
            posTimer.Elapsed += new System.Timers.ElapsedEventHandler(timerTick);
            downloadTLE(tleargs1);
            downloadTLE(tleargs3);
        }
        
        public void selectSatellite(string satName)
        {
            thisSat =  SatTLEs.Find(x => x.satname == satName);
            if(thisSat != null)
            {
                posTimer.Enabled = true;
                SatelliteTrack(thisSat);
            }
            else
            {
                args.satellite = "No TLE";
            }
            
            
        }

        public event EventHandler<SatTrackEventArgs> SatTrackEvent;
        protected virtual void OnSatTrack(object sender, SatTrackEventArgs e)
        {
            if (SatTrackEvent != null)
            {
                if (!disposed) SatTrackEvent(this, e);
            }
        }

        public void downloadTLE(string[] args)
        {
            
            if (args == null || args.Length == 0)
            {
                throw new ApplicationException("Specify the URI of the resource to retrieve.");
            }
            WebClient client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.
            try
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                Stream data = client.OpenRead(args[0]);
                StreamReader reader = new StreamReader(data);
                string tle = reader.ReadToEnd();
                    
                //string pattern = "TO ALL RADIO AMATEURS BT";
                //string tle = Regex.Split(s, pattern)[1].TrimStart('\n');
                // purge empty lines 
                string TLEs = Regex.Replace(tle, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
                string[] tleLines = TLEs.Split('\n');
                    
                   
                for (int x = 0; x < tleLines.Length - 3; x += 3)
                {
                    TLEelement newTle = new TLEelement();
                    if(args[1] == "(" && tleLines[x].Contains('('))
                    {
                        try
                        {
                            newTle.satname = tleLines[x].Trim().Split('(')[1].Split(')')[0];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());    
                            //throw;
                        }
                        
                    }
                    else
                    {
                        newTle.satname = tleLines[x].Trim().Replace(" ","-");
                    }
                    
                    newTle.str1 = tleLines[x + 1];
                    newTle.str2 = tleLines[x + 2];
                    if (tleLines[x].Contains("HOPE"))
                    {
                        Console.Write(newTle.satname);
                    }
                    if (!SatTLEs.Contains(newTle))
                    {
                        SatTLEs.Add(newTle);
                    }
                }
                //Console.WriteLine(s);
                data.Close();
                reader.Close();
            }
            catch (Exception Ex)
            {
                    
                Console.Write("could not retrieve TLE file");
            }
                
            
            
        }
        private void SatelliteTrack(TLEelement thisSat)
        {
            if(thisSat == null) { return; }
            Zeptomoby.OrbitTools.Tle tle = new Zeptomoby.OrbitTools.Tle(thisSat.satname, thisSat.str1, thisSat.str2);

            /*Zeptomoby.OrbitTools.Tle tle = new Zeptomoby.OrbitTools.Tle("CAS - 4B",
                "1 42759U 17034B   21035.82191764  .00000248  00000-0  28364-4 0  9992",
                "2 42759  43.0164 190.1606 0009137  45.3901  98.0634 15.10059949201097");*/

            // create a new satellite using TLE
            Zeptomoby.OrbitTools.Satellite sat = new Zeptomoby.OrbitTools.Satellite(tle);
            // Now create a site object. Site objects represent a location on the 
            // surface of the earth.
            Zeptomoby.OrbitTools.Site site = new Zeptomoby.OrbitTools.Site(29.761660, -95.725200, 0);

            Zeptomoby.OrbitTools.Orbit orbit = new Zeptomoby.OrbitTools.Orbit(tle);
            TimeSpan t = orbit.TPlusEpoch(DateTime.Now.ToUniversalTime());

            //Zeptomoby.OrbitTools.EciTime eciSat =  orbit.PositionEci(DateTime.Now);
            Zeptomoby.OrbitTools.EciTime eciSat = sat.PositionEci(t.TotalMinutes);

            Zeptomoby.OrbitTools.Topo topoLook = site.GetLookAngle(eciSat);
            
            args.position = String.Format("AZ: {0:f3}  EL: {1:f3}",
                        topoLook.AzimuthDeg,
                        topoLook.ElevationDeg);
            args.satellite = thisSat.satname;
            args.rangerate = topoLook.RangeRate;

            /* speed of sound 299 792 458 km/s
             *  df = (dv/c) X f
             * */
            args.doppler = ((topoLook.RangeRate - oldrangerate) / c) * (435000000 - 145000000);
            oldrangerate = topoLook.RangeRate;
            OnSatTrack(this, args);
            
        }
        private void timerTick(object sender, EventArgs e)
        {
            SatelliteTrack(thisSat);
        }

    }
    public class SatTrackEventArgs : EventArgs
    {
        
        public String position { get; set; }
        
        public string satellite { get; set; }
        public double rangerate { get; set; }
        public double doppler { get; set; }

    }

    public class TLEelement
    {
        public string satname { get; set; }
        public string str1 { get; set; }
        public string str2 { get; set; }
        
    }
}
