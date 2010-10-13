using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Net.Sockets;

namespace Network.Rest
{
    public class HttpRequest : HttpMessage, IRequest<HttpRequest>
    {
        public HttpRequest() { }

        public HttpRequest(string uriString) : base(uriString) { }
        public HttpRequest(Uri uri) : base(uri) { }

        public string Method { get; set; }

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
                sb.AppendLine(string.Format("HOST: {0}", Host));
            if (Body.Length > 0)
                Headers["Content-Length"] = Body.Length.ToString();
            foreach (KeyValuePair<string, string> header in Headers)
            {
                if (header.Key != "HOST")
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

        public HttpResponse GetResponse(bool expectResponse)
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
                if (!expectResponse)
                    return null;
                //MemoryStream stream = new MemoryStream();
                NetworkStream clientStream = client.GetStream();
                //do
                //{
                //    byte[] buffer = new byte[1024];
                //    int lengthRead = clientStream.Read(buffer, 0, 1024);
                //    stream.Write(buffer, 0, lengthRead);
                //} while (clientStream.DataAvailable);
                HttpResponse response;
                using (BinaryReader reader = new BinaryReader(clientStream))
                {
                    response = new HttpResponse().GetResponse(reader);
                    if (clientStream.CanRead && response.Headers.ContainsKey("Connection") && response.Headers["Connection"] == "Keep-Alive")
                    {
                        response = response.GetResponse(reader);
                    }
                }
                return response;
            }
            if (Protocol == TransportProtocol.UDP)
            {
                UdpClient client = new UdpClient();
                client.Connect(uri.Host, uri.Port != 0 ? uri.Port : 80);
                byte[] requestBytes = GetBytes();
                client.Send(requestBytes, requestBytes.Length);
                if (!expectResponse)
                    return null;
                System.Net.IPEndPoint ep = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
                return HttpResponse.FromBytes(client.Receive(ref ep));
            }
            return null;
        }

        private static HttpRequest Parse(TextReader reader)
        {
            HttpRequest request = new HttpRequest();
            //METHOD URI VERSION
            string line = reader.ReadLine();
            if (line == null)
                return null;
            string[] firstLine = line.Split(SPACE);
            request.Method = firstLine[0];
            request.Uri = firstLine[1];
            request.HttpVersion = HttpVersion.HTTP11;
            request.ReadHeaders(reader);
            if (request.Host != null)
                request.Host = request.Host.Trim();
            return request;
        }

        #region IRequest<HttpRequest> Members



        public HttpRequest GetRequest(BinaryReader stream)
        {
            TextReader reader = new StreamReader(stream.BaseStream);
            return Parse(reader);

        }

        public HttpRequest GetRequest(byte[] bytes)
        {
            return Parse(Encoding.UTF8.GetString(bytes));
        }

        public static HttpRequest Parse(string requestString)
        {
            StringReader reader = new StringReader(requestString);
            return Parse(reader);
        }

        #endregion

        #region IRequest Members

        public void WriteTo(BinaryWriter stream)
        {
            stream.Write(this.GetBytes());
        }

        #endregion
    }

    public enum TransportProtocol
    {
        TCP,
        UDP
    }
}
