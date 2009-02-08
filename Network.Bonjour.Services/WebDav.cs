using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Network.Bonjour.Pear
{
    [Export("_webdav._tcp._local.")]
    public class WebDav : Service
    {
        public WebDav() { }

        public WebDav(string userName, string password, string path)
        {
            this["u"] = userName;
            this["p"] = password;
            this["path"] = path;
        }
    }
}
