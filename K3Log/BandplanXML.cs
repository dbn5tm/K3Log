using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace K3Log
{
    class BandplanXML
    {
        XDocument xmlDoc;

        public BandplanXML(String fspec)
        {
            try
            {
                xmlDoc = XDocument.Load(fspec);
            }
            catch (Exception)
            {

                //throw;
            }
        }


        public String[] Band(Double freq)
        {
            String[] ret = { "?", "", "", "" };
            try
            {

                IEnumerable<XElement> thisElement = from el in xmlDoc.Element("BandPlan").Elements("Range").Elements("BandPlanRange")
                                                    where (Double)el.Element("Start") <= freq && (Double)el.Element("End") >= freq
                                                    select el;

                ret[0] = thisElement.Single().Element("Band").Value.ToString();
                ret[1] = thisElement.Single().Element("EmissionType").Value.ToString();

                return ret;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return ret;
            }


        }



        public void SaveChannelXML(string fspec)
        {
            xmlDoc.Save(fspec);
        }
    }
}
