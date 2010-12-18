using System;
using System.Collections.Generic;
using System.Text;
using Network.Rest;

namespace Network.Bonjour.DACP
{
    public class DacpRequest : HttpRequest
    {
        public DacpRequest()
        {
            Headers["Viewer-Only-Client"] = "1";
        }
    }
}
