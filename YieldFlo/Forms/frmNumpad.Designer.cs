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

            // ── Title ──────────────────────────────────────────────────────────
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.Location  = new System.Drawing.Point(8, 6);
            this.lblTitle.Size      = new System.Drawing.Size(284, 20);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Display ────────────────────────────────────────────────────────
            this.tboxDisplay.Font        = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold);
            this.tboxDisplay.TextAlign   = System.Windows.Forms.HorizontalAlignment.Right;
            this.tboxDisplay.ReadOnly    = true;
            this.tboxDisplay.BackColor   = System.Drawing.Color.FromArgb(30, 30, 30);
            this.tboxDisplay.ForeColor   = System.Drawing.Color.White;
            this.tboxDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tboxDisplay.Location    = new System.Drawing.Point(8, 30);
            this.tboxDisplay.Size        = new System.Drawing.Size(284, 46);
            this.tboxDisplay.TabStop     = false;

            // ── Min/Max hint ───────────────────────────────────────────────────
            this.lblMinMax.Font      = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblMinMax.ForeColor = System.Drawing.Color.Silver;
            this.lblMinMax.Location  = new System.Drawing.Point(8, 80);
            this.lblMinMax.Size      = new System.Drawing.Size(284, 16);
            this.lblMinMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Row 0  Y=100 ──────────────────────────────────────────────────
            this.btn7.Text = "7"; this.btn7.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn7.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn7.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn7.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn7.ForeColor = System.Drawing.Color.White; this.btn7.Size = new System.Drawing.Size(68, 56); this.btn7.Location = new System.Drawing.Point(8, 100); this.btn7.TabStop = false; this.btn7.UseVisualStyleBackColor = false;
            this.btn8.Text = "8"; this.btn8.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn8.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn8.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn8.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn8.ForeColor = System.Drawing.Color.White; this.btn8.Size = new System.Drawing.Size(68, 56); this.btn8.Location = new System.Drawing.Point(80, 100); this.btn8.TabStop = false; this.btn8.UseVisualStyleBackColor = false;
            this.btn9.Text = "9"; this.btn9.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn9.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn9.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn9.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn9.ForeColor = System.Drawing.Color.White; this.btn9.Size = new System.Drawing.Size(68, 56); this.btn9.Location = new System.Drawing.Point(152, 100); this.btn9.TabStop = false; this.btn9.UseVisualStyleBackColor = false;
            this.btnBack.Text = "⌫"; this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold); this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btnBack.BackColor = System.Drawing.Color.FromArgb(80, 70, 30); this.btnBack.ForeColor = System.Drawing.Color.White; this.btnBack.Size = new System.Drawing.Size(68, 56); this.btnBack.Location = new System.Drawing.Point(224, 100); this.btnBack.TabStop = false; this.btnBack.UseVisualStyleBackColor = false;

            // ── Row 1  Y=160 ──────────────────────────────────────────────────
            this.btn4.Text = "4"; this.btn4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn4.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn4.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn4.ForeColor = System.Drawing.Color.White; this.btn4.Size = new System.Drawing.Size(68, 56); this.btn4.Location = new System.Drawing.Point(8, 160); this.btn4.TabStop = false; this.btn4.UseVisualStyleBackColor = false;
            this.btn5.Text = "5"; this.btn5.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn5.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn5.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn5.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn5.ForeColor = System.Drawing.Color.White; this.btn5.Size = new System.Drawing.Size(68, 56); this.btn5.Location = new System.Drawing.Point(80, 160); this.btn5.TabStop = false; this.btn5.UseVisualStyleBackColor = false;
            this.btn6.Text = "6"; this.btn6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn6.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn6.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn6.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn6.ForeColor = System.Drawing.Color.White; this.btn6.Size = new System.Drawing.Size(68, 56); this.btn6.Location = new System.Drawing.Point(152, 160); this.btn6.TabStop = false; this.btn6.UseVisualStyleBackColor = false;
            this.btnClear.Text = "C"; this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold); this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btnClear.BackColor = System.Drawing.Color.FromArgb(80, 70, 30); this.btnClear.ForeColor = System.Drawing.Color.White; this.btnClear.Size = new System.Drawing.Size(68, 56); this.btnClear.Location = new System.Drawing.Point(224, 160); this.btnClear.TabStop = false; this.btnClear.UseVisualStyleBackColor = false;

            // ── Row 2  Y=220 ──────────────────────────────────────────────────
            this.btn1.Text = "1"; this.btn1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn1.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn1.ForeColor = System.Drawing.Color.White; this.btn1.Size = new System.Drawing.Size(68, 56); this.btn1.Location = new System.Drawing.Point(8, 220); this.btn1.TabStop = false; this.btn1.UseVisualStyleBackColor = false;
            this.btn2.Text = "2"; this.btn2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn2.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn2.ForeColor = System.Drawing.Color.White; this.btn2.Size = new System.Drawing.Size(68, 56); this.btn2.Location = new System.Drawing.Point(80, 220); this.btn2.TabStop = false; this.btn2.UseVisualStyleBackColor = false;
            this.btn3.Text = "3"; this.btn3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn3.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn3.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn3.ForeColor = System.Drawing.Color.White; this.btn3.Size = new System.Drawing.Size(68, 56); this.btn3.Location = new System.Drawing.Point(152, 220); this.btn3.TabStop = false; this.btn3.UseVisualStyleBackColor = false;
            this.btnNeg.Text = "+/-"; this.btnNeg.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold); this.btnNeg.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnNeg.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btnNeg.BackColor = System.Drawing.Color.FromArgb(80, 70, 30); this.btnNeg.ForeColor = System.Drawing.Color.White; this.btnNeg.Size = new System.Drawing.Size(68, 56); this.btnNeg.Location = new System.Drawing.Point(224, 220); this.btnNeg.TabStop = false; this.btnNeg.UseVisualStyleBackColor = false;

            // ── Row 3  Y=280 ──────────────────────────────────────────────────
            this.btnDot.Text = "."; this.btnDot.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btnDot.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDot.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btnDot.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btnDot.ForeColor = System.Drawing.Color.White; this.btnDot.Size = new System.Drawing.Size(68, 56); this.btnDot.Location = new System.Drawing.Point(8, 280); this.btnDot.TabStop = false; this.btnDot.UseVisualStyleBackColor = false;
            this.btn0.Text = "0"; this.btn0.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold); this.btn0.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btn0.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btn0.BackColor = System.Drawing.Color.FromArgb(55, 55, 55); this.btn0.ForeColor = System.Drawing.Color.White; this.btn0.Size = new System.Drawing.Size(68, 56); this.btn0.Location = new System.Drawing.Point(80, 280); this.btn0.TabStop = false; this.btn0.UseVisualStyleBackColor = false;
            this.btnOK.Text = "✓"; this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold); this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btnOK.BackColor = System.Drawing.Color.FromArgb(0, 100, 0); this.btnOK.ForeColor = System.Drawing.Color.White; this.btnOK.Size = new System.Drawing.Size(68, 56); this.btnOK.Location = new System.Drawing.Point(152, 280); this.btnOK.TabStop = false; this.btnOK.UseVisualStyleBackColor = false;
            this.btnCancel.Text = "✗"; this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold); this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90); this.btnCancel.BackColor = System.Drawing.Color.FromArgb(120, 0, 0); this.btnCancel.ForeColor = System.Drawing.Color.White; this.btnCancel.Size = new System.Drawing.Size(68, 56); this.btnCancel.Location = new System.Drawing.Point(224, 280); this.btnCancel.TabStop = false; this.btnCancel.UseVisualStyleBackColor = false;

            // ── Content panel ──────────────────────────────────────────────────
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTitle, tboxDisplay, lblMinMax,
                btn7, btn8, btn9, btnBack,
                btn4, btn5, btn6, btnClear,
                btn1, btn2, btn3, btnNeg,
                btnDot, btn0, btnOK, btnCancel });

            // ── Form ───────────────────────────────────────────────────────────
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
