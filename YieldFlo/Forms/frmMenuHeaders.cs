using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuHeaders : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private List<(int id, string name, string type, double widthM, double fwdOffsetM)> _headers;
        private int _editingId = -1;

        public frmMenuHeaders()
        {
            InitializeComponent();
        }

        private void frmMenuHeaders_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            ApplyUnits();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp   += (s, ev) => _dragging = false;
            }
            LoadList();
            int activeIdx = _headers?.FindIndex(h => h.id == Core.ActiveHeaderId) ?? -1;
            if (activeIdx >= 0)
                lbHeaders.SelectedIndex = activeIdx;
            else if (lbHeaders.Items.Count > 0)
                lbHeaders.SelectedIndex = 0;
            else
                ClearEdit();
            this.Shown += frmMenuHeaders_Shown;
        }

        private void frmMenuHeaders_Shown(object sender, EventArgs e)
        {
            KeyboardHelper.Wire(this, txtHeaderName, "Header Name");
            string unit = Props.IsMetric ? "m" : "ft";
            NumpadHelper.Wire(this, numWidth, (double)numWidth.Minimum, (double)numWidth.Maximum,
                              1, $"Header Width ({unit})");
            NumpadHelper.Wire(this, numOffset, (double)numOffset.Minimum, (double)numOffset.Maximum,
                              1, $"Ahead of Pivot ({unit})");
            btnSave.Focus();
        }

        private void ApplyUnits()
        {
            if (Props.IsMetric)
            {
                lblWidthUnit.Text      = "m";
                numWidth.Minimum       = (decimal)0.5;
                numWidth.Maximum       = 60;
                numWidth.Increment     = (decimal)0.1;
                numWidth.DecimalPlaces = 2;
                numWidth.Value         = System.Math.Max((decimal)0.5, System.Math.Min(60, numWidth.Value));

                lblOffsetUnit.Text      = "m";
                numOffset.Minimum       = -30;
                numOffset.Maximum       = 100;
                numOffset.Increment     = (decimal)0.1;
                numOffset.DecimalPlaces = 2;
                numOffset.Value         = System.Math.Max(-30, System.Math.Min(100, numOffset.Value));
            }
            else
            {
                lblWidthUnit.Text      = "ft";
                numWidth.Minimum       = 2;
                numWidth.Maximum       = 200;
                numWidth.Increment     = (decimal)0.5;
                numWidth.DecimalPlaces = 1;
                numWidth.Value         = System.Math.Max(2, System.Math.Min(200, numWidth.Value));

                lblOffsetUnit.Text      = "ft";
                numOffset.Minimum       = -100;
                numOffset.Maximum       = 330;
                numOffset.Increment     = (decimal)0.5;
                numOffset.DecimalPlaces = 1;
                numOffset.Value         = System.Math.Max(-100, System.Math.Min(330, numOffset.Value));
            }
        }

        // Metres stored in DB → display value
        private decimal MetresToDisplay(double m) =>
            Props.IsMetric ? (decimal)m : (decimal)(m * 3.28084);

        // Display value → metres for DB storage
        private double DisplayToMetres(decimal v) =>
            Props.IsMetric ? (double)v : (double)v / 3.28084;

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);
            pnlTitle.BackColor   = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor   = Color.FromArgb(180, 200, 220);
            btnTitleClose.BackColor = Color.FromArgb(80, 30, 30);
            btnTitleClose.ForeColor = Color.White;
            lbHeaders.BackColor  = ctrl;
            lbHeaders.ForeColor  = fore;
            pnlEdit.BackColor    = back;
            foreach (Control c in pnlEdit.Controls)
            {
                c.ForeColor = fore;
                if (c is TextBox tb)       { tb.BackColor  = ctrl; tb.ForeColor  = fore; }
                if (c is ComboBox cb)      { cb.BackColor  = ctrl; cb.ForeColor  = fore; }
                if (c is NumericUpDown nd) { nd.BackColor  = ctrl; nd.ForeColor  = fore; }
            }
            btnNew.BackColor    = Color.FromArgb(60, 60, 60); btnNew.ForeColor    = Color.White;
            btnSave.BackColor   = Color.FromArgb(0, 90, 0);   btnSave.ForeColor   = Color.White;
            btnDelete.BackColor = Color.FromArgb(100, 0, 0);  btnDelete.ForeColor = Color.White;
            btnHeadersClose.BackColor = Color.FromArgb(60, 60, 60); btnHeadersClose.ForeColor = Color.White;
        }

        private void LoadList()
        {
            _headers = Core.Database.Headers.GetAll();
            lbHeaders.Items.Clear();
            string unit = Props.IsMetric ? "m" : "ft";
            string fmt  = Props.IsMetric ? "F2" : "F1";
            foreach (var h in _headers)
            {
                double display = Props.IsMetric ? h.widthM : h.widthM * 3.28084;
                lbHeaders.Items.Add($"{h.name}  –  {h.type}  –  {display.ToString(fmt)} {unit}");
            }
        }

        private void ClearEdit()
        {
            _editingId = -1;
            txtHeaderName.Text = "";
            cboHeaderType.SelectedIndex = 0;
            // Default 9 m / ~30 ft
            decimal def = Props.IsMetric ? 9 : (decimal)(9 * 3.28084);
            numWidth.Value = System.Math.Max(numWidth.Minimum, System.Math.Min(numWidth.Maximum, def));
            numOffset.Value = 0;
            lbHeaders.ClearSelected();
        }

        private void lbHeaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lbHeaders.SelectedIndex;
            if (idx < 0 || _headers == null || idx >= _headers.Count) return;
            var h = _headers[idx];
            _editingId = h.id;
            txtHeaderName.Text = h.name;
            cboHeaderType.SelectedIndex = cboHeaderType.Items.IndexOf(h.type);
            if (cboHeaderType.SelectedIndex < 0) cboHeaderType.SelectedIndex = 0;
            decimal disp = MetresToDisplay(h.widthM);
            numWidth.Value = System.Math.Max(numWidth.Minimum, System.Math.Min(numWidth.Maximum, disp));
            decimal dispOff = MetresToDisplay(h.fwdOffsetM);
            numOffset.Value = System.Math.Max(numOffset.Minimum, System.Math.Min(numOffset.Maximum, dispOff));
        }

        private void btnNew_Click(object sender, EventArgs e) => ClearEdit();

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtHeaderName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { Props.ShowMessage(Lang.lgEnterHeaderName, "", 2000, true); return; }
            string type   = cboHeaderType.SelectedItem?.ToString() ?? "Draper";
            double width  = DisplayToMetres(numWidth.Value);
            double offset = DisplayToMetres(numOffset.Value);

            if (_editingId < 0)
                Core.Database.Headers.Create(name, type, width, offset);
            else
                Core.Database.Headers.Update(_editingId, name, type, width, offset);

            Core.RaiseHeaderListChanged();
            LoadList();
            ClearEdit();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editingId < 0) return;
            if (_headers.Count <= 1) { Props.ShowMessage(Lang.lgMustHaveOneHeader, "", 2000, true); return; }
            using var dlg = new frmMsgBox(Lang.lgDeleteHeaderPrompt);
            dlg.ShowDialog(this);
            if (!dlg.Result) return;
            Core.Database.Headers.Delete(_editingId);
            Core.RaiseHeaderListChanged();
            LoadList();
            ClearEdit();
        }

        private void btnTitleClose_Click(object sender, EventArgs e)   => this.Close();
        private void btnHeadersClose_Click(object sender, EventArgs e) => this.Close();
    }
}
