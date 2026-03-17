using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace YieldFlo.Classes
{
    public class clsTools
    {
        private Bitmap cScreenBitmap;
        private int cScreenBitmapHeight = 465;
        private int cScreenBitmapWidth = 516;

        public clsTools()
        {
            _ = InitializeAsync();
        }

        public byte BitClear(byte b, int pos)
        {
            byte msk = (byte)(1 << pos);
            msk = (byte)~msk;
            return (byte)(b & msk);
        }

        public bool BitRead(byte b, int pos)
        {
            return ((b >> pos) & 1) != 0;
        }

        public byte BitSet(byte b, int pos)
        {
            return (byte)(b | (1 << pos));
        }

        public string ClipText(string text, int length)
        {
            if (text.Length > length)
                return text.Substring(0, length);
            return text;
        }

        public byte CRC(byte[] Data, int Length, byte Start = 0)
        {
            byte Result = 0;
            if (Length <= Data.Length)
            {
                int CK = 0;
                for (int i = Start; i < Length; i++)
                    CK += Data[i];
                Result = (byte)CK;
            }
            return Result;
        }

        public bool GoodCRC(byte[] Data, byte Start = 0)
        {
            int Length = Data.Length;
            byte cr = CRC(Data, Length - 1, Start);
            return cr == Data[Length - 1];
        }

        public bool PrevInstance()
        {
            string PrsName = Process.GetCurrentProcess().ProcessName;
            Process[] All = Process.GetProcessesByName(PrsName);
            return All.Length > 1;
        }

        public bool UseLightContrast()
        {
            Color background = Properties.Settings.Default.DisplayBackColour;
            double luminance =
                0.2126 * background.R +
                0.7152 * background.G +
                0.0722 * background.B;
            return luminance < 128;
        }

        public Bitmap ScreenBitmap { get { return cScreenBitmap; } }

        public Color ColorFromHSV(float hue, float saturation, float brightness)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = (float)(hue / 60 - Math.Floor(hue / 60));
            brightness = brightness * 255;
            int v = Math.Min(255, Math.Max(0, Convert.ToInt32(brightness)));
            int p = Math.Min(255, Math.Max(0, Convert.ToInt32(brightness * (1 - saturation))));
            int q = Math.Min(255, Math.Max(0, Convert.ToInt32(brightness * (1 - f * saturation))));
            int t = Math.Min(255, Math.Max(0, Convert.ToInt32(brightness * (1 - (1 - f) * saturation))));

            switch (hi)
            {
                case 0: return Color.FromArgb(255, v, t, p);
                case 1: return Color.FromArgb(255, q, v, p);
                case 2: return Color.FromArgb(255, p, v, t);
                case 3: return Color.FromArgb(255, p, q, v);
                case 4: return Color.FromArgb(255, t, p, v);
                default: return Color.FromArgb(255, v, p, q);
            }
        }

        private void CreateColorBitmap()
        {
            cScreenBitmap = new Bitmap(cScreenBitmapWidth, cScreenBitmapHeight);
            for (int x = 0; x < cScreenBitmap.Width; x++)
            {
                for (int y = 0; y < cScreenBitmap.Height; y++)
                {
                    float hue = (float)x / cScreenBitmap.Width;
                    float brightness = 1 - (float)y / cScreenBitmap.Height;
                    cScreenBitmap.SetPixel(x, y, ColorFromHSV(hue * 360, 1, brightness));
                }
            }
        }

        private async Task InitializeAsync()
        {
            await Task.Run(() => CreateColorBitmap());
        }
    }
}
