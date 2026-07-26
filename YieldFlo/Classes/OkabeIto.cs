using System.Drawing;

namespace YieldFlo.Classes
{
    // Okabe-Ito colorblind-safe categorical palette — distinguishable under
    // protanopia/deuteranopia/tritanopia. For discrete states (connected/not,
    // active/paused/stopped), not continuous gradients (e.g. the yield map's
    // heat-map scale needs a perceptually-ordered sequential palette instead).
    public static class OkabeIto
    {
        public static readonly Color Orange        = Color.FromArgb(230, 159, 0);
        public static readonly Color SkyBlue       = Color.FromArgb(86, 180, 233);
        public static readonly Color BluishGreen   = Color.FromArgb(0, 158, 115);
        public static readonly Color Yellow        = Color.FromArgb(240, 228, 66);
        public static readonly Color Blue          = Color.FromArgb(0, 114, 178);
        public static readonly Color Vermillion    = Color.FromArgb(213, 94, 0);
        public static readonly Color ReddishPurple = Color.FromArgb(204, 121, 167);
        public static readonly Color Black         = Color.Black;
    }
}
