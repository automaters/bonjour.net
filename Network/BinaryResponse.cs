using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    public class BinaryResponse : IResponse<BinaryResponse>
    {
        private MemoryStream stream;

        public BinaryResponse()
        {
            stream = new MemoryStream();
        }



        #region IResponse Members

        public void WriteTo(BinaryWriter target)
        {
            this.stream.Position = 0;
            this.stream.WriteTo(target.BaseStream);
        }

        public byte[] GetBytes()
        {
            return stream.ToArray();
        }

        #endregion

        #region IResponse<BinaryResponse> Members

        public BinaryResponse GetResponse(BinaryReader stream)
        {
            byte[] buffer;
            do
            {
                buffer = stream.ReadBytes(1024);
                this.stream.Write(buffer, 0, buffer.Length);
            }
            while (buffer.Length == 1024);
            return this;
        }

        public BinaryResponse GetResponse(byte[] requestBytes)
        {
            stream.Write(requestBytes, 0, requestBytes.Length);
            return this;
        }

        #endregion
    }

    public static class BinaryHelper
    {
        public static byte[] GetBytes(IResponse response)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                response.WriteTo(new BinaryWriter(stream));
                return stream.ToArray();
            }
        }

        public static string ReadLine(BinaryReader reader)
        {
            bool stopReading = false;
            List<char> chars = new List<char>();
            char[] readChar = reader.ReadChars(2);

            if (new string(readChar) == Environment.NewLine)
                stopReading = true;

            while (!stopReading)
            {
                chars.Add(readChar[0]);
                readChar[0] = readChar[1];
                readChar[1] = reader.ReadChar();
                if (new string(readChar) == Environment.NewLine)
                    stopReading = true;
            }

            return new string(chars.ToArray());
        }
    }
}
