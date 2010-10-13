using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Rest
{
    public class HttpRequestEventArgs : RequestEventArgs<HttpRequest, HttpResponse>
    {
        public HttpRequestEventArgs()
            : this(new HttpResponse())
        {
        }

        public HttpRequestEventArgs(HttpResponse response)
        {
            Response = response;
        }

        public HttpRequestEventArgs(HttpRequest request)
        {
            Request = request;
        }
    }
}
