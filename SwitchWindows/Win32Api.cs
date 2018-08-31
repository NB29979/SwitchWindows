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
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const int MOUSEEVENTF_WHEEL = 0x0800;
        public const int MOUSEEVENTF_HWHEEL = 0x01000;
        private const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        private const int LIST_MODULES_ALL = 0x03;
        private delegate bool DelegateEnumWindows(IntPtr hWnd, IntPtr lparam);
        private static List<WindowRowData> visibleWindows = new List<WindowRowData>();

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
        public struct POINT { public int X; public int Y; }
        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("User32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("User32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("Kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        [DllImport("Psapi.dll")]
        private static extern bool EnumProcessModulesEx(IntPtr hProcess, out IntPtr lphModule, int cb, out uint lpcbNeeded, int dwFilterFlag);
        [DllImport("Psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, int nSize);
        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Shell32.dll")]
        private static extern int ExtractIconExA(StringBuilder lpszFile, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);
        [DllImport("User32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);

        private static bool CallbackEnumWindow(IntPtr _hWnd, IntPtr _lparam) {
            int windowTextLength = GetWindowTextLength(_hWnd)+1;

            if (windowTextLength > 1 && IsWindowVisible(_hWnd))
            {
                StringBuilder visibleWindowTitle = new StringBuilder(windowTextLength);
                GetWindowText(_hWnd, visibleWindowTitle, windowTextLength);

                const int pathLength = 1024;
                StringBuilder path2exe = new StringBuilder(pathLength);
                GetExecuteFilePath(visibleWindowTitle.ToString(), _hWnd, path2exe);

                visibleWindows.Add(new WindowRowData(visibleWindowTitle.ToString(), path2exe.ToString()));
            }
            return true;
        }
        private static void GetExecuteFilePath(string _windowTitle, IntPtr _hWnd, StringBuilder _path2exe)
        {
            uint processId;
            GetWindowThreadProcessId(_hWnd, out processId);
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
            if (hProcess != null)
            {
                IntPtr hModule = new IntPtr();
                uint cbNeeded;
                if (EnumProcessModulesEx(hProcess, out hModule,
                    System.Runtime.InteropServices.Marshal.SizeOf(hModule), out cbNeeded, LIST_MODULES_ALL))
                    GetModuleFileNameEx(hProcess, hModule, _path2exe, _path2exe.Capacity);
                CloseHandle(hProcess);
            }
        }

        public static List<WindowRowData> GetVisibleWindows()
        {
            visibleWindows.Clear();
            EnumWindows(new DelegateEnumWindows(CallbackEnumWindow), IntPtr.Zero);
            return visibleWindows;
        }
    }
}
