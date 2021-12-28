using System;
using System.Windows.Forms;

namespace K3Log
{
    public partial class Countries : Form
    {
        countryXML countrylist = new countryXML("E:\\Documents\\N5TM_DB\\country.xml");

        public Countries()
        {
            InitializeComponent();
        }


        private void FillCountriesGrid()
        {
            foreach (Tuple<string, string, string> ctry in countrylist.Countrylist)
            {
                dgvCountries.Rows.Add(new object[] { ctry.Item1, ctry.Item2, ctry.Item3 });
            }
        }

        private void Countries_Load(object sender, EventArgs e)
        {
            FillCountriesGrid();
        }
    }
}
