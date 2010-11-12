using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.ZeroConf;
using System.Xml;
using System.Net;
using Network.Rest;

namespace Network.UPnP
{
    public class Service : IService
    {
        public Service()
        {
            Addresses = new List<Network.Dns.EndPoint>();
            actions = new Dictionary<string, ServiceAction>();
        }

        #region IService Members

        public Network.Dns.DomainName HostName { get; set; }

        public IList<Network.Dns.EndPoint> Addresses { get; private set; }

        public void AddAddress(Network.Dns.EndPoint ep) { Addresses.Add(ep); }

        public string Protocol
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string this[string key]
        {
            get
            {
                //if (properties.ContainsKey(key))
                return properties[key];
                //return Txt.False;
            }
            set
            {
                if (properties.ContainsKey(key))
                {
                    properties[key] = value;
                }
                else
                    properties.Add(key, value);
            }
        }

        public State State
        {
            get;
            set;
        }

        public bool IsOutDated
        {
            get { return Ttl <= 0; }
        }

        public void Publish()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
        }

        public void Merge(IService service)
        {
        }

        public void Resolve()
        {
            if (Location == null)
                return;
            WebRequest request = WebRequest.Create(Location);

            XmlDocument doc = new XmlDocument();

            doc.Load(request.GetResponse().GetResponseStream());

            //IList<Service> s = Service.BuildServices(doc);
            EnhanceService(doc);
        }

        private void EnhanceService(XmlDocument doc)
        {
            //Protocol = doc.SelectSingleNode("/*[local-name()='root']/*[local-name()='device']/*[local-name()='deviceType']").InnerText;
            Name = doc.SelectSingleNode("/*[local-name()='root']/*[local-name()='device']/*[local-name()='friendlyName']").InnerText;
            foreach (var node in doc.SelectNodes("/*[local-name()='root']/*[local-name()='device']/*[local-name()='serviceList']/*[local-name()='service' and ./*[local-name()='serviceType']/text()='" + Protocol + "']/*").OfType<XmlNode>())
            {
                properties[node.LocalName] = node.InnerText;
            }

            if (SCPD == null)
            {
                XmlNode tempNode = doc.SelectSingleNode("/*[local-name()='root']/*[local-name()='device']/*[local-name()='serviceList']/*[local-name()='service' and ./*[local-name()='serviceType']/text()='" + Protocol + "']/*[local-name()='controlURL']/text()");
                if (tempNode != null)
                    Control = new Uri(Location, tempNode.Value);
                tempNode = doc.SelectSingleNode("/*[local-name()='root']/*[local-name()='device']/*[local-name()='serviceList']/*[local-name()='service' and ./*[local-name()='serviceType']/text()='" + Protocol + "']/*[local-name()='SCPDURL']/text()");
                if (tempNode != null)
                    SCPD = new Uri(Location, tempNode.Value);
                tempNode = doc.SelectSingleNode("/*[local-name()='root']/*[local-name()='device']/*[local-name()='serviceList']/*[local-name()='service' and ./*[local-name()='serviceType']/text()='" + Protocol + "']/*[local-name()='eventSubURL']/text()");
                if (tempNode != null)
                    Event = new Uri(Location, tempNode.Value);

                GetDescription();
            }
            this.State = State.UpToDate;
        }

        private void GetDescription()
        {
            if (SCPD == null)
                return;

            WebRequest request = WebRequest.Create(SCPD);

            XmlDocument doc = new XmlDocument();

            doc.Load(request.GetResponse().GetResponseStream());

            foreach (XmlNode node in doc.SelectNodes("/*[local-name()='scpd']/*[local-name()='actionList']/*[local-name()='action']"))
            {
                ServiceAction action = new ServiceAction(node.SelectSingleNode("*[local-name()='name']/text()").Value, Protocol);
                foreach (XmlNode argumentNode in node.SelectNodes("*[local-name()='argumentList']/*[local-name()='argument']"))
                {
                    string argumentName = argumentNode.SelectSingleNode("*[local-name()='name']/text()").Value;
                    string argumentType = argumentNode.SelectSingleNode("*[local-name()='relatedStateVariable']/text()").Value;
                    if (!ArgumentType.IsArgumentType(argumentType))
                        argumentType = doc.SelectSingleNode("/*[local-name()='scpd']/*[local-name()='serviceStateTable']/*[local-name()='stateVariable' and ./*[local-name()='name']/text()='" + argumentType + "']/*[local-name()='dataType']/text()").Value;
                    action.Add(new ServiceActionArgument<string>(argumentName, argumentType, argumentNode.SelectSingleNode("*[local-name()='direction']/text()").Value));
                }
                actions.Add(action.Name, action);
            }
        }

