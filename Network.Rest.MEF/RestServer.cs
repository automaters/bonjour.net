using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace Network.Rest.MEF
{
    public abstract class RestServer : Network.Rest.RestServer
    {
        public ComposablePartCatalog Catalog { get; set; }
        private CompositionBatch batch;

        public RestServer(ushort port, Assembly assemblyContainingCommands)
            : this(port)
        {
            Catalog = new AssemblyCatalog(assemblyContainingCommands);
        }

        public RestServer(ushort port, string pathContainingCommands)
            : this(port)
        {
            Catalog = new DirectoryCatalog(pathContainingCommands, true);
        }

        public RestServer(ushort port)
            : base(port)
        {
            this.HttpRequestReceived += new EventHandler<HttpServerEventArgs>(RestServer_RequestReceived);
            batch = new CompositionBatch();
            batch.AddPart(this);
        }

        protected override void OnStart()
        {
            CompositionContainer container = new CompositionContainer(Catalog);
            container.Compose(batch);
            base.OnStart();
        }

        void RestServer_RequestReceived(object sender, HttpServerEventArgs e)
        {
            Command c = commands.Where(command => command.Metadata["Method"].ToString() == e.Request.Method && command.Metadata.ContainsKey("UriConstraint") ? Regex.IsMatch(e.Request.Uri, command.Metadata["UriConstraint"].ToString()) : true).Single().GetExportedObject();
            c.Execute(e);
        }

        [Import]
        [ImportRequiredMetadata("Method")]
        protected ExportCollection<Command> commands;


    }
}
