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

        private DomainName hostName;
        public DomainName HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                hostName = value;
                if (value != null && needsToBeResolvedLater)
                    Resolve();
            }
        }
        protected DateTime expiration;
        public State State { get; set; }
        public bool IsOutDated { get { return DateTime.Now > expiration; } }

        private Service(Answer a)
            : this()
        {
            EnhanceService(a);
        }

        MDnsServer resolver;
        MDnsServer publisher;
        AutoResetEvent resolved = new AutoResetEvent(false);
        bool needsToBeResolvedLater = false;

        public void Resolve()
        {
            if (resolver != null)
                resolver.Stop();

            if (HostName == null)
            {
                needsToBeResolvedLater = true;
                return;
            }
            needsToBeResolvedLater = false;
            resolver = new MDnsServer().Resolve(HostName);
            resolver.AnswerReceived += client_AnswerReceived;
            resolver.StartUdp();
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
                        AddAddress(new Network.Dns.EndPoint() { DomainName = HostName, Port = ((Srv)a.ResponseData).Port });
                    break;
                default:
                    break;
            }
            if (a.Ttl == 0)
                State = State.Removed;
            else
                expiration = expiration.AddSeconds(a.Ttl);
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

        public void AddAddress(Network.Dns.EndPoint endpoint)
        {
            addresses.Add(endpoint);
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

        public void Publish()
        {
            publisher = new MDnsServer(new IPEndPoint(IPAddress.Any, 5353));
            publisher.QueryReceived += publisher_QueryReceived;
            publisher.StartUdp();
            Renew(500);
        }

        void publisher_QueryReceived(Message item)
        {
            if (item.QueryResponse == Qr.Query)
            {
                foreach (Question q in item.Questions)
                {
                    if (((string)q.DomainName).EndsWith(Protocol))
                    {
                        foreach (Network.Dns.EndPoint ep in Addresses)
                        {
                            foreach (var address in ep.Addresses)
                            {
                                item.Additionals.Add(new Answer() { Class = Class.IN, DomainName = Protocol, Ttl = 5, Type = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? Network.Dns.Type.A : Network.Dns.Type.AAAA, ResponseData = new HostAddress() { Address = address } });
                            }
                            item.Answers.Add(new Answer() { Class = Class.IN, DomainName = Protocol, Ttl = 5, Type = Network.Dns.Type.SRV, ResponseData = new Srv() { Port = ep.Port, Target = ep.DomainName } });
                            item.Answers.Add(new Answer() { Class = Class.IN, DomainName = Protocol, Ttl = 5, Type = Network.Dns.Type.TXT, ResponseData = new Txt() { Properties = properties } });
                        }

                        publisher.Send(item, item.From);
                    }
                }
            }
        }

        public void Stop()
        {
            if (resolver != null)
                resolver.Stop();
            resolved.Set();
            if (publisher != null)
                publisher.Stop();
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
                if (a.Type == Network.Dns.Type.PTR)
                    services.Add(Build(a, m));
                else
                    foreach (Service s in services.Where(s => a.DomainName[0] == s.Name))
                        s.EnhanceService(a);
            }
            foreach (Answer a in m.Authorities)
            {
                if (a.Type == Network.Dns.Type.PTR)
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
            Message m = new Message();
            m.From = MDnsServer.EndPoint;
            m.QueryResponse = Qr.Answer;
            m.OpCode = OpCode.Query;
            m.AuthoritativeAnswer = true;
            m.ID = 0;
            m.ResponseCode = ResponseCode.NoError;
            foreach (Network.Dns.EndPoint ep in Addresses)
            {
                foreach (var address in ep.Addresses)
                    m.Additionals.Add(new Answer() { Class = Class.IN, DomainName = HostName, Ttl = ttl, Type = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? Network.Dns.Type.A : Network.Dns.Type.AAAA, ResponseData = new HostAddress() { Address = address } });
                m.Additionals.Add(new Answer() { Class = Class.IN, DomainName = Name + "." + Protocol, Ttl = ttl, Type = Network.Dns.Type.SRV, ResponseData = new Srv() { Port = ep.Port, Target = ep.DomainName } });
                m.Additionals.Add(new Answer() { Class = Class.IN, DomainName = Name + "." + Protocol, Ttl = ttl, Type = Network.Dns.Type.TXT, ResponseData = new Txt() { Properties = properties } });
                m.Authorities.Add(new Answer() { Class = Class.IN, DomainName = Protocol, Ttl = ttl, Type = Network.Dns.Type.PTR, ResponseData = new Ptr() { DomainName = Name + "." + Protocol } });
            }

            publisher.Send(m, m.From);
        }

        #region IService Members


        public void Merge(IService iService)
        {
            if (iService is Service)
            {
                Service service = (Service)iService;
                foreach (var endpoint in service.addresses)
                {
                    var newEndPoint = addresses.SingleOrDefault(ep => ep.DomainName.ToString() == endpoint.DomainName.ToString() && ep.Port == endpoint.Port);
                    if (newEndPoint != null)
                        newEndPoint.Merge(endpoint);
                    else
                        AddAddress(endpoint);
                }
                State = State.Updated;
            }
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
