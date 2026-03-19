using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmMsgBox : Form
    {
        public bool Result { get; private set; }

        private bool _dragging;
        private Point _dragStart;

        public frmMsgBox(string message, string title = "Confirm")
        {
            TopMost = true;
            InitializeComponent();
            lblTitle.Text = title;
            lblMsg.Text   = message;

            FormPositions.Restore(this);
            this.FormClosed += (s, e) => FormPositions.Save(this);

            foreach (var c in new System.Windows.Forms.Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { _dragging = true; _dragStart = e.Location; } };
                c.MouseMove += (s, e) => { if (_dragging) { Left += e.X - _dragStart.X; Top += e.Y - _dragStart.Y; } };
                c.MouseUp   += (s, e) => _dragging = false;
            }
        }

        private void btnYes_Click(object sender, System.EventArgs e)
        {
            Result = true;
            Close();
        }

        private void btnNo_Click(object sender, System.EventArgs e)
        {
            Result = false;
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Owner?.BringToFront();
        }
    }
}
