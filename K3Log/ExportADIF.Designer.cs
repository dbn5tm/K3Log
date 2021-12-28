
namespace K3Log
{
    partial class ExportADIF
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkTo = new System.Windows.Forms.CheckBox();
            this.chkFromDate = new System.Windows.Forms.CheckBox();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chk2ndFilter = new System.Windows.Forms.CheckBox();
            this.chkFilterBy = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbtnMatch2Like = new System.Windows.Forms.RadioButton();
            this.rbtnMatch2NotEqu = new System.Windows.Forms.RadioButton();
            this.rbtnMatch2Equ = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbtnMatch1Like = new System.Windows.Forms.RadioButton();
            this.rbtnMatch1NotEqu = new System.Windows.Forms.RadioButton();
            this.rbtnMatch1Equ = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbtnOR = new System.Windows.Forms.RadioButton();
            this.rbtnAnd = new System.Windows.Forms.RadioButton();
            this.txtMatch2 = new System.Windows.Forms.TextBox();
            this.txtMatch1 = new System.Windows.Forms.TextBox();
            this.cboField2 = new System.Windows.Forms.ComboBox();
            this.cboField1 = new System.Windows.Forms.ComboBox();
            this.btnToADIFFile = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.LogdataGridView = new System.Windows.Forms.DataGridView();
            this.QsoDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Operator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Call = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Band = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Freq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RstSent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RstRcvd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GridSquare = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Propagation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QsoId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnFillGrid = new System.Windows.Forms.Button();
            this.lblExportProgress = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogdataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkTo);
            this.groupBox1.Controls.Add(this.chkFromDate);
            this.groupBox1.Controls.Add(this.dateTimePickerTo);
            this.groupBox1.Controls.Add(this.dateTimePickerFrom);
            this.groupBox1.Location = new System.Drawing.Point(37, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 126);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Date Filter";
            // 
            // chkTo
            // 
            this.chkTo.AutoSize = true;
            this.chkTo.Location = new System.Drawing.Point(30, 74);
            this.chkTo.Name = "chkTo";
            this.chkTo.Size = new System.Drawing.Size(39, 17);
            this.chkTo.TabIndex = 7;
            this.chkTo.Text = "To";
            this.chkTo.UseVisualStyleBackColor = true;
            // 
            // chkFromDate
            // 
            this.chkFromDate.AutoSize = true;
            this.chkFromDate.Location = new System.Drawing.Point(30, 20);
            this.chkFromDate.Name = "chkFromDate";
            this.chkFromDate.Size = new System.Drawing.Size(49, 17);
            this.chkFromDate.TabIndex = 6;
            this.chkFromDate.Text = "From";
            this.chkFromDate.UseVisualStyleBackColor = true;
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Location = new System.Drawing.Point(30, 97);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerTo.TabIndex = 5;
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Location = new System.Drawing.Point(30, 43);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerFrom.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chk2ndFilter);
            this.groupBox2.Controls.Add(this.chkFilterBy);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.txtMatch2);
            this.groupBox2.Controls.Add(this.txtMatch1);
            this.groupBox2.Controls.Add(this.cboField2);
            this.groupBox2.Controls.Add(this.cboField1);
            this.groupBox2.Location = new System.Drawing.Point(310, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(492, 172);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Field Filter";
            // 
            // chk2ndFilter
            // 
            this.chk2ndFilter.AutoSize = true;
            this.chk2ndFilter.Location = new System.Drawing.Point(64, 120);
            this.chk2ndFilter.Name = "chk2ndFilter";
            this.chk2ndFilter.Size = new System.Drawing.Size(63, 17);
            this.chk2ndFilter.TabIndex = 10;
            this.chk2ndFilter.Text = "Filter By";
            this.chk2ndFilter.UseVisualStyleBackColor = true;
            // 
            // chkFilterBy
            // 
            this.chkFilterBy.AutoSize = true;
            this.chkFilterBy.Location = new System.Drawing.Point(64, 34);
            this.chkFilterBy.Name = "chkFilterBy";
            this.chkFilterBy.Size = new System.Drawing.Size(63, 17);
            this.chkFilterBy.TabIndex = 9;
            this.chkFilterBy.Text = "Filter By";
            this.chkFilterBy.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rbtnMatch2Like);
            this.groupBox5.Controls.Add(this.rbtnMatch2NotEqu);
            this.groupBox5.Controls.Add(this.rbtnMatch2Equ);
            this.groupBox5.Location = new System.Drawing.Point(282, 87);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(60, 75);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            // 
            // rbtnMatch2Like
            // 
            this.rbtnMatch2Like.AutoSize = true;
            this.rbtnMatch2Like.Location = new System.Drawing.Point(6, 56);
            this.rbtnMatch2Like.Name = "rbtnMatch2Like";
            this.rbtnMatch2Like.Size = new System.Drawing.Size(45, 17);
            this.rbtnMatch2Like.TabIndex = 2;
            this.rbtnMatch2Like.Text = "Like";
            this.rbtnMatch2Like.UseVisualStyleBackColor = true;
            // 
            // rbtnMatch2NotEqu
            // 
            this.rbtnMatch2NotEqu.AutoSize = true;
            this.rbtnMatch2NotEqu.Location = new System.Drawing.Point(6, 34);
            this.rbtnMatch2NotEqu.Name = "rbtnMatch2NotEqu";
            this.rbtnMatch2NotEqu.Size = new System.Drawing.Size(34, 17);
            this.rbtnMatch2NotEqu.TabIndex = 1;
            this.rbtnMatch2NotEqu.Text = "!=";
            this.rbtnMatch2NotEqu.UseVisualStyleBackColor = true;
            // 
            // rbtnMatch2Equ
            // 
            this.rbtnMatch2Equ.AutoSize = true;
            this.rbtnMatch2Equ.Checked = true;
            this.rbtnMatch2Equ.Location = new System.Drawing.Point(6, 11);
            this.rbtnMatch2Equ.Name = "rbtnMatch2Equ";
            this.rbtnMatch2Equ.Size = new System.Drawing.Size(31, 17);
            this.rbtnMatch2Equ.TabIndex = 0;
            this.rbtnMatch2Equ.TabStop = true;
            this.rbtnMatch2Equ.Text = "=";
            this.rbtnMatch2Equ.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbtnMatch1Like);
            this.groupBox4.Controls.Add(this.rbtnMatch1NotEqu);
            this.groupBox4.Controls.Add(this.rbtnMatch1Equ);
            this.groupBox4.Location = new System.Drawing.Point(282, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(60, 75);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            // 
            // rbtnMatch1Like
            // 
            this.rbtnMatch1Like.AutoSize = true;
            this.rbtnMatch1Like.Location = new System.Drawing.Point(6, 56);
            this.rbtnMatch1Like.Name = "rbtnMatch1Like";
            this.rbtnMatch1Like.Size = new System.Drawing.Size(45, 17);
            this.rbtnMatch1Like.TabIndex = 2;
            this.rbtnMatch1Like.Text = "Like";
            this.rbtnMatch1Like.UseVisualStyleBackColor = true;
            // 
            // rbtnMatch1NotEqu
            // 
            this.rbtnMatch1NotEqu.AutoSize = true;
            this.rbtnMatch1NotEqu.Location = new System.Drawing.Point(6, 34);
            this.rbtnMatch1NotEqu.Name = "rbtnMatch1NotEqu";
            this.rbtnMatch1NotEqu.Size = new System.Drawing.Size(34, 17);
            this.rbtnMatch1NotEqu.TabIndex = 1;
            this.rbtnMatch1NotEqu.Text = "!=";
            this.rbtnMatch1NotEqu.UseVisualStyleBackColor = true;
            // 
            // rbtnMatch1Equ
            // 
            this.rbtnMatch1Equ.AutoSize = true;
            this.rbtnMatch1Equ.Checked = true;
            this.rbtnMatch1Equ.Location = new System.Drawing.Point(6, 11);
            this.rbtnMatch1Equ.Name = "rbtnMatch1Equ";
            this.rbtnMatch1Equ.Size = new System.Drawing.Size(31, 17);
            this.rbtnMatch1Equ.TabIndex = 0;
            this.rbtnMatch1Equ.TabStop = true;
            this.rbtnMatch1Equ.Text = "=";
            this.rbtnMatch1Equ.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbtnOR);
            this.groupBox3.Controls.Add(this.rbtnAnd);
            this.groupBox3.Location = new System.Drawing.Point(146, 59);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(117, 44);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            // 
            // rbtnOR
            // 
            this.rbtnOR.AutoSize = true;
            this.rbtnOR.Location = new System.Drawing.Point(70, 19);
            this.rbtnOR.Name = "rbtnOR";
            this.rbtnOR.Size = new System.Drawing.Size(41, 17);
            this.rbtnOR.TabIndex = 1;
            this.rbtnOR.Text = "OR";
            this.rbtnOR.UseVisualStyleBackColor = true;
            // 
            // rbtnAnd
            // 
            this.rbtnAnd.AutoSize = true;
            this.rbtnAnd.Checked = true;
            this.rbtnAnd.Location = new System.Drawing.Point(6, 19);
            this.rbtnAnd.Name = "rbtnAnd";
            this.rbtnAnd.Size = new System.Drawing.Size(48, 17);
            this.rbtnAnd.TabIndex = 0;
            this.rbtnAnd.TabStop = true;
            this.rbtnAnd.Text = "AND";
            this.rbtnAnd.UseVisualStyleBackColor = true;
            // 
            // txtMatch2
            // 
            this.txtMatch2.Location = new System.Drawing.Point(348, 118);
            this.txtMatch2.Name = "txtMatch2";
            this.txtMatch2.Size = new System.Drawing.Size(102, 20);
            this.txtMatch2.TabIndex = 5;
            // 
            // txtMatch1
            // 
            this.txtMatch1.Location = new System.Drawing.Point(348, 43);
            this.txtMatch1.Name = "txtMatch1";
            this.txtMatch1.Size = new System.Drawing.Size(102, 20);
            this.txtMatch1.TabIndex = 4;
            // 
            // cboField2
            // 
            this.cboField2.FormattingEnabled = true;
            this.cboField2.Location = new System.Drawing.Point(146, 117);
            this.cboField2.Name = "cboField2";
            this.cboField2.Size = new System.Drawing.Size(118, 21);
            this.cboField2.TabIndex = 1;
            // 
            // cboField1
            // 
            this.cboField1.FormattingEnabled = true;
            this.cboField1.Location = new System.Drawing.Point(146, 32);
            this.cboField1.Name = "cboField1";
            this.cboField1.Size = new System.Drawing.Size(118, 21);
            this.cboField1.TabIndex = 0;
            // 
            // btnToADIFFile
            // 
            this.btnToADIFFile.Location = new System.Drawing.Point(493, 427);
            this.btnToADIFFile.Name = "btnToADIFFile";
            this.btnToADIFFile.Size = new System.Drawing.Size(99, 24);
            this.btnToADIFFile.TabIndex = 6;
            this.btnToADIFFile.Text = "To ADIF File...";
            this.btnToADIFFile.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(598, 429);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(82, 23);
            this.btnExport.TabIndex = 7;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(719, 428);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(73, 23);
            this.Cancel.TabIndex = 8;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // LogdataGridView
            // 
            this.LogdataGridView.AllowUserToOrderColumns = true;
            this.LogdataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogdataGridView.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.LogdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LogdataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QsoDate,
            this.TimeOn,
            this.Operator,
            this.Call,
            this.Mode,
            this.Band,
            this.Freq,
            this.RstSent,
            this.RstRcvd,
            this.Country,
            this.GridSquare,
            this.OpName,
            this.Comment,
            this.Propagation,
            this.QsoId});
            this.LogdataGridView.Location = new System.Drawing.Point(37, 216);
            this.LogdataGridView.Name = "LogdataGridView";
            this.LogdataGridView.Size = new System.Drawing.Size(765, 167);
            this.LogdataGridView.TabIndex = 21;
            // 
            // QsoDate
            // 
            dataGridViewCellStyle1.Format = "G";
            dataGridViewCellStyle1.NullValue = null;
            this.QsoDate.DefaultCellStyle = dataGridViewCellStyle1;
            this.QsoDate.HeaderText = "QSO Date";
            this.QsoDate.Name = "QsoDate";
            this.QsoDate.Width = 70;
            // 
            // TimeOn
            // 
            dataGridViewCellStyle2.Format = "d";
            dataGridViewCellStyle2.NullValue = null;
            this.TimeOn.DefaultCellStyle = dataGridViewCellStyle2;
            this.TimeOn.HeaderText = "End Date";
            this.TimeOn.Name = "TimeOn";
            this.TimeOn.Width = 70;
            // 
            // Operator
            // 
            this.Operator.HeaderText = "Operator";
            this.Operator.Name = "Operator";
            this.Operator.Width = 70;
            // 
            // Call
            // 
            this.Call.HeaderText = "Call";
            this.Call.Name = "Call";
            // 
            // Mode
            // 
            this.Mode.HeaderText = "Mode";
            this.Mode.Name = "Mode";
            this.Mode.Width = 50;
            // 
            // Band
            // 
            this.Band.HeaderText = "Band";
            this.Band.Name = "Band";
            this.Band.Width = 40;
            // 
            // Freq
            // 
            this.Freq.HeaderText = "Freq";
            this.Freq.Name = "Freq";
            this.Freq.Width = 70;
            // 
            // RstSent
            // 
            this.RstSent.HeaderText = "Rcvd";
            this.RstSent.Name = "RstSent";
            this.RstSent.Width = 40;
            // 
            // RstRcvd
            // 
            this.RstRcvd.HeaderText = "Sent";
            this.RstRcvd.Name = "RstRcvd";
            this.RstRcvd.Width = 40;
            // 
            // Country
            // 
            this.Country.HeaderText = "Country";
            this.Country.Name = "Country";
            // 
            // GridSquare
            // 
            this.GridSquare.HeaderText = "Grid";
            this.GridSquare.Name = "GridSquare";
            this.GridSquare.Width = 60;
            // 
            // OpName
            // 
            this.OpName.HeaderText = "Name";
            this.OpName.Name = "OpName";
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            this.Comment.Width = 130;
            // 
            // Propagation
            // 
            this.Propagation.HeaderText = "Prop";
            this.Propagation.Name = "Propagation";
            this.Propagation.Width = 40;
            // 
            // QsoId
            // 
            this.QsoId.HeaderText = "QsoId";
            this.QsoId.Name = "QsoId";
            this.QsoId.ReadOnly = true;
            // 
            // btnFillGrid
            // 
            this.btnFillGrid.Location = new System.Drawing.Point(41, 178);
            this.btnFillGrid.Name = "btnFillGrid";
            this.btnFillGrid.Size = new System.Drawing.Size(75, 23);
            this.btnFillGrid.TabIndex = 22;
            this.btnFillGrid.Text = "Fill";
            this.btnFillGrid.UseVisualStyleBackColor = true;
            this.btnFillGrid.Click += new System.EventHandler(this.btnFillGrid_Click);
            // 
            // lblExportProgress
            // 
            this.lblExportProgress.AutoSize = true;
            this.lblExportProgress.Location = new System.Drawing.Point(57, 400);
            this.lblExportProgress.Name = "lblExportProgress";
            this.lblExportProgress.Size = new System.Drawing.Size(16, 13);
            this.lblExportProgress.TabIndex = 23;
            this.lblExportProgress.Text = "...";
            // 
            // ExportADIF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 464);
            this.Controls.Add(this.lblExportProgress);
            this.Controls.Add(this.btnFillGrid);
            this.Controls.Add(this.LogdataGridView);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnToADIFFile);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ExportADIF";
            this.Text = "ExportADIF";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogdataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkTo;
        private System.Windows.Forms.CheckBox chkFromDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rbtnMatch2Like;
        private System.Windows.Forms.RadioButton rbtnMatch2NotEqu;
        private System.Windows.Forms.RadioButton rbtnMatch2Equ;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbtnMatch1Like;
        private System.Windows.Forms.RadioButton rbtnMatch1NotEqu;
        private System.Windows.Forms.RadioButton rbtnMatch1Equ;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbtnOR;
        private System.Windows.Forms.RadioButton rbtnAnd;
        private System.Windows.Forms.TextBox txtMatch2;
        private System.Windows.Forms.TextBox txtMatch1;
        private System.Windows.Forms.ComboBox cboField2;
        private System.Windows.Forms.ComboBox cboField1;
        private System.Windows.Forms.Button btnToADIFFile;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.CheckBox chkFilterBy;
        private System.Windows.Forms.CheckBox chk2ndFilter;
        private System.Windows.Forms.DataGridView LogdataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn QsoDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Operator;
        private System.Windows.Forms.DataGridViewTextBoxColumn Call;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Band;
        private System.Windows.Forms.DataGridViewTextBoxColumn Freq;
        private System.Windows.Forms.DataGridViewTextBoxColumn RstSent;
        private System.Windows.Forms.DataGridViewTextBoxColumn RstRcvd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Country;
        private System.Windows.Forms.DataGridViewTextBoxColumn GridSquare;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn Propagation;
        private System.Windows.Forms.DataGridViewTextBoxColumn QsoId;
        private System.Windows.Forms.Button btnFillGrid;
        private System.Windows.Forms.Label lblExportProgress;
    }
}