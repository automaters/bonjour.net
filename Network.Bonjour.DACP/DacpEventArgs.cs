using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Bonjour.DACP
{
    public class DacpEventArgs : ClientEventArgs<DacpRequest, DacpResponse>
    {
        public DacpEventArgs(DacpResponse response)
        {
            Response = response;
        }
    }
}
