// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-11-01</date>
// <summary>Class defining an event structure for defining periodic and aperiodic events.</summary>

namespace GES.TimeSystems
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using MTI.Core;

   #endregion

   /// <summary>
   /// Class defining an event structure utilized by the SystemClock for periodic and aperiodic events.
   /// </summary>
   [Provide(Categories = new string[] { "Events", "Time" })]
   public class AperiodicEvent : Event
   {
      #region Fields

      /// <summary>
      /// The date and time when the event goes high
      /// </summary>
      private Time startTime;

      /// <summary>
      /// The date and time when the event goes low
      /// </summary>
      private Time stopTime;

      /// <summary>
      /// The message reader
      /// </summary>
      private MTI.Core.BinaryReader reader;

      /// <summary>
      /// The object nodes of the state variables whose value changed event will trigger the rule to be evaluated.
      /// </summary>
      protected List<ObjectNode> valueChangedEventSourcesNodes = new List<ObjectNode>();
      
      /// <summary>
      /// The ConditionalUpdate messages corresponding to the valueChangedEventSourcesNodes.
      /// </summary>
      protected List<ConditionalUpdates> valueChangedEventSourcesMessages = new List<ConditionalUpdates>();

      /// <summary>
      /// The states of the signals triggering the rule.
      /// </summary>
      protected List<bool> signals = new List<bool>();

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the AperiodicEvent class.
      /// </summary>
      public AperiodicEvent() :
         base()
      {
         this.startTime = new Time(DateTime.Now, TimeStandard.CoordinatedUniversalTime);
         this.stopTime = new Time(DateTime.Now, TimeStandard.CoordinatedUniversalTime);
      }

      /// <summary>
      /// Initializes a new instance of the AperiodicEvent class.
      /// </summary>
      /// <param name="startTime">The time that the event goes high.</param>
      /// <param name="stopTime">The time that the event resets to low.</param>
      /// <param name="removeOnStop">Flag indicating whether to implement a one-shot event</param>
      public AperiodicEvent(Time startTime, Time stopTime, bool removeOnStop) :
         base()
      {
         this.startTime = startTime;
         this.stopTime = stopTime;
         this.RemoveOnStop = removeOnStop;
      }

      /// <summary>
      /// Initializes a new instance of the AperiodicEvent class.
      /// </summary>
      public AperiodicEvent(AperiodicEvent aperiodicEvent) :
         base()
      {
         this.EventId = aperiodicEvent.EventId;
         this.Signal = aperiodicEvent.Signal;
         this.startTime = new Time(aperiodicEvent.StartTime.DateTime, aperiodicEvent.StartTime.BaseStandard);
         this.stopTime = new Time(aperiodicEvent.StopTime.DateTime, aperiodicEvent.StopTime.BaseStandard);
         this.RemoveOnStop = true;
         this.messages = aperiodicEvent.messages;
         //this.onLowMessages = aperiodicEvent.onLowMessages;
         //this.onHighMessages = aperiodicEvent.onHighMessages;
         //this.onDisabledMessages = aperiodicEvent.onDisabledMessages;
      }
      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets a value indicating whether to remove the event from the eventlist on reset
      /// thereby implementing a one-shot event.
      /// </summary>
      public bool RemoveOnStop
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the state of the event
      /// </summary>
      [Provide]
      public override EventSignal Signal
      {
         get
         {
            return base.Signal;
         }

         set
         {
            if (this.Enable)
            {
               switch (value)
               {
                  case EventSignal.High: MTI.Core.Component.SetState(this, ComponentState.Processing); break;
                  case EventSignal.Low: MTI.Core.Component.ClearState(this, ComponentState.Processing); break;
               }
            }

            base.Signal = value;
            this.OnPropertyChanged("Signal");
         }
      }

      /// <summary>
      /// Gets or sets the state of the event
      /// </summary>
      [Provide]
      internal EventSignal SilentSignal
      {
         get
         {
            return base.Signal;
         }

         set
         {
            if (this.Enable)
            {
               switch (value)
               {
                  case EventSignal.High: MTI.Core.Component.SetState(this, ComponentState.Processing); break;
                  case EventSignal.Low: MTI.Core.Component.ClearState(this, ComponentState.Processing); break;
               }
            }

            base.signal = value;
            this.OnPropertyChanged("Signal");
         }
      }

      /// <summary>
      /// Gets or sets the date and time when the event goes high
      /// </summary>
      [Provide]
      public Time StartTime
      {
         get
         {
            return this.startTime;
         }

         set
         {
            this.startTime.BaseStandard = value.BaseStandard;
            this.startTime.DateTime = value.DateTime;
            this.OnPropertyChanged("StartTime");
         }
      }

      /// <summary>
      /// Gets or sets the date and time when the event resets to low
      /// </summary>
      [Provide]
      public Time StopTime
      {
         get
         {
            return this.stopTime;
         }

         set
         {
            this.stopTime.BaseStandard = value.BaseStandard;
            this.stopTime.DateTime = value.DateTime;
            this.OnPropertyChanged("StopTime");
         }
      }

      #endregion
      
      #region Public Methods

      public void Arm()
      {
         this.reader.Reset(0);
         Message message = Message.CreateInstance(this.reader.ReadByte(), this.reader);
         message.ReceiveFrom = null;
         ConditionalUpdates request = message as ConditionalUpdates;
         ObjectNode signalNode = KnowledgeBase.GetObjectNode(request.RemoteDataPath);
         if (signalNode == null)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>(request.RemoteDataPath + " does not exist in knowledgebase", "AperiodicEvent", this));
         }

         signalNode.AddValueChangedEventHandler(this.OnSignalRaised);
         this.valueChangedEventSourcesNodes.Add(signalNode);
         this.valueChangedEventSourcesMessages.Add(request);
         this.signals.Add(false);
      }

      public void Disarm(bool notify = true)
      {
         bool asynch = false;
         foreach (ObjectNode signalNode in this.valueChangedEventSourcesNodes)
         {
            signalNode.RemoveValueChangedEventHandler(this.OnSignalRaised);
            asynch = true;
         }

         if (asynch && notify)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>("Disarming asynchronous event: " + this.EventId, "AperiodicEvent", this));
         }
         else if(notify)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>("End synchronous event: " + this.EventId, "AperiodicEvent", this));
         }

         this.valueChangedEventSourcesNodes.Clear();
         this.valueChangedEventSourcesMessages.Clear();
         this.signals.Clear();

      }

      public void ForceExecute()
      {
         this.Disarm(false);

         // If this has never been armed then move buffer index to the next message so end-user can execute
         if (this.messages.Length > 0)
         {
            this.reader = new MTI.Core.BinaryReader(this.messages);
            if (this.reader.Peek() == 0xB)
            {
               Message message = Message.CreateInstance(this.reader.ReadByte(), this.reader);
            }
         }

         // Dispatch event messages
         while (this.reader.Count > 0)
         {
            byte messageId = this.reader.ReadByte();
            MTI.Core.Message message = MTI.Core.Message.CreateInstance(messageId, reader);
            message.ReceiveFrom = null;
            message.OnReceive();
         }
         
         this.OnSystemNotification(this, new SystemEventArgs<object>("Event triggered: " + this.EventId, "AperiodicEvent", this));
      }

      public string Translate()
      {
         string result = string.Empty;
         this.reader = new MTI.Core.BinaryReader(this.messages);
         // Dispatch event messages
         while (this.reader.Count > 0)
         {
            byte messageId = this.reader.ReadByte();
            result += MTI.Core.Message.Translate(messageId, reader);
         }
          
         this.reader = null;
         return result;
      }

      /// <summary>
      /// Dispatches messages based on trigger
      /// </summary>
      /// <param name="sender">The originator of the event.</param>
      /// <param name="args">The event arguments.</param>
      public override void OnSignal(object state)
      {
         if (this.Signal == EventSignal.High)
         {
            bool isAsynchronous = false;
            if (this.messages.Length > 0)
            {
               bool armed = false;
               this.reader = new MTI.Core.BinaryReader(this.messages);
               while (this.reader.Count > 0 && !armed)
               {
                  switch (this.reader.Peek())
                  {
                     case 0xB:
                        {
                           isAsynchronous = true;
                           this.Arm();
                           this.OnSystemNotification(this, new SystemEventArgs<object>("Arming asynchronous event: " + this.EventId, "AperiodicEvent", this));
                        }

                        break;

                     default:
                        {
                           if (!isAsynchronous)
                           {
                              Message message = Message.CreateInstance(this.reader.ReadByte(), this.reader);
                              message.ReceiveFrom = null;
                              message.OnReceive();
                           }
                           else
                           {
                              armed = true;
                           }
                        }

                        break;
                  }
               }
            }

            if (!isAsynchronous)
            {
               this.OnSystemNotification(this, new SystemEventArgs<object>("Begin synchronous event: " + this.EventId, "AperiodicEvent", this));
            }
         }
         else
         {
            this.Disarm();
         }

         this.OnPropertyChanged("Signal");
      }

      /// <summary>
      /// Event handler that determines which signal went high
      /// </summary>
      /// <param name="sender">The signal source.</param>
      /// <param name="args">The value event changed arguments.</param>
      protected virtual void OnSignalRaised(object sender, ValueChangedEventArgs args)
      {
         if (args.UpdateEvent != ValueChangedEventArgs.Update.Assign)
         {
            return;
         }

         int index = this.valueChangedEventSourcesNodes.IndexOf((ObjectNode)sender);
         if (!this.valueChangedEventSourcesMessages[index].Compare())
         {
            return;
         }

         this.signals[index] = true;
         for (int i = 0; i < this.signals.Count; i++)
         {
            if (!this.signals[i])
            {
               return;
            }
         }

         if (this.Enable)
         {
            MTI.Core.Component.SetState(this, ComponentState.Processing);
            while (this.reader.Count > 0)
            {
               Message message = Message.CreateInstance(this.reader.ReadByte(), this.reader);
               message.ReceiveFrom = null;
               message.OnReceive();
            }

            MTI.Core.Component.ClearState(this, ComponentState.Processing);
            this.OnSystemNotification(this, new SystemEventArgs<object>("Event triggered: " + this.EventId, "AperiodicEvent", this));
            this.Disarm();
         }
      }

      #endregion
   }
}
