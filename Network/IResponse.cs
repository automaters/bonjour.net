using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    public interface IResponse
    {
    }

    public interface IServerResponse : IResponse
    {
        void WriteTo(BinaryWriter writer);
        byte[] GetBytes();
    }

    public interface IClientResponse<TResponse> : IResponse
    {
        TResponse GetResponse(BinaryReader stream);
        TResponse GetResponse(byte[] requestBytes);
    }
}
