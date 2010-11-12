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
            Client client = new Client(new EndPoint() { Port = 3689, Addresses = { System.Net.IPAddress.Parse("192.168.68.180") } });
            client.Login();
            Console.WriteLine(client.SessionId);
            client.GetSpeakers();
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
            request.Uri = "http://" + Host + "/login?hasFP=1&hsgid=00000000-066d-31e9-ed58-2b1c969b49c1";
            DacpResponse response = Send(request);
            if (response.Content.Name == "mlog")
            {
                SessionId = response.Content["mlid"].ToInt32();
            }
            else
                throw new NotSupportedException();
        }

        public DaapMessage GetSpeakers()
        {
            if (SessionId == 0)
                Login();
            DacpRequest request = new DacpRequest();
            request.Uri = "http://" + Host + "/ctrl-int/1/getspeakers?session-id=" + SessionId + "&hsgid=00000000-066d-31e9-ed58-2b1c969b49c1";
            DacpResponse response = Send(request);
            if (response.Content.Name == "casp")
            {
                return response.Content;
            }
            else
                throw new NotSupportedException();
        }
    }
}
