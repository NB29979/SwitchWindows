using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;

namespace SwitchWindows
{
    class Server
    {
        IPAddress ipAddress;
                    bool inCursorMoving = false;

        public Server()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            this.ipAddress = null;
            foreach (var e in ipHostInfo.AddressList)
            {
                if (e.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.ipAddress = e;
                    break;
                }
            }
            Console.WriteLine(ipAddress);
        }
        public async Task InProcessAsync()
        {
            TcpListener tcpListener_ = new TcpListener(IPAddress.Any, 10090);
            tcpListener_.Start();
            CancellationTokenSource cursorMoveCancelTokenSource = new CancellationTokenSource();

            while (true)
            {
                try
                {
                    Console.WriteLine("Listening...");
                    TcpClient tcpClient_ = await tcpListener_.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected");

                    ReceivedData receivedData = await ParseReceivedDataAsync(tcpClient_);

                    if (receivedData.type == "SelectTitle")
                    {
                        SelectWindow(receivedData);
                    }
                    else if (receivedData.type == "MouseEvent")
                    {
                        if (receivedData.message == "SingleTap")
                        {
                            cursorMoveCancelTokenSource.Cancel();
                        }
                        else if (receivedData.message == "RClickDOWN")
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
                            cursorMoveCancelTokenSource.Cancel();
                            if (cursorMoveCancelTokenSource.IsCancellationRequested)
                                cursorMoveCancelTokenSource = new CancellationTokenSource();
                            MoveMouseCursorAsync(receivedData.rad, receivedData.absX, receivedData.absY, cursorMoveCancelTokenSource.Token);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        private async Task<ReceivedData> ParseReceivedDataAsync(TcpClient _tcpClient)
        {
            try
            {
                NetworkStream networkStream_ = _tcpClient.GetStream();
                StreamReader reader_ = new StreamReader(networkStream_);
                string clientReq_ = await reader_.ReadLineAsync();
                _tcpClient.Close();

                return JsonConvert.DeserializeObject<ReceivedData>(clientReq_);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
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
        private async Task MoveMouseCursorAsync(double _rad, double _absX, double _absY, CancellationToken _token)
        {
            double speedX_ = Math.Pow(2,_absX/60);
            double speedY_ = Math.Pow(2,_absY/60);

            for(int i = 0; i < 15; ++i)
            {
                if (_token.IsCancellationRequested) return;
                Win32Api.POINT point_;
                Win32Api.GetCursorPos(out point_);

                Win32Api.SetCursorPos((int)(point_.X+speedX_*Math.Cos(_rad)),
                    (int)(point_.Y+speedY_*Math.Sin(_rad)));
                await Task.Delay(10);
            }
        }
    }
}
