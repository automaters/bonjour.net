using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;

namespace Network.Rest
{
    public abstract class Command
    {
        public Command(string connectionString)
        {
            Uri = new Uri(connectionString);
        }

        public Command(HttpRequest request)
        {
        }

        public abstract void Initialize(HttpRequest request);

        public Uri Uri { get; protected set; }

        protected string Method { get; set; }

        protected virtual HttpRequest BuildRequest()
        {
            HttpRequest request = new HttpRequest(Uri);
            request.KeepAlive = true;
            request.ContentType = "text/xml";
            request.Method = Method;
            return request;
        }

        protected abstract HttpRequest GetRequest();

        public virtual HttpResponse GetHttpResponse()
        {
            HttpRequest request = GetRequest();
            return (HttpResponse)request.GetResponse();
        }

        XmlDocument doc = new XmlDocument();
        bool docLoaded = false;

        public XmlDocument GetResponse()
        {
            if (!docLoaded)
            {
                HttpResponse response = GetHttpResponse();
                docLoaded = true;
                doc.Load(response.Body);
            }
            return doc;
        }

        public abstract void Execute(RequestEventArgs e);
    }
}
