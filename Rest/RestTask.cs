using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Net;

namespace Network.Rest
{
    public class RestTask : Task
    {
        public RestTask() { }

        [Required]
        public string ConnectionString { get; set; }

        public string Command { get; set; }

        protected HttpResponse ExecuteRequest()
        {
            Command command = (Command)Activator.CreateInstance(Type.GetType(Command), ConnectionString);
            BuildCommand(command);
            return command.GetHttpResponse();
        }

        protected virtual void BuildCommand(Command command)
        {
        }

        public override bool Execute()
        {
            //return ExecuteRequest().ResponseCode == System.Net.HttpStatusCode.OK;
            try
            {
                ExecuteRequest();

                return true;
            }
            catch { return false; }
        }
    }
}
