using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Dns;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Network.ZeroConf;

namespace Network.Bonjour
{
    class MDnsClient
    {
        public static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("224.0.0.251"), 5353);

        private UdpClient client;
        private IPEndPoint local;
        //private int lockTimes = 0;
        private short requestId;

        public MDnsClient(IPEndPoint endpoint)
        {
            local = endpoint;
            client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            client.Client.Bind(endpoint);
            //lockTimes++;
            //active.WaitOne();
            receiver = new Thread(StartReceiving);
        }

        public bool IsStarted { get; set; }

        public event ObjectEvent<Message> AnswerReceived;
        public event ObjectEvent<Message> QueryReceived;

        public void Send(Message message, IPEndPoint ep)
        {
            byte[] byteMessage = message.ToByteArray();
            client.Send(byteMessage, byteMessage.Length, ep);
        }

        public void Resolve(string protocol)
        {
            Message message = new Message();
            List<byte> guid = Guid.NewGuid().ToByteArray().Take(2).ToList();
            requestId = (short)(guid[0] * byte.MaxValue + guid[1]);
            message.ID = requestId;
            message.Questions.Add(new Question(protocol));
            Send(message, EndPoint);
        }

        public static MDnsClient CreateAndResolve(string protocol)
        {
            MDnsClient client = new MDnsClient(new IPEndPoint(IPAddress.Any, 5353));
            client.client.MulticastLoopback = true;
            //client.client.MulticastLoopback = true;
            client.client.JoinMulticastGroup(EndPoint.Address, 10);
            client.Resolve(protocol);
            return client;
        }

        private AutoResetEvent active = new AutoResetEvent(false);

        private void StartReceiving()
        {
            while (IsStarted)
            {
                client.BeginReceive(StartReceiving, null);
                active.WaitOne();
            }
        }

        private void StartReceiving(IAsyncResult result)
        {
            result.AsyncWaitHandle.WaitOne();
            IPEndPoint src = new IPEndPoint(IPAddress.Any, 0);
            byte[] response = client.EndReceive(result, ref src);
            Treat(response, src);
        }

        protected void Treat(byte[] bytes, IPEndPoint from)
        {
            Message m = Message.FromBytes(bytes);
            m.From = from;
            short requestId = this.requestId;
            active.Set();
            if ((m.ID == requestId && m.QueryResponse == Qr.Answer) || m.ID == 0)
            {
                if (AnswerReceived != null)
                    AnswerReceived(m);
            }
            if ((m.ID != requestId && m.QueryResponse == Qr.Query) || m.ID == 0)
            {
                this.requestId = 0;
                if (QueryReceived != null)
                    QueryReceived(m);
            }

        }

        public void Stop()
        {
            //active.WaitOne();
            //lockTimes++;
            IsStarted = false;
            active.Set();
            //try
            //{
            //    client.Close();
            //    receiver.Abort();
            //}
            //catch (ThreadAbortException)
            //{
            //}
        }
        Thread receiver;

        public void Start()
        {
            //while (lockTimes > 0)
            //{
            //    lockTimes--;
            //    active.ReleaseMutex();
            //}
            if (!IsStarted)
            {
                IsStarted = true;
                receiver.Start();
            }
        }
    }
}
