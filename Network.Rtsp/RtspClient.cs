using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Network.Rtsp
{
    public class RtspClient : Client<RtspRequest, RtspResponse>
    {
        public RtspClient(string userAgent)
            : base(true)
        {
            this.UserAgent = userAgent;
        }

        protected override ClientEventArgs<RtspRequest, RtspResponse> GetEventArgs(RtspResponse request)
        {
            return new RtspEventArgs(request);
        }

        public string UserAgent { get; set; }
    }
}
