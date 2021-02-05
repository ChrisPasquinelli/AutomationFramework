// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2012-7-16</date>
// <summary>Class representing a relative event.</summary>

namespace GES.TimeSystems
{
   using System;
   #region Directives

   using System.ComponentModel;
   using System.Threading;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Enumeration identifying the state of the event.
   /// </summary>
   [Provide(Categories = new string[] { "Events", "Time" })]
   public enum EventSignal : int
   {
      /// <summary>
      /// Identifies that the event is disabled.
      /// </summary>
      Disabled = -1,

      /// <summary>
      /// Identifies that the event is not active.
      /// </summary>
      Low = 0,

      /// <summary>
      /// Identifies that the event is active.
      /// </summary>
      High = 1,
   }

   /// <summary>
   /// Class defining an event structure utilized by the SystemClock for periodic and aperiodic events.
   /// </summary>
   public class Event : MTI.Core.Component
   {
      #region Fields
      /// <summary>
      /// The state of the event.
      /// </summary>
      protected EventSignal signal;

      /// <summary>
      /// The id of the event.
      /// </summary>
      private string eventId;

      /// <summary>
      /// Active messages
      /// </summary>
      protected byte[] messages;

      /// <summary>
      /// Leading edge messages
      /// </summary>
      protected byte[] onHighMessages;

      /// <summary>
      /// Trailing edge messages
      /// </summary>
      protected byte[] onLowMessages;

      /// <summary>
      /// User triggered messages
      protected byte[] onDisabledMessages;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Event class.
      /// </summary>
      public Event()
      {
         this.signal = EventSignal.Low;
         this.eventId = string.Empty;
         this.messages = new byte[0];
         this.onHighMessages = new byte[0];
         this.onLowMessages = new byte[0];
         this.onDisabledMessages = new byte[0];
      }

      #endregion

      #region Events

      /// <summary>
      /// Event notifying listerners that the event went high
      /// </summary>
      public event EventHandler High;

      /// <summary>
      /// Event notifying listerners that the event went low
      /// </summary>
      public event EventHandler Low;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets a list of messages to be executed when the event is triggered
      /// </summary>
      [Provide(Cut = true)]
      public virtual byte[] Messages
      {
         get
         {
            return this.messages;
         }

         set
         {
            //switch (this.signal)
            //{
            //   case EventSignal.High: this.onHighMessages = value; break;
            //   case EventSignal.Low: this.onLowMessages = value; break;
            //   default: this.onDisabledMessages = value; break;
            //}
            this.messages = value;
            this.OnPropertyChanged("Messages");
         }
      }

      /// <summary>
      /// Gets or sets the description of the event
      /// </summary>
      [Provide]
      public virtual string EventId
      {
         get
         {
            return this.eventId;
         }

         set
         {
            this.eventId = value;
            this.OnPropertyChanged("EventId");
         }
      }

      /// <summary>
      /// Gets or sets the state of the event
      /// </summary>
      [Provide]
      public virtual EventSignal Signal
      {
         get
         {
            return this.signal;
         }

         set
         {
            this.signal = value;

            //switch(this.signal)
            //{
            //   case EventSignal.High: this.messages = this.onHighMessages; break;
            //   case EventSignal.Low: this.messages = this.onLowMessages; break;
            //   default: this.messages = this.onDisabledMessages; break;
            //}

            //if (SystemClock.Instance.PeriodicEvents.Length > 0)
            //{
            //   switch (value)
            //   {
            //      case EventSignal.High:
            //         this.RaiseOnHighEvent(this, new TimeEventArgs(SystemClock.Instance.SimulationTime, SystemClock.Instance.EpochTime, SystemClock.Instance.StopTime));
            //         break;

            //      case EventSignal.Low:
            //         this.RaiseOnLowEvent(this, new TimeEventArgs(SystemClock.Instance.SimulationTime, SystemClock.Instance.EpochTime, SystemClock.Instance.StopTime));
            //         break;
            //   }
            //}
            //else
            if (this.Enable)
            {
               ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnSignal), null);
            }
         }
      }


      #endregion

      #region Internal Properties

      internal bool Enable
      {
         get;
         set;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Dispatches messages based on trigger
      /// </summary>
      /// <param name="sender">The originator of the event.</param>
      /// <param name="args">The event arguments.</param>
      public virtual void OnSignal(object state)
      {
         if (this.messages.Length > 0)
         {
            // Dispatch event messages
            lock (this.messages)
            {
               MTI.Core.BinaryReader reader = new MTI.Core.BinaryReader(this.messages);
               while (reader.Count > 0)
               {
                  byte messageId = reader.ReadByte();
                  Message message = Message.CreateInstance(messageId, reader);
                  message.ReceiveFrom = null;
                  message.OnReceive();
               }
            }
         }

         this.OnPropertyChanged("Signal");
      }

      /// <summary>
      /// Raises the event high message event
      /// </summary>
      /// <param name="sender">The originator of the event.</param>
      /// <param name="args">The event arguments.</param>
      public void RaiseOnHighEvent(object sender, TimeEventArgs args)
      {
         if (this.High != null)
         {
            foreach (Delegate handler in this.High.GetInvocationList())
            {
               handler.DynamicInvoke(sender, args);
               System.Threading.Thread.Sleep(0);
            }
         }
      }

      /// <summary>
      /// Raises the event low message event
      /// </summary>
      /// <param name="sender">The originator of the event.</param>
      /// <param name="args">The event arguments.</param>
      public void RaiseOnLowEvent(object sender, TimeEventArgs args)
      {
         if (this.Low != null)
         {
            this.Low(this, args);
         }
      }

      public override bool OnInitialize()
      {
         return true;
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// The event high worker
      /// </summary>
      /// <param name="state">The time event arguments</param>
      private void OnHigh(object state)
      {
         this.High(this, (TimeEventArgs)state);
      }

      /// <summary>
      /// The event low worker
      /// </summary>
      /// <param name="state">The time event arguments</param>
      private void OnLow(object state)
      {
         this.Low(this, (TimeEventArgs)state);
      }

      #endregion
   }
}
