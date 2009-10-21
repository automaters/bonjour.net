using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Rest
{
    public class HttpRequestEventArgs : RequestEventArgs<HttpRequest, HttpResponse>
    {
        public HttpRequestEventArgs()
        {
            Response = new HttpResponse();
        }
    }
}
