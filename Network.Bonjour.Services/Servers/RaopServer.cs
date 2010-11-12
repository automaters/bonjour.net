using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Rtsp;
using Network.Bonjour.Services.Pear;

namespace Network.Bonjour.Services.Servers
{
    public class RaopServer : RtspServer
    {
        public RaopServer()
            : this(5000)
        {

        }

        public RaopServer(ushort port)
            : base(port)
        {

        }

        public static void Main()
        {
            RaopServer server = new RaopServer(5000);
            Airplay notifier = new Airplay(5000);
            server.StartTcp();
            notifier.Publish();
            Console.Read();
            notifier.Stop();
            server.Stop();
        }
    }
}
