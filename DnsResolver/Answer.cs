using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Dns
{
    public class Answer : IServerResponse
    {
        public DomainName DomainName { get; set; }
        public Type Type { get; set; }
        public Class Class { get; set; }
        public uint Ttl { get; set; }
        public ResponseData ResponseData { get; set; }

        public byte[] GetBytes()
        {
            return BinaryHelper.GetBytes(this);
        }

        public override string ToString()
        {
            return string.Format("{0}, Type: {1}, Class: {2}, TTL: {3} = {4}", DomainName, Type, Class, Ttl, ResponseData);
        }

        internal static Answer Get(System.IO.BinaryReader reader)
        {
            Answer a = new Answer();
            a.DomainName = DomainName.Get(reader);
            ushort s;
            Message.FromBytes(reader.ReadBytes(2), out s);
            a.Type = (Type)s;
            Message.FromBytes(reader.ReadBytes(2), out s);
            a.Class = (Class)s;
            uint ttl;
            Message.FromBytes(reader.ReadBytes(4), out ttl);
            a.Ttl = ttl;
            a.ResponseData = ResponseData.Get(a.Type, reader);
            return a;
        }

        public void WriteTo(System.IO.BinaryWriter writer)
        {
            DomainName.WriteTo(writer);
            writer.Write(Message.ToBytes((ushort)Type));
            writer.Write(Message.ToBytes((ushort)Class));
            writer.Write(Message.ToBytes(Ttl));
            if (ResponseData != null)
                ResponseData.WriteTo(writer);
        }
    }
}
