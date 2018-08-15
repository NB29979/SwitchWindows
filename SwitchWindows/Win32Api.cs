using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SwitchWindows
{
    class Win32Api
    {
        private delegate bool DelegateEnumWindows(IntPtr hWnd, IntPtr lparam);
        private static List<String> visibleWindows = new List<string>();

        [DllImport("User32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int length);
        [DllImport("User32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(DelegateEnumWindows lpEnumFunc, IntPtr lparam);
        [DllImport("User32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder text, int length);
        [DllImport("User32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private static bool CallbackEnumWindow(IntPtr _hWnd, IntPtr _lparam) {
            int windowTextLength = GetWindowTextLength(_hWnd)+1;

            if (windowTextLength > 1 && IsWindowVisible(_hWnd))
            {
                StringBuilder sb = new StringBuilder(windowTextLength);
                GetWindowText(_hWnd, sb, windowTextLength);
                visibleWindows.Add(sb.ToString());
            }
            return true;
        }

        public void DispAllWindows()
        {
            EnumWindows(new DelegateEnumWindows(CallbackEnumWindow), IntPtr.Zero);
        }
        public static List<String> GetVisibleWindows()
        {
            EnumWindows(new DelegateEnumWindows(CallbackEnumWindow), IntPtr.Zero);
            return visibleWindows;
        }
    }
}
