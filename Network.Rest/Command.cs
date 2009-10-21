using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;

namespace Network.Rest
{
   

    public abstract class Command : Command<HttpRequestEventArgs, HttpRequest, HttpResponse>
    {
        public Command(string connectionString)
        {
            Uri = new Uri(connectionString);
        }

        public Command()
        {
        }

        public Uri Uri { get; protected set; }

        protected string Method { get; set; }

        protected override HttpRequest BuildRequest()
        {
            HttpRequest request = new HttpRequest(Uri);
            request.KeepAlive = true;
            request.ContentType = "text/xml";
            request.Method = Method;
            return request;
        }

        protected abstract HttpRequest GetRequest();

        public override HttpResponse GetResponse(bool responseExpected)
        {
            HttpRequest request = GetRequest();
            return (HttpResponse)request.GetResponse(responseExpected);
        }

        XmlDocument doc = new XmlDocument();
        bool docLoaded = false;

        public XmlDocument GetXmlResponse()
        {
            if (!docLoaded)
            {
                HttpResponse response = GetResponse();
                docLoaded = true;
                doc.Load(response.Body);
            }
            return doc;
        }
    }
}
