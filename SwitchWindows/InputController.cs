using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchWindows
{
    class InputController
    {
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
            IntPtr hWnd_ = Win32Api.FindWindow(null, selectedTitle_);
            if (hWnd_ != IntPtr.Zero)
            {
                Win32Api.SetActiveWindow(hWnd_);
                Win32Api.SetForegroundWindow(hWnd_);
                Console.WriteLine("Window selected : {0}", selectedTitle_);
            }
            else
            {
                Console.WriteLine("Failed to select window");
            }
        }
        private void ControlMouse()
        {
            if (receivedData.message == "RClickDOWN")
            {
                Win32Api.mouse_event(Win32Api.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            }
            else if (receivedData.message == "RClickUP")
            {
                Win32Api.mouse_event(Win32Api.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            }
            else if (receivedData.message == "LClickDOWN")
            {
                Win32Api.mouse_event(Win32Api.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            }
            else if (receivedData.message == "LClickUP")
            {
                Win32Api.mouse_event(Win32Api.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            else if (receivedData.message == "MoveCursor")
            {
                MoveMouseCursor(receivedData.variationX, receivedData.variationY);
            }
        }
        private void MoveMouseCursor(double _variationX, double _variationY)
        {
            Win32Api.POINT point_;
            Win32Api.GetCursorPos(out point_);

            Win32Api.SetCursorPos((int)(point_.X+_variationX), (int)(point_.Y+_variationY));
        }
    }
}
