using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Network.Dns
{
    public abstract class ResponseData
    {
        public abstract byte[] ToBytes();

        internal static ResponseData FromBytes(Type type, byte[] bytes, ref int index)
        {
            switch (type)
            {
                case Type.A:
                case Type.AAAA:
                    return HostAddress.FromBytes(bytes, ref index);
                    break;
                case Type.NS:
                    break;
                case Type.MD:
                    break;
                case Type.MF:
                    break;
                case Type.CNAME:
                    return CName.FromBytes(bytes, ref index);
                    break;
                case Type.SOA:
                    break;
                case Type.MB:
                    break;
                case Type.MG:
                    break;
                case Type.MR:
                    break;
                case Type.NULL:
                    break;
                case Type.WKS:
                    break;
                case Type.PTR:
                    return Ptr.FromBytes(bytes, ref index);
                    break;
                case Type.HINFO:
                    break;
                case Type.MINFO:
                    break;
                case Type.MX:
                    break;
                case Type.TXT:
                    return Txt.FromBytes(bytes, ref index);
                    break;
                case Type.SRV:
                    return Srv.FromBytes(bytes, ref index);
                    break;
                default:
                    break;
            }
            throw new NotImplementedException(string.Format("Cannot read {0} response", type));
        }
    }

    public class CName : ResponseData
    {
        public string CNAME { get; set; }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Message.ToBytes((short)Encoding.UTF8.GetByteCount(CNAME)));
            bytes.AddRange(Encoding.UTF8.GetBytes(CNAME));
            return bytes.ToArray();
        }

        internal static CName FromBytes(byte[] bytes, ref int index)
        {
            short byteCount;
            Message.FromBytes(bytes, index, out byteCount);
            index += 2;
            CName cName = new CName();
            cName.CNAME = Encoding.UTF8.GetString(bytes, index + 2, byteCount);
            index += byteCount;
            return cName;
        }
    }

    public class HostAddress : ResponseData
    {
        public IPAddress Address { get; set; }

        internal static HostAddress FromBytes(byte[] bytes, ref int index)
        {
            short byteCount;
            Message.FromBytes(bytes, index, out byteCount);
            index += 2;
            HostAddress ha = new HostAddress();
            ha.Address = new IPAddress(bytes.Skip(index).Take(byteCount).ToArray());
            index += byteCount;
            return ha;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Message.ToBytes((short)4));
            bytes.AddRange(Address.GetAddressBytes());
            return bytes.ToArray();
        }

        public override string ToString()
        {
            return Address.ToString();
        }
    }
    public class Ptr : ResponseData
    {
        public DomainName DomainName { get; set; }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DomainName.ToBytes());
            bytes.InsertRange(0, Message.ToBytes((short)bytes.Count));
            return bytes.ToArray();
        }

        internal static Ptr FromBytes(byte[] bytes, ref int index)
        {
            Ptr p = new Ptr();
            short byteCount;
            Message.FromBytes(bytes, index, out byteCount);
            index += 2;
            p.DomainName = DomainName.FromBytes(bytes, ref index);
            //index += byteCount;
            return p;
        }
    }

    public class Srv : ResponseData
    {
        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Message.ToBytes(Priority));
            bytes.AddRange(Message.ToBytes(Weight));
            bytes.AddRange(Message.ToBytes(Port));
            bytes.AddRange(Target.ToBytes());
            bytes.InsertRange(0, Message.ToBytes((short)bytes.Count));
            return bytes.ToArray();
        }

        public short Priority { get; set; }
        public short Weight { get; set; }
        public short Port { get; set; }
        public DomainName Target { get; set; }

        internal static Srv FromBytes(byte[] bytes, ref int index)
        {
            Srv srv = new Srv();
            short s;
            //Useless Datalength
            Message.FromBytes(bytes, index, out s);
            index += 2;
            Message.FromBytes(bytes, index, out s);
            index += 2;
            srv.Priority = s;
            Message.FromBytes(bytes, index, out s);
            index += 2;
            srv.Weight = s;
            Message.FromBytes(bytes, index, out s);
            index += 2;
            srv.Port = s;
            srv.Target = DomainName.FromBytes(bytes, ref index);
            return srv;
        }
    }

    public class Txt : ResponseData
    {
        public Txt()
        {
            Properties = new Dictionary<string, string>();
        }

        public override byte[] ToBytes()
        {
            short length = 0;
            List<byte> bytes = new List<byte>();
            foreach (KeyValuePair<string, string> kvp in Properties)
            {
                byte[] kvpBytes = Encoding.UTF8.GetBytes(kvp.Key + "=" + kvp.Value);
                bytes.AddRange(kvpBytes);
                length += (short)kvpBytes.Length;
            }
            bytes.InsertRange(0, Message.ToBytes(length));
            return bytes.ToArray();
        }

        public const string True = "true";
        public const string False = "false";

        public void AddProperty(string txt)
        {
            string[] kvp = txt.Split('=');
            if (kvp.Length == 2)
            {
                if (string.Compare(kvp[1], True, true) == 0)
                    Properties.Add(kvp[0], True);
                else if (string.Compare(kvp[1], False, true) == 0)
                    Properties.Add(kvp[0], False);
                else
                    Properties.Add(kvp[0], kvp[1]);
            }
        }

        public bool Contains(string key)
        {
            if (Properties.ContainsKey(key))
                return Properties[key] != False;
            return false;
        }

        public IDictionary<string, string> Properties { get; set; }

        internal static Txt FromBytes(byte[] bytes, ref int index)
        {
            Txt txt = new Txt();
            short byteCount;
            //Useless Datalength
            Message.FromBytes(bytes, index, out byteCount);
            index += 2;
            int stop = index + byteCount;
            while (index < stop)
            {
                txt.AddProperty(Encoding.UTF8.GetString(bytes, index + 1, bytes[index]));
                index += bytes[index] + 1;
            }
            return txt;
        }
    }
}
