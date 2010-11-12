using System;
using System.Collections.Generic;
using System.Text;
using Network.Rest;

namespace Network.Rtsp
{
    public class RtspResponse : HttpResponse, IClientResponse<RtspResponse>
    {
        #region IResponse<RtspResponse> Members

        public new RtspResponse GetResponse(System.IO.BinaryReader stream)
        {
            return base.GetResponse(stream) as RtspResponse;
        }

        RtspResponse IClientResponse<RtspResponse>.GetResponse(byte[] requestBytes)
        {
            return null;
        }

        #endregion
    }
}
