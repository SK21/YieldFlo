using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuCrops : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private List<(int id, string name, string category, double testWeight, double marketMoisture, double dryMoisture, double moistureOffset)> _crops;
        private int _editingId = -1;
        private double _editingMoistureOffset = 0;

        public frmMenuCrops()
        {
            InitializeComponent();
        }

        private void frmMenuCrops_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            lblTestWeightUnit.Text = Props.TestWeightUnit;   // "kg/hL" in metric, "lb/bu" imperial
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
            if (Core.ActiveCropId > 0 && _crops != null)
            {
                int idx = _crops.FindIndex(c => c.id == Core.ActiveCropId);
                if (idx >= 0) lbCrops.SelectedIndex = idx;
            }
            this.Shown += frmMenuCrops_Shown;
        }

        private void frmMenuCrops_Shown(object sender, EventArgs e)
        {
            KeyboardHelper.Wire(this, txtCropName, "Crop Name");
            NumpadHelper.Wire(this, numTestWeight,     0,   200, 0, "Test Weight (" + Props.TestWeightUnit + ")");
            NumpadHelper.Wire(this, numMarketMoisture, 0,    40, 0, "Market Moisture (%)");
            btnSave.Focus();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);
            pnlTitle.BackColor   = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor   = Color.FromArgb(180, 200, 220);
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
                lbCrops.Items.Add($"{c.name}  –  {c.category}  –  {Props.DisplayTestWeight(c.testWeight):F0} {Props.TestWeightUnit}");
        }

        private void ClearEdit()
        {
            _editingId = -1;
            _editingMoistureOffset = 0;
            txtCropName.Text = "";
            cboCropCategory.SelectedIndex = 0;
            numTestWeight.Value     = (decimal)Props.DisplayTestWeight(60);   // 60 lb/bu default, shown in active unit
            numMarketMoisture.Value = 14;
            lbCrops.ClearSelected();
        }

        private void lbCrops_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lbCrops.SelectedIndex;
            if (idx < 0 || _crops == null || idx >= _crops.Count) return;
            var c = _crops[idx];
            _editingId = c.id;
            _editingMoistureOffset = c.moistureOffset;
            txtCropName.Text = c.name;
            cboCropCategory.SelectedIndex = cboCropCategory.Items.IndexOf(c.category);
            if (cboCropCategory.SelectedIndex < 0) cboCropCategory.SelectedIndex = 0;
            numTestWeight.Value     = (decimal)System.Math.Max((double)numTestWeight.Minimum,
                                        System.Math.Min((double)numTestWeight.Maximum, Props.DisplayTestWeight(c.testWeight)));
            numMarketMoisture.Value = (decimal)System.Math.Max((double)numMarketMoisture.Minimum,
                                        System.Math.Min((double)numMarketMoisture.Maximum, c.marketMoisture));
        }

        private void btnNew_Click(object sender, EventArgs e)  => ClearEdit();

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtCropName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { Props.ShowMessage(Lang.lgEnterCropName, "", 2000, true); return; }
            string cat = cboCropCategory.SelectedItem?.ToString() ?? "Cereal";
            double tw  = Props.TestWeightToLbBu((double)numTestWeight.Value);   // store internally as lb/bu
            if (tw <= 0) { Props.ShowMessage(Lang.lgTestWeightRequired, "", 2000, true); return; }
            double mm  = (double)numMarketMoisture.Value;

            int savedId;
            if (_editingId < 0)
                savedId = Core.Database.Crops.Create(name, cat, tw, mm, mm);
            else
            {
                Core.Database.Crops.Update(_editingId, name, cat, tw, mm, mm, _editingMoistureOffset);
                savedId = _editingId;
            }

            Core.RaiseCropListChanged();
            LoadList();
            ClearEdit();
            int sel = _crops?.FindIndex(c => c.id == savedId) ?? -1;
            if (sel >= 0) lbCrops.SelectedIndex = sel;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editingId < 0) return;
            if (_crops.Count <= 1) { Props.ShowMessage(Lang.lgMustHaveOneCrop, "", 2000, true); return; }
            using var dlg = new frmMsgBox(Lang.lgDeleteCropPrompt);
            dlg.ShowDialog(this);
            if (!dlg.Result) return;
            try { Core.Database.Crops.Delete(_editingId); }
            catch (Database.ItemInUseException) { Props.ShowMessage(Lang.lgItemInUseByJob, "", 3000, true); return; }
            Core.RaiseCropListChanged();
            LoadList();
            ClearEdit();
        }

        private void btnCropsClose_Click(object sender, EventArgs e)  => this.Close();
    }
}
