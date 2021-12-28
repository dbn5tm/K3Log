namespace K3Log
{
    partial class Cluster
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
            this.DeleteURLButton = new System.Windows.Forms.Button();
            this.AddDXClusterButton = new System.Windows.Forms.Button();
            this.TelNetURLComboBox = new System.Windows.Forms.ComboBox();
            this.SHDXButton = new System.Windows.Forms.Button();
            this.cmdSend = new System.Windows.Forms.Button();
            this.DXDataGridView = new System.Windows.Forms.DataGridView();
            this.DXTabControl = new System.Windows.Forms.TabControl();
            this.DXSpots = new System.Windows.Forms.TabPage();
            this.Console = new System.Windows.Forms.TabPage();
            this.txtRecv = new System.Windows.Forms.TextBox();
            this.PostButton = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.CommentTextBox = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.FreqTextBox = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.DXTextBox = new System.Windows.Forms.TextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btntest = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnATNO = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnModeWrkd = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.btnShowFilters = new System.Windows.Forms.Button();
            this.btnclr = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DXDataGridView)).BeginInit();
            this.DXTabControl.SuspendLayout();
            this.DXSpots.SuspendLayout();
            this.Console.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DeleteURLButton
            // 
            this.DeleteURLButton.Location = new System.Drawing.Point(240, 19);
            this.DeleteURLButton.Name = "DeleteURLButton";
            this.DeleteURLButton.Size = new System.Drawing.Size(57, 20);
            this.DeleteURLButton.TabIndex = 35;
            this.DeleteURLButton.Text = "Delete";
            this.DeleteURLButton.UseVisualStyleBackColor = true;
            // 
            // AddDXClusterButton
            // 
            this.AddDXClusterButton.Location = new System.Drawing.Point(182, 19);
            this.AddDXClusterButton.Name = "AddDXClusterButton";
            this.AddDXClusterButton.Size = new System.Drawing.Size(45, 20);
            this.AddDXClusterButton.TabIndex = 34;
            this.AddDXClusterButton.Text = "Add";
            this.AddDXClusterButton.UseVisualStyleBackColor = true;
            // 
            // TelNetURLComboBox
            // 
            this.TelNetURLComboBox.FormattingEnabled = true;
            this.TelNetURLComboBox.Items.AddRange(new object[] {
            "dxusa.net",
            "K1RFI.com",
            "dx.w1nr.net",
            "dx.middlebrook.ca:8000"});
            this.TelNetURLComboBox.Location = new System.Drawing.Point(24, 20);
            this.TelNetURLComboBox.Name = "TelNetURLComboBox";
            this.TelNetURLComboBox.Size = new System.Drawing.Size(152, 21);
            this.TelNetURLComboBox.TabIndex = 33;
            // 
            // SHDXButton
            // 
            this.SHDXButton.Location = new System.Drawing.Point(127, 48);
            this.SHDXButton.Name = "SHDXButton";
            this.SHDXButton.Size = new System.Drawing.Size(67, 26);
            this.SHDXButton.TabIndex = 32;
            this.SHDXButton.Text = "Show DX";
            this.SHDXButton.UseVisualStyleBackColor = true;
            this.SHDXButton.Click += new System.EventHandler(this.SHDXButton_Click);
            // 
            // cmdSend
            // 
            this.cmdSend.Location = new System.Drawing.Point(24, 48);
            this.cmdSend.Name = "cmdSend";
            this.cmdSend.Size = new System.Drawing.Size(74, 27);
            this.cmdSend.TabIndex = 31;
            this.cmdSend.Text = "Connect";
            this.cmdSend.UseVisualStyleBackColor = true;
            this.cmdSend.Click += new System.EventHandler(this.cmdSend_Click);
            // 
            // DXDataGridView
            // 
            this.DXDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DXDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DXDataGridView.Location = new System.Drawing.Point(2, 2);
            this.DXDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.DXDataGridView.Name = "DXDataGridView";
            this.DXDataGridView.RowTemplate.Height = 24;
            this.DXDataGridView.Size = new System.Drawing.Size(669, 190);
            this.DXDataGridView.TabIndex = 0;
            this.DXDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DXDataGridView_CellClick);
            // 
            // DXTabControl
            // 
            this.DXTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DXTabControl.Controls.Add(this.DXSpots);
            this.DXTabControl.Controls.Add(this.Console);
            this.DXTabControl.Location = new System.Drawing.Point(12, 118);
            this.DXTabControl.Margin = new System.Windows.Forms.Padding(2);
            this.DXTabControl.Name = "DXTabControl";
            this.DXTabControl.SelectedIndex = 0;
            this.DXTabControl.Size = new System.Drawing.Size(681, 220);
            this.DXTabControl.TabIndex = 36;
            // 
            // DXSpots
            // 
            this.DXSpots.Controls.Add(this.DXDataGridView);
            this.DXSpots.Location = new System.Drawing.Point(4, 22);
            this.DXSpots.Margin = new System.Windows.Forms.Padding(2);
            this.DXSpots.Name = "DXSpots";
            this.DXSpots.Padding = new System.Windows.Forms.Padding(2);
            this.DXSpots.Size = new System.Drawing.Size(673, 194);
            this.DXSpots.TabIndex = 0;
            this.DXSpots.Text = "DX Spots";
            this.DXSpots.UseVisualStyleBackColor = true;
            // 
            // Console
            // 
            this.Console.Controls.Add(this.txtRecv);
            this.Console.Location = new System.Drawing.Point(4, 22);
            this.Console.Margin = new System.Windows.Forms.Padding(2);
            this.Console.Name = "Console";
            this.Console.Padding = new System.Windows.Forms.Padding(2);
            this.Console.Size = new System.Drawing.Size(659, 182);
            this.Console.TabIndex = 1;
            this.Console.Text = "Console";
            this.Console.UseVisualStyleBackColor = true;
            // 
            // txtRecv
            // 
            this.txtRecv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecv.Location = new System.Drawing.Point(2, 2);
            this.txtRecv.Multiline = true;
            this.txtRecv.Name = "txtRecv";
            this.txtRecv.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRecv.Size = new System.Drawing.Size(655, 178);
            this.txtRecv.TabIndex = 15;
            // 
            // PostButton
            // 
            this.PostButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PostButton.Location = new System.Drawing.Point(485, 343);
            this.PostButton.Name = "PostButton";
            this.PostButton.Size = new System.Drawing.Size(174, 25);
            this.PostButton.TabIndex = 43;
            this.PostButton.Text = "Post";
            this.PostButton.UseVisualStyleBackColor = true;
            this.PostButton.Click += new System.EventHandler(this.PostButton_Click);
            // 
            // Label3
            // 
            this.Label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(234, 349);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(51, 13);
            this.Label3.TabIndex = 42;
            this.Label3.Text = "Comment";
            // 
            // CommentTextBox
            // 
            this.CommentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CommentTextBox.Location = new System.Drawing.Point(291, 346);
            this.CommentTextBox.Name = "CommentTextBox";
            this.CommentTextBox.Size = new System.Drawing.Size(180, 20);
            this.CommentTextBox.TabIndex = 41;
            // 
            // Label2
            // 
            this.Label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(123, 350);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(28, 13);
            this.Label2.TabIndex = 40;
            this.Label2.Text = "Freq";
            // 
            // FreqTextBox
            // 
            this.FreqTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FreqTextBox.Location = new System.Drawing.Point(157, 346);
            this.FreqTextBox.Name = "FreqTextBox";
            this.FreqTextBox.Size = new System.Drawing.Size(68, 20);
            this.FreqTextBox.TabIndex = 39;
            // 
            // Label1
            // 
            this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(21, 350);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(22, 13);
            this.Label1.TabIndex = 38;
            this.Label1.Text = "DX";
            // 
            // DXTextBox
            // 
            this.DXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DXTextBox.Location = new System.Drawing.Point(44, 346);
            this.DXTextBox.Name = "DXTextBox";
            this.DXTextBox.Size = new System.Drawing.Size(74, 20);
            this.DXTextBox.TabIndex = 37;
            // 
            // btntest
            // 
            this.btntest.Location = new System.Drawing.Point(315, 16);
            this.btntest.Name = "btntest";
            this.btntest.Size = new System.Drawing.Size(75, 23);
            this.btntest.TabIndex = 44;
            this.btntest.Text = "test";
            this.btntest.UseVisualStyleBackColor = true;
            this.btntest.Click += new System.EventHandler(this.btntest_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.btnATNO);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.btnModeWrkd);
            this.groupBox1.Location = new System.Drawing.Point(264, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 55);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Legend";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Aqua;
            this.button2.Location = new System.Drawing.Point(16, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(57, 22);
            this.button2.TabIndex = 53;
            this.button2.Text = "Wrkd";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // btnATNO
            // 
            this.btnATNO.BackColor = System.Drawing.Color.Red;
            this.btnATNO.Location = new System.Drawing.Point(201, 19);
            this.btnATNO.Name = "btnATNO";
            this.btnATNO.Size = new System.Drawing.Size(57, 22);
            this.btnATNO.TabIndex = 52;
            this.btnATNO.Text = "ATNO";
            this.btnATNO.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Lime;
            this.button1.Location = new System.Drawing.Point(75, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(57, 22);
            this.button1.TabIndex = 51;
            this.button1.Text = "Band";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // btnModeWrkd
            // 
            this.btnModeWrkd.BackColor = System.Drawing.Color.LightYellow;
            this.btnModeWrkd.Location = new System.Drawing.Point(138, 19);
            this.btnModeWrkd.Name = "btnModeWrkd";
            this.btnModeWrkd.Size = new System.Drawing.Size(57, 22);
            this.btnModeWrkd.TabIndex = 49;
            this.btnModeWrkd.Text = "Mode";
            this.btnModeWrkd.UseVisualStyleBackColor = false;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "NA Spots Only",
            "NA DX Only",
            "CW",
            "CW + DATA",
            "DATA",
            "Phone"});
            this.checkedListBox1.Location = new System.Drawing.Point(556, 3);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(103, 94);
            this.checkedListBox1.TabIndex = 50;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // btnShowFilters
            // 
            this.btnShowFilters.Location = new System.Drawing.Point(588, 102);
            this.btnShowFilters.Name = "btnShowFilters";
            this.btnShowFilters.Size = new System.Drawing.Size(71, 27);
            this.btnShowFilters.TabIndex = 51;
            this.btnShowFilters.Text = "show/filter";
            this.btnShowFilters.UseVisualStyleBackColor = true;
            this.btnShowFilters.Click += new System.EventHandler(this.btnShowFilters_Click);
            // 
            // btnclr
            // 
            this.btnclr.Location = new System.Drawing.Point(553, 103);
            this.btnclr.Name = "btnclr";
            this.btnclr.Size = new System.Drawing.Size(29, 26);
            this.btnclr.TabIndex = 52;
            this.btnclr.Text = "clr";
            this.btnclr.UseVisualStyleBackColor = true;
            this.btnclr.Click += new System.EventHandler(this.btnclr_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(537, 377);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(79, 20);
            this.btnClose.TabIndex = 53;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Cluster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 409);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnclr);
            this.Controls.Add(this.btnShowFilters);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btntest);
            this.Controls.Add(this.PostButton);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.CommentTextBox);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.FreqTextBox);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.DXTextBox);
            this.Controls.Add(this.DeleteURLButton);
            this.Controls.Add(this.AddDXClusterButton);
            this.Controls.Add(this.TelNetURLComboBox);
            this.Controls.Add(this.SHDXButton);
            this.Controls.Add(this.cmdSend);
            this.Controls.Add(this.DXTabControl);
            this.Name = "Cluster";
            this.Text = "Cluster";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Cluster_FormClosing);
            this.Load += new System.EventHandler(this.Cluster_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DXDataGridView)).EndInit();
            this.DXTabControl.ResumeLayout(false);
            this.DXSpots.ResumeLayout(false);
            this.Console.ResumeLayout(false);
            this.Console.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button DeleteURLButton;
        internal System.Windows.Forms.Button AddDXClusterButton;
        internal System.Windows.Forms.ComboBox TelNetURLComboBox;
        internal System.Windows.Forms.Button SHDXButton;
        internal System.Windows.Forms.Button cmdSend;
        internal System.Windows.Forms.DataGridView DXDataGridView;
        internal System.Windows.Forms.TabControl DXTabControl;
        internal System.Windows.Forms.TabPage DXSpots;
        internal System.Windows.Forms.TabPage Console;
        internal System.Windows.Forms.TextBox txtRecv;
        internal System.Windows.Forms.Button PostButton;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox CommentTextBox;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox FreqTextBox;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox DXTextBox;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btntest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnATNO;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnModeWrkd;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button btnShowFilters;
        private System.Windows.Forms.Button btnclr;
        private System.Windows.Forms.Button btnClose;
    }
}