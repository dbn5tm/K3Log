using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace K3Log
{
    class countryXML
    {

        XDocument xmlDoc;
        public List<Tuple<string, string, string>> Countrylist;


        public countryXML(String fspec)
        {
            try
            {
                xmlDoc = XDocument.Load(fspec);
                // first build a list of prefixes and Country Names
                Countrylist = xmlDoc.Element("Countries").Element("CountryList").Elements("Country")
                                       .Select(x => new Tuple<string, string, string>(
                                           x.Element("PrefixList").Value,
                                           x.Element("CountryName").Value,
                                           x.Element("Dxcc").Value)).ToList();
            }
            catch (Exception)
            {

                //throw;
            }
        }


        public String[] CountryName(String DXCC)
        {
            String ctry = "";  // = new List<string>();
            String dxcc = "";
            String[] ret = { "", "" };
            try
            {


                // The Countryxml file uses regular expression with^ and |
                // example from country xml  <PrefixList>^YA.*|^T6.*</PrefixList>
                //  another exmple >^E[A-H]6.*|^A[M-O]6.*|^A[M-O]06.*

                foreach (Tuple<string, string, string> item in Countrylist)
                {

                    Regex prfx = new Regex(item.Item1);

                    if (prfx.IsMatch(DXCC))
                    {
                        ctry = item.Item2;
                        dxcc = item.Item3;
                        break;
                    }

                }


                ret[0] = ctry;
                ret[1] = dxcc;

                return ret;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return ret;
            }


        }




    }
}
