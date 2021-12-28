using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K3Log
{
    public partial class GridColors : Form
    {
        public GridColors()
        {
            InitializeComponent();
        }
        

        private void GridColors_Load(object sender, EventArgs e)
        {
            lblBoundaryColor.BackColor = Properties.Settings.Default.BoundaryColor;
            lblInitialSpotColor.BackColor = Properties.Settings.Default.InitalSpotColor;
            lblNewGridSpotColor.BackColor = Properties.Settings.Default.NewGridColor;
            lblWorkedGridColor.BackColor = Properties.Settings.Default.WorkedGridColor;
            lblWorkedSpotColor.BackColor = Properties.Settings.Default.WorkedSpotColor;
        }

        private void lblWorkedGridColor_Click(object sender, EventArgs e)
        {
            ColorDialog colordlg = new ColorDialog();
            colordlg.ShowDialog();
            lblWorkedGridColor.BackColor = colordlg.Color;
        }

        private void lblBoundaryColor_Click(object sender, EventArgs e)
        {
            ColorDialog colordlg = new ColorDialog();
            colordlg.ShowDialog();
            lblBoundaryColor.BackColor = colordlg.Color;
        }

        private void lblInitialSpotColor_Click(object sender, EventArgs e)
        {
            ColorDialog colordlg = new ColorDialog();
            colordlg.ShowDialog();
            lblInitialSpotColor.BackColor = colordlg.Color;
        }

        private void lblWorkedSpotColor_Click(object sender, EventArgs e)
        {
            ColorDialog colordlg = new ColorDialog();
            colordlg.ShowDialog();
            lblWorkedSpotColor.BackColor = colordlg.Color;
        }

        private void lblNewGridSpotColor_Click(object sender, EventArgs e)
        {
            ColorDialog colordlg = new ColorDialog();
            colordlg.ShowDialog();
            lblNewGridSpotColor.BackColor = colordlg.Color;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BoundaryColor = lblBoundaryColor.BackColor;
            Properties.Settings.Default.InitalSpotColor = lblInitialSpotColor.BackColor;
            Properties.Settings.Default.NewGridColor = lblNewGridSpotColor.BackColor;
            Properties.Settings.Default.WorkedGridColor = lblWorkedGridColor.BackColor;
            Properties.Settings.Default.WorkedSpotColor = lblWorkedSpotColor.BackColor;
            Properties.Settings.Default.Save();
        }
    }
}
