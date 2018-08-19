using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

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
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
            //public static implicit operator Point(POINT point)
            //{
            //    return new Point(point.X, point.Y);
            //}
        }
        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);

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
            visibleWindows.Clear();
            EnumWindows(new DelegateEnumWindows(CallbackEnumWindow), IntPtr.Zero);
            return visibleWindows;
        }
    }
}
