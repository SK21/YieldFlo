using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmYieldMap
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.gmap          = new GMap.NET.WindowsForms.GMapControl();
            this.pnlMiniBar    = new System.Windows.Forms.Panel();
            this.lblMiniJob    = new System.Windows.Forms.Label();
            this.btnZoomOut    = new System.Windows.Forms.Button();
            this.btnZoomIn     = new System.Windows.Forms.Button();
            this.btnMiniClose  = new System.Windows.Forms.Button();
            this.pnlToolbar       = new System.Windows.Forms.Panel();
            this.cboJob           = new System.Windows.Forms.ComboBox();
            this.btnZoomOutFull   = new System.Windows.Forms.Button();
            this.btnZoomInFull    = new System.Windows.Forms.Button();
            this.btnPrint      = new System.Windows.Forms.Button();
            this.btnClose      = new System.Windows.Forms.Button();
            this.pnlLegend     = new System.Windows.Forms.Panel();

            this.SuspendLayout();

            // ── GMap ──────────────────────────────────────────────────────────
            this.gmap.Bounds         = new System.Drawing.Rectangle(0, 30, 300, 270);
            this.gmap.CanDragMap     = false;
            this.gmap.MouseDown     += new System.Windows.Forms.MouseEventHandler(this.Gmap_MouseDown);
            this.gmap.MouseUp       += new System.Windows.Forms.MouseEventHandler(this.Gmap_MouseUp);

            // ── Mini bar (title bar + drag + zoom, mini mode only) ────────────
            this.pnlMiniBar.BackColor  = System.Drawing.Color.FromArgb(20, 30, 40);
            this.pnlMiniBar.Bounds     = new System.Drawing.Rectangle(0, 0, 300, 30);

            this.lblMiniJob.AutoSize   = false;
            this.lblMiniJob.Bounds     = new System.Drawing.Rectangle(4, 3, 176, 24);
            this.lblMiniJob.ForeColor  = System.Drawing.Color.Silver;
            this.lblMiniJob.Font       = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblMiniJob.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;

            var miniBtn = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);

            this.btnZoomOut.Text      = "\u2212";
            this.btnZoomOut.Font      = miniBtn;
            this.btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomOut.FlatAppearance.BorderSize = 0;
            this.btnZoomOut.BackColor = System.Drawing.Color.FromArgb(40, 55, 70);
            this.btnZoomOut.ForeColor = System.Drawing.Color.White;
            this.btnZoomOut.Size      = new System.Drawing.Size(24, 24);
            this.btnZoomOut.Location  = new System.Drawing.Point(184, 3);
            this.btnZoomOut.Click    += new System.EventHandler(this.btnZoomOut_Click);

            this.btnZoomIn.Text      = "+";
            this.btnZoomIn.Font      = miniBtn;
            this.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomIn.FlatAppearance.BorderSize = 0;
            this.btnZoomIn.BackColor = System.Drawing.Color.FromArgb(40, 55, 70);
            this.btnZoomIn.ForeColor = System.Drawing.Color.White;
            this.btnZoomIn.Size      = new System.Drawing.Size(24, 24);
            this.btnZoomIn.Location  = new System.Drawing.Point(212, 3);
            this.btnZoomIn.Click    += new System.EventHandler(this.btnZoomIn_Click);

            this.btnMiniClose.Text      = "\u00d7";
            this.btnMiniClose.Font      = miniBtn;
            this.btnMiniClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMiniClose.FlatAppearance.BorderSize = 0;
            this.btnMiniClose.BackColor = System.Drawing.Color.FromArgb(100, 30, 30);
            this.btnMiniClose.ForeColor = System.Drawing.Color.White;
            this.btnMiniClose.Size      = new System.Drawing.Size(24, 24);
            this.btnMiniClose.Location  = new System.Drawing.Point(240, 3);
            this.btnMiniClose.Click    += new System.EventHandler(this.btnMiniClose_Click);

            // Wire drag on bar and label (buttons absorb their own clicks)
            this.pnlMiniBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMiniBar_MouseDown);
            this.pnlMiniBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMiniBar_MouseMove);
            this.pnlMiniBar.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.pnlMiniBar_MouseUp);
            this.lblMiniJob.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMiniBar_MouseDown);
            this.lblMiniJob.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMiniBar_MouseMove);
            this.lblMiniJob.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.pnlMiniBar_MouseUp);

            this.pnlMiniBar.Controls.Add(this.lblMiniJob);
            this.pnlMiniBar.Controls.Add(this.btnZoomOut);
            this.pnlMiniBar.Controls.Add(this.btnZoomIn);
            this.pnlMiniBar.Controls.Add(this.btnMiniClose);

            // ── Toolbar (full mode only) ───────────────────────────────────────
            var btnFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);

            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(25, 35, 50);
            this.pnlToolbar.Bounds    = new System.Drawing.Rectangle(0, 0, 800, 36);
            this.pnlToolbar.Visible   = false;

            this.cboJob.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboJob.Font          = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cboJob.BackColor     = System.Drawing.Color.FromArgb(50, 60, 75);
            this.cboJob.ForeColor     = System.Drawing.Color.White;
            this.cboJob.Bounds        = new System.Drawing.Rectangle(8, 5, 300, 26);
            this.cboJob.SelectedIndexChanged += new System.EventHandler(this.cboJob_SelectedIndexChanged);

            this.btnZoomOutFull.Text      = "\u2212";
            this.btnZoomOutFull.Font      = btnFont;
            this.btnZoomOutFull.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomOutFull.FlatAppearance.BorderSize = 0;
            this.btnZoomOutFull.BackColor = System.Drawing.Color.FromArgb(50, 65, 85);
            this.btnZoomOutFull.ForeColor = System.Drawing.Color.White;
            this.btnZoomOutFull.Size      = new System.Drawing.Size(36, 26);
            this.btnZoomOutFull.Location  = new System.Drawing.Point(700, 5);  // repositioned by SetMiniMode
            this.btnZoomOutFull.Click    += new System.EventHandler(this.btnZoomOut_Click);

            this.btnZoomInFull.Text      = "+";
            this.btnZoomInFull.Font      = btnFont;
            this.btnZoomInFull.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomInFull.FlatAppearance.BorderSize = 0;
            this.btnZoomInFull.BackColor = System.Drawing.Color.FromArgb(50, 65, 85);
            this.btnZoomInFull.ForeColor = System.Drawing.Color.White;
            this.btnZoomInFull.Size      = new System.Drawing.Size(36, 26);
            this.btnZoomInFull.Location  = new System.Drawing.Point(740, 5);  // repositioned by SetMiniMode
            this.btnZoomInFull.Click    += new System.EventHandler(this.btnZoomIn_Click);

            this.btnPrint.Text      = Lang.lgPrint;
            this.btnPrint.Font      = btnFont;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(0, 70, 130);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Size      = new System.Drawing.Size(70, 26);
            this.btnPrint.Location  = new System.Drawing.Point(640, 5);
            this.btnPrint.Click    += new System.EventHandler(this.btnPrint_Click);

            this.btnClose.Text      = Lang.lgClose;
            this.btnClose.Font      = btnFont;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(80, 30, 30);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Size      = new System.Drawing.Size(70, 26);
            this.btnClose.Location  = new System.Drawing.Point(724, 5);
            this.btnClose.Click    += new System.EventHandler(this.btnClose_Click);

            this.pnlToolbar.Controls.AddRange(new System.Windows.Forms.Control[] {
                cboJob, btnZoomOutFull, btnZoomInFull, btnPrint, btnClose });

            // ── Legend (full mode only) ────────────────────────────────────────
            this.pnlLegend.BackColor = System.Drawing.Color.FromArgb(15, 20, 30);
            this.pnlLegend.Bounds    = new System.Drawing.Rectangle(0, 560, 800, 38);
            this.pnlLegend.Visible   = false;
            this.pnlLegend.Paint    += new System.Windows.Forms.PaintEventHandler(this.pnlLegend_Paint);


            // ── Form (starts in mini size, SetMiniMode will resize) ───────────
            this.ClientSize      = new System.Drawing.Size(300, 300);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(0);
            this.BackColor       = System.Drawing.Color.Black;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name            = "frmYieldMap";
            this.Text            = "Yield Map";
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                gmap, pnlMiniBar, pnlToolbar, pnlLegend });
            this.Load += new System.EventHandler(this.frmYieldMap_Load);

            this.ResumeLayout(false);
        }

        private GMap.NET.WindowsForms.GMapControl gmap;
        private System.Windows.Forms.Panel   pnlMiniBar;
        private System.Windows.Forms.Label   lblMiniJob;
        private System.Windows.Forms.Button  btnZoomOut;
        private System.Windows.Forms.Button  btnZoomIn;
        private System.Windows.Forms.Button  btnMiniClose;
        private System.Windows.Forms.Panel    pnlToolbar;
        private System.Windows.Forms.ComboBox cboJob;
        private System.Windows.Forms.Button   btnZoomOutFull;
        private System.Windows.Forms.Button   btnZoomInFull;
        private System.Windows.Forms.Button   btnPrint;
        private System.Windows.Forms.Button   btnClose;
        private System.Windows.Forms.Panel   pnlLegend;
    }
}
