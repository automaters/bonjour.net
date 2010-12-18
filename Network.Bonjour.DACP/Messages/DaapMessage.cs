using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network.Bonjour.DACP
{
    public class DaapMessage
    {
        public DaapMessage()
        {
            Messages = new Dictionary<string, List<DaapMessage>>();
        }

        public List<DaapMessage> this[string name]
        {
            get
            {
                List<DaapMessage> result;
                if (Messages.TryGetValue(name, out result))
                {
                    return result;
                }
                return null;
            }
        }

        public string Name { get; set; }
        public Dictionary<string, List<DaapMessage>> Messages { get; set; }


        public void ReadFrom(Stream s)
        {
            Name = ReadString(s, 4);
            int length = ReadInt32(s);
            long startPosition = s.Position;
            if (startPosition + length == s.Position)
                return;
            if (IsNode(this))
            {
                while (startPosition + length > s.Position)
                {
                    DaapMessage innerMessage = new DaapMessage();
                    innerMessage.ReadFrom(s);
                    List<DaapMessage> messages;
                    if (!Messages.TryGetValue(innerMessage.Name, out messages))
                    {
                        messages = new List<DaapMessage>();
                        Messages.Add(innerMessage.Name, messages);
                    }
                    messages.Add(innerMessage);
                }
            }
            else
            {
                Value = ReadBytes(s, length);
            }

        }

        private static bool IsNode(DaapMessage daapMessage)
        {
            return
                daapMessage.Name == "mdcl" ||
                daapMessage.Name == "mcon" ||
                daapMessage.Name == "mlcl" ||
                daapMessage.Name == "mlit" ||
                daapMessage.Name == "mbcl" ||
                daapMessage.Name == "msrv" ||
                daapMessage.Name == "mccr" ||
                daapMessage.Name == "mlog" ||
                daapMessage.Name == "mupd" ||
                daapMessage.Name == "mudl" ||
                daapMessage.Name == "avdb" ||
                daapMessage.Name == "abro" ||
                daapMessage.Name == "abal" ||
                daapMessage.Name == "abar" ||
                daapMessage.Name == "abcp" ||
                daapMessage.Name == "abgn" ||
                daapMessage.Name == "adbs" ||
                daapMessage.Name == "aply" ||
                daapMessage.Name == "apso" ||
                daapMessage.Name == "prsv" ||
                daapMessage.Name == "arif" ||
                daapMessage.Name == "casp" ||
                daapMessage.Name == "caci"
                ;
        }

        private byte[] ReadBytes(Stream s, int length)
        {
            byte[] bytes = new byte[length];
            int lengthRead = 0;
            do
            {
                lengthRead += s.Read(bytes, 0, length);
            }
            while (lengthRead != length);
            return bytes;
        }

        private string ReadString(Stream s, int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(s, length));
        }

        private string ReadString(Stream s)
        {
            return ReadString(s, ReadInt32(s));
        }

        private int ReadInt32(Stream s)
        {
            byte[] bytes = ReadBytes(s, 4);
            return ToInt32(bytes);
        }

        private int ToInt32(byte[] bytes)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("byte array length must be 4 to be able to parse an int");
            return (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + (bytes[3]);
        }

        public byte[] Value { get; set; }

        public int ToInt32()
        {
            return ToInt32(Value);
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(Value);
        }

        public long ToInt64()
        {
            return ToInt64(Value);
        }

        private long ToInt64(byte[] bytes)
        {
            if (bytes.Length != 8)
                throw new ArgumentException("byte array length must be 8 to be able to parse a long");
            return (bytes[0] << 56) + (bytes[1] << 48) + (bytes[2] << 40) + (bytes[3] << 32) + (bytes[4] << 24) + (bytes[5] << 16) + (bytes[6] << 8) + (bytes[7]);
        }

        public static string ToHexString(long p)
        {
            return ToHexString(ToBytes(p));
        }

        public static string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.AppendFormat("{0:x}", b);
            return sb.ToString();
        }

        public static byte[] ToBytes(long p)
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)(p >> 56);
            p -= bytes[0] << 56;
            bytes[1] = (byte)(p >> 48);
            p -= bytes[1] << 48;
            bytes[2] = (byte)(p >> 40);
            p -= bytes[2] << 40;
            bytes[3] = (byte)(p >> 32);
            p -= bytes[3] << 32;
            bytes[4] = (byte)(p >> 24);
            p -= bytes[4] << 24;
            bytes[5] = (byte)(p >> 16);
            p -= bytes[5] << 16;
            bytes[6] = (byte)(p >> 8);
            p -= bytes[6] << 8;
            bytes[7] = (byte)p;
            return bytes;
        }
    }
}
