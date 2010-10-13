using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.UPnP
{
    public class ServiceAction
    {

        private List<ServiceActionArgument> arguments;
        public ServiceAction(string actionName, string serviceType)
        {
            arguments = new List<ServiceActionArgument>();
            Name = actionName;
            ServiceType = serviceType;
        }

        internal void Add(ServiceActionArgument serviceActionArgument)
        {
            arguments.Add(serviceActionArgument);
        }

        public string Name { get; set; }

        public string ServiceType { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append('(');
            bool isFirst = true;
            foreach (ServiceActionArgument argument in arguments)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(", ");
                sb.Append(argument.Direction);
                sb.Append(' ');
                sb.Append(argument.Type);
                sb.Append(' ');
                sb.Append(argument.Name);
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
}
