using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Net.Sockets;

namespace Network.Rest
{
    public class HttpRequest : HttpMessage
    {
        public HttpRequest() { }

        public HttpRequest(string uriString) : base(uriString) { }
        public HttpRequest(Uri uri) : base(uri) { }

        public string Method { get; set; }

        public static HttpRequest FromBytes(byte[] bytes)
        {
            return Parse(Encoding.UTF8.GetString(bytes));
        }

        public static HttpRequest Parse(string requestString)
        {
            HttpRequest request = new HttpRequest();
            StringReader reader = new StringReader(requestString);
            //METHOD URI VERSION
            string line = reader.ReadLine();
            string[] firstLine = line.Split(SPACE);
            request.Method = firstLine[0];
            request.Uri = firstLine[1];
            request.HttpVersion = HttpVersion.HTTP11;
            request.ReadHeaders(reader);
            request.Host = request.Host.Trim();
            return request;
        }

        public override int ContentLength
        {
            get
            {
                if (Body.Length > 0 && Headers.ContainsKey("Content-Length"))
                    return (int)Body.Length;
                return base.ContentLength;
            }
            set
            {
                base.ContentLength = value;
            }
        }

        public bool KeepAlive
        {
            get { return Headers["KeepAlive"] == "true"; }
            set { Headers["KeepAlive"] = value.ToString(); }
        }

        public string ContentType
        {
            get { return (string)Headers["Content-Type"]; }
            set { Headers["Content-Type"] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Method);
            sb.Append(SPACE);
            sb.Append(Uri);
            sb.Append(SPACE);
            switch (HttpVersion)
            {
                case HttpVersion.HTTP11:
                    sb.Append("HTTP/1.1");
                    break;
                default:
                    break;
            }
            sb.AppendLine();
            if (Host != null)
                sb.AppendLine(string.Format("Host: {0}", Host));
            if (Body.Length > 0)
                Headers["Content-Length"] = Body.Length.ToString();
            foreach (KeyValuePair<string, string> header in Headers)
            {
                if (header.Key != "Host")
                    sb.AppendLine(string.Format("{0}: {1}", header.Key, header.Value));
            }
            sb.AppendLine();
            if (Body.Length > 0)
            {
                Body.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(Body);
                sb.Append(reader.ReadToEnd());
                //reader.Close();
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public TransportProtocol Protocol { get; set; }

        public HttpResponse GetResponse()
        {
            Uri uri = new Uri("http://" + Host + Uri);
            if (Protocol == TransportProtocol.TCP)
            {
                TcpClient client = new TcpClient();
                client.Connect(uri.Host, uri.Port != 0 ? uri.Port : 80);
                //Uri = Uri.Substring(Uri.IndexOf('/', Uri.IndexOf(uri.Host)));
                byte[] requestBytes = GetBytes();
                Uri = uri.ToString();
                client.GetStream().Write(requestBytes, 0, requestBytes.Length);
                MemoryStream stream = new MemoryStream();
                NetworkStream clientStream = client.GetStream();
                do
                {
                    byte[] buffer = new byte[1024];
                    int lengthRead = clientStream.Read(buffer, 0, 1024);
                    stream.Write(buffer, 0, lengthRead);
                } while (clientStream.DataAvailable);
                return HttpResponse.FromBytes(stream.ToArray());
            }
            if (Protocol == TransportProtocol.UDP)
            {
                UdpClient client = new UdpClient();
                client.Connect(uri.Host, uri.Port != 0 ? uri.Port : 80);
                byte[] requestBytes = GetBytes();
                client.Send(requestBytes, requestBytes.Length);
                System.Net.IPEndPoint ep = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
                return HttpResponse.FromBytes(client.Receive(ref ep));
            }
            return null;
        }
    }

    public enum TransportProtocol
    {
        TCP,
        UDP
    }
}
