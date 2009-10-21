using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Rest;
using System.Threading;
using System.Net;

namespace Network.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            RestServer server = new RestServer(IPAddress.Parse("224.0.0.224"), 53186);
            server.RequestReceived += new EventHandler<RequestEventArgs<HttpRequest, HttpResponse>>(server_RequestReceived);
            server.HttpRequestReceived += new EventHandler<HttpRequestEventArgs>(server_HttpRequestReceived);
            server.StartUdp();
            Console.WriteLine("Server Started");
            //HttpRequest request = new HttpRequest("http://224.0.0.224:53186/Lucene/");
            //request.Protocol = TransportProtocol.UDP;
            //request.GetResponse(false);
            //Thread.Sleep(1000);
            //request = new HttpRequest("http://224.0.0.224:53186/Lucene/");
            //request.Protocol = TransportProtocol.UDP;
            //request.GetResponse(false);
            //Thread.Sleep(10000);
            //request = new HttpRequest("http://224.0.0.224:53186/Lucene/");
            //request.Protocol = TransportProtocol.UDP;
            //request.GetResponse(false);
            //Thread.Sleep(30000);
            //request = new HttpRequest("http://224.0.0.224:53186/Lucene/");
            //request.Protocol = TransportProtocol.UDP;
            //request.GetResponse(false);
            Console.Read();
            server.Stop();
            Console.WriteLine("Server Stopped");
        }

        static void server_HttpRequestReceived(object sender, HttpRequestEventArgs e)
        {
            server_RequestReceived(sender, e);
            Console.WriteLine(e.Request.ToString());
        }

        static void server_RequestReceived(object sender, RequestEventArgs<HttpRequest, HttpResponse> e)
        {
            Console.WriteLine("Request received from " + e.Host);
        }
    }
}
