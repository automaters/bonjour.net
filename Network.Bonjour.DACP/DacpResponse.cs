using System;
using System.Collections.Generic;
using System.Text;
using Network.Rest;

namespace Network.Bonjour.DACP
{
    public class DacpResponse : HttpResponse<DacpResponse>
    {

        protected override void LoadContent()
        {
            if (ContentLength == 0)
                return;
            Content = new DaapMessage();
            Content.ReadFrom(Body);
        }

        public DaapMessage Content { get; set; }
    }
}
