using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Rest
{
    public class HttpServerEventArgs : ServerEventArgs<HttpRequest, HttpResponse>
    {
        public HttpServerEventArgs()
            : this(new HttpResponse())
        {
        }

        public HttpServerEventArgs(HttpResponse response)
        {
            Response = response;
        }

        public HttpServerEventArgs(HttpRequest request)
        {
            Request = request;
        }
    }

    public class HttpClientEventArgs : ClientEventArgs<HttpRequest, HttpResponse>
    {
        public HttpClientEventArgs()
            : this(new HttpResponse())
        {
        }

        public HttpClientEventArgs(HttpResponse response)
        {
            Response = response;
        }

        public HttpClientEventArgs(HttpRequest request)
        {
            Request = request;
        }
    }
}
