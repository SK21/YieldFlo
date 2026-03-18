using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmMenuCrops : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private List<(int id, string name, string category, double testWeight, double marketMoisture, double dryMoisture)> _crops;
        private int _editingId = -1;

        public frmMenuCrops()
        {
            InitializeComponent();
        }

        private void frmMenuCrops_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp   += (s, ev) => _dragging = false;
            }
            LoadList();
            ClearEdit();
        }

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
            lbCrops.BackColor    = ctrl;
            lbCrops.ForeColor    = fore;
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
            btnCropsClose.BackColor = Color.FromArgb(60, 60, 60); btnCropsClose.ForeColor = Color.White;
        }

        private void LoadList()
        {
            _crops = Core.Database.Crops.GetAll();
            lbCrops.Items.Clear();
            foreach (var c in _crops)
                lbCrops.Items.Add($"{c.name}  –  {c.category}  –  {c.testWeight:F0} lb/bu");
        }

        private void ClearEdit()
        {
            _editingId = -1;
            txtCropName.Text = "";
            cboCropCategory.SelectedIndex = 0;
            numTestWeight.Value    = 60;
            numMarketMoisture.Value = 14;
            lbCrops.ClearSelected();
        }

        private void lbCrops_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lbCrops.SelectedIndex;
            if (idx < 0 || _crops == null || idx >= _crops.Count) return;
            var c = _crops[idx];
            _editingId = c.id;
            txtCropName.Text = c.name;
            cboCropCategory.SelectedIndex = cboCropCategory.Items.IndexOf(c.category);
            if (cboCropCategory.SelectedIndex < 0) cboCropCategory.SelectedIndex = 0;
            numTestWeight.Value     = (decimal)System.Math.Max(numTestWeight.Minimum,
                                        System.Math.Min(numTestWeight.Maximum, (decimal)c.testWeight));
            numMarketMoisture.Value = (decimal)System.Math.Max(numMarketMoisture.Minimum,
                                        System.Math.Min(numMarketMoisture.Maximum, (decimal)c.marketMoisture));
        }

        private void btnNew_Click(object sender, EventArgs e)  => ClearEdit();

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtCropName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { Props.ShowMessage("Enter a crop name.", "", 2000, true); return; }
            string cat = cboCropCategory.SelectedItem?.ToString() ?? "Cereal";
            double tw  = (double)numTestWeight.Value;
            double mm  = (double)numMarketMoisture.Value;

            if (_editingId < 0)
                Core.Database.Crops.Create(name, cat, tw, mm, mm);
            else
                Core.Database.Crops.Update(_editingId, name, cat, tw, mm, mm);

            LoadList();
            ClearEdit();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editingId < 0) return;
            using var dlg = new frmMsgBox("Delete this crop?", "Confirm", true);
            dlg.ShowDialog();
            if (!dlg.Result) return;
            Core.Database.Crops.Delete(_editingId);
            LoadList();
            ClearEdit();
        }

        private void btnTitleClose_Click(object sender, EventArgs e)  => this.Close();
        private void btnCropsClose_Click(object sender, EventArgs e)  => this.Close();
    }
}
