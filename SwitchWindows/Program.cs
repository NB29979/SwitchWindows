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
            List<String> visibleWindows = Win32Api.GetVisibleWindows().Distinct().ToList();

            Client client = new Client();
            client.Encode(visibleWindows);
            client.Send();
        }
    }
}
