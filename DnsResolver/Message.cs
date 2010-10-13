using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Network.Dns
{
    public class Message : IRequest<Message>, IResponse<Message>
    {
        public Message()
        {
            Questions = new List<Question>();
            Answers = new List<Answer>();
            Authorities = new List<Answer>();
            Additionals = new List<Answer>();
        }

        public IPEndPoint From { get; set; }

        public Qr QueryResponse { get; set; }

        public ushort ID { get; set; }

        public OpCode OpCode { get; set; }

        public bool AuthoritativeAnswer { get; set; }

        public bool Truncated { get; set; }

        public bool RecursionDesired { get; set; }

        public bool RecursionAvailable { get; set; }

        public ResponseCode ResponseCode { get; set; }

        public ushort QuestionEntries
        {
            get { return (ushort)Questions.Count; }
        }

        public ushort AnswerEntries
        {
            get { return (ushort)Answers.Count; }
        }

        public ushort AuthorityEntries { get { return (ushort)Authorities.Count; } }

        public ushort AdditionalEntries { get { return (ushort)Additionals.Count; } }

        public byte[] ToByteArray()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteTo(new BinaryWriter(stream));
                return stream.ToArray();
            }

            //List<byte> bytes = new List<byte>();
            ////ID
            //bytes.AddRange(ToBytes(ID));
            ////Qr, Opcode, Aa, Tc, Rd
            //byte b = 0;
            //b += (byte)QueryResponse;
            //b = (byte)(b << 4);
            //b += (byte)OpCode;
            //b = (byte)(b << 1);
            //b += (AuthoritativeAnswer) ? (byte)1 : (byte)0;
            //b = (byte)(b << 1);
            //b += (Truncated) ? (byte)1 : (byte)0;
            //b = (byte)(b << 1);
            //b += (RecursionDesired) ? (byte)1 : (byte)0;
            //bytes.Add(b);

            ////Ra, Z, Rcode
            //b = 0;
            //b += (RecursionAvailable) ? (byte)1 : (byte)0;
            //b = (byte)(b << 7);
            //b += (byte)ResponseCode;
            //bytes.Add(b);
            //bytes.AddRange(ToBytes(QuestionEntries));
            //bytes.AddRange(ToBytes(AnswerEntries));
            //bytes.AddRange(ToBytes(AuthorityEntries));
            //bytes.AddRange(ToBytes(AdditionalEntries));
            //foreach (Question q in Questions)
            //    bytes.AddRange(q.ToBytes());
            //foreach (Answer a in Answers)
            //    bytes.AddRange(a.ToBytes());
            //foreach (Answer a in Authorities)
            //    bytes.AddRange(a.ToBytes());
            //foreach (Answer a in Additionals)
            //    bytes.AddRange(a.ToBytes());
            //return bytes.ToArray();
        }


        internal static byte[] ToBytes(uint i)
        {
            byte[] bytes = new byte[4];
            bytes[3] = (byte)(i % (byte.MaxValue + 1));
            i = i >> 8;
            bytes[2] = (byte)(i % (byte.MaxValue + 1));
            i = i >> 8;
            bytes[1] = (byte)(i % (byte.MaxValue + 1));
            i = i >> 8;
            bytes[0] = (byte)(i % (byte.MaxValue + 1));
            return bytes;
        }

        internal static byte[] ToBytes(ushort s)
        {
            byte[] bytes = new byte[2];
            bytes[1] = (byte)(s % (byte.MaxValue + 1));
            s = (ushort)(s >> 8);
            bytes[0] = (byte)(s % (byte.MaxValue + 1));
            return bytes;
        }

        internal static long LongFromBytes(byte[] bytes, int offset, int length)
        {
            long result = 0;
            for (int i = offset + length - 1; i >= offset; i--)
            {
                result += bytes[i] << (length - 1 - i + offset) * 8;
            }
            return result;
        }

        internal static void FromBytes(byte[] bytes, int offset, out ushort s)
        {
            s = (ushort)LongFromBytes(bytes, offset, 2);
        }

        internal static void FromBytes(byte[] bytes, out ushort s)
        {
            FromBytes(bytes, 0, out s);
        }

        internal static void FromBytes(byte[] bytes, out uint i)
        {
            FromBytes(bytes, 0, out i);
        }

        internal static void FromBytes(byte[] bytes, int offset, out uint i)
        {
            i = (uint)LongFromBytes(bytes, offset, 4);
        }

        public IList<Question> Questions { get; set; }
        public IList<Answer> Answers { get; set; }
        public IList<Answer> Authorities { get; set; }
        public IList<Answer> Additionals { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine(string.Format("ID : {0}", ID));
            sb.AppendLine(string.Format("Query/Response : {0}", QueryResponse));
            sb.AppendLine(string.Format("OpCode : {0}", OpCode));
            sb.AppendLine(string.Format("Authoritative Answer : {0}", AuthoritativeAnswer));
            sb.AppendLine(string.Format("Truncated : {0}", Truncated));
            sb.AppendLine(string.Format("Recursion Desired : {0}", RecursionDesired));
            sb.AppendLine(string.Format("Recursion Available : {0}", RecursionDesired));
            sb.AppendLine(string.Format("Recursion ResponseCode : {0}", ResponseCode));
            sb.AppendLine(string.Format("Question Entries ({0}) :", QuestionEntries));
            foreach (Question q in Questions)
                sb.AppendLine(q.ToString());
            sb.AppendLine(string.Format("Answer Entries ({0}) :", AnswerEntries));
            foreach (Answer a in Answers)
                sb.AppendLine(a.ToString());

            return sb.ToString();
        }

        #region IResponse Members

        public void WriteTo(BinaryWriter writer)
        {
            //ID
            writer.Write(Message.ToBytes(ID));
            //Qr, Opcode, Aa, Tc, Rd
            byte b = 0;
            b += (byte)QueryResponse;
            b = (byte)(b << 4);
            b += (byte)OpCode;
            b = (byte)(b << 1);
            b += (AuthoritativeAnswer) ? (byte)1 : (byte)0;
            b = (byte)(b << 1);
            b += (Truncated) ? (byte)1 : (byte)0;
            b = (byte)(b << 1);
            b += (RecursionDesired) ? (byte)1 : (byte)0;
            writer.Write(b);

            //Ra, Z, Rcode
            b = 0;
            b += (RecursionAvailable) ? (byte)1 : (byte)0;
            b = (byte)(b << 7);
            b += (byte)ResponseCode;
            writer.Write(b);
            writer.Write(Message.ToBytes(QuestionEntries));
            writer.Write(Message.ToBytes(AnswerEntries));
            writer.Write(Message.ToBytes(AuthorityEntries));
            writer.Write(Message.ToBytes(AdditionalEntries));
            foreach (Question q in Questions)
                q.WriteTo(writer);
            foreach (Answer a in Answers)
                a.WriteTo(writer);
            foreach (Answer a in Authorities)
                a.WriteTo(writer);
            foreach (Answer a in Additionals)
                a.WriteTo(writer);
        }

        public byte[] GetBytes()
        {
            return this.ToByteArray();
        }

        #endregion

        #region IRequest<Message> Members

        public Message GetRequest(BinaryReader reader)
        {
            if (!(reader is BackReferenceBinaryReader))
                reader = new BackReferenceBinaryReader(reader.BaseStream, Encoding.BigEndianUnicode);
            Message m = new Message();
            ushort id;
            int index = 0;
            Message.FromBytes(reader.ReadBytes(2), out id);
            //FromBytes(bytes, out id);
            m.ID = id;
            index++; index++;
            byte b = reader.ReadByte();
            //byte b = bytes[index++];
            //Qr, Opcode, Aa, Tc, Rd
            m.RecursionDesired = (b % 2) == 1;
            b = (byte)(b >> 1);
            m.Truncated = (b % 2) == 1;
            b = (byte)(b >> 1);
            m.AuthoritativeAnswer = (b % 2) == 1;
            b = (byte)(b >> 1);
            int opCodeNumber = b % 16;
            m.OpCode = (OpCode)opCodeNumber;
            b = (byte)(b >> 4);
            m.QueryResponse = (Qr)b;
            //Ra, Z, Rcode
            b = reader.ReadByte();
            //b = bytes[index++];
            m.RecursionAvailable = b > 127;
            b = (byte)((b << 1) >> 1);
            m.ResponseCode = (ResponseCode)b;
            ushort questionEntryCount, answerEntryCount, authorityEntryCount, additionalEntryCount;
            Message.FromBytes(reader.ReadBytes(2), out questionEntryCount);
            Message.FromBytes(reader.ReadBytes(2), out answerEntryCount);
            Message.FromBytes(reader.ReadBytes(2), out authorityEntryCount);
            Message.FromBytes(reader.ReadBytes(2), out additionalEntryCount);
            //FromBytes(new byte[] { bytes[index++], bytes[index++] }, out questionEntryCount);
            //FromBytes(new byte[] { bytes[index++], bytes[index++] }, out answerEntryCount);
            //FromBytes(new byte[] { bytes[index++], bytes[index++] }, out authorityEntryCount);
            //FromBytes(new byte[] { bytes[index++], bytes[index++] }, out additionalEntryCount);
            for (int i = 0; i < questionEntryCount; i++)
                m.Questions.Add(Question.Get(reader));
            for (int i = 0; i < answerEntryCount; i++)
                m.Answers.Add(Answer.Get(reader));
            for (int i = 0; i < authorityEntryCount; i++)
                m.Authorities.Add(Answer.Get(reader));
            for (int i = 0; i < additionalEntryCount; i++)
                m.Additionals.Add(Answer.Get(reader));
            return m;
        }

        public Message GetRequest(byte[] requestBytes)
        {
            using (MemoryStream ms=new MemoryStream(requestBytes))
            {
                ms.Position = 0;
                return GetRequest(new BinaryReader(ms));
            }
        }

        #endregion

        public Message Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteTo(new BinaryWriter(ms));
                ms.Position = 0;
                return GetRequest(new BinaryReader(ms));
            }
        }

        #region IResponse<Message> Members

        public Message GetResponse(BinaryReader stream)
        {
            return GetRequest(stream);
        }

        public Message GetResponse(byte[] requestBytes)
        {
            return GetRequest(requestBytes);
        }

        #endregion
    }
}
