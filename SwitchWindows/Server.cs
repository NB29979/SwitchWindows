using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchWindows
{
    class Server
    {
        static string address = "192.168.1.1";
        static System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(address);
        static int port = 8080;
        static System.Net.Sockets.TcpListener tcpListener;
        static System.Net.Sockets.TcpClient tcpClient;
        static bool isDisconnected = false;
        static System.Text.Encoding encoding;
        static System.Net.Sockets.NetworkStream networkStream;

        public static void init()
        {
           tcpListener = new System.Net.Sockets.TcpListener(ipAddress, port);
        }
        public static void inProcess()
        {
            tcpListener.Start();
            Console.WriteLine("Listening...");

            tcpClient = tcpListener.AcceptTcpClient();
            Console.WriteLine("Client Accepted");
            receiveClientData();
            if (!isDisconnected)
            {
                sendData();
            }
            networkStream.Close();
            tcpClient.Close();
            Console.WriteLine("Connection Closed");

            tcpListener.Stop();
            Console.WriteLine("Listener Closed");
        }

        private static void receiveClientData()
        {
            networkStream = tcpClient.GetStream();
            networkStream.ReadTimeout = 10000; // [ms]
            networkStream.WriteTimeout = 10000; // [ms]

            encoding = System.Text.Encoding.UTF8;
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize = 0;
            do
            {
                resSize = networkStream.Read(resBytes, 0, resBytes.Length);
                if(resSize == 0)
                {
                    isDisconnected = true;
                    Console.WriteLine("Client Disconnected");
                    break;
                }
                memoryStream.Write(resBytes, 0, resSize);
            } while (networkStream.DataAvailable);

            string resMsg = encoding.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            memoryStream.Close();
            Console.WriteLine(resMsg);

        }
        private static void sendData()
        {
            string sendMsg = "hoge";
            byte[] sendBytes = encoding.GetBytes(sendMsg);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            Console.WriteLine("Message sended");
        }
    }
}
