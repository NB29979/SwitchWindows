using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SwitchWindows
{
    class Server
    {
        static IPAddress ipAddress;
        //static TcpClient tcpClient;
        //static bool isDisconnected = false;
        //static Encoding encoding;
        //static NetworkStream networkStream;

        public async Task InProcessAsync()
        {
            ipAddress = IPAddress.Parse("192.168.1.7");
            TcpListener tcpListener_ = new TcpListener(ipAddress, 10090);

            tcpListener_.Start();
            Console.WriteLine("Start listening...");

            while (true)
            {
                try
                {
                    TcpClient tcpClient_ = await tcpListener_.AcceptTcpClientAsync();
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

                // FocusWindow予定地
                Console.WriteLine("{0}", clientReq_);

                _tcpClient.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //public static void inProcess()
        //{
        //    tcpListener.Start();
        //    Console.WriteLine("Listening...");

        //    tcpClient = tcpListener.AcceptTcpClient();
        //    Console.WriteLine("Client Accepted");
        //    receiveClientData();
        //    if (!isDisconnected)
        //    {
        //        sendData();
        //    }
        //    networkStream.Close();
        //    tcpClient.Close();
        //    Console.WriteLine("Connection Closed");

        //    tcpListener.Stop();
        //    Console.WriteLine("Listener Closed");
        //}

        //private static void receiveClientData()
        //{
        //    networkStream = tcpClient.GetStream();
        //    networkStream.ReadTimeout = 10000; // [ms]
        //    networkStream.WriteTimeout = 10000; // [ms]

        //    encoding = System.Text.Encoding.UTF8;
        //    System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
        //    byte[] resBytes = new byte[256];
        //    int resSize = 0;
        //    do
        //    {
        //        resSize = networkStream.Read(resBytes, 0, resBytes.Length);
        //        if(resSize == 0)
        //        {
        //            isDisconnected = true;
        //            Console.WriteLine("Client Disconnected");
        //            break;
        //        }
        //        memoryStream.Write(resBytes, 0, resSize);
        //    } while (networkStream.DataAvailable);

        //    string resMsg = encoding.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        //    memoryStream.Close();
        //    Console.WriteLine(resMsg);

        //}
        //private static void sendData()
        //{
        //    string sendMsg = "hoge";
        //    byte[] sendBytes = encoding.GetBytes(sendMsg);
        //    networkStream.Write(sendBytes, 0, sendBytes.Length);
        //    Console.WriteLine("Message sended");
        //}
    }
}
