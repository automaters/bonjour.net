using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace Network.Rest
{
    public class RestServer : Server<HttpRequest, HttpResponse>
    {
        public RestServer()
            : this(80)
        {

        }

        public RestServer(ushort port)
            : base(port)
        {
        }

        public RestServer(IPAddress host, ushort port)
            : base(host, port)
        {
        }

        public RestServer(IPEndPoint host)
            : base(host)
        {
        }

        public event EventHandler<HttpRequestEventArgs> HttpRequestReceived;

        protected override void OnRequestReceived(RequestEventArgs<HttpRequest, HttpResponse> rea)
        {
            base.OnRequestReceived(rea);
            if (HttpRequestReceived != null)
                HttpRequestReceived(this, (HttpRequestEventArgs)rea);
        }

        protected override RequestEventArgs<HttpRequest, HttpResponse> GetEventArgs(HttpRequest request)
        {
            return new HttpRequestEventArgs() { Request = request };
        }

        protected override void TreatTcp(RequestEventArgs<HttpRequest, HttpResponse> rea, System.Net.Sockets.TcpClient tcpClient)
        {
            try
            {
                base.TreatTcp(rea, tcpClient);
            }
            catch (NotImplementedException e)
            {
                ReplyError(tcpClient, e, HttpStatusCode.NotImplemented);
            }
            catch (Exception e)
            {
                ReplyError(tcpClient, e, HttpStatusCode.BadRequest);
            }
        }

        protected override void TreatUdp(RequestEventArgs<HttpRequest, HttpResponse> rea, System.Net.IPEndPoint client)
        {
            try
            {
                base.TreatUdp(rea, client);
            }
            catch (NotImplementedException e)
            {
                ReplyError(client, e, HttpStatusCode.NotImplemented);
            }
            catch (Exception e)
            {
                ReplyError(client, e, HttpStatusCode.BadRequest);
            }
        }

        private void ReplyError(TcpClient ep, Exception e, HttpStatusCode code)
        {
            HttpResponse response = new HttpResponse();
            StreamWriter sw = new StreamWriter(response.Body);
            sw.Write(e.ToString());
            response.ResponseCode = HttpStatusCode.NotImplemented;
            response.ResponseMessage = e.Message;
            Send(response, ep);
        }

        private void ReplyError(IPEndPoint ep, Exception e, HttpStatusCode code)
        {
            HttpResponse response = new HttpResponse();
            StreamWriter sw = new StreamWriter(response.Body);
            sw.Write(e.ToString());
            response.ResponseCode = HttpStatusCode.NotImplemented;
            response.ResponseMessage = e.Message;
            Send(response, ep);
        }
    }
}
