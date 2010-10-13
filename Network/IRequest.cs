using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    public interface IRequest
    {
        void WriteTo(BinaryWriter stream);
        byte[] GetBytes();
    }

    public interface IRequest<RequestType> : IRequest
    {
        RequestType GetRequest(BinaryReader stream);
        RequestType GetRequest(byte[] requestBytes);
    }
}
