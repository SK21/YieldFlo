using System;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    // The app's themed replacement for MessageBox.Show: Yes/No, or OK only.
    //
    // Sizes itself to the message. A one- or two-line message gets the original 424x232
    // box unchanged; anything longer gets a wider box and as many lines as it needs (up
    // to a cap), so text is never silently clipped the way the fixed 3-line label used to.
    //
    // Sets TopMost on itself, so it can be shown straight from the app's TopMost forms
    // with ShowDialog(owner) and no TopMost juggling in the caller.
    public partial class frmMsgBox : Form
    {
        public bool Result { get; private set; }

        private bool _dragging;
        private Point _dragStart;

        private const int NarrowWidth = 420;    // content width of the original box
        private const int WideWidth = 560;      // for messages over ~3 lines
        private const int MinMsgHeight = 90;
        private const int MaxMsgHeight = 420;   // about 16 lines at the wide width
        private const int MsgTop = 14;
        private const int MsgSidePad = 10;
        private const int ButtonGap = 16;

        public frmMsgBox(string message, string title = "Confirm", bool okOnly = false)
        {
            TopMost = true;
            InitializeComponent();
            lblTitle.Text = title;
            lblMsg.Text   = message;
            btnYes.Text   = okOnly ? "OK" : Lang.lgYes;
            btnNo.Text    = Lang.lgNo;
            btnNo.Visible = !okOnly;
            FitToMessage(okOnly);

            FormPositions.Restore(this);
            this.FormClosed += (s, e) => FormPositions.Save(this);

            foreach (var c in new System.Windows.Forms.Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { _dragging = true; _dragStart = e.Location; } };
                c.MouseMove += (s, e) => { if (_dragging) { Left += e.X - _dragStart.X; Top += e.Y - _dragStart.Y; } };
                c.MouseUp   += (s, e) => _dragging = false;
            }
        }

        /// <summary>Yes/No question. True for Yes.</summary>
        public static bool Ask(IWin32Window owner, string message, string title = "Confirm")
        {
            using (var dlg = new frmMsgBox(message, title))
            {
                if (owner != null) dlg.ShowDialog(owner); else dlg.ShowDialog();
                return dlg.Result;
            }
        }

        /// <summary>A message with an OK button.</summary>
        public static void Inform(IWin32Window owner, string message, string title = "YieldFlo")
        {
            using (var dlg = new frmMsgBox(message, title, true))
            {
                if (owner != null) dlg.ShowDialog(owner); else dlg.ShowDialog();
            }
        }

        private void FitToMessage(bool okOnly)
        {
            int width = NarrowWidth;
            int textH = Measure(width);
            bool wide = textH > MinMsgHeight;
            if (wide)
            {
                width = WideWidth;
                textH = Measure(width);
            }

            int msgH = Math.Max(MinMsgHeight, Math.Min(MaxMsgHeight, textH + 12));
            lblMsg.Location = new Point(MsgSidePad, MsgTop);
            lblMsg.Size = new Size(width - 2 * MsgSidePad, msgH);
            lblMsg.TextAlign = wide ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
            lblMsg.AutoEllipsis = textH + 12 > MaxMsgHeight;

            int btnTop = MsgTop + msgH + ButtonGap;
            if (okOnly)
            {
                btnYes.Location = new Point((width - btnYes.Width) / 2, btnTop);
            }
            else
            {
                int pair = btnYes.Width + 20 + btnNo.Width;
                btnYes.Location = new Point((width - pair) / 2, btnTop);
                btnNo.Location = new Point(btnYes.Right + 20, btnTop);
            }

            // the Designer locks the original size; this box now grows
            MinimumSize = Size.Empty;
            MaximumSize = Size.Empty;
            ClientSize = new Size(width + Padding.Horizontal,
                                  pnlTitle.Height + btnTop + btnYes.Height + ButtonGap + Padding.Vertical);
        }

        private int Measure(int width)
        {
            Size s = TextRenderer.MeasureText(lblMsg.Text, lblMsg.Font,
                new Size(width - 2 * MsgSidePad, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            return s.Height;
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
