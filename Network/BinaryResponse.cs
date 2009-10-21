using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    public class BinaryResponse : IResponse
    {
        private MemoryStream stream;

        public BinaryResponse()
        {
            stream = new MemoryStream();
        }

        #region IResponse Members

        public void WriteTo(BinaryWriter target)
        {
            this.stream.Position = 0;
            this.stream.WriteTo(target.BaseStream);
        }

        public byte[] GetBytes()
        {
            return stream.ToArray();
        }

        #endregion
    }

    public static class BinaryHelper
    {
        public static byte[] GetBytes(IResponse response)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                response.WriteTo(new BinaryWriter(stream));
                return stream.ToArray();
            }
        }
    }
}
