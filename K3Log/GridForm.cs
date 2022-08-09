using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K3Log
{
    public partial class GridForm : Form
    {
        string[] bordergrids = { "EL84","EL94", "EL95", "EL96", "EL97", "EL98", "EL99"
                                ,"EL86", "EL87", "EL88","EL89", "EL79" 
                                , "EM70", "EM60", "EM50", "EL59"
                                ,"EL58", "EL49", "EL39","EL29", "EL28", "EL18"
                                ,"EL17", "EL16", "EL15","EL06", "EL07", "EL08"
                                ,"DL98", "DL88","DL89", "DL79", "DM70"
                                ,"DM71", "DM61", "DM51","DM41", "DM31", "DM32"
                                ,"DM22", "DM12", "DM02", "CM94", "CM93"
                                ,"CM95", "CM87", "CM88", "CM96", "CM86"
                                ,"CM89", "CM79", "CN70","CN71", "CN72"
                                ,"CN73", "CN74", "CN76", "CN75", "CN77"
                                ,"CN78", "CN88", "CN98","DN08", "DN18", "DN28"
                                ,"DN38", "DN48", "DN58","DN68", "DN78", "DN88"
                                ,"DN98", "EN08", "EN18","EN28", "EN29", "EN38"
                                ,"EN48", "EN57", "EN58", "EN67","EN66", "EN76", "EN86"
                                ,"EN85", "EN84", "EN83","EN82", "EN81", "EN91"
                                ,"EN92", "FN02", "FN03","FN13", "FN14", "FN25"
                                ,"FN35", "FN44", "FN45","FN46", "FN56", "FN57"
                                ,"FN67", "FN66", "FN65","FN64", "FN54", "FN53"
                                ,"FN43", "FN42", "FN51","FN41", "FN30"
                                ,"FM29", "FM28", "FM29","FM26", "FM25", "FM14", "FM13", "FM15", "FM27"
                                ,"FM03", "FM02", "EM92","EM91", "EM90"};

        //List<string> markedgrids = new List<string>();
        string lowerleftGrid; 
        string upperrightGrid; 

        public List<String> myWorkedGrids;
        public List<String> myWorkedCallInGrid;
        public List<String> myWorkedDateInGrid;
        public List<String> FFMAGrids = new List<string>();

        List<gridspot> grids = new List<gridspot>();

        Color WorkedGridColor;
        Color NewGridSpotColor;
        Color WorkedSpotColor;
        Color BoundaryColor;
        Color InitialSpotColor;
        Popup gridToolTip;
        GridToolTip gridCustomToolTip;

        public GridForm()
        {
            InitializeComponent();
            gridToolTip = gridToolTip = new Popup(gridCustomToolTip = new GridToolTip());
        }

        private void GridForm_Load(object sender, EventArgs e)
        {
            lowerleftGrid = Properties.Settings.Default.LowerLeftGrid;
            upperrightGrid = Properties.Settings.Default.UpperRightGrid;
            WorkedGridColor = Properties.Settings.Default.WorkedGridColor;
            NewGridSpotColor = Properties.Settings.Default.NewGridColor;
            WorkedSpotColor = Properties.Settings.Default.WorkedSpotColor;
            BoundaryColor = Properties.Settings.Default.BoundaryColor;
            InitialSpotColor = Properties.Settings.Default.InitalSpotColor;
            initdgv();
            int ffmanum = FFMAGridsCount();
            this.Text = "FFMA Grids Worked: " + ffmanum.ToString(); 
        }
        
        public void highlitegrid(string grid, string spottype, string info)
        {
            Color highlite = Color.White;
            if (spottype == "NewGrid") highlite = NewGridSpotColor;
            if(spottype == "WorkedSpot") highlite = WorkedSpotColor;
            if (spottype == "InitialSpot") highlite = InitialSpotColor;
            if (spottype == "WorkedGrid") highlite = WorkedGridColor;
            if (spottype == "Rover") highlite = Color.BlueViolet;

            gridspot gr = grids.Find(x => x.grid == grid);
            if (gr != null)
            {
                if (gr.spottype != spottype)
                {
                    gr.spottype = spottype;
                    gr.timestamp = DateTime.Now;
                    newGridColor(grid, highlite);
                    gr.info = info;
                }
            }
            else
            {
                // this grid not found in the list
                
                addTogridsList(grid, spottype, info);  // do not repeat for same grid
                                                 
                if (grid != "")
                {
                    newGridColor(grid, highlite);
                    
                }

            }

        }

        private void newGridColor(string grid, Color highlite)
        {
            for (int r = 0; r < dgvGrids.Rows.Count - 1; r++)
            {
                //string searchrow = dgvGrids.Rows[r].Cells[1].Value.ToString();
                //if (searchrow.Substring(1, 1) == grid.Substring(1, 1) && searchrow.Substring(3, 1) == grid.Substring(3, 1))
                //{
                for (int c = 1; c < dgvGrids.Rows[r].Cells.Count; c++)
                {
                    if (dgvGrids.Rows[r].Cells[c].Value.ToString() == grid)
                    {
                        dgvGrids.Rows[r].Cells[c].Style.BackColor = highlite;
                        break;
                    }
                }
                //break;
                //}
            }
        }
        private int FFMAGridsCount()
        {
            loadFFMAGridsList();
            List<String> FFMAGridsWorked = new List<String>();  
            
            foreach (string g in myWorkedGrids)
            {
                if (FFMAGrids.Contains(g)) FFMAGridsWorked.Add(g);
            }
            return FFMAGridsWorked.Count();
        }
        private void loadFFMAGridsList()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "K3Log\\FFMAGrids.txt");
            FFMAGrids = File.ReadAllLines(fileName).ToList();
        }
        public static IEnumerable<string> GetColumns(char startchar, char endchar)
        {
            string s = null;
            //for (char c2 = startchar; c2 <= endchar + 1; c2++)
            //{
                for (char c = startchar; c <= endchar; c++)
                {
                    yield return s + c;
                }
                //s = c2.ToString();
            //}
        }
        private void initdgv()
        {
            char mlongstart = lowerleftGrid.Substring(0, 1).ToCharArray()[0];
            char mlatstart = lowerleftGrid.Substring(1, 1).ToCharArray()[0];
            char mlongend = upperrightGrid.Substring(0, 1).ToCharArray()[0];
            char mlatend = upperrightGrid.Substring(1, 1).ToCharArray()[0];
            IEnumerable<string> latgrids = GetColumns(mlatstart, mlatend);
            IEnumerable<string> longgrids = GetColumns(mlongstart, mlongend);

            int mlongnumstart = Convert.ToInt32(lowerleftGrid.Substring(2, 1));
            int mlongnumend = Convert.ToInt32(upperrightGrid.Substring(2, 1));
            int mlatnumstart = Convert.ToInt32(lowerleftGrid.Substring(3, 1));
            int mlatnumend = Convert.ToInt32(upperrightGrid.Substring(3, 1));

            
            string[] longgrid = longgrids.ToArray();  // Array of Maidenhead major Digit West to East
            string[] latgrid = latgrids.Reverse().ToArray();  // Array of Maidenhead major Digit South to North

            for (int k = 0; k < longgrid.Length; k++)
            {
                int startnum = 0;
                int stopnum = 10;
                int startoffset = mlongnumstart;  // start West grids at the begining number in lowerleftGrid
                if (k == 0)
                {
                    startnum = mlongnumstart;
                    startoffset = 0;
                }
                if (k == longgrid.Length -1)
                {
                    stopnum = mlongnumend + 1;  // stop East grids at the ending number in upperrightGrid
                }
                // each major grid column
                for (int i = startnum; i < stopnum; i++)
                {
                    // 10 grid columns created here
                    dgvGrids.Columns.Add(longgrid[k] + i.ToString(), longgrid[k] + latgrid[0] + i.ToString());
                    dgvGrids.Columns[(i - startnum) + ((k * 10) - startoffset)].Width = 38;
                }
            }

            // now add rows for all columns
            for (int NSdesignator = 0; NSdesignator < latgrid.Length; NSdesignator++)
            {
                
                int startnum = 0;
                int stopnum = 10;
                if (NSdesignator == 0)
                {
                    startnum = 10 - (mlatnumend + 1); // start rows at upperrightGrid 
                   
                }
                if(NSdesignator == latgrid.Length -1)
                {
                    stopnum = 10 - mlatnumstart;  // stop rows at lowerleftGrid
                }
                for (int s  = startnum; s < stopnum; s++)
                {


                    DataGridViewRow newrow = new DataGridViewRow();
                    foreach (DataGridViewColumn col in dgvGrids.Columns)
                    {
                        
                        var codecell = new DataGridViewTextBoxCell();
                        codecell.Value = col.HeaderText.Substring(0,1) + latgrid[NSdesignator] + col.HeaderText.Substring(2, 1) + ((9 - s) % 10).ToString();
                        
                        if (bordergrids.Contains(codecell.Value))
                        {
                            codecell.Style.BackColor = Color.LightGreen;
                            codecell.Style.ForeColor = Color.Blue;
                            if (myWorkedGrids.Contains(codecell.Value))
                            {
                                codecell.Style.BackColor = WorkedGridColor;
                                addTogridsList(codecell.Value.ToString(), "WorkedGrid", "");
                            }
                        }
                        else
                        {
                            if (myWorkedGrids.Contains(codecell.Value))
                            {
                                codecell.Style.BackColor = WorkedGridColor;
                                addTogridsList(codecell.Value.ToString(), "WorkedGrid", "");
                            }
                        }
                        
                        
                        newrow.Cells.Add(codecell);
                        

                    }
                    dgvGrids.Rows.Add(newrow);
                }
            }
            
            
        }

        private void addTogridsList(string grid, string type, string info)
        {
            gridspot mGrid = new gridspot();
            mGrid.grid = grid;
            mGrid.spottype = type;
            mGrid.info = info;
            mGrid.timestamp = DateTime.Now;
            grids.Add(mGrid);
            mGrid = null;
        }
        private void GridcolorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridColors SetColors = new GridColors();
            SetColors.Show();
        }

        private void gridRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridRange MapRange = new GridRange();
            MapRange.Show();
        }
        internal class gridspot
        {
            public string grid;
            public string spottype;
            public string info;
            public DateTime timestamp;
        }

        private void dgvGrids_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            /*try
            {
                string thisgrid = dgvGrids.SelectedCells[0].Value.ToString();
                gridspot thisspot = grids.Find(item => item.grid == thisgrid);
                gridCustomToolTip.Controls[1].Text = string.Format("Grid: {0}, Call: {1}", thisgrid, thisspot.info);
                gridCustomToolTip.Controls[0].Text = thisspot.timestamp.ToString();
                //gridCustomToolTip.Controls[1].Text = string.Format("Column: {0}, Row: {1}", e.ColumnIndex, e.RowIndex);
                if (!gridToolTip.Visible)
                {
                    Rectangle rect = dgvGrids.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    gridToolTip.Show(dgvGrids, rect);
                }
            }
            catch (Exception)
            {

                //throw;
            }*/
            
            
        }

        private void dgvGrids_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            //gridCustomToolTip.Controls[0].Text = "Mouse pointer location: " + e.Location.ToString();
        }

        private void dgvGrids_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            gridToolTip.Close();
        }

        private void dgvGrids_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            try
            {
                
                
                 // show callsign and timestamp for spot
                string thisgrid = dgvGrids.SelectedCells[0].Value.ToString();
                gridspot thisspot = grids.Find(item => item.grid == thisgrid);
                // if not a spot, go get the worked grid callsign
                if(thisspot.info == "")
                {
                    int callindex = myWorkedGrids.FindIndex(a => a.Contains(thisgrid));
                    thisspot.info = myWorkedCallInGrid[callindex];
                    thisspot.timestamp = Convert.ToDateTime(myWorkedDateInGrid[callindex]);
                }
                
                gridCustomToolTip.Controls[1].Text = string.Format("Grid: {0}, Call: {1}", thisgrid, thisspot.info);
                gridCustomToolTip.Controls[0].Text = thisspot.timestamp.ToString();
                if (!gridToolTip.Visible)
                {
                    Rectangle rect = dgvGrids.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    gridToolTip.Show(dgvGrids, rect);
                }
 
            }
            catch (Exception)
            {

                //throw;
            }
            
            
            
        }

        private void dgvGrids_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                    string thisgrid = c.Value.ToString();
                    // add rover to grid and color change
                    RoverDialog testDialog = new RoverDialog();

                    // Show testDialog as a modal dialog and determine if DialogResult = OK.
                    if (testDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        // Read the contents of testDialog's TextBox.
                        string result = testDialog.txtRoverCallsign.Text;
                        //string thisgrid = dgvGrids.SelectedCells[0].Value.ToString();
                        //gridspot thisspot = grids.Find(item => item.grid == thisgrid);
                        highlitegrid(thisgrid, "Rover", result);
                    }
                    else
                    {
                        //this.txtResult.Text = "Cancelled";
                    }
                    testDialog.Dispose();
                }

            }
        }
    }


}
