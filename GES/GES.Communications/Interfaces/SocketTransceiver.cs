// <author>Fran Maher</author>
// <email>francis.a.maher@nasa.gov</email>
// <date>2019-02-28</date>
// <summary>Transmits/receives messages over a TCP/IP socket.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.ComponentModel;
   using System.Net;
   using System.Net.NetworkInformation;
   using System.Net.Sockets;
   using System.Text;
   using System.Threading;
   using System.Threading.Tasks;
   using MTI.Common;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Receives messages over a TCP/IP socket
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications.Interfaces" })]
   [Description("Transmits blocks of bytes.")]
   public class SocketTransceiver:
      MTI.Core.Component
   {
      #region Fields

      /// <summary>
      /// The concurrent input queue of messages
      /// </summary>
      private SynchronizedQueue<byte[]> incomingQueue;

      /// <summary>
      /// The concurrent output queue of messages
      /// </summary>
      private SynchronizedQueue<byte[]> outgoingQueue;

      /// <summary>
      /// A value which indicates whether the task is active or not.
      /// </summary>
      private bool active;

      /// <summary>
      /// A value which indicates whether there is a client connected.
      /// </summary>
      private bool connection;

      /// <summary>
      /// The asynchronous task that transmits data
      /// </summary>
      private Task<bool> task;

      /// <summary>
      /// The socket over which bytes are received
      /// </summary>
      private Socket socket;

      private EndPoint remoteEndPoint;

      private Socket listener;

      /// <summary>
      /// Thread signal
      /// </summary>
      private ManualResetEvent ready;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="SocketTransceiver
      /// <param name="queue">the thread safety queue</param>
      public SocketTransceiver()
      {
         this.incomingQueue = new SynchronizedQueue<byte[]>();
         this.outgoingQueue = new SynchronizedQueue<byte[]>();
         this.ready = new ManualResetEvent(false);
         this.BlockSize = 1024;
         this.Protocol = ProtocolType.Tcp;
         this.incomingQueue.DataReceived += this.OnIncomingDataReceived;
      }

      #endregion Constructors

      #region Enumerations

      /// <summary>
      /// The mode in which the tranmitter is executing
      /// </summary>
      public enum SocketMode { Client, Server };

      #endregion Enumerations

      #region Properties

      /// <summary>
      /// Gets or sets a value indicating whether to read the file
      /// </summary>
      [Require(Cut = true)]
      public bool Active
      {
         get
         {
            return this.active;
         }

         set
         {
            if (value && !this.active)
            {
               this.active = value;
               this.task = this.TransmitReceiveAsync(this.AddressOrHostName, this.Port);
               this.task.ConfigureAwait(false);
            }
            else if(this.active && ! value)
            {
               this.socket.Close();
               if(this.Mode == SocketMode.Server)
               {
                  this.listener.Close();
               }
            }

            this.active = value;
         }
      }

      /// <summary>
      /// Gets or sets the incoming queue of message to send
      /// </summary>
      [Require(Cut = true)]
      public SynchronizedQueue<byte[]> IncomingQueue
      {
         get
         {
            return this.incomingQueue;
         }

         set
         {
            this.incomingQueue.DataReceived -= this.OnIncomingDataReceived;
            this.incomingQueue = value;
            this.incomingQueue.DataReceived += this.OnIncomingDataReceived;
            this.OnPropertyChanged("IncomingQueue");
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to read the file
      /// </summary>
      [Provide]
      public bool Connection
      {
         get
         {
            return this.connection;
         }

         set
         {
            this.connection = value;
            this.OnPropertyChanged("Connection");
         }
      }

      /// <summary>
      /// Gets or sets the output queue of message received
      /// </summary>
      [Provide]
      public SynchronizedQueue<byte[]> OutgoingQueue
      {
         get
         {
            return this.outgoingQueue;
         }

         set
         {
            this.outgoingQueue = value;
            this.OnPropertyChanged("OutgoingQueue");
         }
      }

      /// <summary>
      /// Gets or sets the block size to be read each cycle
      /// </summary>
      public int BytesSent
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the socket mode to client or server
      /// </summary>
      public SocketMode Mode
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the address or hostname of the endpoint which to connect
      /// </summary>
      public string AddressOrHostName
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the port of the endpoint which to connect
      /// </summary>
      public int Port
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the block size to be read each cycle
      /// </summary>
      public int BlockSize
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the protocol (TCP, UDP, ...)
      /// </summary>
      public ProtocolType Protocol
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the keep alive time
      /// </summary>
      public int KeepAliveTime
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the keep alive interval in milliseconds
      /// </summary>
      public int KeepAliveInterval
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets a value indicating whether the bytes represented ASCII Hexidecimal for diagnostic purposes
      /// </summary>
      public bool ASCIIHexidecimal
      {
         get;
         set;
      }
      /// <summary>
      /// Gets or sets the keep alive interval in milliseconds
      /// </summary>
      public bool Verbose
      {
         get;
         set;
      }

      bool send;
      public bool Send
      {
         get { return this.send; }
         set { this.send = value; this.BytesSent = this.socket.SendTo(new byte[] { 0x0, 0x1, 0x2 }, this.remoteEndPoint); }
      }

      #endregion Properties

      #region Public Methods

      /// <summary>
      /// Listens for incoming connections
      /// </summary>
      /// <param name="port">the listening port</param>
      /// <returns>a value indicating whether there was a successful connection</returns>
      public bool Listen(int port, string addressOrHostName)
      {
         IPAddress ipAddress = null;
         if (string.IsNullOrEmpty(addressOrHostName))
         {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in ipHostInfo.AddressList)
            {
               if (ip.AddressFamily == AddressFamily.InterNetwork)
               {
                  ipAddress = ip;
                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": server on " + ipAddress.ToString() + ":" + port.ToString(), this.Identifier, this));
                  break;
               }
            }
         }
         else
         {
            ipAddress = IPAddress.Parse(AddressOrHostName);
            this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": server on " + ipAddress.ToString() + ":" + port.ToString(), this.Identifier, this));
         }

         IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
         string ipstr = ipAddress.ToString();
         this.listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
         this.listener.Bind(localEndPoint);
         this.listener.Listen(100);

         while (this.active)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": waiting for a connection...", this.Identifier, this, MessageType.Warning));
            this.ready.Reset();
            this.listener.BeginAccept(new AsyncCallback(this.OnAccept), this.listener);
            this.ready.WaitOne();
            return true;
         }

         return false;
      }

      /// <summary>
      /// Event handler which is called when a connection is accepted
      /// </summary>
      /// <param name="ar">the listening socket</param>
      public void OnAccept(IAsyncResult ar)
      {
         try
         {
            Socket listener = (Socket)ar.AsyncState;
            if (this.socket != null)
            {
               lock (this.socket)
               {
                  this.socket = listener.EndAccept(ar);
               }
            }
            else
            {
               this.socket = listener.EndAccept(ar);
            }

            this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": connection accepted from " + this.socket.RemoteEndPoint.ToString(), this.Identifier, this, MessageType.Warning));
            this.ConfigureSocket(this.socket);

            SocketState state = new SocketState();
            state.socket = this.socket;
            this.remoteEndPoint = this.socket.RemoteEndPoint;
            this.socket.BeginReceive(state.buffer, 0, SocketState.BufferSize, SocketFlags.None, new AsyncCallback(OnReceive), state);
            this.Connection = true;

            this.ready.Set();
         }
         catch (Exception ex)
         {
            string message = ex.Message;
            if (ex.InnerException != null) message += ": " + ex.InnerException.Message;
            this.OnSystemNotification(this, new SystemEventArgs<object>(message, this.Identifier, this));
         }
      }

      public void OnConnect(IAsyncResult ar)
      {
         try
         {

         SocketState state = (SocketState)ar.AsyncState;
         Socket socket = state.socket;
         socket.EndConnect(ar);
         if (this.Protocol == ProtocolType.Tcp)
         {
            state.socket.BeginReceive(state.buffer, 0, SocketState.BufferSize, SocketFlags.None, new AsyncCallback(OnReceive), state);
         }
         else if (this.Protocol == ProtocolType.Udp)
         {
            state.socket.BeginReceiveFrom(state.buffer, 0, SocketState.BufferSize, SocketFlags.None, ref state.remote, new AsyncCallback(OnReceive), state);
         }

         this.Connection = true;
         this.ready.Set();
         }
         catch(Exception ex)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, this.Identifier, this));
         }
      }

      public void OnReceive(IAsyncResult ar)
      {
         SocketState state = (SocketState)ar.AsyncState;
         Socket socket = state.socket;

         try
         {
            // Read data from the client socket.   
            int bytesRead = socket.EndReceive(ar);
            if (bytesRead > 0)
            {
               byte[] bytesReceived = new byte[bytesRead];
               Array.Copy(state.buffer, bytesReceived, bytesRead);
               if(this.Verbose)
               {
                  string message = ASCIIEncoding.ASCII.GetString(bytesReceived);
                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + " received: " + message, this.Identifier, this));
               }

               this.outgoingQueue.Enqueue(bytesReceived);
               if (this.active)
               {
                  lock(socket)
                  {
                     socket.BeginReceive(state.buffer, 0, SocketState.BufferSize, 0, new AsyncCallback(OnReceive), state);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            string message = ex.Message;
            if (ex.InnerException != null) message += ": " + ex.InnerException.Message;
            this.OnSystemNotification(this, new SystemEventArgs<object>(message, this.Identifier, this));
         }
      }

      public void OnReceiveFrom(IAsyncResult ar)
      {
         SocketState state = (SocketState)ar.AsyncState;
         Socket socket = state.socket;

         try
         {
            // Read data from the client socket.   
            int bytesRead = socket.EndReceive(ar);
            if (bytesRead > 0)
            {
               byte[] bytesReceived = new byte[bytesRead];
               if (this.Verbose)
               {
                  string message = ASCIIEncoding.ASCII.GetString(bytesReceived);
                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + " received: " + message, this.Identifier, this));
               }

               Array.Copy(state.buffer, bytesReceived, bytesRead);
               this.outgoingQueue.Enqueue(bytesReceived);
               if (this.active)
               {
                  lock (socket)
                  {
                     socket.BeginReceiveFrom(state.buffer, 0, SocketState.BufferSize, 0, ref state.remote, new AsyncCallback(OnReceive), state);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            string message = ex.Message;
            if (ex.InnerException != null) message += ": " + ex.InnerException.Message;
            this.OnSystemNotification(this, new SystemEventArgs<object>(message, this.Identifier, this));
         }
      }

      /// <summary>
      /// The task that transmits messages over the sockets
      /// </summary>
      /// <param name="addressOrHostName">the internet protocol address or the host name</param>
      /// <param name="port">the port</param>
      /// <returns>a task that returns a value indicating the status of the results</returns>
      public async Task<bool> TransmitReceiveAsync(string addressOrHostName, int port)
      {
         bool result = await System.Threading.Tasks.Task.Run(async () =>
         {
            if (this.Mode == SocketMode.Server)
            {
               Task<bool> listening = System.Threading.Tasks.Task.Run(async () => this.Listen(port, addressOrHostName));
               return await listening;
            }
            else if (this.Protocol == ProtocolType.Tcp)
            {
               IPAddress ipAddress;
               if (!this.GetIPAddress(addressOrHostName, out ipAddress))
               {
                  Console.WriteLine(this.Identifier + ": unable to identify address or hostname: " + addressOrHostName);
               }

               this.remoteEndPoint = new IPEndPoint(ipAddress, port);
               this.socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
               try
               {
                  SocketState socketState = new SocketState();
                  socketState.socket = this.socket;
                  this.socket.BeginConnect(this.remoteEndPoint, new AsyncCallback(this.OnConnect), socketState);
                  this.ready.WaitOne();
               }
               catch (Exception ex)
               {
                  string message = ex.Message;
                  if (ex.InnerException != null) message += ": " + ex.InnerException.Message;
                  this.OnSystemNotification(this, new SystemEventArgs<object>(message, this.Identifier, this));
               }

               this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": connected to " + addressOrHostName + ":" + port.ToString(), this.Identifier, this));
            }
            else if (this.Protocol == ProtocolType.Udp)
            {
               IPAddress ipAddress;
               if (!this.GetIPAddress(addressOrHostName, out ipAddress))
               {
                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": unable to identify address or hostname: " + addressOrHostName, this.Identifier, this));
               }

               this.remoteEndPoint = new IPEndPoint(ipAddress, port);

               ///UdpClient socket = new UdpClient(new IPEndPoint(ipAddress, port));
               //        socket.BeginReceive(OnUdpData, socket);
               try
               {
                  this.socket = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
               }
               catch(Exception ex )
               {
                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": unable to create socket: " + addressOrHostName, this.Identifier, this));
               }

               SocketState state = new SocketState();
               state.socket = this.socket;
               state.remote = this.remoteEndPoint;

               IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
               EndPoint senderRemote = (EndPoint)sender;

               // Binding is required with ReceiveFrom calls.
               this.socket.Bind(this.remoteEndPoint);
               try
               {
                  this.socket.BeginReceive(state.buffer, 0, SocketState.BufferSize, SocketFlags.None, new AsyncCallback(this.OnReceive), state);
               }
               catch (Exception ex)
               {
                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": unable to create socket: " + addressOrHostName, this.Identifier, this));
               }

               this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + ": listening to " + addressOrHostName + ":" + port.ToString(), this.Identifier, this));
            }

            return true;
         });

         return result;
      }

      #endregion Public Methods

      #region Private Methods

      private void OnUdpData(IAsyncResult result)
      {
         // this is what had been passed into BeginReceive as the second parameter:
         UdpClient socket = result.AsyncState as UdpClient;
         // points towards whoever had sent the message:
         IPEndPoint source = new IPEndPoint(0, 0);
         // get the actual message and fill out the source:
         byte[] message = socket.EndReceive(result, ref source);
         // do what you'd like with `message` here:
         Console.WriteLine("Got " + message.Length + " bytes from " + source);
         // schedule the next receive operation once reading is done:
         socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
      }

      private void OnIncomingDataReceived(object sender, EventArgs e)
      {
         if (this.incomingQueue.Count > 0)
         {
            byte[] bytes = this.incomingQueue.Dequeue();
            if (this.Verbose)
            {
               if (this.ASCIIHexidecimal)
               {
                  string message = ASCIIEncoding.ASCII.GetString(bytes);
                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + " transmitting: " + message, this.Identifier, this));
               }
               else
               {
                  string message = string.Empty;
                  foreach (byte b in bytes)
                  {
                     message += "0x" + Convert.ToString(b, 16) + " ";
                  }

                  this.OnSystemNotification(this, new SystemEventArgs<object>(this.Identifier + " transmitting: " + message, this.Identifier, this));
               }
            }

            try
            {
               switch (this.Protocol)
               {
                  case ProtocolType.Tcp: this.BytesSent = this.socket.Send(bytes); break;
                  case ProtocolType.Udp: this.BytesSent = this.socket.SendTo(bytes, this.remoteEndPoint); break;
               }
            }
            catch (Exception ex)
            {
               this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, "DuplexSocket.TransmitReceiveAsync", this));
            }
         }
      }

      private void ConfigureSocket(Socket socket)
      {
         socket.Blocking = false;
         socket.ReceiveTimeout = this.KeepAliveInterval;

         byte[] inOptions = new byte[12];
         byte[] outOptions = new byte[12];

         if (this.KeepAliveInterval > 0)
         {
            BitConverter.GetBytes((uint)1).CopyTo(inOptions, 0);
            BitConverter.GetBytes((uint)this.KeepAliveTime).CopyTo(inOptions, 4);
            BitConverter.GetBytes((uint)this.KeepAliveInterval).CopyTo(inOptions, 8);
         }
         else
         {
            BitConverter.GetBytes((uint)0).CopyTo(inOptions, 0);
            BitConverter.GetBytes((uint)0).CopyTo(inOptions, 4);
            BitConverter.GetBytes((uint)0).CopyTo(inOptions, 8);
         }

         socket.IOControl(IOControlCode.KeepAliveValues, inOptions, outOptions);
         object result = socket.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive);
      }

      /// <summary>
      /// Gets the internet protocol address structure
      /// </summary>
      /// <param name="addressOrHostName">the address or hostname</param>
      /// <param name="ipAddress">the return address</param>
      /// <returns>a value indicating whether the address is known</returns>
      private bool GetIPAddress(string addressOrHostName, out IPAddress ipAddress)
      {
         if (!IPAddress.TryParse(addressOrHostName, out ipAddress))
         {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(addressOrHostName);
            if (ipHostInfo.AddressList.Length == 0)
            {
               return false;
            }
            else if(ipAddress == null)
            {
               ipAddress = ipHostInfo.AddressList[0];
            }
         }

         return true;
      }

      #endregion Private Methods
      
      #region Nested Types

      public class SocketState
      {
         // Size of receive buffer.  
         public const int BufferSize = 1024;
         // Client  socket.  
         public Socket socket = null;
         // Receive buffer.  
         public byte[] buffer = new byte[BufferSize];

         public EndPoint remote;
      }

      #endregion Nested Types
   }
}
