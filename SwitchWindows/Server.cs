using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            while (true)
            {
                try
                {
                    Console.WriteLine("Listening...");
                    TcpClient tcpClient_ = await tcpListener_.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected");

                    await FocusWindowAsync(tcpClient_);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        private async Task FocusWindowAsync(TcpClient _tcpClient)
        {
            try
            {
                NetworkStream networkStream_ = _tcpClient.GetStream();
                StreamReader reader_ = new StreamReader(networkStream_);
                string clientReq_ = await reader_.ReadLineAsync();

                ReceivedData receivedData_ = JsonConvert.DeserializeObject<ReceivedData>(clientReq_);

                if (receivedData_.type == "SelectTitle")
                {
                    string selectedTitle_ = receivedData_.message;
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
                _tcpClient.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
