using System;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmMsgBox : Form
    {
        private bool cResult;

        public frmMsgBox(string message, string title = "Help", bool shrink = false)
        {
            InitializeComponent();
            this.Text = title;
            label1.Text = message;

            if (shrink)
            {
                panel1.Height = 60;
                this.Height = 198;
                btnCancel.Top = 78;
                btnOK.Top = 78;
            }
            else
            {
                panel1.Height = 200;
                this.Height = 310;
                btnCancel.Top = 218;
                btnOK.Top = 218;
            }
        }

        public bool Result { get => cResult; set => cResult = value; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = true;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = false;
            this.Hide();
        }

        private void frmMsgBox_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            this.ForeColor = Properties.Settings.Default.MainForeColour;
            label1.ForeColor = Properties.Settings.Default.MainForeColour;
            btnOK.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            btnOK.ForeColor = System.Drawing.Color.White;
            btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCancel.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            btnCancel.ForeColor = System.Drawing.Color.White;
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }
    }
}
