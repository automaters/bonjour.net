using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Network.Dns;
using Network.ZeroConf;
using System.Collections.Specialized;

namespace Network.Bonjour
{

    public class BonjourServiceResolver : IServiceResolver, IDisposable
    {
        MDnsClient client;
        IList<IService> services;
        private StringCollection protocols;

        public BonjourServiceResolver()
        {
            protocols = new StringCollection();
            services = new List<IService>();
        }

        public event ObjectEvent<IService> ServiceFound;
        public event ObjectEvent<IService> ServiceRemoved;

        #region IServiceResolver Members

        public void Resolve(string protocol)
        {
            if (!protocol.EndsWith("."))
            {
                if (protocol.EndsWith(".local"))
                    protocol += ".";
                else
                    protocol += ".local.";
            }
            else
            {
                if (!protocol.EndsWith("local."))
                    protocol += "local.";
            }
            protocols.Add(protocol);
            if (client == null)
            {
                client = MDnsClient.CreateAndResolve(protocol);
                client.AnswerReceived += client_AnswerReceived;
            }
            else
                client.Resolve(protocol);
            if (!client.IsStarted)
                client.Start();
        }

        public IList<IService> Resolve(string protocol, TimeSpan timeout, int minCountServices, int maxCountServices)
        {
            return new ResolverHelper().Resolve(this, protocol, timeout, minCountServices, maxCountServices);
        }

        void client_AnswerReceived(Message m)
        {
            IList<IService> services = Service.BuildServices(m);
            MergeServices(services);
            //Service s = Service.Build(m);
            //if (s != null)
            //{
            //    if (!this.services.Any(service => s.Protocol == service.Protocol && s.Name == service.Name))
            //    {
            //        AddService(s);
            //    }
            //    else
            //    {
            //        IService rightService = services.SingleOrDefault(service => s.Protocol == service.Protocol && s.Name == service.Name);
            //        if (rightService != null)
            //            rightService.Merge(s);
            //    }
            //}
        }

        private void MergeServices(IList<IService> updatedList)
        {
            foreach (var service in updatedList)
            {
                IService rightService = services.SingleOrDefault(s => s.Protocol == service.Protocol && s.Name == service.Name);
                if (rightService != null)
                {
                    switch (service.State)
                    {
                        case State.Added:
                            rightService.Merge(service);
                            break;
                        case State.Removed:
                            services.Remove(rightService);
                            if (ServiceRemoved != null)
                                ServiceRemoved(rightService);
                            break;
                        case State.Updated:
                            rightService.Merge(service);
                            break;
                        case State.UpToDate:
                            break;
                        default:
                            break;
                    }
                }
                else
                    AddService(service);
            }
        }

        private void AddService(IService service)
        {
            if (!protocols.Contains(service.Protocol))
                return;
            this.services.Add(service);
            if (ServiceFound != null)
                ServiceFound(service);
            ((Service)service).State = State.UpToDate;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IService service in services)
                service.Stop();
            client.Stop();
        }

        #endregion
    }
}
