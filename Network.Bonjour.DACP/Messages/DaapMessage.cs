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
            Messages = new Dictionary<string, DaapMessage>();
        }

        public DaapMessage this[string name]
        {
            get { return Messages[name]; }
        }

        public string Name { get; set; }
        public Dictionary<string, DaapMessage> Messages { get; set; }


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
                    Messages.Add(innerMessage.Name, innerMessage);
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
                daapMessage.Name == "casp"
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
    }
}
