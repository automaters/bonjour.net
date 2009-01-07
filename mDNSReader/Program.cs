using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Network.Bonjour;

namespace mDNSReader
{
    class Program
    {
        static void Main(string[] args)
        {
            BonjourServiceResolver bsr = new BonjourServiceResolver();
            bsr.ServiceFound += new Network.ZeroConf.ObjectEvent<Network.ZeroConf.IService>(bsr_ServiceFound);
            bsr.Resolve("_http._tcp.");

            //Service s = new Service();
            //s.AddAddress(new Network.Dns.EndPoint() { DomainName = "ASPERGE.local.", Port = 80, Addresses = new List<IPAddress>() { IPAddress.Parse("192.168.1.19") } });
            //s.Protocol = "_dacp._tcp.local.";
            //s.Name = "IIS";
            //s.HostName = "ASPERGE.local.";
            //s.Publish();
            //Thread.Sleep(3600000);
            //s.Stop();
        }

        static void bsr_ServiceFound(Network.ZeroConf.IService item)
        {
            Console.WriteLine(item);
        }
    }
}
