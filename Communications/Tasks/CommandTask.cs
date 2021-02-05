// <copyright file="CommandTask.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Task that serializes commands into a byte stream.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Threading.Tasks;

   using BKSystem.IO;

   using MTI.Common;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Task that serializes commands into a byte stream.
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications" })]
   [Description("Command Task")]
   public class CommandTask :
      Task
   {
      #region Fields

      /// <summary>
      /// The binay serializers for processing this task's commands
      /// </summary>
      private ICommand[] commands;


      /// <summary>
      /// queue of uplink messages
      /// </summary>
      protected SynchronizedQueue<byte[]> uplinkQueue;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the TelemetryList class.
      /// </summary>
      public CommandTask() :
         base()
      {
         this.uplinkQueue = new SynchronizedQueue<byte[]>();
         this.commands = new ICommand[0];
      }

      #endregion Constructors

      #region Public Propreties



      [Require]
      public override bool Active { get => base.Active; set => base.Active = value; }

      /// <summary>
      /// Gets or sets the command/telemetry packet definitions
      /// </summary>
      [Require]
      public virtual ICommand[] Commands
      {
         get
         {
            return this.commands;
         }

         set
         {
            this.commands = value;
            this.OnPropertyChanged("Commands");
         }
      }

      [Provide]
      public virtual SynchronizedQueue<byte[]> UplinkQueue
      {
         get
         {
            return this.uplinkQueue;
         }

         set
         {
            this.uplinkQueue = value;
            this.OnPropertyChanged("UplinkQueue");
         }
      }

      /// <summary>
      /// Gets or sets a value indicating to execute a telemetry request or commad
      /// </summary>
      [Provide]
      public override string SendMessage
      {
         get
         {
            return base.SendMessage;
         }

         set
         {
            base.SendMessage = value;
            for(int i = 0; i < this.commands.Length; i++)
            {
               if (this.commands[i].Name == base.SendMessage)
               {
                  List<ICommand> primitives = new List<ICommand>();
                  this.commands[i].Expand(primitives);
                  foreach (ICommand command in primitives)
                  {
                     BitStream bitStream = new BitStream();
                     byte[] bytes = this.commands[i].Serialize(bitStream);
                     this.uplinkQueue.Enqueue(bytes);
                  }
               }
            }

            this.OnPropertyChanged("SendMessage");
         }
      }

      #endregion Public Propreties

      #region Public Methods

      /// <summary>
      /// The packet aquisition and frame dispatching task
      /// </summary>
      /// <returns>a task that returns a value indicating the status of the results</returns>
      public virtual async Task<bool> ProcessAsync()
      {
         bool result = await System.Threading.Tasks.Task.Run(() =>
         {
            return true;
         });

         return result;
      }

      public virtual void OnIncomingMessageReceived(object sender, EventArgs args)
      {
      }

      //public CCSDSPacketTemplate GetCCSDSPacketTemplate(ushort applicationId, ushort functionCode)
      //{
      //   foreach (MessageTemplate packet in this.PacketTemplates)
      //   {
      //      if (packet is CCSDSPacketTemplate)
      //      {
      //         CCSDSPacketTemplate ccsds = packet as CCSDSPacketTemplate;
      //         if (ccsds.ApplicationID == applicationId && ccsds.FunctionCode == functionCode)
      //         {
      //            return ccsds;
      //         }
      //      }
      //   }

      //   return null;
      //}
      #endregion Public Methods

      #region Framework Event Handlers
      #endregion Framework Event Handlers

      #region Private Methods
      #endregion
   }
}
