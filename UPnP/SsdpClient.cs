using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Dns;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Network.ZeroConf;
using System.Xml;
using Network.Rest;
using Network.UPnP;

namespace Network.UPnP
{
    class SsdpClient : Client<HttpRequest, HttpResponse>
    {
        public static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);

        private IPEndPoint local;

        public SsdpClient(ushort port)
            : base(false)
        {
            StartUdp(EndPoint.Address, port);

            //local = endpoint;
            //client = new UdpClient();
            //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            //client.Client.Bind(endpoint);
            ////lockTimes++;
            ////active.WaitOne();
            //receiver = new Thread(StartReceiving);
        }

        public event ObjectEvent<HttpRequest> QueryReceived;
        public event ObjectEvent<HttpResponse> AnswerReceived;

        public void Resolve(string protocol)
        {
            IsUdp = true;
            HttpRequest request = new HttpRequest();
            request.Method = "M-SEARCH";
            request.Host = EndPoint.ToString();
            request.Headers.Add("ST", protocol);
            request.Headers.Add("MAN", "\"ssdp:discover\"");
            request.Headers.Add("MX", "3");
            SendOneWay(request, EndPoint);
        }

        public static SsdpClient CreateAndResolve(string protocol)
        {
            SsdpClient client = new SsdpClient(0);
            //client.client.MulticastLoopback = true;
            //client.client.MulticastLoopback = true;
            //client.client.JoinMulticastGroup(EndPoint.Address, 255);
            client.Resolve(protocol);
            return client;
        }

        //private AutoResetEvent active = new AutoResetEvent(false);

        //private void StartReceiving()
        //{
        //    while (IsStarted)
        //    {
        //        client.BeginReceive(StartReceiving, null);
        //        active.WaitOne();
        //    }
        //}

        //private void StartReceiving(IAsyncResult result)
        //{
        //    result.AsyncWaitHandle.WaitOne();
        //    IPEndPoint src = new IPEndPoint(IPAddress.Any, 0);
        //    byte[] response = client.EndReceive(result, ref src);
        //    active.Set();
        //    if (src == EndPoint)
        //        TreatQuery(response);
        //    else
        //        TreatAnswer(response);
        //}

        protected void TreatQuery(HttpRequest request)
        {
            if (QueryReceived != null)
                QueryReceived(request);
        }

        protected void TreatAnswer(HttpResponse response)
        {
            if (AnswerReceived != null)
                AnswerReceived(response);
        }

        //public void Stop()
        //{
        //    //active.WaitOne();
        //    //lockTimes++;
        //    IsStarted = false;
        //    active.Set();
        //    //try
        //    //{
        //    //    client.Close();
        //    //    receiver.Abort();
        //    //}
        //    //catch (ThreadAbortException)
        //    //{
        //    //}
        //}
        //Thread receiver;

        //public void Start()
        //{
        //    //while (lockTimes > 0)
        //    //{
        //    //    lockTimes--;
        //    //    active.ReleaseMutex();
        //    //}
        //    if (!IsStarted)
        //    {
        //        IsStarted = true;
        //        receiver.Start();
        //    }
        //}

        //protected override RequestEventArgs<HttpRequest, HttpResponse> GetEventArgs(HttpRequest request)
        //{
        //    return new HttpRequestEventArgs(request);
        //}

        protected override ClientEventArgs<HttpRequest, HttpResponse> GetEventArgs(HttpResponse response)
        {
            return new HttpClientEventArgs(response);
        }
    }
}
