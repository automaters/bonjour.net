using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Dns
{
    public class DomainName : List<string>
    {
        public DomainName() { }

        public DomainName(IEnumerable<string> domainName) : base(domainName) { }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach (string label in this)
            {
                bytes.Add((byte)Encoding.UTF8.GetByteCount(label));
                if (label.Length == 0)
                    break;
                bytes.AddRange(Encoding.UTF8.GetBytes(label));
            }
            return bytes.ToArray();
        }


        public static DomainName FromBytes(byte[] bytes, ref int index)
        {
            if (bytes[index] >> 6 == 3)
            {
                //In case of pointer
                short ptr;
                bytes[index] -= 3 << 6;
                Message.FromBytes(bytes, index, out ptr);
                bytes[index] += 3 << 6;
                index += 2;
                ptr = (short)(ptr << 2 >> 2);
                int iPtr = ptr;
                return FromBytes(bytes, ref iPtr);
            }
            else
            {
                DomainName dn = new DomainName();

                if (bytes[index] != 0)
                {
                    dn.Add(Encoding.UTF8.GetString(bytes, index + 1, bytes[index]));
                    index += bytes[index] + 1;
                    dn.AddRange(DomainName.FromBytes(bytes, ref index));
                }
                else
                    index++;
                return dn;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string label in this)
                sb.Append(label + '.');
            return sb.ToString();
        }

        public static implicit operator string(DomainName dn)
        {
            return dn.ToString();
        }

        public static implicit operator DomainName(string s)
        {
            DomainName dn = new DomainName();
            dn.AddRange(s.Split('.'));
            return dn;
        }
    }
}
