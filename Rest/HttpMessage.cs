using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace Network.Rest
{
    public class HttpMessage
    {
        public HttpMessage()
        {
            Headers = new Dictionary<string, string>();
            Encoding = Encoding.UTF8;
            Uri = "*";
            Body = new MemoryStream();
        }

        public HttpMessage(string uriString) : this(new Uri(uriString)) { }
        public HttpMessage(Uri uri)
            : this()
        {
            Uri = uri.PathAndQuery;
            if (uri.Port > 0)
                Host = uri.Host + ":" + uri.Port;
            else
                Host = uri.Host;
        }

        public MemoryStream Body { get; protected set; }

        public string Uri { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public Encoding Encoding { get; set; }

        public virtual int ContentLength
        {
            get
            {
                if (Headers.ContainsKey("Content-Length"))
                    return int.Parse(Headers["Content-Length"]);
                return 0;
            }
            set
            {
                Headers["Content-Length"] = value.ToString();
            }
        }

        public string Host
        {
            get
            {
                if (Headers.ContainsKey("Host"))
                    return Headers["Host"];
                return null;
            }
            set
            {
                if (Headers.ContainsKey("Host"))
                    Headers["Host"] = value;
                else
                    Headers.Add("Host", value);
            }
        }

        public HttpVersion HttpVersion { get; set; }

        public byte[] GetBytes()
        {
            return Encoding.GetBytes(this.ToString());
        }

        protected const char SPACE = ' ';



        protected void ReadHeaders(StringReader reader)
        {
            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                string[] header = line.Split(':');
                Headers.Add(header[0], line.Substring(header[0].Length + 1));
            }
        }
    }

    public enum HttpVersion
    {
        HTTP11
    }

}
