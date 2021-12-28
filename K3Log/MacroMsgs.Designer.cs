namespace K3Log
{
    partial class MacroMsgs
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
            this.txtMacroMsg = new System.Windows.Forms.TextBox();
            this.txtBtnLabel = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMacroName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboShortCut = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMacNumber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtMacroMsg
            // 
            this.txtMacroMsg.Location = new System.Drawing.Point(19, 24);
            this.txtMacroMsg.Multiline = true;
            this.txtMacroMsg.Name = "txtMacroMsg";
            this.txtMacroMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMacroMsg.Size = new System.Drawing.Size(301, 143);
            this.txtMacroMsg.TabIndex = 0;
            // 
            // txtBtnLabel
            // 
            this.txtBtnLabel.Location = new System.Drawing.Point(40, 190);
            this.txtBtnLabel.Name = "txtBtnLabel";
            this.txtBtnLabel.Size = new System.Drawing.Size(116, 20);
            this.txtBtnLabel.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(306, 189);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 21);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(379, 189);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(52, 21);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Button Label";
            // 
            // txtMacroName
            // 
            this.txtMacroName.Location = new System.Drawing.Point(162, 190);
            this.txtMacroName.Name = "txtMacroName";
            this.txtMacroName.Size = new System.Drawing.Size(116, 20);
            this.txtMacroName.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(159, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Macro Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(340, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Short Cut";
            // 
            // cboShortCut
            // 
            this.cboShortCut.FormattingEnabled = true;
            this.cboShortCut.Items.AddRange(new object[] {
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.cboShortCut.Location = new System.Drawing.Point(343, 99);
            this.cboShortCut.Name = "cboShortCut";
            this.cboShortCut.Size = new System.Drawing.Size(74, 21);
            this.cboShortCut.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(344, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Macro#";
            // 
            // txtMacNumber
            // 
            this.txtMacNumber.Location = new System.Drawing.Point(394, 31);
            this.txtMacNumber.Name = "txtMacNumber";
            this.txtMacNumber.Size = new System.Drawing.Size(18, 20);
            this.txtMacNumber.TabIndex = 11;
            // 
            // MacroMsgs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 255);
            this.Controls.Add(this.txtMacNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboShortCut);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMacroName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtBtnLabel);
            this.Controls.Add(this.txtMacroMsg);
            this.Name = "MacroMsgs";
            this.Text = "MacroMsgs";
            this.Load += new System.EventHandler(this.MacroMsgs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMacroMsg;
        private System.Windows.Forms.TextBox txtBtnLabel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMacroName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboShortCut;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMacNumber;
    }
}