using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SwitchWindows
{
    class Client
    {
        private string strVisibleWindows;
        private static IPAddress iPAddress;

        public void Encode(List<String> _visibleWindows)
        {
            // カンマを区切り文字にしてリストを文字列に変換
            strVisibleWindows = string.Join(",", _visibleWindows.ToArray());
        }
        public async Task SendDataAsync()
        {
            try
            {
                TcpClient sender = new TcpClient();
                iPAddress = IPAddress.Parse("192.168.1.16");
                await sender.ConnectAsync(iPAddress, 10080);

                NetworkStream networkStream_ = sender.GetStream();
                StreamWriter writer_ = new StreamWriter(networkStream_);
                writer_.AutoFlush = true;
                await writer_.WriteLineAsync(strVisibleWindows);

                sender.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        } 
    }
}
