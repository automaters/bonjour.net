using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Network.Rest
{
    public class UdpRestServer : RestServer
    {
        public UdpClient client;

        public UdpRestServer(ushort port)
            : base(port)
        {
            client = new UdpClient(port);
        }

        public UdpRestServer(IPAddress hostAddress, ushort port)
            : base(port)
        {
            if (!IsMulticast(hostAddress))
                client = new UdpClient(new IPEndPoint(hostAddress, port));
            else
            {
                client = new UdpClient(port);
                client.JoinMulticastGroup(hostAddress);
            }
        }

        private bool IsMulticast(IPAddress hostAddress)
        {
            if (hostAddress.IsIPv6Multicast)
                return true;
            byte[] addressBytes = hostAddress.GetAddressBytes();
            if (addressBytes[0] >= 224 && addressBytes[0] <= 239)
                return true;
            return false;
        }

        protected override void OnStart()
        {
            client.BeginReceive(ReceiveRequest, null);
            base.OnStart();
        }

        private void ReceiveRequest(IAsyncResult result)
        {
            RequestEventArgs rea = new RequestEventArgs();
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, Port);
            byte[] requestBytes = client.EndReceive(result, ref ep);
            if (IsStarted)
                client.BeginReceive(ReceiveRequest, null);
            HttpRequest request = HttpRequest.FromBytes(requestBytes);
            rea.Request = request;
            rea.Host = ep;
            try
            {
                OnRequestReceived(rea);
                Reply(ep, rea.Response);
            }
            catch (NotImplementedException e)
            {
                ReplyError(ep, e, HttpStatusCode.NotImplemented);
            }
            catch (Exception e)
            {
                ReplyError(ep, e, HttpStatusCode.BadRequest);
            }
        }

        private void ReplyError(IPEndPoint ep, Exception e, HttpStatusCode code)
        {
            HttpResponse response = new HttpResponse();
            StreamWriter sw = new StreamWriter(response.Body);
            sw.Write(e.ToString());
            response.ResponseCode = HttpStatusCode.NotImplemented;
            response.ResponseMessage = e.Message;
            Reply(ep, response);
        }

        private void Reply(IPEndPoint ep, HttpResponse response)
        {
            byte[] responseBytes = response.GetBytes();
            client.Send(responseBytes, responseBytes.Length, ep);
        }
    }
}
