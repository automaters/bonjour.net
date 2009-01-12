using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Network.Rest
{
    public abstract class Command
    {
        public Command(string connectionString)
        {
            Uri = new Uri(connectionString);
        }

        public Uri Uri { get; protected set; }

        protected string Method { get; set; }

        protected virtual HttpRequest BuildRequest()
        {
            HttpRequest request = new HttpRequest(Uri);
            request.Method = Method;
            return request;
        }

        protected abstract HttpRequest GetRequest();

        public virtual HttpResponse GetHttpResponse()
        {
            HttpRequest request = GetRequest();
            return request.GetResponse();
        }

        public XmlDocument GetResponse()
        {
            return GetHttpResponse().Document;
        }
    }
}
