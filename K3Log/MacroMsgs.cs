using System;
using System.Windows.Forms;


namespace K3Log
{
    public partial class MacroMsgs : Form
    {
        //List<K3Log.macro> macroList;


        private macro _thisMacro;
        public macro thisMacro
        {
            get
            {
                return _thisMacro;
            }
            set
            {
                _thisMacro = value;
                this.txtMacNumber.Text = _thisMacro.number.ToString();
                this.txtMacroName.Text = _thisMacro.macroName;
                this.txtBtnLabel.Text = _thisMacro.btnLabel;
                this.txtMacroMsg.Text = _thisMacro.macroAction;
                this.cboShortCut.SelectedIndex = _thisMacro.macroShortCut - 112;
            }
        }
        public class MacroButtonEventArgs : EventArgs
        {
            public macro mac;
        }

        MacroButtonEventArgs args = new MacroButtonEventArgs();
        public event EventHandler<MacroButtonEventArgs> MacUpdate;
        protected virtual void OnK3Rcvd(object sender, MacroButtonEventArgs e)
        {
            MacUpdate?.Invoke(this, e);
        }

        public MacroMsgs()
        {
            InitializeComponent();
        }

        private void MacroMsgs_Load(object sender, EventArgs e)
        {



        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            _thisMacro.number = Convert.ToInt16(txtMacNumber.Text);
            _thisMacro.macroName = this.txtMacroName.Text;
            _thisMacro.btnLabel = this.txtBtnLabel.Text;
            _thisMacro.macroAction = this.txtMacroMsg.Text;
            _thisMacro.macroShortCut = this.cboShortCut.SelectedIndex + 112;
            args.mac = thisMacro;
            MacUpdate(this, args);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
