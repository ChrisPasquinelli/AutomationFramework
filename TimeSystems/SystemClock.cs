// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-07-08</date>
// <summary>The system clock object to which time-varying objects synchronize.</summary>

namespace GES.TimeSystems
{ 
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Runtime.InteropServices;
   using MTI.Core;

   #endregion

   /// <summary>
   /// The system clock object to which time-varying objects synchronize.
   /// </summary>
   [Provide(Categories = new string[] { "Mathemetics", "Time" })]
   public partial class SystemClock :
      MTI.Core.Component
   {
      #region P/Invoke declarations

      [StructLayout(LayoutKind.Explicit)]
      public struct MmTime
      {
         [FieldOffset(0)] public UInt32 wType;
         [FieldOffset(4)] public UInt32 ms;
         [FieldOffset(4)] public UInt32 sample;
         [FieldOffset(4)] public UInt32 cb;
         [FieldOffset(4)] public UInt32 ticks;
         [FieldOffset(4)] public Byte smpteHour;
         [FieldOffset(5)] public Byte smpteMin;
         [FieldOffset(6)] public Byte smpteSec;
         [FieldOffset(7)] public Byte smpteFrame;
         [FieldOffset(8)] public Byte smpteFps;
         [FieldOffset(9)] public Byte smpteDummy;
         [FieldOffset(10)] public Byte smptePad0;
         [FieldOffset(11)] public Byte smptePad1;
         [FieldOffset(4)] public UInt32 midiSongPtrPos;
      }// Represents the method that is called by Windows when a timer event occurs.

      private delegate void TimerCallback(int id, int msg, int user, int param1, int param2);

      [DllImport("winmm.dll")]
      private static extern int timeSetEvent(int delay, int resolution, TimerCallback proc, int user, int mode);
      [DllImport("winmm.dll")]
      private static extern int timeKillEvent(int id);
      [DllImport("winmm.dll")]
      private static extern int timeBeginPeriod(int msec);
      [DllImport("winmm.dll")]
      private static extern int timeEndPeriod(int msec);
      [DllImport("winmm.dll")]
      private static extern uint timeGetTime();
      [DllImport("winmm.dll")]
      public static extern int timeGetSystemTime(ref MmTime lpTime, int uSize);

      [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
      private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

      #endregion

      #region Constants

      /// <summary>
      /// Program timer for single event
      /// </summary>
      private const int TIME_ONESHOT = 0x0000;

      /// <summary>
      /// Program timer for continuous periodic even
      /// </summary>
      private const int TIME_PERIODIC = 0x0001;

      /// <summary>
      /// Callback is a function
      /// </summary>
      private const uint TIME_CALLBACK_FUNCTION = 0x0000;

      /// <summary>
      /// Callback is an event -- use SetEvent
      /// </summary>
      private const uint TIME_CALLBACK_EVENT_SET = 0x0010;

      /// <summary>
      /// Callback is an event -- use PulseEvent
      /// </summary>
      private const uint TIME_CALLBACK_EVENT_PULSE = 0x0020;

      /// <summary>
      /// This flag prevents the event from occurring after the user calls timeKillEvent() to
      /// destroy it.
      /// </summary>
      private const uint TIME_KILL_SYNCHRONOUS = 0x0100;

      #endregion

      #region Fields

      /// <summary>
      /// The single instance
      /// </summary>
      private static SystemClock instance;

      /// <summary>
      /// The elapsed seconds relative to the epoch time.
      /// </summary>
      private double elapsedSeconds = 0.0;

      /// <summary>
      /// The interval multiplied by the real-time scale factor
      /// </summary>
      private double scaledInterval;

      /// <summary>
      /// The multimedia timer id
      /// </summary>
      private int timerId;

      /// <summary>
      /// The multimedia timer id
      /// </summary>
      private TimerCallback timerEventHandler;

      /// <summary>
      /// The multimedia timer id
      /// </summary>
      private int startTimerId;

      /// <summary>
      /// The multimedia timer id
      /// </summary>
      private TimerCallback startTimerEventHandler;

      /// <summary>
      /// The current configured periodic and aperiodic events.
      /// </summary>
      private PeriodicEvent[] periodic = new PeriodicEvent[0];

      /// <summary>
      /// The current configured periodic and aperiodic events.
      /// </summary>
      private AperiodicEvent[] aperiodic = new AperiodicEvent[0];

      /// <summary>
      /// The frame count within the currrent major cycle.
      /// </summary>
      private long currentFrame;

      /// <summary>
      /// The total number of frames executied.
      /// </summary>
      private long frameCounter;

      private int delay;
      private System.Threading.Timer longTermTimer;
      private Stopwatch lagStopWatch;
      private Stopwatch frameStopWatch;

      /// <summary>
      /// The next scheduled event to break on during stepping mode.
      /// </summary>
      private int selectedEvent;

      /// <summary>
      /// Flag indicating whether the executive is in stepping mode.
      /// </summary>
      private bool stepping;

      /// <summary>
      /// Flag indicating whether the executive is in fast-forward mode.
      /// </summary>
      private bool fastForward;

      /// <summary>
      /// Flag indicating whether the executive should exit stepping mode and continue.
      /// </summary>
      private bool paused;

      /// <summary>
      /// Flag indicating whether the executive is active.
      /// </summary>
      private bool executing;

      /// <summary>
      /// Flag indicating whether the timer has been initialized
      /// </summary>
      private bool timerInitialized;

      /// <summary>
      /// Flag indicating whether the system models have been initialized
      /// </summary>
      private bool initialize;

      /// <summary>
      /// Epoch time
      /// </summary>
      private Time epochTime;

      /// <summary>
      /// Start time
      /// </summary>
      private Time scheduledStartTime;

      /// <summary>
      /// Start time
      /// </summary>
      private Time startTime;

      /// <summary>
      /// Stop time
      /// </summary>
      private Time stopTime;

      /// <summary>
      /// Simulation time
      /// </summary>
      private Time simulationTime;

      List<PeriodicEvent> triggeredPeriodicEvents;
      List<AperiodicEvent> activeAperiodicEvents;
      List<AperiodicEvent> inactiveAperiodicEvents;

      AperiodicEvent latestEvent;
      AperiodicEvent[] triggeredAperiodicEvents;
      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the SystemClock class.
      /// </summary>
      public SystemClock()
      {
         SystemClock.instance = this;
         this.epochTime = Time.Now;
         this.scheduledStartTime = Time.Now;
         this.startTime = Time.Now;
         this.stopTime = this.epochTime.AddYears(1);
         this.RealTimeScaleFactor = 1.0;
         this.simulationTime = new Time();
         this.simulationTime.EnableDateTimeChangedNotification = false;
         this.Identifier = "SystemClock";
         this.MinorCycle = 0u;
         this.EnableThrottleControl = true;
         this.selectedEvent = -1;
         this.triggeredPeriodicEvents = new List<PeriodicEvent>();
         this.triggeredAperiodicEvents = new AperiodicEvent[0];
         this.activeAperiodicEvents = new List<AperiodicEvent>();
         this.inactiveAperiodicEvents = new List<AperiodicEvent>();
         this.latestEvent = new AperiodicEvent();
      }

      #endregion

      #region Private Delegates

      /// <summary>
      /// The signiture of the time event handler
      /// </summary>
      /// <param name="id">The id of the event.</param>
      /// <param name="msg">The message.</param>
      /// <param name="user">User data.</param>
      /// <param name="dw1">First parameter.</param>
      /// <param name="dw2">Second parameter.</param>
      private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);

      #endregion

      #region Enumerations

      public enum OSClockSync
      {
         None,
         OnSecond,
         OnMinute,
         OnHour,
         OnDay,
         OnScheduledStartTime
      };

      #endregion Enumerations

      #region Events

      /// <summary>
      /// Event notifying time dependent listeners to initialize.
      /// </summary>
      public event EventHandler<TimeEventArgs> Initialize;

      #endregion

      #region Properties
      
      /// <summary>
      /// Gets the singleton instance of SystemClock
      /// </summary>
      public static SystemClock Instance
      {
         get 
         {
            if (SystemClock.instance == null)
            {
               SystemClock.instance = new SystemClock();
            }

            return SystemClock.instance; 
         }
      }

      public double AsynchronousProcessingTime
      {
         get;
         private set;

      }

      /// <summary>
      /// Gets the current simulation time.
      /// </summary>
      public DateTime Now
      {
         get
         {
            return this.UtcNow.ToLocalTime();
         }
      }

      /// <summary>
      /// Gets the high prceision UTC time
      /// </summary>
      public DateTime UtcNow
      {
         get
         {
            long preciseTime;
            GetSystemTimePreciseAsFileTime(out preciseTime);
            return DateTime.FromFileTimeUtc(preciseTime);
         }
      }


   /// <summary>
   /// Gets or sets the interval in seconds for simulated time
   /// </summary>
   public double Interval
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the time scale for simulated time
      /// </summary>
      [Require(Cut = true)]
      public double RealTimeScaleFactor
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the reference epoch time from which simulation time is reckoned.
      /// </summary>
      [Require(Cut = true)]
      public Time EpochTime
      {
         get
         {
            return new Time(this.epochTime);
         }

         set
         {
            this.epochTime = value;
            this.OnPropertyChanged("EpochTime");
         }
      }

      /// <summary>
      /// Gets or sets the time at which the simulation is executed
      /// </summary>
      [Require(Cut = true)]
      public Time ScheduledStartTime
      {
         get
         {
            return this.scheduledStartTime;
         }

         set
         {
            this.scheduledStartTime.DateTime = value.DateTime;
         }
      }

      /// <summary>
      /// Gets or sets the time at which the simulation ends
      /// </summary>
      public Time StartTime
      {
         get
         {
            return this.startTime;
         }

         set
         {
            this.startTime.DateTime = value.DateTime;
         }
      }

      /// <summary>
      /// Gets or sets the time at which the simulation ends
      /// </summary>
      [Require(Cut = true)]
      public Time StopTime
      {
         get
         {
            return this.stopTime;
         }

         set
         {
            this.stopTime.DateTime = value.DateTime;
         }
      }

      /// <summary>
      /// Gets the duration in milliseconds of the current frame.
      /// </summary>
      public long CurrentFrameDuration
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets or sets a value indicating whether throttling is enabled.
      /// </summary>
      public bool EnableThrottleControl
      {
         get;
         set;
      }

      /// <summary>
      /// Gets the current simulation time.
      /// </summary>
      [Provide]
      public Time SimulationTime
      {
         get
         {
            lock (this.simulationTime)
            {
               return this.simulationTime;
            }
         }

         set
         {
            lock (this.simulationTime)
            {
               if (this.Executing)
               {
                  return;
               }

               if (value.BaseStandard != this.simulationTime.BaseStandard)
               {
                  throw new InvalidOperationException("Value assigned to SystemClock.SimulationTime must have compatible time standard");
               }

               this.elapsedSeconds = (value.DateTime - this.epochTime.DateTime).TotalSeconds;
               this.simulationTime.DateTime = value.DateTime;
            }
         }
      }

      /// <summary>
      /// Gets or sets the elapsed simulation seconds relative to the epoch time.
      /// </summary>
      [Provide]
      public double ElapsedSimulationSeconds
      {
         get
         {
            return this.elapsedSeconds;
         }

         set
         {
            this.elapsedSeconds = value;
         }
      }

      /// <summary>
      /// Gets or sets the periodic events
      /// </summary>
      [Require(Cut = true)]
      public PeriodicEvent[] PeriodicEvents
      {
         get
         {
            return this.periodic;
         }

         set
         {
            this.periodic = value;
            this.OnPropertyChanged("PeriodicEvents");
         }
      }

      /// <summary>
      /// Gets or sets the aperiodic events
      /// </summary>
      /// <summary>
      /// Gets or sets the default delimiters separating record fields
      /// </summary>
      [Require(Cut = true)]
      public AperiodicEvent[] AperiodicEvents
      {
         get
         {
            return this.aperiodic;
         }

         set
         {
            this.aperiodic = value;

            this.activeAperiodicEvents.Clear();
            for (int i = 0; i < this.aperiodic.Length; i++)
            {
               //this.aperiodic[i].Signal = EventSignal.Low;
               //if (this.aperiodic[i].StartTime >= this.simulationTime || this.aperiodic[i].StopTime > this.simulationTime)
               {
                  this.activeAperiodicEvents.Add(this.aperiodic[i]);
               }
            }
            
            this.OnPropertyChanged("AperiodicEvents");
         }
      }

      /// <summary>
      /// Gets the time at which the simulation is executed
      /// </summary>
      public DateTime ActualStartTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the number of frames executed
      /// </summary>
      [Provide]
      public long FrameCounter
      {
         get
         {
            return this.frameCounter;
         }

         set
         {
            this.frameCounter = value;
            this.OnPropertyChanged("FrameCounter");
         }
      }

      [Provide]
      public AperiodicEvent LatestEvent
      {
         get
         {
            return this.latestEvent;
         }

         set
         {
            this.latestEvent.EventId = value.EventId;
            this.latestEvent.StartTime = value.StartTime;
            this.latestEvent.StopTime = value.StopTime;
            this.latestEvent.Signal = value.Signal;
            this.latestEvent.Messages = value.Messages;
            this.OnPropertyChanged("LatestEvent");
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the executive is active.
      /// </summary>
      [Require(Cut = true)]
      public bool Executing
      {
         get
         {
            return this.executing;
         }

         set
         {
            if(this.MajorCycle == 0)
            {
               this.MajorCycle = this.ComputeMajorCycle();
            }

            if (!value && this.executing)
            {
               this.PauseSystemClock();
               MTI.Core.Component.ClearState(this, ComponentState.Processing);
               this.executing = false;
            }

            if (this.selectedEvent != -1)
            {
               this.elapsedSeconds = this.FrameCounter * this.scaledInterval;
            }
            else
            {
               double elapsedMilliseconds = (this.startTime - this.epochTime).Ticks / TimeSpan.TicksPerMillisecond;
               double elapsedMinorFrames = elapsedMilliseconds / this.MinorCycle;
               this.frameCounter = (long)elapsedMinorFrames;
               this.elapsedSeconds = elapsedMilliseconds / 1000.0;
               if (this.MajorCycle != 0)
               {
                  this.currentFrame = this.FrameCounter % this.MajorCycle;
               }
            }

            this.Interval = this.MinorCycle / 1000d;
            this.scaledInterval = this.RealTimeScaleFactor * this.Interval;
            this.simulationTime.BaseStandard = this.epochTime.BaseStandard;
            this.simulationTime.DateTime = this.epochTime.DateTime.AddSeconds(this.elapsedSeconds);

            if (value && !this.executing)
            {
               try
               {
                  this.ResumeSystemClock(this.OSClockSynchronization);
                  this.executing = true;
               }
               catch(Exception ex)
               {
                  this.OnSystemNotification(this, new MTI.Core.SystemEventArgs<object>(ex.Message, this.Identifier, this, MessageType.Error));
               }
            }

            this.OnPropertyChanged("Executing");

         }
      }

      public bool PendingStart
      {
         get { return !this.timerInitialized; }
      }

      public OSClockSync OSClockSynchronization
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the identifier of the next event to break on.
      /// </summary>
      [Require(Cut = true)]
      public bool StepToNextAperiodicEvent
      {
         get
         {
            return this.stepping;
         }

         set
         {
            if (value)
            {
               Time nextEventTime = this.simulationTime;
               int selectedEvent = -1;
               this.PauseSystemClock();

               for (int i = 0; i < this.aperiodic.Length; i++)
               {
                  if (this.aperiodic[i].Signal == EventSignal.High)
                  {
                     if (this.aperiodic[i].StopTime > nextEventTime)
                     {
                        nextEventTime = this.aperiodic[i].StopTime;
                        selectedEvent = i;
                     }
                  }
               }

               for (int i = 0; i < this.aperiodic.Length; i++)
               {
                  if (this.aperiodic[i].Signal != EventSignal.Disabled && this.aperiodic[i].StartTime >= this.simulationTime)
                  {
                     if ((nextEventTime == this.simulationTime) || ((this.aperiodic[i].StartTime < nextEventTime && this.aperiodic[i].StopTime > this.simulationTime)))
                     {
                        nextEventTime = this.aperiodic[i].StartTime;
                        selectedEvent = i;
                     }
                  }
               }

               if (selectedEvent < 0 && this.activeAperiodicEvents.Count > 0)
               {  // event queue sorted by start time
                  nextEventTime = this.activeAperiodicEvents[0].StartTime;
               }

               if (selectedEvent > -1)
               {
                  bool updateCounters = this.IncrementTime(nextEventTime);
                  this.EnableThrottleControl = false;
                  this.InitiateTerminateActiveAperiodicSignals();
                  this.IncrementCounters(updateCounters);
                  this.stepping = true;
               }
            }
            else
               this.stepping = false;
         }
      }

      public bool FastForwardToNextAperiodicEvent
      {
         get
         {
            return this.fastForward;
         }

         set
         {
            this.fastForward = value;
            this.OnPropertyChanged("FastForwardToNextAperiodicEvent");
            if (this.fastForward)
            {
               Time nextEventTime = null;
               int selectedEvent = -1;
               this.PauseSystemClock();

               if (this.activeAperiodicEvents.Count == 0)
               {
                  for (int i = 0; i < this.aperiodic.Length; i++)
                  {
                     //if (this.aperiodic[i].StartTime >= this.simulationTime || this.aperiodic[i].StopTime > this.simulationTime)
                     {
                        this.activeAperiodicEvents.Add(this.aperiodic[i]);
                     }
                  }
               }

               for (int i = this.inactiveAperiodicEvents.Count - 1; i >= 0; i--)
               {  // check if any events were reconfigured with new trigger times
                  if (this.inactiveAperiodicEvents[i].StartTime > this.simulationTime)
                  {
                     this.activeAperiodicEvents.Add(this.inactiveAperiodicEvents[i]);
                     this.inactiveAperiodicEvents.RemoveAt(i);
                  }
               }

               for (int i = 0; i < this.aperiodic.Length; i++)
               {
                  if (this.aperiodic[i].Signal == EventSignal.High)
                  {
                     if (nextEventTime == null || (this.aperiodic[i].StopTime < nextEventTime && this.aperiodic[i].StopTime >= this.simulationTime))
                     {
                        nextEventTime = this.aperiodic[i].StopTime;
                        selectedEvent = i;
                     }
                  }
               }

               for (int i = 0; i < this.aperiodic.Length; i++)
               {
                  if (this.aperiodic[i].Signal != EventSignal.Disabled && this.aperiodic[i].StartTime >= this.simulationTime)
                  {
                     if (nextEventTime == null || (this.aperiodic[i].StartTime < nextEventTime && this.aperiodic[i].Signal == EventSignal.Low))
                     {
                        nextEventTime = this.aperiodic[i].StartTime;
                        selectedEvent = i;
                     }
                  }
               }

               System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() =>
               {
                  do
                  {
                     this.OnMinorCycle(null);
                     System.Threading.Thread.Sleep(0);
                  } while (this.simulationTime < nextEventTime);

                  this.stepping = true;
                  this.fastForward = false;
                  this.OnPropertyChanged("FastForwardToNextAperiodicEvent");
               });
               task.ConfigureAwait(false);
            }
         }
      }

      /// <summary>
      /// Gets or sets the identifier of the next event to break on.
      /// </summary>
      [Require(Cut = true)]
      public int StepToNextPeriodicEvent
      {
         get
         {
            return this.selectedEvent;
         }

         set
         {
            this.selectedEvent = value;

            if ((this.ComponentState & ComponentState.Initialized) == 0)
            {
               return;
            }

            //THIS NEEDS TO BE SCHEDULED !!!!!!!!!!!!!
            if (this.selectedEvent >= 0 && this.selectedEvent < this.periodic.Length)
            {
               if(this.lagStopWatch == null)
               {
                  this.lagStopWatch = Stopwatch.StartNew();
               }

               if(this.frameStopWatch == null)
               {
                  this.frameStopWatch = Stopwatch.StartNew();
               }

               bool enableThrottleControl = this.EnableThrottleControl;
               this.EnableThrottleControl = false;
               this.Interval = this.MinorCycle / 1000d;
               this.scaledInterval = this.RealTimeScaleFactor * this.Interval;
               this.simulationTime.BaseStandard = this.epochTime.BaseStandard;
               this.stepping = true;
               do
               {
                  this.OnMinorCycle(null);
               }
               while (this.stepping);

               this.EnableThrottleControl = enableThrottleControl;
            }
            else if (this.selectedEvent != -1)
            {
               this.OnSystemNotification(this, new SystemEventArgs<object>("Select event index [0, " + (this.PeriodicEvents.Length - 1) + "]", this.Identifier, this));
            }
            else
            {
               this.stepping = false;
            }
         }
      }

      public bool Pause
      {
         get
         {
            return this.stepping;
         }

         set
         {
            if (!value && this.stepping)
            {
               this.stepping = false;
               this.ResumeSystemClock(OSClockSync.OnSecond);
            }
         }
      }

      /// <summary>
      /// Gets or sets the clock bias in milliseconds
      /// </summary>
      public int ClockBias
      {
         get;
         set;
      }

      /// <summary>
      /// Gets the total lag time in milliseconds from exact system time
      /// </summary>
      public int LagTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the number of frames that the executive is lagging.
      /// </summary>
      public long LagFrames
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the major cycle in milliseconds
      /// </summary>
      public long MajorCycle
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the major cycle in frames
      /// </summary>
      public long MajorCycleFrames
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the minor cycle in milliseconds
      /// </summary>
      public long MinorCycle
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets a value indicating whether the epoch 
      /// uses a simulated time or the start time of the real time clock
      /// </summary>
      public bool SynchEpochToStartTime
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets a value indicating whether the clock 
      /// continues executing until the end of the current major cycle
      /// when terminated.
      /// </summary>
      public bool TerminateOnMajorCycle
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets a value indicating whether to display diagnostic messages
      /// </summary>
      public bool Verbose
      {
         get;
         set;
      }
      #endregion

      #region Component overrides

      /// <summary>
      /// This event handler is called by the framework as a notification for the component
      /// to initialize/reinitialize its members.  If the component fails to completely initialize
      /// due to a connected component pending intiailization then it will be called back until the
      /// connection is initialized before giving up after some number of retries.
      /// </summary>
      /// <returns>A value indicating whether or not the component successfully initialized.</returns>
      public override bool OnInitialize()
      {
         this.Reset();

         return true;
      }

      public void Reset()
      {
         this.selectedEvent = -1;
         this.CurrentFrameDuration = 0;
         this.LagFrames = 0;
         this.Executing = false;
         this.MajorCycle = this.ComputeMajorCycle();
      }

      #endregion

      #region Provided Methods

      /// <summary>
      /// Adds a periodic event to the event list
      /// </summary>
      /// <param name="periodicEvent">The periodic event to be added</param>
      [Provide]
      public void AddPeriodicEvent(PeriodicEvent periodicEvent)
      {
         PeriodicEvent[] periodic = new PeriodicEvent[this.periodic.Length + 1];
         for(int i = 0; i < this.periodic.Length; i++)
         {
            periodic[i] = this.periodic[i];
         }

         periodic[this.periodic.Length] = periodicEvent;
         this.periodic = periodic;
         this.OnPropertyChanged("PeriodicEvents");
      }

      /// <summary>
      /// Removes a periodic event from the event list.
      /// </summary>
      /// <param name="periodicEvent">The periodic event to be removed</param>
      [Provide]
      public void RemovePeriodicEvent(PeriodicEvent periodicEvent)
      {
         PeriodicEvent[] periodic = new PeriodicEvent[this.periodic.Length - 1];
         for (int i = 0, j = 0; i < this.periodic.Length; i++)
         {
            if(this.periodic[j] != periodicEvent) periodic[j++] = this.periodic[i];
         }

         this.periodic = periodic;
         this.OnPropertyChanged("PeriodicEvents");
      }

      /// <summary>
      /// Adds a a range of aperiodic events to the event list
      /// </summary>
      /// <param name="aperiodicEvents">The aperiodic events to be added</param>
      [Provide]
      public void AddAperiodicEvent(AperiodicEvent aperiodicEvent)
      {
         int i = 0;
         while (i < this.aperiodic.Length && aperiodicEvent.StartTime >= this.aperiodic[i].StartTime)
         {
            i++;
         }

         lock (this.aperiodic)
         {
            List<AperiodicEvent> temp = new List<AperiodicEvent>(this.aperiodic);
            temp.Insert(i, new AperiodicEvent(aperiodicEvent));
            this.AperiodicEvents = temp.ToArray();
         }
         
         i = 0;
         while (i < this.activeAperiodicEvents.Count && aperiodicEvent.StartTime >= this.activeAperiodicEvents[i].StartTime)
         {
            i++;
         }

         lock (this.activeAperiodicEvents)
         {
            this.activeAperiodicEvents.Insert(i, new AperiodicEvent(aperiodicEvent));
         }
      }

      /// <summary>
      /// Removes an aperiodic event from the event list.
      /// </summary>
      /// <param name="aperiodicEvent">The aperiodic event to be removed</param>
      [Provide]
      public void RemoveAperiodicEvent(AperiodicEvent aperiodicEvent)
      {
         List<AperiodicEvent> temp = new List<AperiodicEvent>(this.aperiodic);
         temp.Remove(aperiodicEvent);
         this.AperiodicEvents = temp.ToArray();
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// <summary>
      /// Gets an aperiodic event.
      /// </summary>
      /// <param name="identifier">The identifier of the aperiodic event to get.</param>
      /// <returns>The aperiodic event parameters</returns>
      public AperiodicEvent GetAperiodicEvent(string identifier)
      {
         foreach (AperiodicEvent aperiodic in this.aperiodic)
         {
            if (aperiodic.Identifier == identifier)
            {
               return aperiodic;
            }
         }

         return null;
      }

      /// <summary>
      /// Gets a periodic event.
      /// </summary>
      /// <param name="identifier">The identifier of the periodic event to get.</param>
      /// <returns>The periodic event parameters</returns>
      public PeriodicEvent GetPeriodicEvent(string identifier)
      {
         foreach (PeriodicEvent periodic in this.periodic)
         {
            if (periodic.Identifier == identifier)
            {
               return periodic;
            }
         }

         return null;
      }

      /// <summary>
      /// Gets a periodic event.
      /// </summary>
      /// <param name="dutyCycle">The duty cycle of the periodic event to get.</param>
      /// <param name="phase">The phase of the periodic event to get.</param>
      /// <returns>The periodic event parameters</returns>
      public PeriodicEvent GetPeriodicEvent(uint dutyCycle, uint phase)
      {
         foreach (PeriodicEvent periodic in this.periodic)
         {
            if (periodic.DutyCycle == dutyCycle && periodic.Phase == phase)
            {
               return periodic;
            }
         }

         return null;
      }


      /// <summary>
      /// Gets a periodic event.
      /// </summary>
      /// <param name="dutyCycle">The duty cycle of the periodic event to get.</param>
      /// <param name="phase">The phase of the periodic event to get.</param>
      /// <returns>The periodic event parameters</returns>
      public PeriodicEvent[] GetPeriodicEvents()
      {
         return this.periodic;
      }

      /// Increments the system clock by the given amount
      /// </summary>
      /// <param name="increment">the seconds added to the system clock</param>
      /// <returns>the simulated elapsed seconds since epoch</returns>
      public double Increment(double increment)
      {
         this.elapsedSeconds += increment;
         return this.elapsedSeconds;
      }

      /// <summary>
      /// Increments the system clock by the configured amount
      /// </summary>
      /// <returns>the simulated elapsed seconds since epoch</returns>
      public double Increment()
      {
         this.elapsedSeconds += this.RealTimeScaleFactor * this.Interval;
         return this.elapsedSeconds;
      }

      #endregion

      #region Private Methods

      private void PauseSystemClock()
      {
         if (this.executing)
         {
            if(this.lagStopWatch != null) this.lagStopWatch.Stop();
            if (this.frameStopWatch != null) this.frameStopWatch.Stop();
            System.Threading.Thread.Sleep(100); // Ensure callbacks are drained.
            SystemClock.timeEndPeriod(1);
            int error = SystemClock.timeKillEvent(this.timerId);
            this.simulationTime.DisableCoordinate = false;
            this.simulationTime.DateTime = this.simulationTime.DateTime; // set julian data
         }
      }

      private void ResumeSystemClock(OSClockSync sync)
      {
         this.timerInitialized = false;
         this.simulationTime.DisableCoordinate = true;

         this.startTimerEventHandler = new TimerCallback(this.StartMultimediaTimerCallback);

         switch (sync)
         {
            case OSClockSync.OnScheduledStartTime:
               {
                  int minutes = (int)(this.scheduledStartTime.DateTime - DateTime.Now).TotalMinutes;
                  if (minutes > 10)
                  {
                     long milliseconds = (minutes - 3) * 60 * 1000;
                     this.longTermTimer = new System.Threading.Timer(this.OnScheduleStartTime);
                     if (this.longTermTimer.Change(milliseconds, System.Threading.Timeout.Infinite))
                     {
                        this.OnSystemNotification(this, new SystemEventArgs<object>("SystemClock will be scheduled in " + milliseconds + " milliseconds", this.Identifier, this));
                     }
                     else
                     {
                        this.OnSystemNotification(this, new SystemEventArgs<object>("ERROR: SystemClock will NOT be scheduled in " + milliseconds + " milliseconds", this.Identifier, this, MessageType.Error));
                     }
                  }

                  this.delay = (int)((this.scheduledStartTime.DateTime - DateTime.Now).TotalMilliseconds - this.ClockBias);

                  break;
               }

            case OSClockSync.OnSecond:
               {
                  this.delay = (int)((1000 - DateTime.Now.Millisecond) - this.ClockBias);
                  this.scheduledStartTime.DateTime = this.Now.AddMilliseconds(delay);
                  if (this.delay < 0)
                  {
                     this.OnSystemNotification(this, new SystemEventArgs<object>("Clock start later than synchronization time", this.Identifier, this, MessageType.Error));
                     this.delay = 1;
                  }

                  break;
               }

            case OSClockSync.OnMinute:
               {
                  DateTime now = DateTime.Now;
                  DateTime dt = now.AddMinutes(1.0);
                  this.scheduledStartTime.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
                  this.delay = (int)((this.scheduledStartTime.DateTime - now).TotalMilliseconds - this.ClockBias);
                  if (this.delay < 0)
                  {
                     this.OnSystemNotification(this, new SystemEventArgs<object>("Clock start later than synchronization time", this.Identifier, this, MessageType.Error));
                     this.delay = 1;
                  }

                  break;
               }

            case OSClockSync.OnHour:
               {
                  DateTime now = DateTime.Now;
                  DateTime dt = now.AddHours(1.0);
                  this.scheduledStartTime.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                  int minutes = (int)(this.scheduledStartTime.DateTime - DateTime.Now).TotalMinutes;
                  if (minutes > 10)
                  {
                     long milliseconds = (minutes - 3) * 60 * 1000;
                     this.longTermTimer = new System.Threading.Timer(this.OnScheduleStartTime);
                     this.longTermTimer.Change(milliseconds, System.Threading.Timeout.Infinite);
                     this.OnSystemNotification(this, new SystemEventArgs<object>("SystemClock will be scheduled in " + milliseconds + " milliseconds", this.Identifier, this, MessageType.Error));
                     return;
                  }
                  else
                  {
                     this.delay = (int)((this.scheduledStartTime.DateTime - now).TotalMilliseconds - this.MinorCycle - this.ClockBias);
                     if (this.delay < 0)
                     {
                        this.OnSystemNotification(this, new SystemEventArgs<object>("Clock start later than synchronization time", this.Identifier, this, MessageType.Error));
                        this.delay = 1;
                     }
                  }

                  break;
               }
         }
          
         SystemClock.timeBeginPeriod(1);
         this.startTimerId = SystemClock.timeSetEvent(this.delay, 1, this.startTimerEventHandler, 0, TIME_ONESHOT);
         if(this.startTimerId == 0)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>("SystemClock Error : " + delay + " milliseconds exceeded maximum delay limit", this.Identifier, this, MessageType.Error));
         }
      }

      /// <summary>
      /// TimerCallback for handling long term timer events
      /// </summary>
      /// <param name="state"></param>
      private void OnScheduleStartTime(object state)
      {
         SystemClock.timeBeginPeriod(1);
         this.delay = (int)((this.scheduledStartTime.DateTime - DateTime.Now).TotalMilliseconds - this.MinorCycle - this.ClockBias);
         if (this.delay < 0)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>("Clock start later than synchronization time", this.Identifier, this, MessageType.Error));
            this.delay = 1;
         }

