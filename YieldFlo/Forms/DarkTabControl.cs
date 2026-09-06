using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YieldFlo.Forms
{
    /// <summary>
    /// A TabControl that paints its own chrome, so the tab strip matches the dark
    /// theme the rest of the app uses.
    ///
    /// Owner-drawing the items alone is not enough: with visual styles enabled
    /// (Program.cs calls EnableVisualStyles) a TabControl ignores BackColor and
    /// paints the strip, the gap beside the tabs and the border around the page
    /// from the system theme. Those are drawn by the control itself in WM_PAINT,
    /// so the only way to reach them is to take over painting entirely — hence
    /// UserPaint rather than a DrawItem handler.
    ///
    /// The pages are child controls with their own handles and paint themselves,
    /// so taking over here affects the chrome only.
    /// </summary>
    public class DarkTabControl : TabControl
    {
        /// <summary>Strip behind and beside the tabs.</summary>
        public Color HeaderBack { get; set; } = Color.FromArgb(45, 45, 45);

        /// <summary>Body behind the active page — only its border ring shows.</summary>
        public Color PageBack { get; set; } = Color.FromArgb(45, 45, 45);

        public Color TabBack { get; set; } = Color.FromArgb(60, 60, 60);
        public Color TabSelected { get; set; } = Color.FromArgb(0, 70, 120);
        public Color TabFore { get; set; } = Color.Silver;
        public Color TabForeSelected { get; set; } = Color.White;

        /// <summary>
        /// Outline around the page and the tabs. Without it the tabs read as three
        /// flat coloured blocks rather than a tab control — nothing tells the
        /// operator the strip and the panel below it are one thing.
        /// </summary>
        public Color BorderColour { get; set; } = Color.FromArgb(150, 150, 150);

        /// <summary>Outline thickness. 2 reads as a deliberate frame; 1 disappears.</summary>
        public int BorderWidth { get; set; } = 2;

        public DarkTabControl()
        {
            // Fixed so ItemSize is honoured — the default shrinks each tab to its
            // text, which on a touch screen gives targets too small to hit.
            //
            // Applied in BOTH modes, deliberately: the strip height comes straight
            // off the page area, so a design-time strip shorter than the runtime one
            // means controls laid out against the taller design page get clipped when
            // the app runs. At 26 the page interior is 123px (131 with the default
            // strip) — measured, not guessed.
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(210, 26);

            // Custom painting is runtime-only. A TabControl with UserPaint upsets the
            // WinForms design host — moving a child control inside a page throws in
            // there rather than in this code — and the designer has no need of the
            // dark chrome. LicenseManager rather than DesignMode because DesignMode
            // is not yet valid this early in construction.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Design time, or anything that left UserPaint off: let the stock control
            // draw itself. Painting here without that style set produces a blank
            // control rather than a themed one.
            if (!GetStyle(ControlStyles.UserPaint) || TabPages.Count == 0)
            {
                base.OnPaint(e);
                return;
            }

            var g = e.Graphics;
            g.Clear(HeaderBack);

            // The page panel starts where the tab strip ends. Taken from the first
            // tab rather than ItemSize so it stays right if the strip is ever laid
            // out differently.
            int stripBottom = GetTabRect(0).Bottom;
            var page = new Rectangle(0, stripBottom, Width - 1, Height - stripBottom - 1);

            using (var b = new SolidBrush(PageBack))
                g.FillRectangle(b, page);

            // A pen straddles the path it is given, so every outline here is inset by
            // half its width — otherwise the outer half falls off the control edge and
            // the frame looks thinner on two sides than the other two.
            int inset = BorderWidth / 2;
            using (var p = new Pen(BorderColour, BorderWidth))
                g.DrawRectangle(p, new Rectangle(page.X + inset, page.Y + inset,
                                                 page.Width - BorderWidth, page.Height - BorderWidth));

            for (int i = 0; i < TabPages.Count; i++)
            {
                Rectangle r = PaintRect(i, page);
                bool selected = (SelectedIndex == i);

                // All tabs are the same height. The selected one additionally runs
                // one pixel INTO the page border below it, so the two join into a
                // single outline instead of sitting as a block above a box — that
                // join, and the fill colour, are what mark it as active.
                Rectangle t = selected
                    ? new Rectangle(r.X, r.Y, r.Width, r.Height + 1)
                    : r;

                using (var b = new SolidBrush(selected ? TabSelected : TabBack))
                    g.FillRectangle(b, t);
                using (var p = new Pen(BorderColour, BorderWidth))
                    g.DrawRectangle(p, new Rectangle(t.X + inset, t.Y + inset,
                                                     t.Width - BorderWidth, t.Height - BorderWidth));

                TextRenderer.DrawText(g, TabPages[i].Text, Font, t,
                    selected ? TabForeSelected : TabFore,
                    TextFormatFlags.HorizontalCenter
                  | TextFormatFlags.VerticalCenter
                  | TextFormatFlags.EndEllipsis);
            }

            // Erase the page's top border where the selected tab meets it, so the tab
            // opens into the panel the way a tab control is expected to. Filled to the
            // full border thickness — a single line would leave a seam once the
            // outline is more than one pixel.
            Rectangle sr = PaintRect(SelectedIndex < 0 ? 0 : SelectedIndex, page);
            int keep = BorderWidth;   // leave the tab's own side edges standing
            using (var b = new SolidBrush(TabSelected))
                g.FillRectangle(b, new Rectangle(sr.X + keep, page.Top,
                                                 sr.Width - (keep * 2), BorderWidth));
        }

        /// <summary>
        /// Where a tab is drawn. Everything follows GetTabRect except the first tab,
        /// which is stretched left to meet the page frame: WinForms insets the tab
        /// strip a couple of pixels from the control edge, so the first tab would
        /// otherwise sit slightly inboard of the outline running below it.
        ///
        /// Paint geometry only — the control keeps its own hit regions, so the few
        /// pixels gained are cosmetic and not clickable.
        /// </summary>
        private Rectangle PaintRect(int index, Rectangle page)
        {
            Rectangle r = GetTabRect(index);
            if (index != 0) return r;

            int dx = r.X - page.X;
            return dx > 0 ? new Rectangle(page.X, r.Y, r.Width + dx, r.Height) : r;
        }

        // Repaint the whole strip on selection change: only the two tabs that
        // swapped state are invalidated otherwise, which leaves the old selection
        // highlighted until something else forces a redraw.
        protected override void OnSelectedIndexChanged(System.EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Invalidate();
        }
    }
}
