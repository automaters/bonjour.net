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
            bsr.Resolve("_touch-able._tcp.");
            Console.ReadLine();
            //Service s = new Service();
            //s.AddAddress(new Network.Dns.EndPoint() { DomainName = "ASPERGE.local.", Port = 50508, Addresses = new List<IPAddress>() { IPAddress.Parse("192.168.1.19") } });
            //s.Protocol = "_touch-remote._tcp.local.";
            //s.Name = "MyName";
            //s.HostName = "ASPERGE.local.";
            //s["DvNm"] = "PC Remote";
            //s["RemV"] = "10000";
            //s["DvTy"] = "iPod";
            //s["RemN"] = "Remote";
            //s["txtvers"] = "1";
            //s["Pair"] = "0000000000000001";
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
