using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using Network.Bonjour;
using Network.ZeroConf;

namespace MEF.Bonjour
{
    public class ServicePart : ComposablePart
    {
        IService service;

        public ServicePart(IService service)
        {
            this.service = service;
        }


        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return ExportDefinition(service); }
        }

        internal static IEnumerable<ExportDefinition> ExportDefinition(IService service)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> property in service.Properties)
                properties.Add(property.Key, property.Value);

            if (service.Protocol.EndsWith("."))
                service.Protocol = service.Protocol.Substring(0, service.Protocol.Length - 1);
            if (service.Protocol.EndsWith(".local"))
                service.Protocol = service.Protocol.Substring(0, service.Protocol.Length - 6);
            string[] protocols = new string[] { service.Protocol, service.Protocol + ".", service.Protocol + ".local", service.Protocol + ".local." };
            foreach (string protocol in protocols)
            {
                ExportDefinition ed = new ExportDefinition(protocol, properties);
                yield return ed;
            }
        }

        public override object GetExportedObject(ExportDefinition definition)
        {
            return service;
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return new ImportDefinition[] { }; }
        }

        public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
        {

        }
    }
}
