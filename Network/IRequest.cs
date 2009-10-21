using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    public interface IRequest<RequestType>
    {
        RequestType GetRequest(BinaryReader stream);
        RequestType GetRequest(byte[] requestBytes);
        //void WriteTo(Stream stream);
        byte[] GetBytes();
    }
}
