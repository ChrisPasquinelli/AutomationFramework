// <copyright file="TelemetryTask.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Task that deserializes a byte stream into telemety packets.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.ComponentModel;
   using System.Threading.Tasks;

   using BKSystem.IO;

   using MTI.Common;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Task that deserializes a byte stream into telemetry packets.
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications" })]
   [Description("Telemetry processing task")]
   public class TelemetryTask :
      Task
   {
      #region Fields

      /// <summary>
      /// <summary>
      /// The binay serializers for processing this task's commands
      /// </summary>
      private ITelemetry[] telemetry;

      /// <summary>
      /// queue of incoming messages
      /// </summary>
      protected SynchronizedQueue<byte[]> downlinkQueue;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the TelemetryList class.
      /// </summary>
      public TelemetryTask() :
         base()
      {
         this.downlinkQueue = new SynchronizedQueue<byte[]>();
         this.telemetry = new ITelemetry[0];
      }

      #endregion Constructors

      #region Public Propreties

      [Require]
      public override bool Active { get => base.Active; set => base.Active = value; }

      /// <summary>
      /// Gets or sets the command/telemetry packet definitions
      /// </summary>
      [Require(Cut = true)]
      public virtual ITelemetry[] Telemetry
      {
         get
         {
            return this.telemetry;
         }

         set
         {
            this.telemetry = value;
            this.OnPropertyChanged("Telemetry");
         }
      }

      /// <summary>
      /// Gets or sets the incoming message queue
      /// </summary>
      [Require]
      public virtual SynchronizedQueue<byte[]> DownlinkQueue
      {
         get
         {
            return this.downlinkQueue;
         }

         set
         {
            this.downlinkQueue.DataReceived -= this.OnDownlinkMessageReceived;
            this.downlinkQueue = value;
            this.downlinkQueue.DataReceived += this.OnDownlinkMessageReceived;
            this.OnPropertyChanged("DownlinkQueue");
         }
      }

      #endregion Public Propreties

      #region Public Methods

      public virtual void OnDownlinkMessageReceived(object sender, EventArgs args)
      {
         byte[] message = this.downlinkQueue.Dequeue();
         ushort us;
         byte[] bytes;
         if (message.Length >= 6)
         {
            BitStream bitStream = new BitStream();
            bitStream.Write(message);
            bitStream.Position = 0;

            CCSDSPrimaryHeader primaryHeader = new CCSDSPrimaryHeader();
            CCSDSPacketSerializer.DeserializePrimaryHeader(bitStream, primaryHeader);

            foreach (ITelemetry telemetry in this.telemetry)
            {
               if (telemetry is CCSDSTelemetry)
               {
                  CCSDSTelemetry ccsds = telemetry as CCSDSTelemetry;
                  if((ushort)ccsds.PacketDefinition.PrimaryHeader.ApplicationId == primaryHeader.ApplicationId)
                  {
                     bitStream.Position = 0;
                     ccsds.Deserialize(bitStream);
                  }
               }
            }
         }
      }

      #endregion Public Methods

      #region Framework Event Handlers
      #endregion Framework Event Handlers

      #region Private Methods
      #endregion
   }
}
