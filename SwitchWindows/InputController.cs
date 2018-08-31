using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SwitchWindows
{
    class InputController
    {
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const int MOUSEEVENTF_WHEEL = 0x0800;
        public const int MOUSEEVENTF_HWHEEL = 0x01000;

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


        public ReceivedData receivedData;
        public void ControlComputer()
        {
            switch (receivedData.type)
            {
                case "SelectTitle":
                    SelectWindow(receivedData);
                    break;
                case "MouseEvent":
                    ControlMouse();
                    break;
            }
        }
        private void SelectWindow(ReceivedData _receivedData)
        {
            string selectedTitle_ = _receivedData.message;
            IntPtr hWnd_ = FindWindow(null, selectedTitle_);
            if (hWnd_ != IntPtr.Zero)
            {
                SetActiveWindow(hWnd_);
                SetForegroundWindow(hWnd_);
                Console.WriteLine("Window selected : {0}", selectedTitle_);
            }
            else
            {
                Console.WriteLine("Failed to select window");
            }
        }
        private void ControlMouse()
        {
            switch (receivedData.message)
            {
                case "RClickDOWN":
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    break;
                case "RClickUP":
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    break;
                case "LClickDOWN":
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    break;
                case "LClickUP":
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    break;
                case "MoveCursor":
                    MoveMouseCursor(receivedData.variationX, receivedData.variationY);
                    break;
                case "ScrollWindow":
                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (int)receivedData.variationY, 0);
                    break;
                case "HScrollWindow":
                    mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, (int)receivedData.variationX, 0);
                    break;
            }
        }
        private void MoveMouseCursor(double _variationX, double _variationY)
        {
            POINT point_;
            GetCursorPos(out point_);

            SetCursorPos((int)(point_.X+_variationX), (int)(point_.Y+_variationY));
        }
    }
}
