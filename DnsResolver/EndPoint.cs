using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Network.Dns
{
    public class EndPoint
    {
        public EndPoint()
        {
            Addresses = new List<IPAddress>();
        }

        public DomainName DomainName { get; set; }

        public ushort Port { get; set; }

        public IList<IPAddress> Addresses { get; set; }

        public void Resolve(Answer a)
        {
            if (a.Type == Type.SRV)
            {
                DomainName = ((Srv)a.ResponseData).Target;
                Port = ((Srv)a.ResponseData).Port;
            }
            if (a.Type == Type.A || a.Type == Type.AAAA)
            {
                IPAddress address = ((HostAddress)a.ResponseData).Address;
                if (!Addresses.Contains(address))
                    Addresses.Add(address);
            }
        }

        public void Merge(EndPoint newEndPoint)
        {
            foreach (var address in newEndPoint.Addresses)
            {
                if (!Addresses.Contains(address))
                    Addresses.Add(address);
            }
        }
    }
}
