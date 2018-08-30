using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using WebSocket4Net;

namespace SwitchWindows
{
    class RealTimeServer
    {
        IPAddress ipAddress;
        public RealTimeServer()
        {
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //this.ipAddress = null;
            //foreach (var e in ipHostInfo.AddressList)
            //{
            //    if (e.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        this.ipAddress = e;
            //        break;
            //    }
            //}
            //Console.WriteLine(ipAddress);
        }
    }
}
