using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Rtp
{
    public enum RtpVersion : byte
    {
        Version2 = 2
    }

    public enum PayloadType : sbyte
    {
        None = 0
    }

    public class RtpMessage : IClientRequest
    {
        public RtpVersion Version { get; set; }
        public bool HasPadding { get; set; }
        public RtpExtension Extension { get; set; }
        public List<CSRCIdentifier> Identifiers { get; set; }
        public bool IsKeyFrame { get; set; }
        public PayloadType PayloadType { get; set; }
        public ushort SequenceNumber { get; set; }
        public uint Timestamp { get; set; }
        public uint SSRC { get; set; }

        #region IClientRequest Members

        public void WriteTo(System.IO.BinaryWriter stream)
        {
            byte b;
            b = (byte)((byte)Version << 6);
            if (HasPadding)
                b += 1 << 5;
            if (Extension != null)
                b += 1 << 4;
            b += (byte)Identifiers.Count;
            stream.Write(b);
            b = (byte)(IsKeyFrame ? 1<<7: 0);
            b += (byte)PayloadType;
            stream.Write(b);
            stream.Write(Timestamp);
            stream.Write(SSRC);
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
