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
            this.IsStateLess = true;
        }

        public RestServer(ushort port)
            : base(port)
        {
            this.IsStateLess = true;
        }

        public RestServer(IPAddress host, ushort port)
            : base(host, port)
        {
            this.IsStateLess = true;
        }

        public RestServer(IPEndPoint host)
            : base(host)
        {
            this.IsStateLess = true;
        }

        public event EventHandler<HttpServerEventArgs> HttpRequestReceived;

        protected override void OnRequestReceived(RequestEventArgs<HttpRequest, HttpResponse> rea)
        {
            base.OnRequestReceived(rea);
            if (HttpRequestReceived != null)
                HttpRequestReceived(this, (HttpServerEventArgs)rea);
        }

        protected override RequestEventArgs<HttpRequest, HttpResponse> GetEventArgs(HttpRequest request)
        {
            return new HttpServerEventArgs(request) { Response = new HttpResponse() };
        }

        protected override void Treat(RequestEventArgs<HttpRequest, HttpResponse> rea, Stream client)
        {
            try
            {
                base.Treat(rea, client);
                if (rea.Response.Headers.ContainsKey("Connection") && rea.Response.Headers["Connection"] == "Keep-Alive")
                {
                    Treat(GetEventArgs(rea.Request), client);
                }
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

        private void ReplyError(Stream ep, Exception e, HttpStatusCode code)
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
