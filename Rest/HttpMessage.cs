using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network.Rest
{
    public class HttpMessage
    {
        public HttpMessage()
        {
            Headers = new Dictionary<string, string>();
            Encoding = Encoding.UTF8;
            Uri = "*";
        }

        public string Uri { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public Encoding Encoding { get; set; }

        public string Method { get; set; }

        public string Host
        {
            get
            {
                return Headers["Host"];
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
            sb.AppendLine(string.Format("HOST:{0}", Host));
            foreach (KeyValuePair<string, string> header in Headers)
            {
                if (header.Key != "Host")
                    sb.AppendLine(string.Format("{0}:{1}", header.Key, header.Value));
            }
            sb.AppendLine();
            return sb.ToString();
        }

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
