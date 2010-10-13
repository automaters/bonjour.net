using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    public interface IResponse
    {
        void WriteTo(BinaryWriter writer);
        byte[] GetBytes();
    }

    public interface IResponse<TResponse> : IResponse
    {
        TResponse GetResponse(BinaryReader stream);
        TResponse GetResponse(byte[] requestBytes);
    }
}
