using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Network.Rest
{
    public abstract class CommandFactory<EventArgs, RequestType, ResponseType>
        where EventArgs : RequestEventArgs<RequestType, ResponseType>
        where ResponseType : IResponse
        where RequestType : IRequest<RequestType>, new()
    {
        protected IDictionary<string, IDictionary<Regex, Type>> registeredCommands = new Dictionary<string, IDictionary<Regex, Type>>();

        public void RegisterCommand<T>()
            where T : Command<EventArgs, RequestType, ResponseType>
        {
            foreach (CommandDescriptorAttribute attribute in typeof(T).GetCustomAttributes(typeof(CommandDescriptorAttribute), true))
            {
                IDictionary<Regex, Type> dict;
                if (!registeredCommands.ContainsKey(attribute.Method))
                {
                    dict = new Dictionary<Regex, Type>();
                    registeredCommands.Add(attribute.Method, dict);
                }
                else
                    dict = registeredCommands[attribute.Method];
                if (string.IsNullOrEmpty(attribute.UriRegex))
                    dict.Add(new Regex("^/.*"), typeof(T));
            }
        }

        public Command<EventArgs, RequestType, ResponseType> GetCommand(RequestType request, IServiceProvider server)
        {
            Command<EventArgs, RequestType, ResponseType> cmd = GetCommandInternal(request);
            return cmd.Initialize(request, server);
            //return cmd;
        }

        protected internal abstract Command<EventArgs, RequestType, ResponseType> GetCommandInternal(RequestType request);

        public T GetCommand<T>(RequestType request, IServiceProvider server)
            where T : Command<RequestEventArgs<RequestType, ResponseType>, RequestType, ResponseType>
        {
            return GetCommand(request, server) as T;
        }
    }
}
