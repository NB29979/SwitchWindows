using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SwitchWindows
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> oldVisibleWindows = new List<string>();
            List<string> newVisibleWindows = new List<string>();
            Client client = new Client();

            Console.WriteLine("Window watching...");
            while (true)
            {
                newVisibleWindows = Win32Api.GetVisibleWindows().Distinct().ToList();

                // 新しくウインドウが開かれた場合と既存のウインドウが閉じられた場合
                if (newVisibleWindows.Except(oldVisibleWindows).ToList().Count != 0 ||
                    oldVisibleWindows.Except(newVisibleWindows).ToList().Count != 0)
                {
                    Console.WriteLine("Changed");

                    client.Encode(newVisibleWindows);
                    client.Send();
                    Console.WriteLine("Windows status sent");
                    oldVisibleWindows = newVisibleWindows;
                }
            }
        }
        static void Debug(List<String> _list)
        {
            foreach(var e in _list){
                Console.Write("{0},", e);
            }
            Console.WriteLine();
        }
    }
}
