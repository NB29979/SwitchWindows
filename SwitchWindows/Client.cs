using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchWindows
{
    class Client
    {
        public void send()
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPAddress iPAddress = IPAddress.Parse("192.168.1.16");
                IPEndPoint remoteEP = new IPEndPoint(iPAddress, 10080);

                Socket sender = new Socket(iPAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                    byte[] msg = Encoding.ASCII.GetBytes("this is test");

                    int bytesSent = sender.Send(msg);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch(ArgumentNullException ane)
                {
                    Console.WriteLine("Argument Null Exception : {0}", ane.ToString());
                }
                catch(SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine("Unexcepted exception : {0}", e.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
