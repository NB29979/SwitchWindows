using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace SwitchWindows
{
    class Client
    {
        private static IPAddress iPAddress;

        public string Encode(List<String> _visibleWindows)
        {
            // カンマを区切り文字にしてリストを文字列に変換
            return string.Join(",", _visibleWindows.ToArray());
        }
        public async Task SendDataAsync(string _sendStr)
        {
            try
            {
                TcpClient sender = new TcpClient();
                iPAddress = IPAddress.Parse("192.168.1.16");
                await sender.ConnectAsync(iPAddress, 10080);

                NetworkStream networkStream_ = sender.GetStream();
                StreamWriter writer_ = new StreamWriter(networkStream_);
                writer_.AutoFlush = true;
                await writer_.WriteLineAsync(_sendStr);

                sender.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        } 
        public async Task InProgressAsync()
        {
            List<string> oldVisibleWindows_ = new List<string>();
            List<string> newVisibleWindows_ = new List<string>();
            Console.WriteLine("Window watching...");
            while (true)
            {
                newVisibleWindows_ = Win32Api.GetVisibleWindows().Distinct().ToList();

                // 新しくウインドウが開かれた場合と
                // 既存のウインドウが閉じられた場合にウインドウ一覧を送る
                if (newVisibleWindows_.Except(oldVisibleWindows_).ToList().Count != 0 ||
                    oldVisibleWindows_.Except(newVisibleWindows_).ToList().Count != 0)
                {
                    Console.WriteLine("Window List was Changed");
                    await SendDataAsync(JsonConvert.SerializeObject(newVisibleWindows_));
                   
                    Console.WriteLine("WindowList was sent to device");
                    oldVisibleWindows_ = newVisibleWindows_;
                }
            }
        }
    }
}
