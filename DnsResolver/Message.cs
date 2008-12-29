using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Dns
{
    public class Message
    {
        public Message()
        {
            Questions = new List<Question>();
            Answers = new List<Answer>();
            Authorities = new List<Answer>();
            Additionals = new List<Answer>();
        }

        public Qr QueryResponse { get; set; }

        public short ID { get; set; }

        public OpCode OpCode { get; set; }

        public bool AuthoritativeAnswer { get; set; }

        public bool Truncated { get; set; }

        public bool RecursionDesired { get; set; }

        public bool RecursionAvailable { get; set; }

        public ResponseCode ResponseCode { get; set; }

        public short QuestionEntries
        {
            get { return (short)Questions.Count; }
        }

        public short AnswerEntries
        {
            get { return (short)Answers.Count; }
        }

        public short AuthorityEntries { get; set; }

        public short AdditionalEntries { get; set; }

        public byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();
            //ID
            bytes.AddRange(ToBytes(ID));
            //Qr, Opcode, Aa, Tc, Rd
            byte b = 0;
            b += (byte)QueryResponse;
            b = (byte)(b << 5);
            b += (byte)OpCode;
            b = (byte)(b << 1);
            b += (AuthoritativeAnswer) ? (byte)1 : (byte)0;
            b = (byte)(b << 1);
            b += (Truncated) ? (byte)1 : (byte)0;
            b = (byte)(b << 1);
            b += (RecursionDesired) ? (byte)1 : (byte)0;
            bytes.Add(b);

            //Ra, Z, Rcode
            b = 0;
            b += (RecursionAvailable) ? (byte)1 : (byte)0;
            b = (byte)(b << 7);
            b += (byte)ResponseCode;
            bytes.Add(b);
            bytes.AddRange(ToBytes(QuestionEntries));
            bytes.AddRange(ToBytes(AnswerEntries));
            bytes.AddRange(ToBytes(AuthorityEntries));
            bytes.AddRange(ToBytes(AdditionalEntries));
            foreach (Question q in Questions)
            {
                bytes.AddRange(q.ToBytes());
            }
            return bytes.ToArray();
        }


        internal static byte[] ToBytes(int i)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(i % (byte.MaxValue + 1));
            i = i >> 8;
            bytes[1] = (byte)(i % (byte.MaxValue + 1));
            i = i >> 8;
            bytes[2] = (byte)(i % (byte.MaxValue + 1));
            i = i >> 8;
            bytes[3] = (byte)(i % (byte.MaxValue + 1));
            return bytes;
        }

        internal static byte[] ToBytes(short s)
        {
            byte[] bytes = new byte[2];
            bytes[1] = (byte)(s % (byte.MaxValue + 1));
            s = (short)(s >> 8);
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

        internal static void FromBytes(byte[] bytes, int offset, out short s)
        {
            s = (short)LongFromBytes(bytes, offset, 2);
        }

        internal static void FromBytes(byte[] bytes, out short s)
        {
            FromBytes(bytes, 0, out s);
        }

        internal static void FromBytes(byte[] bytes, out int i)
        {
            FromBytes(bytes, 0, out i);
        }

        internal static void FromBytes(byte[] bytes, int offset, out int i)
        {
            i = (int)LongFromBytes(bytes, offset, 4);
        }

        public IList<Question> Questions { get; set; }
        public IList<Answer> Answers { get; set; }
        public IList<Answer> Authorities { get; set; }
        public IList<Answer> Additionals { get; set; }

        public static Message FromBytes(byte[] bytes)
        {
            Message m = new Message();
            short id;
            int index = 0;
            FromBytes(bytes, out id);
            m.ID = id;
            index++; index++;
            byte b = bytes[index++];
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
            b = bytes[index++];
            m.RecursionAvailable = b > 127;
            b = (byte)((b << 1) >> 1);
            m.ResponseCode = (ResponseCode)b;
            short questionEntryCount, answerEntryCount, authorityEntryCount, additionalEntryCount;
            FromBytes(new byte[] { bytes[index++], bytes[index++] }, out questionEntryCount);
            FromBytes(new byte[] { bytes[index++], bytes[index++] }, out answerEntryCount);
            FromBytes(new byte[] { bytes[index++], bytes[index++] }, out authorityEntryCount);
            FromBytes(new byte[] { bytes[index++], bytes[index++] }, out additionalEntryCount);
            for (int i = 0; i < questionEntryCount; i++)
                m.Questions.Add(Question.FromBytes(bytes, ref index));
            for (int i = 0; i < answerEntryCount; i++)
                m.Answers.Add(Answer.FromBytes(bytes, ref index));
            for (int i = 0; i < authorityEntryCount; i++)
                m.Authorities.Add(Answer.FromBytes(bytes, ref index));
            for (int i = 0; i < additionalEntryCount; i++)
                m.Additionals.Add(Answer.FromBytes(bytes, ref index));
            return m;
        }

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
    }
}
