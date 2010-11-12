using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Network.Rest
{
    public class CommandFactory : CommandFactory<HttpServerEventArgs, HttpRequest, HttpResponse>
    {
        protected override Command<HttpServerEventArgs, HttpRequest, HttpResponse> GetCommandInternal(HttpRequest request)
        {
            IDictionary<Regex, Type> possibleCommands = registeredCommands[request.Method];
            foreach (KeyValuePair<Regex, Type> command in possibleCommands)
            {
                if (command.Key.IsMatch(request.Uri))
                    return (Command<HttpServerEventArgs, HttpRequest, HttpResponse>)Activator.CreateInstance(command.Value);
            }
            throw new NotSupportedException("No command have been registered for this kind of request");
        }
    }
}
