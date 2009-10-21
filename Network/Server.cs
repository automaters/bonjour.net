using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Collections;

namespace Network
{
    public abstract class Server<TRequest, TResponse> : IServiceProvider
        where TResponse : IResponse
        where TRequest : IRequest<TRequest>, new()
    {
        public Server(ushort port)
            : this(IPAddress.Any, port)
        {

        }

        public Server(IPAddress address, ushort port)
            : this(new IPEndPoint(address, port))
        {

        }

        public Server(IPEndPoint host)
        {
            Host = host;
        }

        public IPEndPoint Host { get; protected set; }

        protected TcpListener tcp;
        protected UdpClient udp;

        public bool IsTcp { get; set; }
        public bool IsUdp { get; set; }
        public bool IsStarted { get; set; }

        #region Start

        public void StartTcp()
        {
            tcp = new TcpListener(Host);
            tcp.Start();
            tcp.BeginAcceptTcpClient(ReceiveTcpRequest, null);
            OnStart();
            IsStarted = true;
        }

        public void StartUdp()
        {
            if (IsStarted)
                throw new NotSupportedException("You cannot start the server twice");
            if (!IsMulticast(Host.Address))
                udp = new UdpClient(Host);
            else
            {
                udp = new UdpClient(Host.Port);
                udp.JoinMulticastGroup(Host.Address, 5);
                udp.BeginReceive(ReceiveUdpRequest, null);
                OnStart();
                IsStarted = true;
            }
        }

        #endregion

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<RequestEventArgs<TRequest, TResponse>> RequestReceived;

        protected abstract RequestEventArgs<TRequest, TResponse> GetEventArgs(TRequest request);

        private void ReceiveTcpRequest(IAsyncResult result)
        {

            TcpClient tcpClient = tcp.EndAcceptTcpClient(result);
            if (IsStarted)
                tcp.BeginAcceptTcpClient(ReceiveTcpRequest, null);
            RequestEventArgs<TRequest, TResponse> rea = GetEventArgs(new TRequest().GetRequest(new BinaryReader(tcpClient.GetStream())));
            rea.Host = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            TreatTcp(rea, tcpClient);
        }

        protected virtual void TreatTcp(RequestEventArgs<TRequest, TResponse> rea, TcpClient tcpClient)
        {
            OnRequestReceived(rea);
            if (rea.Response != null)
                Send(rea.Response, tcpClient);
        }

        protected virtual void TreatUdp(RequestEventArgs<TRequest, TResponse> rea, IPEndPoint client)
        {
            OnRequestReceived(rea);
            if (rea.Response != null)
                Send(rea.Response, client);
        }

        private void ReceiveUdpRequest(IAsyncResult result)
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            byte[] requestBytes;
            try
            {
                requestBytes = udp.EndReceive(result, ref remote);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == (int)SocketError.ConnectionAborted || e.ErrorCode == (int)SocketError.ConnectionReset)
                    return;
                throw e;
            }
            finally
            {
                if (IsStarted)
                    udp.BeginReceive(ReceiveUdpRequest, null);
            }
            if (requestBytes == null)
                return;
            RequestEventArgs<TRequest, TResponse> rea = GetEventArgs(new TRequest().GetRequest(requestBytes));
            rea.Host = remote;
            TreatUdp(rea, remote);
        }

        protected void Send(TResponse response, TcpClient tcpClient)
        {
            response.WriteTo(new BinaryWriter(tcpClient.GetStream()));
        }

        protected void Send(TResponse response, IPEndPoint remote)
        {
            byte[] responseBytes = response.GetBytes();
            udp.Send(responseBytes, responseBytes.Length, remote);
        }

        public void Stop()
        {
            if (IsStarted)
            {
                if (IsTcp)
                    tcp.Stop();
                if (IsUdp)
                    udp.Close();
                OnStop();
                IsStarted = false;
            }
        }

        public void Restart()
        {
            Stop();
            if (IsTcp)
                StartTcp();
            if (IsUdp)
                StartUdp();
        }

        protected virtual void OnStart()
        {
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        protected virtual void OnStop()
        {
            if (Stopped != null)
                Stopped(this, EventArgs.Empty);
        }

        protected virtual void OnRequestReceived(RequestEventArgs<TRequest, TResponse> rea)
        {
            if (RequestReceived != null)
                RequestReceived(this, rea);
        }

        protected bool IsMulticast(IPAddress hostAddress)
        {
            if (hostAddress.IsIPv6Multicast)
                return true;
            byte[] addressBytes = hostAddress.GetAddressBytes();
            if (addressBytes[0] >= 224 && addressBytes[0] <= 239)
                return true;
            return false;
        }

        protected IDictionary<Type, object> services = new Dictionary<Type, object>();

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return services[serviceType];
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        #endregion
    }
}
