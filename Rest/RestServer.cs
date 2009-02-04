using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Network.Rest
{
    public abstract class RestServer
    {
        public ushort Port { get; private set; }

        public RestServer(ushort port)
        {
            Port = port;
        }

        public bool IsStarted { get; private set; }

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<RequestEventArgs> RequestReceived;

        public void Start()
        {
            OnStart();
            IsStarted = true;
        }
        public void Stop()
        {
            OnStop();
            IsStarted = false;
        }

        protected virtual void OnStart()
        {
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        protected virtual void OnStop()
        {
            if (Stopped != null)
                Stopped(this, EventArgs.Empty);
        }

        public void OnRequestReceived(RequestEventArgs rea)
        {
            if (RequestReceived != null)
                RequestReceived(this, rea);
        }
    }

    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs()
        {
            Response = new HttpResponse();
        }

        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }

        public IPEndPoint Host { get; set; }
    }
}
