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
}
