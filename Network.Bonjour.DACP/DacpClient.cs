using System;
using System.Collections.Generic;
using System.Text;
using Network.ZeroConf;
using Network.Dns;

namespace Network.Bonjour.DACP
{
    public class Client : Client<DacpRequest, DacpResponse>
    {
        public static void Main()
        {
            Client client = new Client(new EndPoint() { Port = 3689, Addresses = { System.Net.IPAddress.Parse("192.168.1.15") } });
            client.Login();
            Console.WriteLine(client.SessionId);
            DaapMessage speakers = client.GetSpeakers();
            byte[] speakerId = null;
            foreach (DaapMessage speaker in speakers.Messages["mdcl"])
            {
                if (speaker["caia"] != null)
                    Console.Write("[X] ");
                else
                {
                    Console.Write("[ ] ");
                    speakerId = speaker["msma"][0].Value;
                }
                Console.Write(speaker["minm"][0].ToString() + " ");
                Console.WriteLine("(" + speaker["msma"][0].ToInt64() + ")");
            }
            client.SetSpeakers(new byte[1] { 0 }, speakerId);
            speakers = client.GetSpeakers();
            foreach (DaapMessage speaker in speakers.Messages["mdcl"])
            {
                if (speaker["caia"] != null)
                    Console.Write("[X] ");
                else
                {
                    Console.Write("[ ] ");
                }
                Console.Write(speaker["minm"][0].ToString() + " ");
                Console.WriteLine("(" + speaker["msma"][0].ToInt64() + ")");
            }

            Console.ReadLine();
        }

        public Client(IService service)
            : this(service.Addresses[0])
        {
        }

        public Client(EndPoint ep)
            : base(true)
        {
            this.Host = new System.Net.IPEndPoint(ep.Addresses[0], ep.Port);
            StartTcp();
        }

        protected override ClientEventArgs<DacpRequest, DacpResponse> GetEventArgs(DacpResponse response)
        {
            return new DacpEventArgs(response);
        }

        public int SessionId { get; set; }

        public void Login()
        {
            DacpRequest request = new DacpRequest();
            request.Uri = "/login?hasFP=1&hsgid=00000000-066d-31e9-ed58-2b1c969b49c1";
            DacpResponse response = Send(request);
            if (response.Content.Name == "mlog")
            {
                SessionId = response.Content["mlid"][0].ToInt32();
                request = new DacpRequest();
                request.Uri = "/ctrl-int";
                response = Send(request);

            }
            else
                throw new NotSupportedException();
        }

        public DaapMessage GetSpeakers()
        {
            if (SessionId == 0)
                Login();
            DacpRequest request = new DacpRequest();
            request.Uri = "/ctrl-int/1/getspeakers?session-id=" + SessionId + "&hsgid=00000000-066d-31e9-ed58-2b1c969b49c1";
            DacpResponse response = Send(request);
            if (response.Content.Name == "casp")
            {
                return response.Content;
            }
            else
                throw new NotSupportedException();
        }

        public void SetSpeakers(params byte[][] ids)
        {
            DacpRequest request = new DacpRequest();
            StringBuilder uriBuilder = new StringBuilder("/ctrl-int/1/setspeakers?speaker-id=");
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i].Length == 1 && ids[i][0] == 0)
                    uriBuilder.Append("0");
                else
                    uriBuilder.AppendFormat("0x{0}", DaapMessage.ToHexString(ids[i]));
                if (i < ids.Length - 1)
                    uriBuilder.Append(",");
            }
            uriBuilder.Append("&session-id=");
            uriBuilder.Append(SessionId);
            uriBuilder.Append("&hsgid=00000000-066d-31e9-ed58-2b1c969b49c1");
            request.Uri = uriBuilder.ToString();
            Send(request);
        }
    }
}
