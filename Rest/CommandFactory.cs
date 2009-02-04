using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Network.Rest
{
    public class CommandFactory
    {
        private IDictionary<string, IDictionary<Regex, Type>> registeredCommands = new Dictionary<string, IDictionary<Regex, Type>>();

        public void RegisterCommand<T>()
            where T : Command
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

        public Command GetCommand(HttpRequest request)
        {
            Command cmd = GetCommandInternal(request);
            cmd.Initialize(request);
            return cmd;
        }

        internal Command GetCommandInternal(HttpRequest request)
        {
            IDictionary<Regex, Type> possibleCommands = registeredCommands[request.Method];
            foreach (KeyValuePair<Regex, Type> command in possibleCommands)
            {
                if (command.Key.IsMatch(request.Uri))
                    return (Command)Activator.CreateInstance(command.Value, request);
            }
            throw new NotSupportedException("No command have been registered for this kind of request");
        }

        public T GetCommand<T>(HttpRequest request)
            where T : Command
        {
            return GetCommand(request) as T;
        }
    }
}
