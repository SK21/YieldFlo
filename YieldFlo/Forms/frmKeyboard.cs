using System;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    /// <summary>
    /// Finger-friendly QWERTY keyboard for tablet text input.
    /// Usage: using (var kb = new frmKeyboard(current, title)) {
    ///            if (kb.ShowDialog() == DialogResult.OK) txt.Text = kb.ReturnValue; }
    /// </summary>
    public partial class frmKeyboard : Form
    {
        private bool  _shift    = true;   // start uppercase
        private bool  _overwrite = true;  // first keypress replaces existing text
        private bool  _dragging;
        private Point _dragStart;

        private static readonly Color ShiftOn  = Color.FromArgb(0, 80, 140);
        private static readonly Color ShiftOff = Color.FromArgb(80, 70, 30);

        // All letter buttons in order — used by ToggleShift
        private Button[] _letterBtns;
        private string[] _letters = {
            "Q","W","E","R","T","Y","U","I","O","P",
            "A","S","D","F","G","H","J","K","L",
            "Z","X","C","V","B","N","M"
        };

        public string ReturnValue { get; private set; } = "";

        public frmKeyboard(string current, string title = "")
        {
            InitializeComponent();

            lblTitle.Text    = title;
            tboxDisplay.Text = current;

            _letterBtns = new Button[] {
                btnQ, btnW, btnE, btnR, btnT, btnY, btnU, btnI, btnO, btnP,
                btnA, btnS, btnD, btnF, btnG, btnH, btnJ, btnK, btnL,
                btnZ, btnX, btnC, btnV, btnB, btnN, btnM
            };

            UpdateShiftVisual();
            WireButtons();

            FormPositions.Restore(this);
            this.FormClosed += (s, e) => FormPositions.Save(this);

            foreach (var ctl in new Control[] { tboxDisplay, pnlContent })
            {
                ctl.MouseDown += (s, e) => { if (((MouseEventArgs)e).Button == MouseButtons.Left) { _dragging = true;  _dragStart = ((MouseEventArgs)e).Location; } };
                ctl.MouseMove += (s, e) => { if (_dragging) { Left += ((MouseEventArgs)e).X - _dragStart.X; Top += ((MouseEventArgs)e).Y - _dragStart.Y; } };
                ctl.MouseUp   += (s, e) => _dragging = false;
            }
        }

        private void WireButtons()
        {
            btn1.Click += (s, e) => Append("1");
            btn2.Click += (s, e) => Append("2");
            btn3.Click += (s, e) => Append("3");
            btn4.Click += (s, e) => Append("4");
            btn5.Click += (s, e) => Append("5");
            btn6.Click += (s, e) => Append("6");
            btn7.Click += (s, e) => Append("7");
            btn8.Click += (s, e) => Append("8");
            btn9.Click += (s, e) => Append("9");
            btn0.Click += (s, e) => Append("0");
            btnQ.Click += (s, e) => AppendLetter("Q");
            btnW.Click += (s, e) => AppendLetter("W");
            btnE.Click += (s, e) => AppendLetter("E");
            btnR.Click += (s, e) => AppendLetter("R");
            btnT.Click += (s, e) => AppendLetter("T");
            btnY.Click += (s, e) => AppendLetter("Y");
            btnU.Click += (s, e) => AppendLetter("U");
            btnI.Click += (s, e) => AppendLetter("I");
            btnO.Click += (s, e) => AppendLetter("O");
            btnP.Click += (s, e) => AppendLetter("P");
            btnA.Click += (s, e) => AppendLetter("A");
            btnS.Click += (s, e) => AppendLetter("S");
            btnD.Click += (s, e) => AppendLetter("D");
            btnF.Click += (s, e) => AppendLetter("F");
            btnG.Click += (s, e) => AppendLetter("G");
            btnH.Click += (s, e) => AppendLetter("H");
            btnJ.Click += (s, e) => AppendLetter("J");
            btnK.Click += (s, e) => AppendLetter("K");
            btnL.Click += (s, e) => AppendLetter("L");
            btnZ.Click    += (s, e) => AppendLetter("Z");
            btnX.Click    += (s, e) => AppendLetter("X");
            btnC.Click    += (s, e) => AppendLetter("C");
            btnV.Click    += (s, e) => AppendLetter("V");
            btnB.Click    += (s, e) => AppendLetter("B");
            btnN.Click    += (s, e) => AppendLetter("N");
            btnM.Click    += (s, e) => AppendLetter("M");
            btnDash.Click += (s, e) => Append("-");
            btnDot.Click  += (s, e) => Append(".");
            btnShift.Click  += (s, e) => ToggleShift();
            btnSpace.Click  += (s, e) => Append(" ");
            btnBack.Click   += (s, e) => Backspace();
            btnClear.Click  += (s, e) => { tboxDisplay.Text = ""; _overwrite = false; };
            btnOK.Click     += btnOK_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        private void Append(string ch)
        {
            if (_overwrite) { tboxDisplay.Text = ""; _overwrite = false; }
            tboxDisplay.Text += ch;
        }

        private void AppendLetter(string upper)
        {
            Append(_shift ? upper : upper.ToLower());
        }

        private void Backspace()
        {
            _overwrite = false;
            if (tboxDisplay.Text.Length > 0)
                tboxDisplay.Text = tboxDisplay.Text.Substring(0, tboxDisplay.Text.Length - 1);
        }

        private void ToggleShift()
        {
            _shift = !_shift;
            UpdateShiftVisual();
        }

        private void UpdateShiftVisual()
        {
            for (int i = 0; i < _letterBtns.Length; i++)
                _letterBtns[i].Text = _shift ? _letters[i] : _letters[i].ToLower();
            btnShift.BackColor = _shift ? ShiftOn : ShiftOff;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ReturnValue  = tboxDisplay.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
