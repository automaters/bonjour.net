using System;
using System.Collections.Generic;
using System.Text;
using Network.Dns;

namespace Network.ZeroConf
{
    public interface IService
    {
        DomainName HostName { get; }
        IList<EndPoint> Addresses { get; }
        string Protocol { get; set; }
        string Name { get; set; }
        string this[string key] { get; }
        State State { get; }
        bool IsOutDated { get; }
        void Publish();
        void Stop();
        void Merge(IService service);
        void Resolve();
    }

    public enum State
    {
        Added,
        Removed,
        Updated,
        UpToDate
    }
}
