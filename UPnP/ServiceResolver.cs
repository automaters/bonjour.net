using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.ZeroConf;
using Network.Rest;
using System.Net;
using System.Xml;

namespace Network.UPnP
{
    public class ServiceResolver : IServiceResolver
    {
        protected IList<string> protocols = new List<string>();
        private SsdpClient client;
        protected TtlCollection<Service> services;

        #region IServiceResolver Members

        public event ObjectEvent<IService> ServiceFound;

        public event ObjectEvent<IService> ServiceRemoved;

        public void Resolve(string protocol)
        {
            protocols.Add(protocol);
            if (client == null)
            {
                client = SsdpClient.CreateAndResolve(protocol);
                client.QueryReceived += client_QueryReceived;
                client.AnswerReceived += client_AnswerReceived;

                client.Start();
            }
            else
                client.Resolve(protocol);
        }

        void client_QueryReceived(HttpRequest item)
        {
            if (item.Method == "NOTIFY")
            {
                Service s = Service.BuildService(item);
                MergeServices(s);
            }
        }

        private void MergeServices(Service service)
        {
            Service rightService = services.Where(s => s.Protocol == service.Protocol && s.HostName == service.HostName).SingleOrDefault();
            if (rightService != null)
            {
                switch (service.State)
                {
                    case State.Removed:
                        services.Remove(rightService);
                        if (ServiceRemoved != null)
                            ServiceRemoved(rightService);
                        break;
                    case State.Added:
                        rightService.Renew(service.Ttl);
                        break;
                }
            }
            else
            {
                if (service.State == State.Added)
                {
                    services.Add(service);
                    if (ServiceFound != null)
                        ServiceFound(service);
                }
            }
        }

        void client_AnswerReceived(HttpResponse item)
        {
            Service s = Service.BuildService(item);
            if (s != null && ServiceFound != null)
                ServiceFound(s);
        }

        private void MergeServices(IList<Service> s)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IServiceResolver Members


        public IList<IService> Resolve(string protocol, TimeSpan timeout, int minCountServices, int maxCountServices)
        {
            return new ResolverHelper().Resolve(this, protocol, timeout, minCountServices, maxCountServices);
        }

        #endregion
    }
}
