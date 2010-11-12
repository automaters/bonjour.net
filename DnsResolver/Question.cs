using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;

namespace Network.Dns
{
    public class Question : IServerResponse
    {
        private Question()
        {
            DomainName = new DomainName();

        }

        public Question(string domainName)
            : this()
        {
            if (!domainName.EndsWith("."))
            {
                if (domainName.EndsWith(".local"))
                    domainName += ".";
                else
                    domainName += ".local.";
            }
            else
            {
                if (!domainName.EndsWith("local."))
                    domainName += "local.";
            }
            DomainName.AddRange(domainName.Split('.'));
            Type = QType.ALL;
            Class = QClass.ALL;
        }

        public DomainName DomainName { get; private set; }

        public QType Type { get; set; }
        public QClass Class { get; set; }

        public bool CacheFlush { get; set; }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DomainName.ToBytes());
            bytes.AddRange(Message.ToBytes((ushort)Type));
            bytes.AddRange(Message.ToBytes((ushort)Class));
            //bytes.AddRange(Message.ToBytes((ushort)(((ushort)(ushort.MaxValue >> 15 << 15)) + (ushort)Class)));
            return bytes.ToArray();
        }

        public static Question FromBytes(byte[] bytes, ref int index)
        {
            Question q = new Question();
            q.DomainName = DomainName.FromBytes(bytes, ref index);
            ushort s;
            Message.FromBytes(bytes, index, out s);
            index += 2;
            q.Type = (QType)s;
            Message.FromBytes(bytes, index, out s);
            q.Class = (QClass)s;
            index += 2;
            return q;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DomainName);
            sb.AppendFormat(" QType : {0},", Type);
            sb.AppendFormat(" QClass : {0}", Class);
            return sb.ToString();
        }

        #region IResponse Members

        public void WriteTo(BinaryWriter writer)
        {
            DomainName.WriteTo(writer);
            writer.Write(Message.ToBytes((ushort)Type));
            writer.Write(Message.ToBytes((ushort)Class));
        }

        public byte[] GetBytes()
        {
            return BinaryHelper.GetBytes(this);
        }

        #endregion

        public static Question Get(BinaryReader reader)
        {
            Question q = new Question();
            q.DomainName = DomainName.Get(reader);
            ushort s;
            Message.FromBytes(reader.ReadBytes(2), out s);
            q.Type = (QType)s;
            Message.FromBytes(reader.ReadBytes(2), out s);
            q.Class = (QClass)((ushort)(s << 1) >> 1);
            q.CacheFlush = ((ushort)q.Class) != s;
            return q;
        }
    }
}
