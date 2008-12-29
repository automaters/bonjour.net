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
            bsr.Resolve("_dacp._tcp.");
            Thread.Sleep(3600000);
        }

        static void bsr_ServiceFound(Network.ZeroConf.IService item)
        {
            Console.WriteLine(item);
        }
    }
}