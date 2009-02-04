using System;
using System.Collections.Generic;
using System.Text;
using Network.ZeroConf;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace MEF.Bonjour.Tests
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        [Import("_touch-able._tcp")]
        private IService service;

        public void Run()
        {
            using (Catalog catalog = new Catalog())
            {
                CompositionContainer container = new CompositionContainer(catalog);
                CompositionBatch batch = new CompositionBatch();
                batch.AddPart(this);
                container.Compose(batch);
                Console.WriteLine(service.Name);
                //foreach (IService service in services)
                //    Console.WriteLine(service.Name);
            }
        }
    }
}
