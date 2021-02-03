// <copyright file="Task.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Base Task implementation.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.ComponentModel;
   using System.Threading.Tasks;
   using MTI.Common;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Base Task Implementation
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications" })]
   [Description("Base Task Component")]
   public class Task :
      MTI.Core.Component,
      IGraphUpdateSubscriber
   {
      #region Fields

      /// <summary>
      /// The value indicating whether the task is active
      /// </summary>
      private bool active;

      /// <summary>
      /// The message to enqueue on the Outgoing queue
      /// </summary>
      private string sendMessage;

      /// <summary>
      /// The binay serializers for processing this task's commands and telemetry
      /// </summary>
      private Serializer[] serializers;

      /// <summary>
      /// queue of incoming messages
      /// </summary>
      protected SynchronizedQueue<Message> incomingQueue;

      /// <summary>
      /// queue of outgoing messages
      /// </summary>
      protected SynchronizedQueue<Message> outgoingQueue;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the TelemetryList class.
      /// </summary>
      public Task() :
         base()
      {
         this.incomingQueue = new SynchronizedQueue<Message>();
         this.outgoingQueue = new SynchronizedQueue<Message>();
         this.serializers = new Serializer[0];
         this.Statistics = new Statistics();
      }

      #endregion Constructors

      #region Public Propreties


      /// <summary>
      /// Gets or sets a value indicating whether the task is active
      /// </summary>
      [Require(Cut = true)]
      public virtual bool Active
      {
         get
         {
            return this.active;
         }

         set
         {
            if (value && !this.active)
            {
               this.active = true;
               MTI.Core.Component.SetState(this, ComponentState.Processing);
            }
            else
            {
               MTI.Core.Component.ClearState(this, ComponentState.Processing);
               this.active = value;
            }

            this.OnPropertyChanged("Active");
         }
      }

      /// <summary>
      /// Gets or sets a value indicating to execute a telemetry request or commad
      /// </summary>
      [Provide]
      public virtual string SendMessage
      {
         get
         {
            return this.sendMessage;
         }

         set
         {
            this.sendMessage = value;
            this.OnPropertyChanged("SendMessage");
         }
      }

      /// <summary>
      /// Gets or sets the incoming message queue
      /// </summary>
      public virtual SynchronizedQueue<Message> IncomingQueue
      {
         get
         {
            return this.incomingQueue;
         }

         set
         {
            this.incomingQueue.DataReceived -= this.OnIncomingMessageReceived;
            this.incomingQueue = value;
            this.incomingQueue.DataReceived += this.OnIncomingMessageReceived;
            this.OnPropertyChanged("IncomingQueue");
         }
      }

      /// <summary>
      /// Gets or sets the outgoing message queue
      /// </summary>
      public virtual SynchronizedQueue<Message> OutgoingQueue
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

      public int TaskId
      {
         get;
         set;
      }

      public bool Verbose
      {
         get;
         set;
      }

      public virtual Statistics Statistics
      {
         get;
         set;
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
            while (this.Active)
            {
               try
               {
                  DateTime startTime = DateTime.Now;

                  if (this.incomingQueue.Count > 0)
                  {
                     Message message = this.incomingQueue.Dequeue();
                  }

                  this.Statistics.UpdateStatistics<Message, Message>(startTime, this.incomingQueue, this.outgoingQueue);
               }
               catch (Exception ex)
               {
                  Console.WriteLine(ex.Message);
               }

               System.Threading.Thread.Sleep(0);
            }

            return true;
         });

         return result;
      }

      public virtual void OnIncomingMessageReceived(object sender, EventArgs args)
      {
      }

      #endregion Public Methods

      #region Framework Event Handlers

      /// <summary>
      /// Event handler that initializes this instance.
      /// </summary>
      /// <returns>A value indicating whether the object successfully initialized.</returns>
      public override bool OnInitialize()
      {
         return base.OnInitialize();
      }

      public void OnGraphUpdate(ObjectGraph thisGraph, ObjectNode thisNode)
      {
         //ObjectNode attributesNode = thisNode.PropertiesByName["TaskId"];
         //if (KnowledgeBase.InitialValues != null && !KnowledgeBase.InitialValues.Contains(attributesNode.FullPath))
         //{
         //   KnowledgeBase.InitialValues.Add(attributesNode.FullPath);
         //}
      }


      #endregion Framework Event Handlers

      #region Private Methods
      #endregion
   }
}
