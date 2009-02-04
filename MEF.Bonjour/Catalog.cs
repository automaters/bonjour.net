using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using Network.Bonjour;
using Network.ZeroConf;
using System.Linq.Expressions;

namespace MEF.Bonjour
{
    public class Catalog : ComposablePartCatalog
    {
        public TimeSpan TimeOut { get; set; }

        BonjourServiceResolver resolver = new BonjourServiceResolver();

        public Catalog(TimeSpan timeOut)
        {
            resolver.ServiceFound += new Network.ZeroConf.ObjectEvent<Network.ZeroConf.IService>(resolver_ServiceFound);
            resolver.ServiceRemoved += new Network.ZeroConf.ObjectEvent<Network.ZeroConf.IService>(resolver_ServiceRemoved);
            TimeOut = timeOut;
        }

        public Catalog()
            : this(new TimeSpan(0, 0, 30))
        {
        }

        private IDictionary<IService, ServicePartDefinition> parts = new Dictionary<IService, ServicePartDefinition>();

        void resolver_ServiceRemoved(Network.ZeroConf.IService item)
        {
            if (parts.ContainsKey(item))
                parts.Remove(item);
        }

        void resolver_ServiceFound(Network.ZeroConf.IService item)
        {
            if (!parts.ContainsKey(item))
                parts.Add(item, new ServicePartDefinition(item));
        }

        public override IEnumerable<TempTuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            LambdaExpression lambda = definition.Constraint;
            if (lambda.Body is BinaryExpression)
            {
                BinaryExpression binaryExpression = (BinaryExpression)lambda.Body;
                if (binaryExpression.Left is MemberExpression && ((MemberExpression)binaryExpression.Left).Member.Name == "ContractName")
                {
                    IList<IService> services = resolver.Resolve((string)((ConstantExpression)binaryExpression.Right).Value, TimeOut, definition.Cardinality == ImportCardinality.ZeroOrOne ? 0 : 1, definition.Cardinality == ImportCardinality.ZeroOrMore ? int.MaxValue : 1);
                    foreach (IService service in services)
                        resolver_ServiceFound(service);
                }
            }
            var exports = new List<TempTuple<ComposablePartDefinition, ExportDefinition>>();
            foreach (var part in this.Parts.ToArray())
            {
                foreach (var export in part.ExportDefinitions)
                {
                    if (definition.Constraint.Compile().Invoke(export))
                    {
                        exports.Add(new TempTuple<ComposablePartDefinition, ExportDefinition>(part, export));
                    }
                }
            }
            return exports;
        }

        public override System.Linq.IQueryable<ComposablePartDefinition> Parts
        {
            get { return parts.Values.AsQueryable().OfType<ComposablePartDefinition>(); }
        }

        protected override void Dispose(bool disposing)
        {
            resolver.ServiceFound -= resolver_ServiceFound;
            resolver.ServiceRemoved -= resolver_ServiceRemoved;
            if (disposing)
                resolver.Dispose();
            base.Dispose(disposing);
        }
    }
}
