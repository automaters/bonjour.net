using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Rtsp
{
    public class RtspEventArgs : ClientEventArgs<RtspRequest, RtspResponse>
    {
        public RtspEventArgs(RtspRequest request)
        {
            Request = request;
        }

        public RtspEventArgs(RtspResponse response)
        {
            Response = response;
        }

        public RtspEventArgs()
        {
            Response = new RtspResponse();
        }
    }
}
