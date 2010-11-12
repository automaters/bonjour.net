using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Rtsp
{
    public class RtspServer : Server<RtspRequest, RtspResponse>
    {
        public RtspServer(ushort port)
            : base(port)
        {

        }



        protected override RequestEventArgs<RtspRequest, RtspResponse> GetEventArgs(RtspRequest request)
        {
            return new RtspEventArgs(request);
        }
    }
}
