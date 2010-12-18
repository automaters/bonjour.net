using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Rtsp;
using System.Net;
using System.Security.Cryptography;
using System.IO;

namespace Network.Bonjour.RAOP
{
    public class RaopClient : Client<RaopMessage, RaopMessage>
    {
        public RaopClient(IPEndPoint endpoint)
            : this(endpoint, "Bonjour Client")
        {
        }

        public RaopClient(IPEndPoint endpoint, string userAgent)
            : base(true)
        {

            string n =
            "59dE8qLieItsH1WgjrcFRKj6eUWqi+bGLOX1HL3U3GhC/j0Qg90u3sG/1CUtwC" +
            "5vOYvfDmFI6oSFXi5ELabWJmT2dKHzBJKa3k9ok+8t9ucRqMd6DZHJ2YCCLlDR" +
            "KSKv6kDqnw4UwPdpOMXziC/AMj3Z/lUVX1G7WSHCAWKf1zNS1eLvqr+boEjXuB" +
            "OitnZ/bDzPHrTOZz0Dew0uowxf/+sG+NCK3eQJVxqcaJ/vEHKIVd2M+5qL71yJ" +
            "Q+87X6oV3eaYvt3zWZYD6z5vYTcrtij2VZ9Zmni/UAaHqn9JdsBWLUEpVviYnh" +
            "imNVvYFZeCXg/IdTQ+x4IRdiXNv5hEew==";
            string e = "AQAB";
            rtsp = new RtspClient("zeroClient");
            rtsp.StartTcp(endpoint);
            RSAParameters key = new RSAParameters();
            key.Modulus = Convert.FromBase64String(n);
            key.Exponent = Convert.FromBase64String(e);
            rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            dataCrypto = Rijndael.Create();
            dataCrypto.Mode = CipherMode.CBC;
            dataCrypto.Padding = PaddingMode.None;
            dataCrypto.KeySize = 128;

            dataCrypto.GenerateKey();
            dataCrypto.GenerateIV();
            StartTcp(endpoint);
        }



        RSACryptoServiceProvider rsa;
        private RtspClient rtsp;
        private Rijndael dataCrypto;

        public static void Main()
        {
            int sequenceId = 1;
            RaopClient client = new RaopClient(new IPEndPoint(IPAddress.Parse("192.168.1.16"), 5000));
            var request = new RtspRequest();
            request.Method = "OPTIONS";
            request.Uri = "*";
            request.Headers["CSeq"] = sequenceId.ToString();
            request.Headers["User-Agent"] = client.rtsp.UserAgent;
            RtspResponse response = client.rtsp.Send(request);
            Console.Write(response);

            request = new RtspRequest();
            request.Method = "ANNOUNCE";
            request.Uri = "rtsp://" + client.Host.Address + "/123";
            request.Headers["CSeq"] = sequenceId.ToString();
            request.Headers["Content-Type"] = "application/sdp";
            request.Headers["User-Agent"] = client.rtsp.UserAgent;
            var sw = new StreamWriter(request.Body);
            sw.WriteLine("v=0");
            sw.WriteLine("c=IN" + (client.Host.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? " IP4 " : " IP6 ") + client.Host.Address);
            sw.WriteLine("t=0 0");
            sw.WriteLine("m=audio RTP/AVP 96");
            sw.WriteLine("a=rtpmap:96 AppleLossless");
            string key = Convert.ToBase64String(client.RSAEncrypt(client.dataCrypto.Key));
            string iv = Convert.ToBase64String(client.dataCrypto.IV);
            sw.WriteLine("a=rsaaeskey:{0}", key);
            sw.WriteLine("a=aesiv:{0}", iv);
            sw.WriteLine("a=fmtp:96 4096 0 16 40 10 14 2 255 0 0 44100");
            response = client.rtsp.Send(request);
            Console.Write(response);

            sequenceId++;
            request = new RtspRequest();
            request.Method = "SETUP";
            request.Headers["CSeq"] = sequenceId.ToString();
            request.Uri = "rtsp://" + client.Host.Address + "/123";
            request.Headers["Transport"] = "RTP/AVP/TCP;unicast;interleaved=0-1;mode=record";
            request.Headers["User-Agent"] = client.rtsp.UserAgent;
            response = client.rtsp.Send(request);
            string sessionId = response.Headers["SESSION"];
            Console.Write(response);

            //sequenceId++;
            //request = new RtspRequest();
            //request.Method = "SET_PARAMETER";
            //request.Uri = "rtsp://" + client.Host.Address + "/123";
            //request.Headers["CSeq"] = sequenceId.ToString();
            //request.Headers["Session"] = sessionId;
            //request.Headers["User-Agent"] = client.UserAgent;
            //request.Headers["Content-Type"] = "text/parameters";
            //sw = new StreamWriter(request.Body);
            //sw.WriteLine("volume: -15.000711");
            //response = client.Send(request);
            //Console.Write(response);

            sequenceId++;
            request = new RtspRequest();
            request.Method = "RECORD";
            request.Uri = "rtsp://" + client.Host.Address + "/123";
            request.Headers["CSeq"] = sequenceId.ToString();
            request.Headers["Session"] = sessionId;
            request.Headers["User-Agent"] = client.rtsp.UserAgent;

            response = client.rtsp.Send(request);
            Console.Write(response);


            sequenceId++;
            request = new RtspRequest();
            request.Method = "TEARDOWN";
            request.Uri = "rtsp://" + client.Host.Address + "/123";
            request.Headers["CSeq"] = sequenceId.ToString();
            request.Headers["Session"] = sessionId;
            request.Headers["User-Agent"] = client.rtsp.UserAgent;
            response = client.rtsp.Send(request);
            Console.Write(response);
            Console.Read();
        }

        private byte[] RSAEncrypt(byte[] p)
        {
            return rsa.Encrypt(p, true);
        }

        private void Encrypt(byte[] Buffer, int Offset, int Count)
        {
            MemoryStream ms = new MemoryStream();
            ICryptoTransform ct = dataCrypto.CreateEncryptor();

            CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(Buffer, Offset, (Count / 16) * 16);
            cs.Close();

            ms.ToArray().CopyTo(Buffer, Offset);
        }

        public void SendSample(byte[] Sample, int Pos, int Count)
        {
            byte[] header = new byte[16]
        {
            0x24, 0x00, 0x00, 0x00,
            0xF0, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00
        };

            byte[] data = new byte[Count + header.Length];
            header.CopyTo(data, 0);

            short len = Convert.ToInt16(Count + 12);
            byte[] ab = BitConverter.GetBytes(len);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ab, 0, ab.Length);
            ab.CopyTo(data, 2);

            Buffer.BlockCopy(Sample, Pos, data, header.Length, Count);
            Encrypt(data, header.Length, Count);

            //nsdata.Write(data, 0, data.Length);
        }


        protected override ClientEventArgs<RaopMessage, RaopMessage> GetEventArgs(RaopMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
