using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SwitchWindows
{
    public class IconExtractor
    {
        [DllImport("Shell32.dll")]
        private static extern int ExtractIconExA(string lpszFile, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);
        [DllImport("User32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);

        public static void ExtractIcons(List<WindowRowData> _visibleWindows)
        {
            _visibleWindows.ForEach(w => 
            {
                Icon icon_ = ExtractIcon(w.icon);
                string base64ImageString_ = "null";

                if (icon_ != null)
                {
                    Bitmap bmp = icon_.ToBitmap();

                    System.IO.MemoryStream ms_ = new System.IO.MemoryStream();
                    bmp.Save(ms_, System.Drawing.Imaging.ImageFormat.Png);
                    bmp.Dispose();

                    base64ImageString_ = Convert.ToBase64String(ms_.ToArray());
                }

                w.icon = base64ImageString_;
            });
        }
        private static Icon ExtractIcon(string _exePath)
        {
            IntPtr largeIcon_ = IntPtr.Zero;
            IntPtr smallIcon_ = IntPtr.Zero;
            ExtractIconExA(_exePath, 0, ref largeIcon_, ref smallIcon_, 1);

            if (largeIcon_ != IntPtr.Zero)
            {
                DestroyIcon(smallIcon_);
                return Icon.FromHandle(largeIcon_);
            }
            else if (smallIcon_ != IntPtr.Zero)
            {
                DestroyIcon(largeIcon_);
                return Icon.FromHandle(smallIcon_);
            }
            else
                return null;
        }
        public void Encode() { }
    }
}
