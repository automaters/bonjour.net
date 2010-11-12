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
            resolver.Resolve("urn:schemas-upnp-org:service:ContentDirectory:1");
            //resolver.Resolve("upnp:rootdevice");
            //resolver.Resolve("urn:schemas-upnp-org:service:RenderingControl:1");
            //Thread.Sleep(10000);
//            IPEndPoint server = new IPEndPoint(IPAddress.Parse("192.168.1.19"), 2869);
//            string content = @"<?xml version=""1.0""?>
//<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><SOAP-ENV:Body><m:Browse xmlns:m=""urn:schemas-upnp-org:service:ContentDirectory:1""><ObjectID xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">13</ObjectID><BrowseFlag xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">BrowseDirectChildren</BrowseFlag><Filter xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">dc:title</Filter><StartingIndex xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""ui4"">0</StartingIndex><RequestedCount xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""ui4"">200</RequestedCount><SortCriteria xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string""></SortCriteria></m:Browse></SOAP-ENV:Body></SOAP-ENV:Envelope>
//";
//            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
//            HttpRequest request = new HttpRequest(string.Format("http://{0}/upnphost/udhisapi.dll?control=uuid:1bd41465-7f98-432f-a91c-2fe6404a82fd+urn:upnp-org:serviceId:ContentDirectory",server));
//            request.Method = "POST";
//            request.Headers["User-Agent"] = "Mozilla/4.0 (compatible; UPnP/1.0; Windows 9x)";
//            //boris
//            //request.Uri = "/upnphost/udhisapi.dll?control=uuid:0c22dbbf-9bdd-41a7-b61b-16502aa0d3fc+urn:upnp-org:serviceId:ContentDirectory";
//            request.Headers["Content-Length"] = (contentBytes.Length).ToString();
//            request.Headers["Content-Type"] = "text/xml; charset=\"utf-8\"";
//            request.Headers["Connection"] = "Close";
//            request.Headers["Cache-Control"] = "no-cache";
//            request.Headers["Pragma"] = "no-cache";
//            request.Headers["SOAPAction"] = @"""urn:schemas-upnp-org:service:ContentDirectory:1#Browse""";
//            TcpClient client = new TcpClient();
//            client.Connect(server);
//            byte[] requestBytes = request.GetBytes();
//            client.GetStream().Write(requestBytes, 0, requestBytes.Length);
//            StreamReader reader = new StreamReader(client.GetStream());
//            while (client.GetStream().DataAvailable)
//            {
//                Console.WriteLine(reader.ReadToEnd());
//            }
//            client.GetStream().Write(contentBytes, 0, contentBytes.Length);
//            while (client.GetStream().DataAvailable)
//            {
//                Console.WriteLine(reader.ReadToEnd());
//            }
//            //StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.UTF8);
//            //sw.Write(content);
//            //sw.Close();
//            reader = new StreamReader(client.GetStream());
//            //Thread.Sleep(10000);
            Console.WriteLine("Press enter to exit");
            Console.Read();
            resolver.ServiceFound -= resolver_ServiceFound;
            resolver.Dispose();
        }

        static void resolver_ServiceFound(Network.ZeroConf.IService item)
        {
            item.Resolve();
            Console.WriteLine(item);
        }
    }
}
