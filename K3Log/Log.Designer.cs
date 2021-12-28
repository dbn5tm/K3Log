namespace K3Log
{
    partial class Log
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
            this.LogdataGridView = new System.Windows.Forms.DataGridView();
            this.QSODate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UTC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Operator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Callsign = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Band = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Freq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RstRcvd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RstSent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Propagation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QSOid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.LogdataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // LogdataGridView
            // 
            this.LogdataGridView.AllowUserToOrderColumns = true;
            this.LogdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LogdataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QSODate,
            this.UTC,
            this.Operator,
            this.Callsign,
            this.Mode,
            this.Band,
            this.Freq,
            this.RstRcvd,
            this.RstSent,
            this.Country,
            this.OpName,
            this.Comment,
            this.Propagation,
            this.QSOid});
            this.LogdataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogdataGridView.Location = new System.Drawing.Point(0, 0);
            this.LogdataGridView.Name = "LogdataGridView";
            this.LogdataGridView.Size = new System.Drawing.Size(826, 400);
            this.LogdataGridView.TabIndex = 0;
            // 
            // QSODate
            // 
            this.QSODate.HeaderText = "QSO Date";
            this.QSODate.Name = "QSODate";
            this.QSODate.Width = 70;
            // 
            // UTC
            // 
            this.UTC.HeaderText = "UTC";
            this.UTC.Name = "UTC";
            this.UTC.Width = 70;
            // 
            // Operator
            // 
            this.Operator.HeaderText = "Operator";
            this.Operator.Name = "Operator";
            // 
            // Callsign
            // 
            this.Callsign.HeaderText = "Call";
            this.Callsign.Name = "Callsign";
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
            // RstRcvd
            // 
            this.RstRcvd.HeaderText = "Rcvd";
            this.RstRcvd.Name = "RstRcvd";
            this.RstRcvd.Width = 40;
            // 
            // RstSent
            // 
            this.RstSent.HeaderText = "Sent";
            this.RstSent.Name = "RstSent";
            this.RstSent.Width = 40;
            // 
            // Country
            // 
            this.Country.HeaderText = "Country";
            this.Country.Name = "Country";
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
            // 
            // Propagation
            // 
            this.Propagation.HeaderText = "Prop";
            this.Propagation.Name = "Propagation";
            this.Propagation.Width = 40;
            // 
            // QSOid
            // 
            this.QSOid.HeaderText = "Qsoid";
            this.QSOid.Name = "QSOid";
            // 
            // Log
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 400);
            this.Controls.Add(this.LogdataGridView);
            this.Name = "Log";
            this.Text = "Log";
            this.Load += new System.EventHandler(this.Log_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LogdataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView LogdataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn QSODate;
        private System.Windows.Forms.DataGridViewTextBoxColumn UTC;
        private System.Windows.Forms.DataGridViewTextBoxColumn Operator;
        private System.Windows.Forms.DataGridViewTextBoxColumn Callsign;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Band;
        private System.Windows.Forms.DataGridViewTextBoxColumn Freq;
        private System.Windows.Forms.DataGridViewTextBoxColumn RstRcvd;
        private System.Windows.Forms.DataGridViewTextBoxColumn RstSent;
        private System.Windows.Forms.DataGridViewTextBoxColumn Country;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn Propagation;
        private System.Windows.Forms.DataGridViewTextBoxColumn QSOid;
    }
}