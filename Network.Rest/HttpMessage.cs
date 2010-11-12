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
                string length = TryGetHeader("CONTENT-LENGTH");
                if(!string.IsNullOrEmpty(length))
                    return int.Parse(length);
                return 0;
            }
            set
            {
                Headers["CONTENT-LENGTH"] = value.ToString();
            }
        }

        public string Host
        {
            get
            {
                return TryGetHeader("HOST");
            }
            set
            {
                if (Headers.ContainsKey("HOST"))
                    Headers["HOST"] = value;
                else
                    Headers.Add("HOST", value);
            }
        }

        public string Protocol { get; set; }

        public virtual byte[] GetBytes()
        {
            return Encoding.GetBytes(this.ToString());
        }

        protected const char SPACE = ' ';

        protected void ReadHeaders(BinaryReader reader)
        {
            string line;
            while (!string.IsNullOrEmpty(line = BinaryHelper.ReadLine(reader)))
            {
                string[] header = line.Split(':');
                if (!Headers.ContainsKey(header[0].ToUpper()))
                    Headers.Add(header[0].ToUpper(), line.Substring(header[0].Length + 1).Trim());
            }
        }

        protected string TryGetHeader(string key)
        {
            string value;
            if (Headers.TryGetValue(key, out value))
                return value;
            return null;
        }

        protected void ReadHeaders(TextReader reader)
        {
            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                string[] header = line.Split(':');
                if (!Headers.ContainsKey(header[0].ToUpper()))
                    Headers.Add(header[0].ToUpper(), line.Substring(header[0].Length + 1).Trim());
            }
        }
    }

    public static class HttpProtocol
    {
        public const string HTTP11 = "HTTP/1.1";
    }

}
