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
    public partial class GridRange : Form
    {
        public GridRange()
        {
            InitializeComponent();
        }

        private void GridRange_Load(object sender, EventArgs e)
        {
            txtLowerLeftGrid.Text = Properties.Settings.Default.LowerLeftGrid;
            txtUpperRightGrid.Text = Properties.Settings.Default.UpperRightGrid;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UpperRightGrid = txtUpperRightGrid.Text;
            Properties.Settings.Default.LowerLeftGrid = txtLowerLeftGrid.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
