// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-07-01</date>
// <summary>Class representing time event arguments.</summary>

namespace GES.TimeSystems
{
   #region Directives

   using System;

   #endregion

   /// <summary>
   /// Arguments for system clock events.
   /// </summary>
   public class TimeEventArgs :
      EventArgs
   {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the TimeEventArgs class.
      /// </summary>
      /// <param name="currentTime">The time at which the event occurred.</param>
      /// <param name="startTime">The start time currently defined.</param>
      /// <param name="stopTime">The stop time currently defined.</param>
      public TimeEventArgs(Time currentTime, Time startTime, Time stopTime)
      {
         this.CurrentTime = currentTime;
         this.StartTime = startTime;
         this.StopTime = stopTime;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets thr time corresponding to event
      /// </summary>
      public Time StartTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets thr time corresponding to event
      /// </summary>
      public Time StopTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets thr time corresponding to event
      /// </summary>
      public Time CurrentTime
      {
         get;
         private set;
      }

      #endregion
   }
}
