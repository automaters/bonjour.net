using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.Rest;

namespace Network.UPnP.DLNA
{
    public abstract class Command : Network.Rest.Command
    {
        public Command(string connectionString) : base(connectionString) { }

        protected override Network.Rest.HttpRequest BuildRequest()
        {
            HttpRequest request = base.BuildRequest();
            request.Headers["User-Agent"] = "Mozilla/4.0 (compatible; UPnP/1.0; Windows 9x)";
            request.Headers["Content-Type"] = "text/xml; charset=\"utf-8\"";
            request.Headers["Connection"] = "Close";
            request.Headers["Cache-Control"] = "no-cache";
            request.Headers["Pragma"] = "no-cache";
            return request;
        }
    }
}
