using System;
using System.Collections.Generic;
using System.Text;

namespace Network.ZeroConf
{
    public interface IExpirable
    {
        uint Ttl { get; }
        void Renew(uint ttl);
    }
}
