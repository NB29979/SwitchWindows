using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            InputController controller;
            CancellationTokenSource cancellationToken = new CancellationTokenSource();

            wsServer = new WebSocketServer();
            controller = new InputController();

            int port = 10090;
            wsServer.Setup(port);

            wsServer.NewSessionConnected += (_session) => 
            {
                cancellationToken = new CancellationTokenSource();
                Console.WriteLine("New session connected");

                Console.WriteLine("Window watching...");
                Task.Factory.StartNew(() => 
                {
                    List<WindowRowData> oldVisibleWindows_ = new List<WindowRowData>();
                    List<WindowRowData> newVisibleWindows_ = new List<WindowRowData>();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        newVisibleWindows_ = WindowInformationProvider.GetVisibleWindows().Distinct().ToList();

                        if(!newVisibleWindows_.SequenceEqual(oldVisibleWindows_, new WindowRowDataComparer()))
                        {
                            IconExtractor.ExtractIcons(newVisibleWindows_);
                            _session.Send(JsonConvert.SerializeObject(newVisibleWindows_));

                            Console.WriteLine("Window list sent to device");
                            oldVisibleWindows_ = newVisibleWindows_;
                        }
                    }
                });
            };
            wsServer.NewMessageReceived += (_session, _message) => 
            {
                controller.receivedData = JsonConvert.DeserializeObject<ReceivedData>(_message);
                controller.ControlComputer();
            };
            wsServer.NewDataReceived += (_session, _data) => { Console.WriteLine("New data received"); };
            wsServer.SessionClosed += (_session, _reason) => 
            {
                Console.WriteLine("Session closed : {0}", _reason.ToString());
                cancellationToken.Cancel();
            };

            wsServer.Start();
            Console.WriteLine("Server is running on the port {0}", port);

            Console.ReadKey();
            wsServer.Stop();

        }
    }
}