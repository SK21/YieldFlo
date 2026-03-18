namespace YieldFlo.Forms
{
    partial class frmNumpad
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlContent     = new System.Windows.Forms.Panel();
            this.lblTitle       = new System.Windows.Forms.Label();
            this.tboxDisplay    = new System.Windows.Forms.TextBox();
            this.lblMinMax      = new System.Windows.Forms.Label();
            this.btn7           = new System.Windows.Forms.Button();
            this.btn8           = new System.Windows.Forms.Button();
            this.btn9           = new System.Windows.Forms.Button();
            this.btnBack        = new System.Windows.Forms.Button();
            this.btn4           = new System.Windows.Forms.Button();
            this.btn5           = new System.Windows.Forms.Button();
            this.btn6           = new System.Windows.Forms.Button();
            this.btnClear       = new System.Windows.Forms.Button();
            this.btn1           = new System.Windows.Forms.Button();
            this.btn2           = new System.Windows.Forms.Button();
            this.btn3           = new System.Windows.Forms.Button();
            this.btnNeg         = new System.Windows.Forms.Button();
            this.btnDot         = new System.Windows.Forms.Button();
            this.btn0           = new System.Windows.Forms.Button();
            this.btnOK          = new System.Windows.Forms.Button();
            this.btnCancel      = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // Title
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.Location  = new System.Drawing.Point(8, 6);
            this.lblTitle.Size      = new System.Drawing.Size(284, 20);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Display  (shifted down 28px from previous Y=8)
            this.tboxDisplay.Font          = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold);
            this.tboxDisplay.TextAlign     = System.Windows.Forms.HorizontalAlignment.Right;
            this.tboxDisplay.ReadOnly      = true;
            this.tboxDisplay.BackColor     = System.Drawing.Color.FromArgb(30, 30, 30);
            this.tboxDisplay.ForeColor     = System.Drawing.Color.White;
            this.tboxDisplay.BorderStyle   = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tboxDisplay.Location      = new System.Drawing.Point(8, 30);
            this.tboxDisplay.Size          = new System.Drawing.Size(284, 46);
            this.tboxDisplay.TabStop       = false;

            // Min/Max hint  (shifted down 28px from previous Y=58)
            this.lblMinMax.Font      = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblMinMax.ForeColor = System.Drawing.Color.Silver;
            this.lblMinMax.Location  = new System.Drawing.Point(8, 80);
            this.lblMinMax.Size      = new System.Drawing.Size(284, 16);
            this.lblMinMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Button layout: 4 cols × 4 rows — grid starts at Y=100 (was 80)
            var digitFont = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            var opFont    = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            var digitBack = System.Drawing.Color.FromArgb(55, 55, 55);
            var opBack    = System.Drawing.Color.FromArgb(80, 70, 30);

            void SetBtn(System.Windows.Forms.Button b, string text, int col, int row,
                        System.Drawing.Font f, System.Drawing.Color back)
            {
                b.Text      = text;
                b.Font      = f;
                b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                b.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
                b.BackColor = back;
                b.ForeColor = System.Drawing.Color.White;
                b.Size      = new System.Drawing.Size(68, 56);
                b.Location  = new System.Drawing.Point(8 + col * 72, 100 + row * 60);
                b.TabStop   = false;
                b.UseVisualStyleBackColor = false;
            }

            SetBtn(btn7,      "7",   0, 0, digitFont, digitBack);
            SetBtn(btn8,      "8",   1, 0, digitFont, digitBack);
            SetBtn(btn9,      "9",   2, 0, digitFont, digitBack);
            SetBtn(btnBack,   "⌫",   3, 0, opFont,    opBack);

            SetBtn(btn4,      "4",   0, 1, digitFont, digitBack);
            SetBtn(btn5,      "5",   1, 1, digitFont, digitBack);
            SetBtn(btn6,      "6",   2, 1, digitFont, digitBack);
            SetBtn(btnClear,  "C",   3, 1, opFont,    opBack);

            SetBtn(btn1,      "1",   0, 2, digitFont, digitBack);
            SetBtn(btn2,      "2",   1, 2, digitFont, digitBack);
            SetBtn(btn3,      "3",   2, 2, digitFont, digitBack);
            SetBtn(btnNeg,    "+/-", 3, 2, opFont,    opBack);

            SetBtn(btnDot,    ".",   0, 3, digitFont, digitBack);
            SetBtn(btn0,      "0",   1, 3, digitFont, digitBack);
            SetBtn(btnOK,     "✓",   2, 3, opFont,    System.Drawing.Color.FromArgb(0, 100, 0));
            SetBtn(btnCancel, "✗",   3, 3, opFont,    System.Drawing.Color.FromArgb(120, 0, 0));

            btn7.Click      += (s, e) => Append("7");
            btn8.Click      += (s, e) => Append("8");
            btn9.Click      += (s, e) => Append("9");
            btnBack.Click   += (s, e) => Backspace();
            btn4.Click      += (s, e) => Append("4");
            btn5.Click      += (s, e) => Append("5");
            btn6.Click      += (s, e) => Append("6");
            btnClear.Click  += (s, e) => { tboxDisplay.Text = ""; _overwrite = false; };
            btn1.Click      += (s, e) => Append("1");
            btn2.Click      += (s, e) => Append("2");
            btn3.Click      += (s, e) => Append("3");
            btnNeg.Click    += (s, e) => ToggleNeg();
            btnDot.Click    += (s, e) => AppendDot();
            btn0.Click      += (s, e) => Append("0");
            btnOK.Click     += new System.EventHandler(this.btnOK_Click);
            btnCancel.Click += (s, e) => this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            // Content panel
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTitle, tboxDisplay, lblMinMax,
                btn7, btn8, btn9, btnBack,
                btn4, btn5, btn6, btnClear,
                btn1, btn2, btn3, btnNeg,
                btnDot, btn0, btnOK, btnCancel });

            this.ClientSize      = new System.Drawing.Size(300, 370);
            this.MinimumSize     = new System.Drawing.Size(300, 370);
            this.MaximumSize     = new System.Drawing.Size(300, 370);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor       = System.Drawing.Color.White;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name            = "frmNumpad";
            this.Text            = "Enter Value";
            this.Controls.Add(this.pnlContent);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel   pnlContent;
        private System.Windows.Forms.Label   lblTitle;
        private System.Windows.Forms.TextBox tboxDisplay;
        private System.Windows.Forms.Label   lblMinMax;
        private System.Windows.Forms.Button  btn7, btn8, btn9, btnBack;
        private System.Windows.Forms.Button  btn4, btn5, btn6, btnClear;
        private System.Windows.Forms.Button  btn1, btn2, btn3, btnNeg;
        private System.Windows.Forms.Button  btnDot, btn0, btnOK, btnCancel;
    }
}
