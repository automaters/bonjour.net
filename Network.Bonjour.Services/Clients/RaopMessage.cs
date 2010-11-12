using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Bonjour.Services.Clients
{
    public class RaopMessage : IClientRequest, IClientResponse<RaopMessage>
    {
        #region IResponse<RaopMessage> Members

        public RaopMessage GetResponse(System.IO.BinaryReader stream)
        {
            throw new NotImplementedException();
        }

        public RaopMessage GetResponse(byte[] requestBytes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IResponse Members

        public void WriteTo(System.IO.BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRequest<RaopMessage> Members

        public RaopMessage GetRequest(System.IO.BinaryReader stream)
        {
            throw new NotImplementedException();
        }

        public RaopMessage GetRequest(byte[] requestBytes)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
