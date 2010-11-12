using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Collections;

namespace Network
{
    public abstract class Server : IServiceProvider
    {
        public Server(IPEndPoint host)
        {
            Host = host;
        }

        public IPEndPoint Host { get; protected set; }

        //protected TcpListener tcp;
        //protected UdpClient udp;
        protected Socket server;
        public bool IsTcp { get; set; }
        public bool IsUdp { get; set; }
        public bool IsStarted { get; set; }

        #region Start

        [SocketPermission(System.Security.Permissions.SecurityAction.Assert)]
        private void Start()
        {
            if (IsStarted)
                throw new NotSupportedException("You cannot start the server twice");
            if (IsTcp)
                server = new Socket(Host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (IsUdp)
                server = new Socket(Host.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            if (!IsMulticast(Host.Address))
            {
                server.Bind(Host);
            }
            else
            {
                server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                server.Bind(new IPEndPoint(IPAddress.Any, Host.Port));
                JoinMulticastGroup(Host.Address, 5);
            }
            if (IsTcp)
                server.Listen(10);
            OnStart();
            IsStarted = true;
            if (IsTcp)
                server.BeginAccept(ReceiveRequest, null);
            if (IsUdp)
            {
                byte[] buffer = new byte[65536];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0); ;
                server.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remote, ReceiveRequest, buffer);
            }
        }

        public void StartTcp()
        {
            IsTcp = true;
            IsUdp = false;
            Start();
        }

        public void StartUdp()
        {
            IsTcp = false;
            IsUdp = true;
            Start();
        }

        private void JoinMulticastGroup(IPAddress multicastAddr, byte timeToLive)
        {
            MulticastOption optionValue = new MulticastOption(multicastAddr);
            server.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, optionValue);
            server.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, timeToLive);
        }

        #endregion

        public event EventHandler Started;
        public event EventHandler Stopped;

        private void ReceiveRequest(IAsyncResult result)
        {
            byte[] buffer = null;
            try
            {
                if (IsTcp)
                {
                    Socket client = server.EndAccept(result);

                    if (IsStarted)
                        server.BeginAccept(ReceiveRequest, null);

                    Treat(new NetworkStream(client), client.RemoteEndPoint as IPEndPoint);
                    if (IsStateLess)
                    {
                        client.Shutdown(SocketShutdown.Send);
                        client.Close();
                    }
                }
                if (IsUdp)
                {
                    EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    int length = server.EndReceiveFrom(result, ref remote);
                    EndPoint client = remote;
                    buffer = result.AsyncState as byte[];
                    byte[] bytes = new byte[length];
                    Buffer.BlockCopy(buffer, 0, bytes, 0, length);
                    if (IsStarted)
                    {
                        remote = new IPEndPoint(IPAddress.Any, 0);
                        server.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remote, ReceiveRequest, buffer);
                    }
                    Treat(new MemoryStream(bytes), client as IPEndPoint);
                }

            }
            catch (ObjectDisposedException)
            {

            }
            catch (Exception)
            {
                if (IsStarted)
                {
                    if (IsTcp)
                        server.BeginAccept(ReceiveRequest, null);
                    if (IsUdp)
                    {
                        EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                        if (buffer == null)
                            buffer = new byte[65536];
                        server.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remote, ReceiveRequest, buffer);
                    }
                }
            }
        }

        protected abstract void Treat(Stream client, IPEndPoint remote);

        public void Stop()
        {
            if (IsStarted)
            {
                server.Close();
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

        static internal protected bool IsMulticast(IPAddress hostAddress)
        {
            if (hostAddress.IsIPv6Multicast)
                return true;
            byte[] addressBytes = hostAddress.GetAddressBytes();
            if (addressBytes[0] >= 224 && addressBytes[0] <= 239)
                return true;
            return false;
        }

        public bool IsStateLess { get; set; }

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

    public abstract class Server<TRequest, TResponse> : Server
        where TResponse : IServerResponse, new()
        where TRequest : IServerRequest<TRequest>, new()
    {
        internal Server(Socket server, Client client)
            : this(0)
        {
            this.server = server;
            Host = server.LocalEndPoint as IPEndPoint;
            IsStarted = server.IsBound;
            IsTcp = client.IsTcp;
            IsUdp = client.IsUdp;
        }

        public Server(ushort port)
            : this(IPAddress.Loopback, port)
        {

        }

        public Server(IPAddress address, ushort port)
            : this(new IPEndPoint(address, port))
        {

        }

        public Server(IPEndPoint host)
            : base(host)
        {
        }

        public event EventHandler<RequestEventArgs<TRequest, TResponse>> RequestReceived;

        protected abstract RequestEventArgs<TRequest, TResponse> GetEventArgs(TRequest request);

        protected override void Treat(Stream client, IPEndPoint remote)
        {
            RequestEventArgs<TRequest, TResponse> rea = GetEventArgs(new TRequest().GetRequest(new BinaryReader(client)));
            rea.Host = remote;
            Treat(rea, client);
        }

        protected virtual void Treat(RequestEventArgs<TRequest, TResponse> rea, Stream client)
        {
            OnRequestReceived(rea);
            if (rea.Response != null)
            {
                if (IsUdp && client is MemoryStream)
                {
                    Send(rea.Response, rea.Host);
                }
                else
                    Send(rea.Response, client);
            }
            client.Flush();
            if (IsStateLess)
                client.Close();
        }

        protected void Send(TResponse response, Stream client)
        {
            if (client.CanWrite)
            {
                using (BinaryWriter writer = new BinaryWriter(client, Encoding.UTF8))
                {
                    response.WriteTo(writer);
                    writer.Flush();
                }
            }
        }

        public void Send(TResponse response, IPEndPoint client)
        {
            if (server.IsBound)
            {
                MemoryStream stream = new MemoryStream();
                Send(response, stream);
                server.SendTo(stream.ToArray(), client);
            }
        }

        protected virtual void OnRequestReceived(RequestEventArgs<TRequest, TResponse> rea)
        {
            if (RequestReceived != null)
                RequestReceived(this, rea);
        }
    }
}
