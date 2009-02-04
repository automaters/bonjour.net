using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using Network.ZeroConf;

namespace MEF.Bonjour
{
    public class ServicePartDefinition : ComposablePartDefinition
    {
        IService service;

        public ServicePartDefinition(IService service)
        {
            this.service = service;
        }

        public override ComposablePart CreatePart()
        {
            return new ServicePart(service);
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return ServicePart.ExportDefinition(service); }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return null; }
        }
    }
}
