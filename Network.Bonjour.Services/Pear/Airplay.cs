using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.ZeroConf;
using Network.Dns;
using System.Threading;

namespace Network.Bonjour.Services.Pear
{
    public class Airplay : Service
    {
        public Airplay() { }

        public Airplay(ushort port)
        {
            HostName = Environment.MachineName + ".local.";
            Name = ResolverHelper.GetMacAddresses().First() + "@" + Environment.MachineName;
            Protocol = "_raop._tcp.local.";
            properties.Add("txtvers", "1");
            properties.Add("vn", "3");
            properties.Add("pw", "false");
            properties.Add("sr", "44100");
            properties.Add("ss", "16");
            properties.Add("ch", "2");

            properties.Add("cn", "0,1");
            properties.Add("et", "0,1");
            properties.Add("ek", "1");
            properties.Add("sv", "false");
            properties.Add("sm", "false");
            properties.Add("tp", "TCP,UDP");
            EndPoint ep = ResolverHelper.GetEndPoint();
            ep.Port = port;
            this.addresses.Add(ep);
        }

        public static void Main()
        {
            IService s = new Airplay(5000);
            s.Publish();
            Console.Read();
        }
    }
}
