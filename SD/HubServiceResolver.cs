using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.ZeroConf
{
    public class HubServiceResolver : IServiceResolver
    {
        List<IServiceResolver> resolvers = new List<IServiceResolver>();

        public void AddResolver(IServiceResolver resolver)
        {
            resolvers.Add(resolver);
            resolver.ServiceFound += resolver_ServiceFound;
            resolver.ServiceRemoved += resolver_ServiceRemoved;
        }

        public void RemoveResolver(IServiceResolver resolver)
        {
            resolvers.Add(resolver);
            resolver.ServiceFound -= resolver_ServiceFound;
            resolver.ServiceRemoved -= resolver_ServiceRemoved;
        }

        void resolver_ServiceRemoved(IService item)
        {
            if (ServiceRemoved != null)
                ServiceRemoved(item);
        }

        void resolver_ServiceFound(IService item)
        {
            if (ServiceFound != null)
                ServiceFound(item);
        }

        #region IServiceResolver Members

        public event ObjectEvent<IService> ServiceFound;

        public event ObjectEvent<IService> ServiceRemoved;

        public void Resolve(string protocol)
        {
            resolvers.ForEach(delegate(IServiceResolver resolver) { resolver.Resolve(protocol); });
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            resolvers.ForEach(delegate(IServiceResolver resolver) { resolver.Dispose(); });
        }

        #endregion
    }
}
