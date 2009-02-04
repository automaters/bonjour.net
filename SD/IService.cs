using System;
using System.Collections.Generic;
using System.Text;
using Network.Dns;

namespace Network.ZeroConf
{
    public interface IService : IExpirable
    {
        DomainName HostName { get; set; }
        IList<EndPoint> Addresses { get; }
        void AddAddress(EndPoint ep);
        string Protocol { get; set; }
        string Name { get; set; }
        string this[string key] { get; set; }
        IEnumerable<KeyValuePair<string, string>> Properties { get; }
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
