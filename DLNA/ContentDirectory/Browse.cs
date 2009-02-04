using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Rest;
using System.IO;
using System.Xml.Linq;
using System.Net;

namespace DLNA.ContentDirectory
{
    public class Browse : Command
    {
        public Browse(string connectionString) : base(connectionString) { }

        public override void Initialize(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        protected override HttpRequest GetRequest()
        {
            HttpRequest request = BuildRequest();
            request.Headers["SOAPAction"] = @"""urn:schemas-upnp-org:service:ContentDirectory:1#Browse""";
            StreamWriter sw = new StreamWriter(request.Body, Encoding.UTF8);
            XNamespace soapEnv = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/");
            XNamespace contentDirectory = XNamespace.Get("urn:schemas-upnp-org:service:ContentDirectory:1");
            XNamespace dt = XNamespace.Get("urn:schemas-microsoft-com:datatypes");

            sw.WriteLine(new XElement(soapEnv + "Envelope",
                new XAttribute(soapEnv + "encodingStyle", "http://schemas.xmlsoap.org/soap/encoding/"),
                new XElement(soapEnv + "Body",
                    new XElement(contentDirectory + "Browse",
                        new XElement(contentDirectory + "ObjectID", new XAttribute(dt + "dt", "string"), "13"),
                        new XElement(contentDirectory + "BrowserFlag", new XAttribute(dt + "dt", "string"), "BrowseDirectChildren"),
                        new XElement(contentDirectory + "Filter", new XAttribute(dt + "dt", "string"), "dc:title"),
                        new XElement(contentDirectory + "StartingIndex", new XAttribute(dt + "dt", "ui4"), "0"),
                        new XElement(contentDirectory + "RequestedCount", new XAttribute(dt + "dt", "ui4"), "200"),
                        new XElement(contentDirectory + "SortCriteria", new XAttribute(dt + "dt", "string"), "")))));
            //request.Headers["Content-Length"] = (request.ContentType.Length).ToString();
            return request;
        }
    }
}
