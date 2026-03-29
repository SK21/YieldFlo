using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YieldFlo.Printing
{
    public class RichTextBoxPrinterRTF
    {
        private readonly RichTextBox _rtb;
        private int _charFrom;

        public RichTextBoxPrinterRTF(RichTextBox rtb)
        {
            _rtb = rtb ?? throw new ArgumentNullException(nameof(rtb));
        }

        public void Print(string documentName = null)
        {
            var pd = new PrintDocument();
            pd.BeginPrint += (s, e) => _charFrom = 0;
            pd.PrintPage  += OnPrintPage;

            var dlg = new PrintDialog { Document = pd };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // PDF/XPS drivers show their own Save dialog before the spooler job
            // is registered, so DocumentName never reaches it. Bypass by using
            // PrintToFile with our own SaveFileDialog.
            string printer = pd.PrinterSettings.PrinterName;
            bool isPdf = printer.IndexOf("pdf", StringComparison.OrdinalIgnoreCase) >= 0;
            bool isXps = printer.IndexOf("xps", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isPdf || isXps)
            {
                string ext    = isXps ? "xps" : "pdf";
                string filter = isXps ? "XPS Document (*.xps)|*.xps" : "PDF Document (*.pdf)|*.pdf";
                using var sfd = new SaveFileDialog
                {
                    FileName   = documentName ?? "report",
                    Filter     = filter,
                    DefaultExt = ext
                };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                pd.PrinterSettings.PrintToFile   = true;
                pd.PrinterSettings.PrintFileName = sfd.FileName;
            }
            else if (!string.IsNullOrEmpty(documentName))
            {
                pd.DocumentName = documentName;
            }

            pd.Print();
        }

        private void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            _charFrom = FormatRange(e, _charFrom, _rtb.TextLength);
            e.HasMorePages = (_charFrom < _rtb.TextLength);
            if (!e.HasMorePages)
                FormatRangeDone(_rtb);
        }

        #region Native Interop

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE { public int cpMin, cpMax; }

        [StructLayout(LayoutKind.Sequential)]
        private struct FORMATRANGE
        {
            public IntPtr hdc, hdcTarget;
            public RECT rc, rcPage;
            public CHARRANGE chrg;
        }

        private const int WM_USER        = 0x0400;
        private const int EM_FORMATRANGE = WM_USER + 57;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private int FormatRange(PrintPageEventArgs e, int charFrom, int charTo)
        {
            RECT rcPage = new RECT
            {
                Top    = Twips(e.PageBounds.Top),
                Bottom = Twips(e.PageBounds.Bottom),
                Left   = Twips(e.PageBounds.Left),
                Right  = Twips(e.PageBounds.Right)
            };
            RECT rc = new RECT
            {
                Top    = Twips(e.MarginBounds.Top),
                Bottom = Twips(e.MarginBounds.Bottom),
                Left   = Twips(e.MarginBounds.Left),
                Right  = Twips(e.MarginBounds.Right)
            };
            CHARRANGE cr = new CHARRANGE { cpMin = charFrom, cpMax = charTo };

            IntPtr hdc = e.Graphics.GetHdc();
            FORMATRANGE fr = new FORMATRANGE { hdc = hdc, hdcTarget = hdc, rc = rc, rcPage = rcPage, chrg = cr };

            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
            Marshal.StructureToPtr(fr, lParam, false);
            IntPtr res = SendMessage(_rtb.Handle, EM_FORMATRANGE, (IntPtr)1, lParam);
            Marshal.FreeCoTaskMem(lParam);
            e.Graphics.ReleaseHdc(hdc);

            return res.ToInt32();
        }

        private void FormatRangeDone(RichTextBox rtb)
        {
            SendMessage(rtb.Handle, EM_FORMATRANGE, (IntPtr)0, IntPtr.Zero);
        }

        private static int Twips(int hundredthInch) => (int)(hundredthInch * 14.4);

        #endregion
    }
}
