using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Dns
{
    public class Answer
    {
        public DomainName DomainName { get; set; }
        public Type Type { get; set; }
        public Class Class { get; set; }
        public uint Ttl { get; set; }
        public ResponseData ResponseData { get; set; }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DomainName.ToBytes());
            bytes.AddRange(Message.ToBytes((ushort)Type));
            bytes.AddRange(Message.ToBytes((ushort)Class));
            bytes.AddRange(Message.ToBytes(Ttl));
            bytes.AddRange(ResponseData.ToBytes());
            return bytes.ToArray();
        }

        public static Answer FromBytes(byte[] bytes, ref int index)
        {
            Answer a = new Answer();
            a.DomainName = DomainName.FromBytes(bytes, ref index);
            ushort s;
            Message.FromBytes(bytes, index, out s);
            a.Type = (Type)s;
            index += 2;
            Message.FromBytes(bytes, index, out s);
            a.Class = (Class)s;
            index += 2;
            uint ttl;
            Message.FromBytes(bytes, index, out ttl);
            a.Ttl = ttl;
            index += 4;
            a.ResponseData = ResponseData.FromBytes(a.Type, bytes, ref index);
            return a;
        }

        public override string ToString()
        {
            return string.Format("{0}, Type: {1}, Class: {2}, TTL: {3} = {4}", DomainName, Type, Class, Ttl, ResponseData);
        }
    }
}
