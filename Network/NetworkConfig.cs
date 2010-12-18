using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class NetworkConfig
    {
        public NetworkConfig(bool isStateLess)
            : this(isStateLess, false)
        {
        }

        public NetworkConfig(bool isStateLess, bool isOneWayOnly)
            : this(isStateLess, isOneWayOnly, false)
        {
        }

        public NetworkConfig(bool isStateLess, bool isOneWayOnly, bool supportsUdp)
            : this(isStateLess, isOneWayOnly, supportsUdp, !supportsUdp)
        {
        }

        public NetworkConfig(bool isStateLess, bool isOneWayOnly, bool supportsUdp, bool supportsTcp)
        {
            IsStateLess = isStateLess;
            IsOneWayOnly = isOneWayOnly;
            SupportsUdp = supportsUdp;
            SupportsTcp = supportsTcp;
        }


        public bool IsStateLess { get; private set; }
        public bool IsOneWayOnly { get; private set; }
        public bool SupportsUdp { get; private set; }
        public bool SupportsTcp { get; private set; }
    }
}
