using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchWindows
{
    class ReceivedData
    {
        public string type { set;  get; }
        public string message { set; get; }
        public int direction { set; get; }
        public int speed { set; get; }

        public void Debug()
        {
            Console.WriteLine("type:{0}, receivedMessage:{1}", type, message);
        }
    }
}
