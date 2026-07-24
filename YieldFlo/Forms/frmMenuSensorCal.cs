using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuSensorCal : Form
    {
        private bool _dragging;
        private Point _dragStart;

        private List<(int id, string name, string category, double testWeight, double marketMoisture, double dryMoisture, double moistureOffset)> _crops;
        private List<(int id, string name, string combineId, double tempOffset, double tempScale, double moistScale)> _profiles;

        private int _selectedCropId    = -1;
        private int _selectedProfileId = -1;

        public frmMenuSensorCal()
        {
            InitializeComponent();
        }

        private void frmMenuSensorCal_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => { FormPositions.Save(this); tmrLive.Stop(); };
            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp   += (s, ev) => _dragging = false;
            }

            LoadCrops();
            LoadProfiles();
            this.Shown += frmMenuSensorCal_Shown;
        }

        private void frmMenuSensorCal_Shown(object sender, EventArgs e)
        {
            NumpadHelper.Wire(this, numCalMeter,    0,  40, 1, "Meter Reading (%)");
            NumpadHelper.Wire(this, numMoistOffset, -25,   25,    1, "Moisture Offset (%)");
            NumpadHelper.Wire(this, numMoistScale,  0.0001, 0.01, 4, "Scale (%/count)");
            NumpadHelper.Wire(this, numCalThermo,  -20,   60,    1, "Meter (°C)");
            NumpadHelper.Wire(this, numTempOffset, -10,   10,    1, "Temp Offset (°C)");
            NumpadHelper.Wire(this, numTempScale,  0.001, 0.1, 4, "Scale (°C/count)");
            tmrLive.Start();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);
            pnlTitle.BackColor   = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor   = Color.FromArgb(180, 200, 220);
            foreach (Control c in pnlContent.Controls)
            {
                c.ForeColor = fore;
                if (c is ComboBox cb)      { cb.BackColor = ctrl; cb.ForeColor = fore; }
                if (c is NumericUpDown nd) { nd.BackColor = ctrl; nd.ForeColor = fore; }
            }
            btnApplyMoist.BackColor = Color.FromArgb(0, 70, 120); btnApplyMoist.ForeColor = Color.White;
            btnApplyTemp.BackColor  = Color.FromArgb(0, 70, 120); btnApplyTemp.ForeColor  = Color.White;
            btnSave.BackColor       = Color.FromArgb(0, 90, 0);   btnSave.ForeColor       = Color.White;
            btnSCClose.BackColor    = Color.FromArgb(60, 60, 60); btnSCClose.ForeColor    = Color.White;
        }

        private void LoadCrops()
        {
            _crops = Core.Database.Crops.GetAll();
            cboCrop.Items.Clear();
            foreach (var c in _crops)
                cboCrop.Items.Add(c.name);

            int defaultIdx = _crops.FindIndex(c => c.id == Core.ActiveCropId);
            cboCrop.SelectedIndex = defaultIdx >= 0 ? defaultIdx : (_crops.Count > 0 ? 0 : -1);
        }

        private void LoadProfiles()
        {
            _profiles = Core.Database.Profiles.GetAll();
            cboProfile.Items.Clear();
            foreach (var p in _profiles)
                cboProfile.Items.Add(p.name);

            int defaultIdx = _profiles.FindIndex(p => p.id == Core.ActiveProfileId);
            cboProfile.SelectedIndex = defaultIdx >= 0 ? defaultIdx : (_profiles.Count > 0 ? 0 : -1);
        }

        private void cboCrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboCrop.SelectedIndex;
            if (idx < 0 || _crops == null || idx >= _crops.Count) return;
            var c = _crops[idx];
            _selectedCropId = c.id;
            numMoistOffset.Value = (decimal)System.Math.Max((double)numMoistOffset.Minimum,
                                   System.Math.Min((double)numMoistOffset.Maximum, c.moistureOffset));
        }

        private void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboProfile.SelectedIndex;
            if (idx < 0 || _profiles == null || idx >= _profiles.Count) return;
            var p = _profiles[idx];
            _selectedProfileId  = p.id;
            numTempOffset.Value = (decimal)System.Math.Max((double)numTempOffset.Minimum,
                                  System.Math.Min((double)numTempOffset.Maximum, p.tempOffset));
            double tscale = p.tempScale > 0 ? p.tempScale : 0.0125;
            numTempScale.Value  = (decimal)System.Math.Max((double)numTempScale.Minimum,
                                  System.Math.Min((double)numTempScale.Maximum, tscale));
            double mscale = p.moistScale > 0 ? p.moistScale : 0.001;
            numMoistScale.Value = (decimal)System.Math.Max((double)numMoistScale.Minimum,
                                  System.Math.Min((double)numMoistScale.Maximum, mscale));
        }

        private void tmrLive_Tick(object sender, EventArgs e)
        {
            double m = Core.LastMoisture;
            lblMoistLive.Text = Core.LastMoistureOk ? $"{m:F1}%" : "—";

            double t = Core.LastTemperature;
            lblTempLive.Text = Core.LastTemperatureOk ? $"{t:F1}°C" : "—";
        }

        private void btnApplyMoist_Click(object sender, EventArgs e)
        {
            double appReading = Core.LastMoisture;
            if (!Core.LastMoistureOk) { Props.ShowMessage(Lang.lgNoLiveMoistReading, "", 2000, true); return; }
            double offset = System.Math.Round((double)numCalMeter.Value - appReading, 1);
            offset = System.Math.Max((double)numMoistOffset.Minimum,
                     System.Math.Min((double)numMoistOffset.Maximum, offset));
            numMoistOffset.Value = (decimal)offset;
        }

        private void btnApplyTemp_Click(object sender, EventArgs e)
        {
            double appReading = Core.LastTemperature;
            if (!Core.LastTemperatureOk) { Props.ShowMessage(Lang.lgNoLiveTempReading, "", 2000, true); return; }
            double offset = System.Math.Round((double)numCalThermo.Value - appReading, 1);
            offset = System.Math.Max((double)numTempOffset.Minimum,
                     System.Math.Min((double)numTempOffset.Maximum, offset));
            numTempOffset.Value = (decimal)offset;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_selectedCropId > 0)
            {
                double mo = (double)numMoistOffset.Value;
                Core.Database.Crops.UpdateMoistureOffset(_selectedCropId, mo);
                if (_selectedCropId == Core.ActiveCropId)
                    Core.ActiveMoistureOffset = mo;
            }

            if (_selectedProfileId > 0)
            {
                double to = (double)numTempOffset.Value;
                double ts = (double)numTempScale.Value;
                double ms = (double)numMoistScale.Value;
                Core.Database.Profiles.UpdateTempOffset(_selectedProfileId, to);
                Core.Database.Profiles.UpdateTempScale(_selectedProfileId, ts);
                Core.Database.Profiles.UpdateMoistScale(_selectedProfileId, ms);
                if (_selectedProfileId == Core.ActiveProfileId)
                {
                    Core.ActiveTempOffset = to;
                    Core.ActiveTempScale  = ts;
                    Core.ActiveMoistScale = ms;
                }
            }

            Props.ShowMessage(Lang.lgSaved, "", 1500, true);
        }

        private void btnSCClose_Click(object sender, EventArgs e)    => this.Close();
    }
}
