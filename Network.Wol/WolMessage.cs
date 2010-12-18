using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.IO;

namespace Network.Wol
{
    public class WolMessage : IClientResponse<WolMessage>, IClientRequest
    {
        static WolMessage()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 255;
        }

        public WolMessage()
        {

        }

        public WolMessage(string address)
            : this(PhysicalAddress.Parse(address.ToUpper().Replace(':', '-')))
        {

        }

        public WolMessage(PhysicalAddress address)
        {
            MacAddress = address;
        }

        public PhysicalAddress MacAddress { get; set; }

        #region IClientResponse<WolMessage> Members

        public WolMessage GetResponse(System.IO.BinaryReader stream)
        {
            throw new NotImplementedException();
        }

        public WolMessage GetResponse(byte[] requestBytes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IClientRequest Members

        static readonly byte[] buffer = new byte[6];

        public void WriteTo(System.IO.BinaryWriter stream)
        {
            byte[] buffer = WolMessage.buffer;
            stream.Write(buffer);
            buffer = MacAddress.GetAddressBytes();
            for (int i = 0; i < 16; i++)
                stream.Write(buffer);
        }

        public byte[] GetBytes()
        {
            return BinaryHelper.GetBytes(this);
        }

        #endregion
    }
}
