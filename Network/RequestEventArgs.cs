using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Network
{
    public class RequestEventArgs : RequestEventArgs<BinaryRequest, BinaryResponse>
    {
    }

    public class RequestEventArgs<RequestType, ResponseType> : EventArgs
        where ResponseType : IResponse<ResponseType>
    {
        public RequestEventArgs()
        {
            //Response = new HttpResponse();
        }

        public RequestType Request { get; set; }
        public ResponseType Response { get; set; }

        public virtual IPEndPoint Host { get; set; }
    }
}
