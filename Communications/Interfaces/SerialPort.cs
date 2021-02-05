// <author>Fran Maher</author>
// <email>francis.a.maher@nasa.gov</email>
// <date>2019-06-07</date>
// <summary>Transmits/receives messages over a serial port.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.IO;
   using System.IO.Ports;
   using System.Text;
   using System.Threading;
   using System.Threading.Tasks;
   using MTI.Common;
   using MTI.Core;

   #endregion Directives

   [Provide(Categories = new string[] { "GES.Communications.Interfaces" })]
   [Description("Serial port.")]
   public class SerialPort:
      MTI.Core.Component
   {
      #region Fields

      private System.IO.Ports.SerialPort serialPort;
      private bool connect;
      private bool connected;
      private SynchronizedQueue<byte[]> uplinkQueue;
      private SynchronizedQueue<byte[]> downlinkQueue;
      CancellationTokenSource cancellationTokenSource;
      CancellationToken cancellationToken;
      bool writeToFile = false;
      private List<string>  all = new List<string>();

      #endregion Fields

      #region Constructors

      public SerialPort()
      {
         this.PortName = string.Empty;
         this.BaudRate = 9600;
         this.Parity = Parity.None;
         this.DataBits = 8;
         this.StopBits = StopBits.One;
         this.uplinkQueue = new SynchronizedQueue<byte[]>();
         this.uplinkQueue.DataReceived += OnUplinkDataReceived;
         this.downlinkQueue = new SynchronizedQueue<byte[]>();
         this.ReadBufferSize = 4096;
         this.WriteBufferSize = 2048;
         this.MaxQueueSize = 1024;
         this.LastestReceived = new byte[0];
         this.LatestTransmitted = new byte[0];
         this.Statistics = new Statistics();
         this.Verbose = false;
      }

      #endregion Constructors

      #region Enumerations

      public enum Mode
      {
         EventDriven,
         Poll
      };

      #endregion Enumerations


      #region Properties

      /// <summary>
      /// Gets or sets a value indicating whether the serial port is connected
      /// </summary>
      [Require(Cut = true)]
      public bool Connect
      {
         get
         {
            return this.connect;
         }

         set
         {
            if (value && !this.connect)
            {
               try
               {
                  this.connect = value;
                  this.serialPort = new System.IO.Ports.SerialPort(this.PortName, this.BaudRate, this.Parity, this.DataBits, this.StopBits);
                  this.serialPort.RtsEnable = false;
                  this.serialPort.DtrEnable = false;
                  this.serialPort.ReadBufferSize = 128;
                  this.serialPort.WriteBufferSize = 128;
                  this.serialPort.Handshake = Handshake.None;
                  this.serialPort.Open();
                  this.serialPort.DiscardOutBuffer();
                  this.serialPort.DiscardInBuffer();
                  this.Connected = this.serialPort.IsOpen;
                  this.serialPort.PinChanged += this.OnPinChanged;
                  switch (this.OperationMode)
                  {
                     case Mode.EventDriven:
                        this.serialPort.DataReceived += OnDownlinkDataReceived;
                        break;

                     case Mode.Poll:
                        this.cancellationTokenSource = new CancellationTokenSource();
                        this.cancellationToken = this.cancellationTokenSource.Token;
                        Task<bool> task = this.ProcessAsync(this.cancellationToken);
                        task.ConfigureAwait(false);
                        break;
                  }
               }
               catch (Exception ex)
               {
                  this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, this.Identifier, this));
               }
            }
            else
            {
               this.connect = value;
               if (this.serialPort != null)
               {
                  this.serialPort.Close();
                  this.serialPort.Dispose();
                  this.serialPort = null;
               }

               this.Connected = false;
            }
         }
      }

      private void OnPinChanged(object sender, SerialPinChangedEventArgs e)
      {
         switch(e.EventType)
         {
            case SerialPinChange.Break: break;
            case SerialPinChange.CDChanged: break;
            case SerialPinChange.CtsChanged: break;
            case SerialPinChange.DsrChanged: break;
            case SerialPinChange.Ring: break;
            default: break;
         }
      }

      [Require(Cut = true)]
      public SynchronizedQueue<byte[]> UplinkQueue
      {
         get
         {
            return this.uplinkQueue;
         }

         set
         {
            this.uplinkQueue = value;
            this.uplinkQueue.DataReceived -= OnUplinkDataReceived;
            this.uplinkQueue.DataReceived += OnUplinkDataReceived;
            this.OnPropertyChanged("UplinkQueue");
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the serial port is connected
      /// </summary>
      [Provide]
      public bool Connected
      {
         get
         {
            return this.connected;
         }

         set
         {
            this.connected = value;
            this.OnPropertyChanged("Connected");
         }
      }

      [Provide]
      public SynchronizedQueue<byte[]> DownlinkQueue
      {
         get
         {
            return this.downlinkQueue;
         }

         set
         {
            this.downlinkQueue = value;
            this.OnPropertyChanged("DownlinkQueue");
         }
      }

      public string portName;
      public string PortName
      {
         get
         {
            return this.portName;
         }

         set
         {
            this.portName = value;
            if(this.connected)
            {
               this.Connect = false;
               this.Connect = true;
            }
         }
      }

      public int BaudRate
      {
         get;
         set;
      }

      public Parity Parity
      {
         get;
         set;
      }

      public int DataBits
      {
         get;
         set;
      }

      public StopBits StopBits
      {
         get;
         set;
      }

      public Mode OperationMode
      {
         get;
         set;
      }

      public bool Loopback
      {
         get;
         set;
      }

      [Editor(typeof(System.ComponentModel.Design.ArrayEditor), typeof(System.Drawing.Design.UITypeEditor))]
      [Require(Cut = true)]
      public byte[] LatestTransmitted
      {
         get;
         set;
      }

      [Editor(typeof(System.ComponentModel.Design.ArrayEditor), typeof(System.Drawing.Design.UITypeEditor))]
      [Provide(Cut = true)]
      public byte[] LastestReceived
      {
         get;
         set;
      }

      public int ReadBufferSize
      {
         get;
         set;
      }

      public int WriteBufferSize
      {
         get;
         set;
      }

      public bool WriteToTextFile
      {
         get
         {
            return writeToFile;
         }

         set
         {
            writeToFile = value;
            if(value)
            {
               File.WriteAllLines("d:\\results.txt", all.ToArray());
            }
         }
      }

      bool send;
      public bool Send
      {
         get
         {
            return this.send;
         }

         set
         {
            this.serialPort.Write(this.LatestTransmitted, 0, this.LatestTransmitted.Length);
            this.send = value;
         }
      }

      /// <summary>
      /// Gets or sets the maximumum queue size to throtte
      /// the processing back until existing items are consumed
      /// </summary>
      public int MaxQueueSize
      {
         get;
         set;
      }

      public bool Verbose
      {
         get;
         set;
      }

      public Statistics Statistics
      {
         get;
         set;
      }

      #endregion Properties

      #region Public Methods

      #endregion Public Methods

      #region Private Methods

      private async Task<bool> ProcessAsync(CancellationToken token)
      {
         bool result = await Task<bool>.Run(() =>
         {
 //           byte[] buffer = new byte[this.ReadBufferSize];
            try
            {
               while (!token.IsCancellationRequested)
               {
                  string buffer = this.serialPort.ReadLine();
                  if (this.Verbose)
                  {
                     string line = DateTime.Now.ToString("hh:mm:ss.fff") + " " + this.Identifier + " " + buffer.Length.ToString() + " bytes read, " + this.serialPort.BytesToRead.ToString() + " bytes in buffer, Receiving " + buffer;
                     all.Add(line);
                     this.OnSystemNotification(this, new SystemEventArgs<object>(line, this.Identifier, this));

                  }

                  this.downlinkQueue.Enqueue(ASCIIEncoding.ASCII.GetBytes(buffer));
               }
            }
            catch (Exception ex)
            {
               this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, this.Identifier, this));
               return false;
            }

            return true;
         }, cancellationToken);

         return result;
      }

      private void OnUplinkDataReceived(object sender, EventArgs e)
      {
         if (this.serialPort == null || !this.serialPort.IsOpen)
         {
            return;
         }

         if (this.uplinkQueue.Count > 0)
         {
            byte[] bytes = this.uplinkQueue.Dequeue();
            lock (this.serialPort)
            {
               this.serialPort.Write(bytes, 0, bytes.Length);
            }

            if (this.Verbose)
            {
               string line = DateTime.Now.ToString("hh:mm:ss.fff") + " " + this.Identifier + ": " + this.PortName + ": Sending " + Encoding.ASCII.GetString(bytes);
               this.all.Add(line);
               this.OnSystemNotification(this, new SystemEventArgs<object>(line, this.Identifier, this));
            }
         }
      }

      private void OnDownlinkDataReceived(object sender, SerialDataReceivedEventArgs e)
      {
         byte[] buffer = new byte[this.serialPort.BytesToRead];
         int bytesRead = this.serialPort.Read(buffer, 0, buffer.Length);
         this.LastestReceived = buffer;
         if(this.Verbose)
         {
            string line = DateTime.Now.ToString("hh:mm:ss.fff") + " " + this.Identifier + ": " + this.PortName + " " + bytesRead.ToString() + " bytes read, " + this.serialPort.BytesToRead.ToString() + " bytes in buffer, Receiving " + Encoding.ASCII.GetString(buffer);
            all.Add(line);
            this.OnSystemNotification(this, new SystemEventArgs<object>(line, this.Identifier, this));
            
         }

         if(this.Loopback)
         {
            this.serialPort.Write(buffer, 0, buffer.Length);
         }

         this.downlinkQueue.Enqueue(buffer);
      }

      #endregion Private Methods
   }
}
