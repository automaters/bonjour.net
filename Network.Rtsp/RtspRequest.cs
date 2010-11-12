using System;
using System.Collections.Generic;
using System.Text;
using Network.Rest;

namespace Network.Rtsp
{
    public class RtspRequest : HttpRequest, IServerRequest<RtspRequest>
    {
        public RtspRequest()
        {
            Protocol = "RTSP/1.0";
        }

        #region IRequest<RtspRequest> Members

        public new RtspRequest GetRequest(System.IO.BinaryReader stream)
        {
            return base.GetRequest(stream) as RtspRequest;
        }

        RtspRequest IServerRequest<RtspRequest>.GetRequest(byte[] requestBytes)
        {
            return null;
        }

        #endregion
    }
}
