using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    public interface IRequest
    {
    }

    public interface IClientRequest : IRequest
    {
        void WriteTo(BinaryWriter stream);
        byte[] GetBytes();
    }

    public interface IServerRequest<RequestType> : IRequest
    {
        RequestType GetRequest(BinaryReader stream);
        RequestType GetRequest(byte[] requestBytes);
    }
}
