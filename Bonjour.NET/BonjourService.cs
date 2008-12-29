using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Dns;
using Network.ZeroConf;
using System.Net;
using System.Collections.ObjectModel;
using System.Threading;

namespace Network.Bonjour
{
    public class Service : IService
    {
        public Service()
        {
            properties = new Dictionary<string, string>();
            addresses = new List<Network.Dns.EndPoint>();
        }

        public DomainName HostName { get; set; }
        protected DateTime expiration;
        public State State { get; set; }
        public bool IsOutDated { get { return DateTime.Now > expiration; } }

        private Service(Answer a)
            : this()
        {
            EnhanceService(a);
        }

        MDnsClient resolver;
        AutoResetEvent resolved = new AutoResetEvent(false);

        public void Resolve()
        {
            if (resolver != null)
                resolver.Stop();

            resolver = MDnsClient.CreateAndResolve(HostName);
            resolver.AnswerReceived += client_AnswerReceived;
            resolver.Start();
            resolved.WaitOne();
        }

        void client_AnswerReceived(Message message)
        {
            foreach (Answer a in message.Answers)
                EnhanceService(a);
            resolver.Stop();
        }

        protected void EnhanceService(Answer a)
        {
            switch (a.Type)
            {
                case Network.Dns.Type.A:
                case Network.Dns.Type.AAAA:
                    foreach (Network.Dns.EndPoint ep in addresses)
                    {
                        if (a.DomainName.ToString() == ep.DomainName.ToString())
                            ep.Resolve(a);
                        if (ep.Addresses.Count > 0)
                            resolved.Set();
                    }
                    break;
                case Network.Dns.Type.NS:
                    break;
                case Network.Dns.Type.CNAME:
                    break;
                case Network.Dns.Type.SOA:
                    break;
                case Network.Dns.Type.WKS:
                    break;
                case Network.Dns.Type.PTR:
                    if (string.IsNullOrEmpty(Name))
                        Name = ((Ptr)a.ResponseData).DomainName[0];
                    if (string.IsNullOrEmpty(Protocol))
                        Protocol = a.DomainName;
                    break;
                case Network.Dns.Type.HINFO:
                    break;
                case Network.Dns.Type.MINFO:
                    break;
                case Network.Dns.Type.MX:
                    break;
                case Network.Dns.Type.TXT:
                    if (properties == null || properties.Count == 0)
                        properties = ((Txt)a.ResponseData).Properties;
                    break;
                case Network.Dns.Type.SRV:
                    if (HostName == null)
                        HostName = ((Srv)a.ResponseData).Target;
                    if (Name == null)
                        Name = a.DomainName[0];
                    if (Protocol == null)
                        Protocol = new DomainName(a.DomainName.Skip(1));
                    if (addresses.Count == 0)
                        addresses.Add(new Network.Dns.EndPoint() { DomainName = HostName });
                    break;
                default:
                    break;
            }
            if (a.Ttl == 0)
                State = State.Removed;
            else
                Renew(a.Ttl);
        }

        public static Service Build(Message m)
        {
            if (m.AnswerEntries == 0)
                return null;
            return Build(m.Answers[0], m);
        }

        #region IService Members

        public string Protocol { get; set; }

        protected List<Network.Dns.EndPoint> addresses;

        public IList<Network.Dns.EndPoint> Addresses
        {
            get { return new ReadOnlyCollection<Network.Dns.EndPoint>(addresses); }
        }


        public string Name { get; set; }

        private IDictionary<string, string> properties;

        public string this[string key]
        {
            get
            {
                if (properties.ContainsKey(key))
                    return properties[key];
                return Txt.False;
            }
        }

        public void Publish()
        {
        }

        public void Stop()
        {
            if (resolver != null)
                resolver.Stop();
            resolved.Set();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} {1}", Name, Protocol));
            sb.AppendLine("Properties :");
            foreach (KeyValuePair<string, string> kvp in properties)
                sb.AppendLine(string.Format("\t{0}={1}", kvp.Key, kvp.Value));
            return sb.ToString();
        }

        #endregion

        internal static IList<IService> BuildServices(Message m)
        {
            List<IService> services = new List<IService>();
            foreach (Answer a in m.Answers)
            {
                if (a.Type == Network.Dns.Type.SRV || (a.Type == Network.Dns.Type.PTR && a.Ttl == 0))
                    services.Add(Build(a, m));
                else
                    foreach (Service s in services.Where(s => a.DomainName[0] == s.Name))
                        s.EnhanceService(a);
            }
            foreach (Answer a in m.Authorities)
            {
                if (a.Type == Network.Dns.Type.SRV)
                    services.Add(Build(a, m));
                else
                    foreach (Service s in services.Where(s => a.DomainName[0] == s.Name))
                        s.EnhanceService(a);
            }
            foreach (Answer a in m.Additionals)
            {
                if (a.Type == Network.Dns.Type.SRV)
                    services.Add(Build(a, m));
                else
                    foreach (Service s in services.Where(s => a.DomainName[0] == s.Name))
                        s.EnhanceService(a);
            }
            return services;
        }


        private static Service Build(Answer answer, Message message)
        {
            Service s = new Service(answer);
            foreach (Answer a in message.Additionals)
                s.EnhanceService(a);
            return s;
        }

        protected void Renew(int ttl)
        {
            if (expiration == DateTime.MinValue)
                expiration = DateTime.Now.AddSeconds(ttl);
            else
                expiration = expiration.AddSeconds(ttl);
        }

        #region IService Members


        public void Merge(IService iService)
        {
            if (iService is Service)
            {
                Service service = (Service)iService;
                foreach (var endpoint in Addresses)
                {
                    var newEndPoint = service.addresses.SingleOrDefault(ep => ep.DomainName.ToString() == endpoint.DomainName.ToString() && ep.Port == endpoint.Port);
                    if (newEndPoint != null)
                        endpoint.Merge(newEndPoint);
                    else
                        addresses.Add(endpoint);
                }
            }
        }

        #endregion
    }
}
