using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using SuperWebSocket;

namespace SwitchWindows
{
    class Program
    {
        static void Main(string[] args)
        {
            WebSocketServer wsServer;
            Client client;
            InputController controller;

            wsServer = new WebSocketServer();
            client = new Client();
            controller = new InputController();

            int port = 10090;
            wsServer.Setup(port);

            wsServer.NewSessionConnected += (_session) => { Console.WriteLine("New session connected"); };
            wsServer.NewMessageReceived += (_session, _message) => {
                controller.receivedData = JsonConvert.DeserializeObject<ReceivedData>(_message);
                controller.ControlComputer();
            };
            wsServer.NewDataReceived += (_session, _data) => { Console.WriteLine("New data received"); };
            wsServer.SessionClosed += (_session, _reason) => { Console.WriteLine("Session closed : {0}", _reason.ToString()); };

            wsServer.Start();
            Console.WriteLine("Server is running on the port {0}", port);
            try
            {
                Task.WaitAll(client.InProgressAsync());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
            wsServer.Stop();
        }
    }
}