using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Dns;
using System.Net;
using Network.ZeroConf;

namespace Network.Bonjour
{
    public class MDnsServer : DnsServer
    {
        public static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("224.0.0.251"), 5353);

        public MDnsServer(ushort port)
            : base(port)
        {

        }

        public MDnsServer(IPAddress address, ushort port)
            : base(address, port)
        {

        }

        public MDnsServer(IPEndPoint host)
            : base(host)
        {
        }



        public MDnsServer()
            : this(EndPoint)
        {
            this.Started += new EventHandler(MDnsServer_Started);
        }

        void MDnsServer_Started(object sender, EventArgs e)
        {
            if (this.IsUdp)
                server.MulticastLoopback = true;
        }

        public event ObjectEvent<Message> AnswerReceived;
        public event ObjectEvent<Message> QueryReceived;

        ushort requestId;

        public MDnsServer Resolve(string dname)
        {
            Message message = new Message();
            List<byte> guid = Guid.NewGuid().ToByteArray().Take(2).ToList();
            requestId = (ushort)(guid[0] * byte.MaxValue + guid[1]);
            message.ID = requestId;
            message.Questions.Add(new Question(dname));
            if (!IsStarted)
                StartUdp();
            Send(message, EndPoint);
            return this;
        }

        protected override void OnRequestReceived(RequestEventArgs<Message, Message> rea)
        {
            base.OnRequestReceived(rea);
            if ((rea.Request.ID == requestId && rea.Request.QueryResponse == Qr.Answer) || rea.Request.ID == 0)
            {
                if (AnswerReceived != null)
                    AnswerReceived(rea.Request.Clone());
            }
            if ((rea.Request.ID != requestId || rea.Request.ID == 0) && rea.Request.QueryResponse == Qr.Query)
            {
                this.requestId = 0;
                Message response = rea.Request.Clone();
                response.From = rea.Host;
                if (QueryReceived != null)
                    QueryReceived(response);
                if (response.AnswerEntries > 0)
                    rea.Response = response;
            }
        }
    }
}
