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
            Client client = new Client();
            Server server = new Server();

            try
            {
                Task.WaitAll(
                    server.InProcessAsync(),
                    client.InProgressAsync()
                );
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
