using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;

namespace Network.Rest
{
    public class HttpRequest : HttpMessage
    {
        public HttpRequest()
        {
            Headers = new Dictionary<string, string>();
            Uri = "*";
        }

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
            return request;
        }
    }
}
