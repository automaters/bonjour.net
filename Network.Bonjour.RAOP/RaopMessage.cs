using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Bonjour.RAOP
{
    public enum AudioMode : byte
    {
        Mono,
        Stereo
    }

    public class RaopMessage : IClientRequest, IClientResponse<RaopMessage>
    {
        public AudioMode AudioMode { get; set; }

        public bool IsCompressed { get; set; }

        public byte[] Sample { get; set; }

        #region IClientRequest Members

        public void WriteTo(System.IO.BinaryWriter stream)
        {
            byte[] header = new byte[3];
            header[0] = (byte)((byte)AudioMode << 1);
            header[2] = (byte)(IsCompressed ? 0 : 2);
            stream.Write(header);
            for (int i = 0; i < Sample.Length; i += 2)
            {
                stream.Write(Sample[i + 1]);
                stream.Write(Sample[i]);
            }
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IClientResponse<RaopMessage> Members

        public RaopMessage GetResponse(System.IO.BinaryReader stream)
        {
            throw new NotImplementedException();
        }

        public RaopMessage GetResponse(byte[] requestBytes)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