         this.startTimerId = SystemClock.timeSetEvent(this.delay, 1, this.startTimerEventHandler, 0, TIME_ONESHOT);
         if (this.startTimerId == 0)
         {
            this.OnSystemNotification(this, new SystemEventArgs<object>("SystemClock Error : " + delay + " milliseconds exceeded maximum delay limit", this.Identifier, this, MessageType.Error));
         }
      }

      /// <summary>
      /// Computes the major cycle based on the configured alarms
      /// </summary>
      /// <returns>The major cycle</returns>
      private long ComputeMajorCycle()
      {
         long[] dutyCycles = new long[this.periodic.Length];
         for (int i = 0; i < this.periodic.Length; i++) dutyCycles[i] = this.periodic[i].DutyCycle;
         return this.LeastCommonMultiple(dutyCycles);
      }

      /// <summary>
      /// Computes statistical values indicating the performance of the schedule
      /// </summary>
      /// <returns>The number of frames the executive is lagging.</returns>
      private long Diagnostics()
      {
         this.CurrentFrameDuration = this.frameStopWatch.ElapsedMilliseconds ;
         long elapsedMilliseconds = this.lagStopWatch.ElapsedMilliseconds;
         this.LagTime = (int)(elapsedMilliseconds - this.FrameCounter * this.MinorCycle);
         this.LagFrames = this.LagTime / this.MinorCycle;
         if (this.Verbose)
         {
            //string message = string.Format("Actual time = {1} computed time {2} simulation time {3}", this.Now.ToString("hh:mm:ss.fff t"), this.ActualStartTime.AddMilliseconds(this.FrameCounter * this.MinorCycle).ToString("hh:mm:ss.fff t"), this.SimulationTime.DateTime.ToString("hh:mm:ss.fff t"));
            //this.OnSystemNotification(this, new SystemEventArgs<object>(string.Format("Diagnostics: total lag {0} ms, lag frames {1}, latest frame duration {2} ms", this.LagTime, this.LagFrames, this.CurrentFrameDuration), this.Identifier, this));
         }

         this.frameStopWatch.Restart();
         return this.LagFrames;
      }

      /// <summary>
      /// The greatest common divisor of a pair of integers
      /// </summary>
      /// <param name="argument1">The first argument</param>
      /// <param name="argument2">The second argument</param>
      /// <returns>The greatest common divisor.</returns>
      private long GreatestCommonDivisor(long argument1, long argument2)
      {
         while (argument1 > 0)
         {
            long remainder = argument2 % argument1;
            argument2 = argument1;
            argument1 = remainder;
         }

         return argument2;
      }

      /// <summary>
      /// Increments frame counters based on current simulation time
      /// </summary>
      /// <param name="simulationTime">Current simulation time</param>
      private void IncrementCounters(bool updateCounters)
      {
         if (updateCounters)
         {
            double elapsedMilliseconds = (this.simulationTime - this.epochTime).Ticks / TimeSpan.TicksPerMillisecond;
            double elapsedMinorFrames = elapsedMilliseconds / this.MinorCycle;
            this.FrameCounter = (long)elapsedMinorFrames;
            this.currentFrame = this.FrameCounter % this.MajorCycle;
         }
         else
         {  // trigger events
            this.FrameCounter = this.FrameCounter;
         }
      }

      /// <summary>
      /// Increments frame counters and current simulated time
      /// </summary>
      private void IncrementTime()
      {
         this.FrameCounter++;
         this.elapsedSeconds = this.FrameCounter * this.scaledInterval;
         this.simulationTime.DateTime = this.epochTime.DateTime.AddSeconds(this.elapsedSeconds);

         if (this.currentFrame == this.MajorCycle - 1)
         {
            this.currentFrame = 0;
         }
         else
         {
            this.currentFrame++;
         }
      }

      /// <summary>
      /// Increments frame counters based on current simulation time
      /// </summary>
      /// <param name="simulationTime">Current simulation time</param>
      private bool IncrementTime(Time simulationTime)
      {
         if (simulationTime != this.simulationTime)
         {
            long elapsedTicks = (simulationTime - this.epochTime).Ticks;
            this.elapsedSeconds = elapsedTicks / TimeSpan.TicksPerSecond;
            this.simulationTime.DateTime = this.epochTime.DateTime.AddTicks(elapsedTicks);
            if (this.simulationTime.DateTime.Ticks != simulationTime.DateTime.Ticks)
            {
               this.OnSystemNotification(this, new SystemEventArgs<object>("Error: SystemClock.IncrementingTime", "SystemClock", this));
            }

            return true;
         }

         return false;
      }

      /// <summary>
      /// Raise active periodic signals
      /// </summary>
      private void InitiateActivePeriodicSignals()
      {
         for (int i = 0; i < this.PeriodicEvents.Length; i++)
         {
            PeriodicEvent periodicEvent = this.PeriodicEvents[i];
            long offset = this.currentFrame - periodicEvent.Phase;
            if ((periodicEvent.Signal != EventSignal.Disabled) && offset >= 0 && offset % periodicEvent.DutyCycle == 0)
            { // currentFrame = periodicEvent.Phase + n * periodicEvent.DutyCycle, n = 0, 1, 2, ... results in signal going high
               periodicEvent.CurrentTime = this.SimulationTime; //Update time before raising the signal
               periodicEvent.Signal = EventSignal.High;
            }

            if (periodicEvent.Signal == EventSignal.High)
            {
               this.triggeredPeriodicEvents.Add(periodicEvent);

               if (this.stepping && this.selectedEvent == i)
               {
                  this.selectedEvent = -1;
                  string message = string.Format("Selected event signaling: frame = {0}, duty cycle = {1}, phase = {2}", this.currentFrame, periodicEvent.DutyCycle, periodicEvent.Phase);
                  this.OnSystemNotification(this, new SystemEventArgs<object>(message, this.Identifier, this));
               }
            }
         }
      }

      /// <summary>
      /// Raise active aperiodic signals
      /// </summary>
      private void InitiateTerminateActiveAperiodicSignals()
      {
         List<AperiodicEvent> triggeredEvents = new List<AperiodicEvent>();
         for (int i = this.activeAperiodicEvents.Count - 1; i >= 0; i--)
         {
            if (this.activeAperiodicEvents[i].Signal == EventSignal.High && this.SimulationTime >= this.activeAperiodicEvents[i].StopTime)
            {
               this.activeAperiodicEvents[i].Signal = EventSignal.Low;
               this.LatestEvent = this.activeAperiodicEvents[i];
               triggeredEvents.Add(this.activeAperiodicEvents[i]);
               this.inactiveAperiodicEvents.Add(this.activeAperiodicEvents[i]);
               this.activeAperiodicEvents.RemoveAt(i);
            }
            else if (this.activeAperiodicEvents[i].Signal == EventSignal.Low && this.SimulationTime >= this.activeAperiodicEvents[i].StartTime && this.SimulationTime <= this.activeAperiodicEvents[i].StopTime)
            {
               this.activeAperiodicEvents[i].Enable = true;
               if (this.activeAperiodicEvents[i].StartTime >= this.epochTime)
               {
                  this.activeAperiodicEvents[i].Signal = EventSignal.High;
               }
               else
               {
                  this.activeAperiodicEvents[i].SilentSignal = EventSignal.High;
               }

               this.LatestEvent = this.activeAperiodicEvents[i];
               triggeredEvents.Add(this.activeAperiodicEvents[i]);
               MTI.Core.Component.SetState(this.activeAperiodicEvents[i], ComponentState.Processing);
            }
         }

         this.triggeredAperiodicEvents = triggeredEvents.ToArray();
      }

      /// <summary>
      /// Computes least common multiple of a list of numbers based on 
      /// associate law of least common multiples, i.e. LCM(a, b, c) = LCM(LCM(a, b), c)
      /// </summary>
      /// <param name="arguments">The list of integers from which the least common multiple is determined.</param>
      /// <returns>The least common muliple of all of the list of integers</returns>
      private long LeastCommonMultiple(params long[] arguments)
      {
         if (arguments.Length > 0)
         {
            long gcd = arguments[0];
            long lcm = arguments[0];
            for (int i = 1; i < arguments.Length; i++)
            {
               lcm = this.LeastCommonMultiple(lcm, arguments[i]);
               gcd = this.GreatestCommonDivisor(gcd, arguments[i]);
            }

            return lcm;
         }

         return 0;
      }

      /// <summary>
      /// The least common multiple of a pair of integers
      /// </summary>
      /// <param name="argument1">The first argument</param>
      /// <param name="argument2">The second argument</param>
      /// <returns>The least common multiple.</returns>
      private long LeastCommonMultiple(long argument1, long argument2)
      {
         return argument1 * argument2 / this.GreatestCommonDivisor(argument1, argument2);
      }

      /// <summary>
      /// Event handler for Timer Tick event
      /// </summary>
      /// <param name="id">The parameter is not used.</param>
      /// <param name="msg">The parameter is not used.</param>
      /// <param name="user">The parameter is not used.</param>
      /// <param name="dw1">The parameter is not used.</param>
      /// <param name="dw2">The parameter is not used.</param>
      private void StartMultimediaTimerCallback(int id, int msg, int user, int param1, int param2)
      {
         MTI.Core.Component.SetState(this, ComponentState.Processing);
         if(this.Verbose) this.OnSystemNotification(this, new SystemEventArgs<object>(this.Now.ToString("hh:mm:ss.fff t") + ": Starting clock ...", this.Identifier, this));

         int error = SystemClock.timeKillEvent(this.startTimerId);
         SystemClock.timeEndPeriod(1);
         this.timerEventHandler = new TimerCallback(this.MultimediaTimerCallback);
         SystemClock.timeBeginPeriod(1);
         this.timerId = SystemClock.timeSetEvent((int)this.MinorCycle, 0, this.timerEventHandler, 0, TIME_PERIODIC);
      }

      /// <summary>
      /// Event handler for Timer Tick event
      /// </summary>
      /// <param name="id">The parameter is not used.</param>
      /// <param name="msg">The parameter is not used.</param>
      /// <param name="user">The parameter is not used.</param>
      /// <param name="dw1">The parameter is not used.</param>
      /// <param name="dw2">The parameter is not used.</param>
      private void MultimediaTimerCallback(int id, int msg, int user, int param1, int param2)
      {
         if (!this.timerInitialized)
         {
            this.lagStopWatch = Stopwatch.StartNew();
            this.frameStopWatch = Stopwatch.StartNew();
            this.ActualStartTime = this.Now;
            this.timerInitialized = true;
         }

         this.OnMinorCycle(null);
      }

      /// <summary>
      /// Event handler for Timer Tick event
      /// </summary>
      /// <param name="state">Not used</param>
      private void OnMinorCycle(object state)
      {
         try
         {
            long lag = this.Diagnostics();

            if (!this.EnableThrottleControl)
            {
               lag = 0;
            }
            else if (lag < 0)
            {
               return;
            }

            do
            {
               this.TerminateInactivePeriodicSignals();
               this.InitiateActivePeriodicSignals();
               this.InitiateTerminateActiveAperiodicSignals();
               this.IncrementTime();

               if (this.stepping && this.selectedEvent == -1)
               {
                  this.stepping = false;
               }
            }
            while (lag-- > 0);
         }
         catch
         {
         }
      }

      /// <summary>
      /// Lower expired periodic signals
      /// </summary>
      private void TerminateInactivePeriodicSignals()
      {
         foreach (PeriodicEvent periodicEvent in this.triggeredPeriodicEvents)
         {
            if (periodicEvent.AutoReset && periodicEvent.Signal == EventSignal.High)
            {
               periodicEvent.Signal = EventSignal.Low;
            }
         }

         this.triggeredPeriodicEvents.Clear();
      }

      #endregion
   }
}