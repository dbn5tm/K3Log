namespace K3Log
{
    partial class ImportADIF
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
            this.txtAdifPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoADIF = new System.Windows.Forms.RadioButton();
            this.rdoLoTW = new System.Windows.Forms.RadioButton();
            this.txtImport = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAdifPath
            // 
            this.txtAdifPath.Location = new System.Drawing.Point(55, 41);
            this.txtAdifPath.Name = "txtAdifPath";
            this.txtAdifPath.Size = new System.Drawing.Size(421, 20);
            this.txtAdifPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ADIF Path";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(492, 38);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(67, 25);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(219, 115);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(82, 40);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoADIF);
            this.groupBox1.Controls.Add(this.rdoLoTW);
            this.groupBox1.Location = new System.Drawing.Point(55, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(127, 77);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type";
            // 
            // rdoADIF
            // 
            this.rdoADIF.AutoSize = true;
            this.rdoADIF.Location = new System.Drawing.Point(17, 42);
            this.rdoADIF.Name = "rdoADIF";
            this.rdoADIF.Size = new System.Drawing.Size(81, 17);
            this.rdoADIF.TabIndex = 1;
            this.rdoADIF.TabStop = true;
            this.rdoADIF.Text = "ADIF Import";
            this.rdoADIF.UseVisualStyleBackColor = true;
            // 
            // rdoLoTW
            // 
            this.rdoLoTW.AutoSize = true;
            this.rdoLoTW.Location = new System.Drawing.Point(17, 19);
            this.rdoLoTW.Name = "rdoLoTW";
            this.rdoLoTW.Size = new System.Drawing.Size(93, 17);
            this.rdoLoTW.TabIndex = 0;
            this.rdoLoTW.TabStop = true;
            this.rdoLoTW.Text = "LoTW Update";
            this.rdoLoTW.UseVisualStyleBackColor = true;
            // 
            // txtImport
            // 
            this.txtImport.Location = new System.Drawing.Point(55, 192);
            this.txtImport.Multiline = true;
            this.txtImport.Name = "txtImport";
            this.txtImport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtImport.Size = new System.Drawing.Size(455, 79);
            this.txtImport.TabIndex = 5;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(548, 241);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 29);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ImportADIF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 296);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtImport);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAdifPath);
            this.Name = "ImportADIF";
            this.Text = "Import";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAdifPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoADIF;
        private System.Windows.Forms.RadioButton rdoLoTW;
        private System.Windows.Forms.TextBox txtImport;
        private System.Windows.Forms.Button btnClose;
    }
}