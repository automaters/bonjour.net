using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Network.Rest;
using Network.UPnP;
using System.Threading;

namespace UPnPReader
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceResolver resolver = new ServiceResolver();
            resolver.ServiceFound += new Network.ZeroConf.ObjectEvent<Network.ZeroConf.IService>(resolver_ServiceFound);
            //resolver.Resolve("urn:schemas-upnp-org:service:ContentDirectory:1");
            //resolver.Resolve("upnp:rootdevice");
            resolver.Resolve("urn:schemas-upnp-org:service:RenderingControl:1");
            Thread.Sleep(3600);
        }

        static void resolver_ServiceFound(Network.ZeroConf.IService item)
        {
            item.Resolve();
            Console.WriteLine(item);
        }
    }
}
