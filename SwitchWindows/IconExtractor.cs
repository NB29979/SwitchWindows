using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SwitchWindows
{
    class IconExtractor
    {
        [DllImport("Shell32.dll")]
        private static extern int ExtractIconExA(StringBuilder lpszFile, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);
        [DllImport("User32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);
    }
}
