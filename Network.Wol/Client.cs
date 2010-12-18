using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Network.Wol
{
    public class Client : Client<WolMessage, WolMessage>
    {
        public Client()
            : base(new NetworkConfig(true, true, true))
        {

        }

        public static void Main()
        {
            Client c = new Client();
            c.StartUdp(IPAddress.Any, 0);
            c.SendOneWay(new WolMessage("00:1d:92:6b:a8:1c"), new System.Net.IPEndPoint(IPAddress.Broadcast, 7));
        }

        protected override ClientEventArgs<WolMessage, WolMessage> GetEventArgs(WolMessage response)
        {
            return new ClientEventArgs<WolMessage, WolMessage>() { Response = response };
        }
    }
}
