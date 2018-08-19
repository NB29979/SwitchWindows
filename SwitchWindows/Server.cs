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
                        cursorMoveCancelTokenSource.Cancel();
                        if (cursorMoveCancelTokenSource.IsCancellationRequested)
                            cursorMoveCancelTokenSource = new CancellationTokenSource();
                        MoveMouseCursorAsync(receivedData.rad, cursorMoveCancelTokenSource.Token);
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
        private async Task MoveMouseCursorAsync(double _rad, CancellationToken _token)
        {
            Console.WriteLine(_token.IsCancellationRequested);
            for(int i = 0; i < 1000; ++i)
            {
                if (_token.IsCancellationRequested) return;
                Win32Api.POINT point_;
                Win32Api.GetCursorPos(out point_);

                Win32Api.SetCursorPos((int)(point_.X+5*Math.Cos(_rad)), (int)(point_.Y+5*Math.Sin(_rad)));
                await Task.Delay(5);
            }
        }
    }
}