        #endregion

        internal static IList<Service> BuildServices(System.Xml.XmlDocument doc)
        {
            List<Service> services = new List<Service>();

            return services;
        }

        protected IDictionary<string, string> properties;

        internal static Service BuildService(HttpResponse item)
        {
            Service s = new Service();
            if (item.Headers.ContainsKey("ST"))
                s.Protocol = item.Headers["ST"];
            if (item.Headers.ContainsKey("NT"))
                s.Protocol = item.Headers["NT"];
            s.properties = item.Headers;
            if (item.Headers.ContainsKey("LOCATION"))
            {
                s.Location = new Uri(item.Headers["LOCATION"]);
                s.properties.Remove("LOCATION");
            }
            s.properties.Remove("ST");
            s.Addresses.Add(new Network.Dns.EndPoint() { });
            if (s.Location != null)
                s.Addresses[0].Addresses.Add(IPAddress.Parse(s.Location.Host));
            if (item.Headers.ContainsKey("CACHE-CONTROL"))
            {
                string cacheControl = item.Headers["CACHE-CONTROL"];
                int startOfMaxAge = cacheControl.IndexOf("max-age=", StringComparison.InvariantCultureIgnoreCase);
                int endOfMaxAge = cacheControl.IndexOf(";", startOfMaxAge);
                if (endOfMaxAge == -1)
                    endOfMaxAge = cacheControl.Length;
                string ttl = cacheControl.Substring(startOfMaxAge + 8, endOfMaxAge - (startOfMaxAge + 8));
                s.expiration = DateTime.Now.AddSeconds(int.Parse(ttl));
            }
            return s;
        }

        public Uri Location { get; set; }

        public Uri Control { get; set; }
        public Uri SCPD { get; set; }
        public Uri Event { get; set; }

        internal static Service BuildService(HttpRequest item)
        {
            Service s = new Service();
            if (item.Headers.ContainsKey("NT"))
                s.Protocol = item.Headers["NT"];
            s.properties = item.Headers;
            s.properties.Remove("NT");
            s.Addresses.Add(new Network.Dns.EndPoint() { });
            s.Addresses[0].Addresses.Add(IPAddress.Parse(new Uri(s["LOCATION"]).Host));
            if (item.Headers.ContainsKey("NTS"))
            {
                switch (item.Headers["NTS"])
                {
                    case "ssdp:alive":
                        s.State = State.Added;
                        break;
                    case "ssdp:byebye":
                        s.State = State.Removed;
                        break;
                }
            }
            if (item.Headers.ContainsKey("CACHE-CONTROL"))
            {
                string cacheControl = item.Headers["CACHE-CONTROL"];
                string ttl = cacheControl.Substring(cacheControl.IndexOf("max-age=") + 8, 4);
                s.expiration = DateTime.Now.AddSeconds(int.Parse(ttl));
            }
            return s;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} {1}", Name, Protocol));
            sb.AppendLine("Properties :");
            foreach (KeyValuePair<string, string> kvp in properties)
                sb.AppendLine(string.Format("\t{0}={1}", kvp.Key, kvp.Value));
            sb.AppendLine("Actions :");
            foreach (ServiceAction action in actions.Values)
                sb.AppendLine(action.ToString());
            return sb.ToString();
        }

        #region IExpirable Members

        DateTime expiration;
        private Dictionary<string, ServiceAction> actions;

        public uint Ttl
        {
            get
            {
                int seconds = (int)expiration.Subtract(DateTime.Now).TotalSeconds;
                if (seconds < 0)
                    return 0;
                return (uint)seconds;
            }
        }


        public void Renew(uint ttl)
        {
            expiration = DateTime.Now.AddSeconds(ttl);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerable<KeyValuePair<string, string>> Properties
        {
            get
            {
                return properties;
            }
        }

        #endregion
    }
}
