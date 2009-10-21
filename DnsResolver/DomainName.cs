using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Network.Dns
{
    public class DomainName : List<string>, IResponse
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

        public ushort GetByteCount()
        {
            ushort result = 1;
            foreach (string label in this)
            {
                if (label.Length == 0)
                    break;
                int bytes = Encoding.UTF8.GetByteCount(label);
                result += (ushort)(bytes + 1);
                //labels.Add(new KeyValuePair<byte[], byte>(bytes, (byte)bytes.Length));
                //totalLength += (ushort)(bytes.Length + 1);
            }
            return result;
        }

        public static DomainName FromBytes(byte[] bytes, ref int index)
        {
            if (bytes[index] >> 6 == 3)
            {
                //In case of pointer
                ushort ptr;
                bytes[index] -= 3 << 6;
                Message.FromBytes(bytes, index, out ptr);
                bytes[index] += 3 << 6;
                index += 2;
                ptr = (ushort)(ptr << 2 >> 2);
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

        #region IResponse Members

        public void WriteTo(BinaryWriter writer)
        {
            //ushort totalLength = 0;
            //List<KeyValuePair<byte[], byte>> labels = new List<KeyValuePair<byte[], byte>>();
            foreach (string label in this)
            {
                if (label.Length == 0)
                    break;
                byte[] bytes = Encoding.UTF8.GetBytes(label);
                //labels.Add(new KeyValuePair<byte[], byte>(bytes, (byte)bytes.Length));
                //totalLength += (ushort)(bytes.Length + 1);
                writer.Write((byte)bytes.Length);
                writer.Write(bytes);
            }
            writer.Write((byte)0);
            //writer.Write(totalLength);
            //foreach (var label in labels)
            //{
            //    writer.Write(label.Value);
            //    writer.Write(label.Key);
            //}
        }

        public byte[] GetBytes()
        {
            return BinaryHelper.GetBytes(this);
        }

        #endregion

        public static DomainName Get(BinaryReader reader)
        {
            byte stringLength = reader.ReadByte();
            if (stringLength >> 6 == 3)
            {

                if (!(reader is BackReferenceBinaryReader))
                    throw new NotSupportedException("The given binary reader does not support back reference");
                //In case of pointer
                ushort ptr;
                Message.FromBytes(new byte[] { (byte)(stringLength - (3 << 6)), reader.ReadByte() }, out ptr);
                return ((BackReferenceBinaryReader)reader).Get<DomainName>(ptr);
            }
            else
            {
                DomainName dn = new DomainName();
                if (reader is BackReferenceBinaryReader)
                    ((BackReferenceBinaryReader)reader).Register((int)reader.BaseStream.Position - 1, dn);
                //stringLength = reader.ReadByte();
                if (stringLength != 0)
                {
                    dn.Add(Encoding.UTF8.GetString(reader.ReadBytes(stringLength)));
                    //dn.Add(Encoding.UTF8.GetString(bytes, index + 1, bytes[index]));

                    dn.AddRange(DomainName.Get(reader));
                }
                //else
                //    index++;
                return dn;
            }
        }
    }
}
