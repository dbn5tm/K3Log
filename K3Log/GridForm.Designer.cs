namespace K3Log
{
    partial class GridForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvGrids = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GridcolorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrids)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvGrids
            // 
            this.dgvGrids.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGrids.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGrids.Location = new System.Drawing.Point(0, 24);
            this.dgvGrids.Name = "dgvGrids";
            this.dgvGrids.Size = new System.Drawing.Size(1223, 716);
            this.dgvGrids.TabIndex = 3;
            this.dgvGrids.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvGrids_CellMouseClick);
            this.dgvGrids.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvGrids_CellMouseDown);
            this.dgvGrids.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGrids_CellMouseEnter);
            this.dgvGrids.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGrids_CellMouseLeave);
            this.dgvGrids.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvGrids_CellMouseMove);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OptionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1223, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GridcolorsToolStripMenuItem,
            this.gridRangeToolStripMenuItem});
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.OptionsToolStripMenuItem.Text = "Options";
            // 
            // GridcolorsToolStripMenuItem
            // 
            this.GridcolorsToolStripMenuItem.Name = "GridcolorsToolStripMenuItem";
            this.GridcolorsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.GridcolorsToolStripMenuItem.Text = "Colors...";
            this.GridcolorsToolStripMenuItem.Click += new System.EventHandler(this.GridcolorsToolStripMenuItem_Click);
            // 
            // gridRangeToolStripMenuItem
            // 
            this.gridRangeToolStripMenuItem.Name = "gridRangeToolStripMenuItem";
            this.gridRangeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.gridRangeToolStripMenuItem.Text = "Grid Range...";
            this.gridRangeToolStripMenuItem.Click += new System.EventHandler(this.gridRangeToolStripMenuItem_Click);
            // 
            // GridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1223, 740);
            this.Controls.Add(this.dgvGrids);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GridForm";
            this.Text = "GridForm";
            this.Load += new System.EventHandler(this.GridForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrids)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvGrids;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GridcolorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridRangeToolStripMenuItem;
    }
}