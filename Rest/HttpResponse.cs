using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace Network.Rest
{
    public class HttpResponse : HttpMessage
    {
        public HttpResponse()
        {
            Body = new MemoryStream();
        }

        public static HttpResponse FromBytes(byte[] bytes)
        {
            return Parse(Encoding.UTF8.GetString(bytes));
        }

        public HttpStatusCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public Stream Body { get; protected set; }

        private static HttpResponse Parse(string responseString)
        {
            HttpResponse response = new HttpResponse();
            StringReader reader = new StringReader(responseString);
            string line = reader.ReadLine();
            string[] firstLine = line.Split(' ');
            //VERSION RESPONSECODE RESPONSEMESSAGE
            response.HttpVersion = HttpVersion.HTTP11;
            response.ResponseCode = (HttpStatusCode)int.Parse(firstLine[1]);
            response.ResponseMessage = firstLine[2];
            response.ReadHeaders(reader);
            return response;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("HTTP/1.1 {0} {1}", (int)ResponseCode, ResponseMessage));
            foreach (KeyValuePair<string, string> header in Headers)
            {
                if (header.Key != "Host")
                    sb.AppendLine(string.Format("{0}:{1}", header.Key, header.Value));
            }
            sb.Append("Body");
            sb.Append(Encoding.UTF8.GetString(((MemoryStream)Body).ToArray()));
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
