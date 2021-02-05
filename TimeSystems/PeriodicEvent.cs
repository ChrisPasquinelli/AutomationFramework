// <copyright file="PeriodicEvent.cs">
// Copyright (c) 2011 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-11-01</date>
// <summary>Class defining an event structure for defining periodic and aperiodic events.</summary>

namespace GES.TimeSystems
{
   #region Directives

   using System;
   using System.ComponentModel;
   using System.Threading;
   using MTI.Core;

   #endregion

   /// <summary>
   /// Class defining an event structure utilized by the SystemClock for periodic and aperiodic events.
   /// </summary>
   [Provide(Categories = new string[] { "Events", "Time" })]
   public class PeriodicEvent : Event
   {
      #region Fields

      /// <summary>
      /// The duty cycle of the event within the executive schedule.
      /// </summary>
      private long dutyCycle;

      /// <summary>
      /// The offset of the event within the executive schedule.
      /// </summary>
      private long phase;

      /// <summary>
      /// Flag indicating that the event should be reset to low just before the next executive cycle.
      /// </summary>
      private bool autoReset;

      /// <summary>
      /// The date and time when the event goes high
      /// </summary>
      private Time currentTime;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the PeriodicEvent class.
      /// </summary>
      public PeriodicEvent()
      {
         this.Enable = true;
         this.currentTime = new Time(DateTime.Now, TimeStandard.CoordinatedUniversalTime);
         this.AutoReset = true;
         this.Signal = EventSignal.Low;
      }

      /// <summary>
      /// Initializes a new instance of the PeriodicEvent class.
      /// </summary>
      /// <param name="dutyCycle">The duty cycle of the event within the executive schedule.</param>
      /// <param name="phase">The offset of the event within the executive schedule.</param>
      /// <param name="enabled">Flag indicating whether the event is enabled</param>
      public PeriodicEvent(uint dutyCycle, uint phase, bool enabled)
      {
         this.Enable = true;
         this.DutyCycle = dutyCycle;
         this.Phase = phase;
         this.AutoReset = true;
         if (!enabled)
         {
            this.Signal = EventSignal.Disabled;
         }
         else
         {
            this.Signal = EventSignal.Low;
         }
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the date and time when the event goes high
      /// </summary>
      [Provide]
      public Time CurrentTime
      {
         get
         {
            return this.currentTime;
         }

         set
         {
            this.currentTime.BaseStandard = value.BaseStandard;
            this.currentTime.DateTime = value.DateTime;
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

            if (this.Enable)
            {
               switch (this.signal)
               {
                  case EventSignal.High:
                     //this.OnSignal(null);
                     ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnSignal), null);
                     break;
                  default:
                     break;
               }
            }
         }
      }

      /// <summary>
      /// Gets or sets the frequency in milliseconds of the event within the executive schedule
      /// </summary>
      public long DutyCycle
      {
         get
         {
            return this.dutyCycle;
         }

         set
         {
            this.dutyCycle = value;
            this.OnPropertyChanged("DutyCycle");
         }
      }

      /// <summary>
      /// Gets or sets the offset in milliseconds of the event within the executive schedule
      /// </summary>
      public long Phase 
      {
         get
         {
            return this.phase;
         }

         set
         {
            this.phase = value;
            this.OnPropertyChanged("Phase");
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the event is automatically reset after the current cycle.
      /// </summary>
      public bool AutoReset 
      {
         get
         {
            return this.autoReset;
         }

         set
         {
            this.autoReset = value;
            this.OnPropertyChanged("AutoReset");
         }
      }

      /// <summary>
      /// Gets or sets the frequency in frames of the event within the executive schedule
      /// </summary>
      [Browsable(false)]
      public long DutyCycleFrames 
      { 
         get; 
         set; 
      }

      /// <summary>
      /// Gets or sets the offset in frames of the event within the executive schedule
      /// </summary>
      [Browsable(false)]
      public long PhaseFrames
      { 
         get;
         set;
      }

      #endregion
   }
}

