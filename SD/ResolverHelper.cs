using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Network.ZeroConf
{
    public class ResolverHelper
    {
        private ManualResetEvent syncLock = new ManualResetEvent(false);
        private IList<IService> services = new List<IService>();

        public IList<IService> Resolve(IServiceResolver resolver, string protocol, TimeSpan timeout, int minServiceCountFound, int maxServiceCountFound)
        {
            resolver.ServiceFound += new ObjectEvent<IService>(resolver_ServiceFound);
            resolver.ServiceRemoved += new ObjectEvent<IService>(resolver_ServiceRemoved);
            resolver.Resolve(protocol);
            for (int i = minServiceCountFound; i <= maxServiceCountFound; i++)
                syncLock.WaitOne(timeout);
            resolver.ServiceFound -= resolver_ServiceFound;
            resolver.ServiceRemoved -= resolver_ServiceRemoved;
            return services;
        }

        void resolver_ServiceRemoved(IService item)
        {
            services.Remove(item);
        }

        void resolver_ServiceFound(IService item)
        {
            services.Add(item);
            syncLock.Set();
        }
    }
}
