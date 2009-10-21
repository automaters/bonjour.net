using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Rest
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandDescriptorAttribute : Attribute
    {
        public string Method { get; set; }
        public string UriRegex { get; set; }

        public CommandDescriptorAttribute(string method)
            : base()
        {
            Method = method;
        }
    }
}
