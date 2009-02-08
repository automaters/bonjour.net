using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.ZeroConf;
using System.Threading;
using System.ComponentModel.Composition;

namespace Network.Bonjour.Services.Pear
{
    [Export("_adisk._tcp._local.")]
    public class AirDiskService : Bonjour.Service
    {
        public AirDiskService() { }

        public AirDiskService(string diskName, string sys, params string[] dk)
        {
            HostName = Environment.MachineName + ".local.";
            Name = diskName;
            Protocol = "_adisk._tcp._local.";
            this.addresses.Add(ResolverHelper.GetEndPoint());
            this["sys"] = sys;
            for (int i = 0; i < dk.Length; i++)
                this["dk" + i] = dk[i];
        }

        public static void Main()
        {
            IService s = new AirDiskService("toto", string.Format("waMA={0}", ResolverHelper.GetMacAddresses().First().ToString()), string.Format("adVF=0xa1,adVN=toto,adVU={0}", Guid.NewGuid()));
            s.Publish();
            Thread.Sleep(10000);
            s.Stop();
        }
    }
}
