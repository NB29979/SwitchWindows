using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchWindows
{
    class ReceivedData
    {
        public string type { set;  get; } //将来的にEnum
        public string message { set; get; }
        public double absX { set; get; }
        public double absY { set; get; }
        public double rad { set; get; }
        public int pointerCount { set; get; }
    }
}
