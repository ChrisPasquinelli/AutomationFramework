// <email>FranMaher@comcast.net</email>
// <date>2012-7-16</date>
// <summary>Class representing a standard time.</summary>

namespace GES.TimeSystems
{
   #region Directives

   using System;
   using System.ComponentModel;
   using MTI.Core;

   #endregion

   /// <summary>
   /// Class representing a standard time.
   /// </summary>
   [Provide(Categories = new string[] { "GES.Timesystems" })]
   [Description("Event in which the start time is specified as an offset duration from a refererce time.")]
   public partial class Time :
      IComparable,
      INotifyPropertyChanged
   {
      /// <summary>
      /// The internal julianDate;
      /// </summary>
      private double julianDate;

      /// <summary>
      /// The Datetime representation of this instance
      /// </summary>
      private DateTime dateTime;

      /// <summary>
      /// The standard in which the time is represented
      /// </summary>
      private TimeStandard baseStandard;

      /// <summary>
      /// Initializes a new instance of the Time class.
      /// </summary>
      public Time()
      {
         this.baseStandard = TimeStandard.NotSpecified;
         this.DateTime = new DateTime();
         this.FormatString = "yyyy-doy/hh:mm:ss.fff";
         this.EnableAllChangedNotifications = true;
         this.EnableDateTimeChangedNotification = true;
      }

      /// <summary>
      /// Initializes a new instance of the Time class.
      /// </summary>
      /// <param name="time">The time to which this instance is set</param>
      public Time(Time time)
      {
         this.baseStandard = time.BaseStandard;
         this.DateTime = time.dateTime;
         this.FormatString = "yyyy-doy/hh:mm:ss.fff";
         this.EnableAllChangedNotifications = true;
         this.EnableDateTimeChangedNotification = true;
      }

      /// <summary>
      /// Initializes a new instance of the Time class.
      /// </summary>
      /// <param name="dateTime">The dateTime to which this instance is set</param>
      /// <param name="baseStandard">The standard to which this instance is set</param>
      public Time(DateTime dateTime, TimeStandard baseStandard)
      {
         this.baseStandard = baseStandard;
         this.DateTime = dateTime;
         this.FormatString = string.Empty;
         this.FormatString = "yyyy-doy/hh:mm:ss.fff";
         this.EnableAllChangedNotifications = true;
         this.EnableDateTimeChangedNotification = true;
      }

      /// <summary>
      /// Initializes a new instance of the Time class.
      /// </summary>
      /// <param name="julianDate">The julianDate to which this instance is set</param>
      /// <param name="baseStandard">The standard to which this instance is set</param>
      public Time(double julianDate, TimeStandard baseStandard)
      {
         this.baseStandard = baseStandard;
         this.DateTime = Time.ToDateTime(julianDate);
         this.FormatString = string.Empty;
         this.FormatString = "yyyy-doy/hh:mm:ss.fff";
         this.EnableDateTimeChangedNotification = true;
         this.EnableAllChangedNotifications = true;
      }

      /// <summary>
      /// Initializes a new instance of the Time class.
      /// </summary>
      /// <param name="year">The year to which this instance is set</param>
      /// <param name="doy">The day of year to which this instance is set</param>
      /// <param name="baseStandard">The standard to which this instance is set</param>
      public Time(int year, int doy, int hour, int minute, int second, int millisecond, TimeStandard baseStandard)
      {
         this.baseStandard = baseStandard;
         this.DateTime = (new DateTime(year, 1, 1, hour, minute, second, millisecond)).AddDays(doy - 1);
         this.FormatString = string.Empty;
         this.FormatString = "yyyy-doy/hh:mm:ss.fff";
         this.EnableDateTimeChangedNotification = true;
         this.EnableAllChangedNotifications = true;
      }

      /// <summary>
      /// Initializes a new instance of the Time class.
      /// </summary>
      /// <param name="year">The year to which this instance is set</param>
      /// <param name="month">The month to which this instance is set</param>
      /// <param name="day">The day to which this instance is set</param>
      /// <param name="hour">The hour to which this instance is set</param>
      /// <param name="minute">The minute to which this instance is set</param>
      /// <param name="second">The second to which this instance is set</param>
      /// <param name="millisecond">The millisecond to which this instance is set</param>
      /// <param name="baseStandard">The standard to which this instance is set</param>
      public Time(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeStandard baseStandard)
      {
         this.DateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
         this.baseStandard = baseStandard;
         this.FormatString = "yyyy-doy/hh:mm:ss.fff";
         this.EnableDateTimeChangedNotification = true;
         this.EnableAllChangedNotifications = true;
      }

      #region Events

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      #region Public Static Properties

      /// <summary>
      /// Gets a System.DateTime object that is set to the current date and time on this computer, expressed as the local time.
      /// </summary>
      public static Time Now
      {
         get { return new Time(DateTime.Now, TimeStandard.CoordinatedUniversalTime); }
      }

      /// <summary>
      /// Gets the current date, with the time component set to 00:00:00.
      /// </summary>
      public static Time Today
      {
         get { return new Time(DateTime.Today, TimeStandard.CoordinatedUniversalTime); }
      }

      /// <summary>
      ///  Gets a System.DateTime object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
      /// </summary>
      public static Time UtcNow
      {
         get { return new Time(DateTime.UtcNow, TimeStandard.CoordinatedUniversalTime); }
      }

      #endregion

      #region Public  Properties

      /// <summary>
      /// Gets or sets the time standard
      /// </summary>
      [DesignOnly(true)]
      public TimeStandard BaseStandard
      {
         get
         {
            return this.baseStandard;
         }

         set
         {
            this.baseStandard = value;
         }
      }

      /// <summary>
      /// Gets or sets the date/time
      /// </summary>
      [Provide]
      public virtual DateTime DateTime
      {
         get
         {
            return this.dateTime;
         }

         set
         {
            this.dateTime = value;
            if (!this.DisableCoordinate)
            {
               this.julianDate = Time.ToJulianDate(this.dateTime);
            }

            if (this.EnableDateTimeChangedNotification && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("DateTime"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the day of the month
      /// </summary>
      public int Day
      {
         get
         {
            return this.dateTime.Day;
         }

         set
         {
            this.DateTime = (new DateTime(this.dateTime.Year, this.dateTime.Month, 1, this.dateTime.Hour, this.dateTime.Minute, this.dateTime.Second, this.dateTime.Millisecond)).AddDays(value - 1);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("Day"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the day of the year
      /// </summary>
      public int DayOfYear
      {
         get
         {
            return this.dateTime.DayOfYear;
         }

         set
         {
            this.DateTime = new DateTime(this.dateTime.Year, 1, 1, this.dateTime.Hour, this.dateTime.Minute, this.dateTime.Second, this.dateTime.Millisecond).AddDays(value - 1);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("DayOfYear"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the format string used for parsing
      /// </summary>
      public string FormatString
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the hour [0, 23]
      /// </summary>
      public int Hour
      {
         get
         {
            return this.dateTime.Hour;
         }

         set
         {
            this.DateTime = new DateTime(this.dateTime.Year, this.dateTime.Month, this.dateTime.Day, value, this.dateTime.Minute, this.dateTime.Second, this.dateTime.Millisecond);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("Hour"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the full Julian date 
      /// </summary>
      [Provide]
      public double JulianDate
      {
         get
         {
            return this.julianDate;
         }

         set
         {
            this.julianDate = value;
            this.dateTime = Time.ToDateTime(value);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("JulianDate"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the milliseconds [0, 999]
      /// </summary>
      public int Millisecond
      {
         get
         {
            return this.dateTime.Millisecond;
         }

         set
         {         this.DateTime = new DateTime(this.dateTime.Year, this.dateTime.Month, this.dateTime.Day, this.dateTime.Hour, this.dateTime.Minute, this.dateTime.Second, value);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("Millisecond"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the minute [0,59]
      /// </summary>
      public int Minute
      {
         get
         {
            return this.dateTime.Minute;
         }

         set
         {
            this.DateTime = new DateTime(this.dateTime.Year, this.dateTime.Month, this.dateTime.Day, this.dateTime.Hour, value, this.dateTime.Second, this.dateTime.Millisecond);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("Minute"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the month [1, 12]
      /// </summary>
      public int Month
      {
         get
         {
            return this.dateTime.Month;
         }

         set
         {
            int day = Math.Min(DateTime.DaysInMonth(this.dateTime.Year, value), this.dateTime.Day);
            this.DateTime = new DateTime(this.dateTime.Year, value, day, this.dateTime.Hour, this.dateTime.Minute, this.dateTime.Second, this.dateTime.Millisecond);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("Month"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the second [0,59]
      /// </summary>
      public int Second
      {
         get
         {
            return this.dateTime.Second;
         }

         set
         {
            this.DateTime = new DateTime(this.dateTime.Year, this.dateTime.Month, this.dateTime.Day, this.dateTime.Hour, this.dateTime.Minute, value, this.dateTime.Millisecond);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("Second"));
            }
         }
      }

      /// <summary>
      /// Gets or sets the year
      /// </summary>
      public int Year
      {
         get
         {
            return this.dateTime.Year;
         }

         set
         {
            int day = Math.Min(DateTime.DaysInMonth(value, this.dateTime.Month), this.dateTime.Day);
            this.DateTime = new DateTime(value, this.dateTime.Month, day, this.dateTime.Hour, this.dateTime.Minute, this.dateTime.Second, this.dateTime.Millisecond);
            if (this.EnableAllChangedNotifications && this.PropertyChanged != null)
            {
               this.PropertyChanged(this, new PropertyChangedEventArgs("Year"));
            }
         }
      }

      /// <summary>
      /// Gets the year
      /// </summary>
      public System.DayOfWeek DayOfWeek
      {
         get
         {
            return this.dateTime.DayOfWeek;
         }
      }

      public bool OnLeapSecond
      {
         get;
         set;
      }

      public bool EnableDateTimeChangedNotification
      {
         get;
         set;
      }

      private bool EnableAllChangedNotifications
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets a value indicating whether the base coordinate is updated
      /// </summary>
      internal bool DisableCoordinate
      {
         get;
         set;
      }

      #endregion

      #region Operators

      /// <summary>
      /// Subtracts a specified date and time from another specified date and time
      /// and returns a time interval.
      /// </summary>
      /// <param name="d1">A System.DateTime (the minuend).</param>
      /// <param name="d2">A System.DateTime (the subtrahend).</param>
      /// <returns>A System.TimeSpan that is the time interval between d1 and d2; that is, d1 minus d2.</returns>
      public static TimeSpan operator -(Time d1, Time d2)
      {
         return d1.dateTime - d2.ConvertTo(d1.BaseStandard).dateTime;
      }

      /// <summary>
      /// Subtracts a specified time interval from a specified date and time and returns
      /// a new date and time.
      /// </summary>
      /// <param name="d">A System.DateTime.</param>
      /// <param name="t">A System.TimeSpan.</param>
      /// <returns>A System.DateTime whose value is the value of d minus the value of t.</returns>
      public static Time operator -(Time d, TimeSpan t)
      {
         return new Time(d.dateTime - t, d.BaseStandard);
      }

      /// <summary>
      /// Determines whether two specified instances of System.DateTime are not equal.
      /// </summary>
      /// <param name="d1">The first System.DateTime.</param>
      /// <param name="d2">The second System.DateTime.</param>
      /// <returns>true if d1 and d2 do not represent the same date and time; otherwise, false.</returns>
      public static bool operator !=(Time d1, Time d2)
      {
         return d1.dateTime != d2.ConvertTo(d1.BaseStandard).dateTime;
      }

      /// <summary>
      /// Adds a specified time interval to a specified date and time, yielding a new date and time.
      /// </summary>
      /// <param name="d">A System.DateTime.</param>
      /// <param name="t">A System.TimeSpan.</param>
      /// <returns>A System.DateTime that is the sum of the values of d and t.</returns>
      public static Time operator +(Time d, TimeSpan t)
      {
         return new Time(d.dateTime + t, d.BaseStandard);
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is less than another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is less than t2; otherwise, false.</returns>
      public static bool operator <(Time t1, Time t2)
      {
         return t1.dateTime < t2.ConvertTo(t1.BaseStandard).dateTime;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is less than another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is less than t2; otherwise, false.</returns>
      public static bool operator <(Time t1, DateTime t2)
      {
         return t1.dateTime < t2;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is less than another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is less than t2; otherwise, false.</returns>
      public static bool operator <(DateTime t1, Time t2)
      {
         return t1 < t2.dateTime;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is less than or equal to
      /// another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first Time.</param>
      /// <param name="t2">The second Time.</param>
      /// <returns>true if t1 is less than or equal to t2; otherwise, false.</returns>
      public static bool operator <=(Time t1, Time t2)
      {
         return t1.dateTime <= t2.ConvertTo(t1.BaseStandard).dateTime;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is less than or equal to
      /// another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first Time.</param>
      /// <param name="t2">The second Time.</param>
      /// <returns>true if t1 is less than or equal to t2; otherwise, false.</returns>
      public static bool operator <=(Time t1, DateTime t2)
      {
         return t1.dateTime <= t2;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is less than or equal to
      /// another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first Time.</param>
      /// <param name="t2">The second Time.</param>
      /// <returns>true if t1 is less than or equal to t2; otherwise, false.</returns>
      public static bool operator <=(DateTime t1, Time t2)
      {
         return t1 <= t2.dateTime;
      }

      /// <summary>
      ///  Determines whether two specified instances of System.DateTime are equal.
      /// </summary>
      /// <param name="d1">The first System.DateTime.</param>
      /// <param name="d2">The second System.DateTime.</param>
      /// <returns>true if d1 and d2 represent the same date and time; otherwise, false.</returns>
      public static bool operator ==(Time d1, Time d2)
      {
         if (((object)d1) == null)
         {
            if (((object)d2) == null)
            {
               return true;
            }

            return false;
         }

         if (((object)d2) == null)
         {
            return false;
         }

         return d1.dateTime == d2.ConvertTo(d1.BaseStandard).dateTime;
      }

      /// <summary>
      ///  Determines whether two specified instances of System.DateTime are equal.
      /// </summary>
      /// <param name="d1">The first System.DateTime.</param>
      /// <param name="d2">The second System.DateTime.</param>
      /// <returns>true if d1 and d2 represent the same date and time; otherwise, false.</returns>
      public static bool operator ==(Time d1, DateTime d2)
      {
         return d1.dateTime == d2;
      }

      /// <summary>
      ///  Determines whether two specified instances of System.DateTime are equal.
      /// </summary>
      /// <param name="d1">The first System.DateTime.</param>
      /// <param name="d2">The second System.DateTime.</param>
      /// <returns>true if d1 and d2 represent the same date and time; otherwise, false.</returns>
      public static bool operator ==(DateTime d1, Time d2)
      {
         return d1 == d2.dateTime;
      }

      /// <summary>
      ///  Determines whether two specified instances of System.DateTime are equal.
      /// </summary>
      /// <param name="d1">The first System.DateTime.</param>
      /// <param name="d2">The second System.DateTime.</param>
      /// <returns>true if d1 and d2 represent the same date and time; otherwise, false.</returns>
      public static bool operator !=(Time d1, DateTime d2)
      {
         return d1.dateTime != d2;
      }

      /// <summary>
      ///  Determines whether two specified instances of System.DateTime are equal.
      /// </summary>
      /// <param name="d1">The first System.DateTime.</param>
      /// <param name="d2">The second System.DateTime.</param>
      /// <returns>true if d1 and d2 represent the same date and time; otherwise, false.</returns>
      public static bool operator !=(DateTime d1, Time d2)
      {
         return d1 != d2.dateTime;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is greater than another
      /// specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is greater than t2; otherwise, false.</returns>
      public static bool operator >(Time t1, Time t2)
      {
         return t1.dateTime > t2.ConvertTo(t1.BaseStandard).dateTime;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is greater than another
      /// specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is greater than t2; otherwise, false.</returns>
      public static bool operator >(Time t1, DateTime t2)
      {
         return t1.dateTime > t2;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is greater than another
      /// specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is greater than t2; otherwise, false.</returns>
      public static bool operator >(DateTime t1, Time t2)
      {
         return t1 > t2.dateTime;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is greater than or equal
      /// to another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is earlier or equal to time t2</returns>
      public static bool operator >=(Time t1, Time t2)
      {
         return t1.dateTime >= t2.ConvertTo(t1.BaseStandard).dateTime;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is greater than or equal
      /// to another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is earlier or equal to time t2</returns>
      public static bool operator >=(Time t1, DateTime t2)
      {
         return t1.dateTime >= t2;
      }

      /// <summary>
      /// Determines whether one specified System.DateTime is greater than or equal
      /// to another specified System.DateTime.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns>true if t1 is earlier or equal to time t2</returns>
      public static bool operator >=(DateTime t1, Time t2)
      {
         return t1 >= t2.dateTime;
      }

      #endregion

      #region Static Methods

      /// <summary>
      /// Compares two instances of System.DateTime and returns an integer that indicates
      ///  whether the first instance is earlier than, the same as, or later than the
      ///  second instance.
      /// </summary>
      /// <param name="t1">The first System.DateTime.</param>
      /// <param name="t2">The second System.DateTime.</param>
      /// <returns> 
      /// A signed number indicating the relative values of t1 and t2.Value Type Condition
      /// Less than zero t1 is earlier than t2. Zero t1 is the same as t2. Greater
      /// than zero t1 is later than t2.
      /// </returns>
      public static int Compare(Time t1, Time t2)
      {
         return DateTime.Compare(t1.dateTime, t2.ConvertTo(t1.BaseStandard).dateTime);
      }

      /// <summary>
      /// Returns a value indicating whether two instances of System.DateTime are equal.
      /// </summary>
      /// <param name="t1">The first System.DateTime instance.</param>
      /// <param name="t2">The second System.DateTime instance.</param>
      /// <returns>true if the two System.DateTime values are equal; otherwise, false.</returns>
      public static bool Equals(Time t1, Time t2)
      {
         return t1.dateTime.Equals(t2.ConvertTo(t1.BaseStandard));
      }

      /// <summary>
      /// Parses the string 
      /// </summary>
      /// <param name="time"> time instance intialized with date/time parsed from input string</param>
      /// <param name="inDate">A string representing a date/time</param>
      public static bool TryParse(Time time, string inDate)
      {
         DateTime dateTime;
         if (DateTime.TryParse(inDate, out dateTime))
         {
            time.DateTime = dateTime;
            return true;
         }

         // Why? Why?? Why must we still parse???
         bool isDoy = false;
         int year = 0;
         int month = 0;
         int doy = 0;
         int dom = 0;
         int hour = 0;
         int minute = 0;
         int second = 0;
         int millisecond = 0;
         string format = time.FormatString;

         for (int i = 0; i < format.Length; i++)
         {
            switch (format[i])
            {
               case 'd':

                  if ((i + 1) < format.Length && format[i + 1] == 'd')
                  {
                     dom = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else if ((i + 2) < format.Length && format[i + 1] == 'o' && format[i + 2] == 'y')
                  {
                     isDoy = true;
                     doy = int.Parse(inDate.Substring(i, 3));
                     i += 2;
                  }

                  break;

               case 'y':

                  if ((i + 3) < format.Length && format[i + 1] == 'y' && format[i + 2] == 'y' && format[i + 3] == 'y')
                  {
                     year = int.Parse(inDate.Substring(i, 4));
                     i += 3;
                  }
                  else if ((i + 1) < format.Length && format[i + 1] == 'y')
                  {
                     year = 2000 + int.Parse(inDate.Substring(i, 2));
                     i++;
                  }

                  break;

               case 'h':

                  if ((i + 1) < format.Length && format[i + 1] == 'h')
                  {
                     hour = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else
                  {
                     hour = int.Parse(inDate.Substring(i, 1));
                  }

                  break;

               case 'm':

                  if ((i + 1) < format.Length && format[i + 1] == 'm')
                  {
                     minute = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else
                  {
                     minute = int.Parse(inDate.Substring(i, 1));
                  }

                  break;

               case 'M':

                  if ((i + 1) < format.Length && format[i + 1] == 'M')
                  {
                     month = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else
                  {
                     month = int.Parse(inDate.Substring(i, 1));
                  }

                  break;

               case 's':

                  if ((i + 1) < format.Length && format[i + 1] == 's')
                  {
                     second = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }

                  if ((i + 1) < format.Length && format[i + 1] == '.')
                  {
                     i++;
                     if ((i + 1) < format.Length && format[i + 1] == 'f')
                     {
                        i++;
                        int decimalCount = 0;
                        do
                        {
                           short digit = short.Parse(new string(new char[] { inDate[i] }));
                           millisecond = 10 * millisecond + digit;
                           i++;
                           decimalCount++;
                        }
                        while (i < format.Length && format[i] == 'f');
                     }
                  }

                  break;
            }
         }

         try
         {
            if (isDoy)
            {
               time.DateTime = new DateTime(year, 1, 1, hour, minute, second, millisecond).AddDays(doy - 1);
            }
            else
            {
               time.DateTime = new DateTime(year, month, dom, hour, minute, second, millisecond);
            }
         }
         catch
         {
            return false;
         }

         return true;
      }

      /// <summary>
      /// Parses the string 
      /// </summary>
      /// <param name="time"> time instance intialized with date/time parsed from input string</param>
      /// <param name="inDate">A string representing a date/time</param>
      public static bool TryParse(Time time, string inDate, string format)
      {
         DateTime dateTime;
         if (DateTime.TryParse(inDate, out dateTime))
         {
            time.DateTime = dateTime;
         }

         // Why? Why?? Why must we still parse???
         bool isDoy = false;
         int year = 0;
         int month = 0;
         int doy = 0;
         int dom = 0;
         int hour = 0;
         int minute = 0;
         int second = 0;
         int millisecond = 0;

         for (int i = 0; i < format.Length; i++)
         {
            switch (format[i])
            {
               case 'd':

                  if ((i + 1) < format.Length && format[i + 1] == 'd')
                  {
                     dom = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else if ((i + 2) < format.Length && format[i + 1] == 'o' && format[i + 2] == 'y')
                  {
                     isDoy = true;
                     doy = int.Parse(inDate.Substring(i, 3));
                     i += 2;
                  }

                  break;

               case 'y':

                  if ((i + 3) < format.Length && format[i + 1] == 'y' && format[i + 2] == 'y' && format[i + 3] == 'y')
                  {
                     year = int.Parse(inDate.Substring(i, 4));
                     i += 3;
                  }
                  else if ((i + 1) < format.Length && format[i + 1] == 'y')
                  {
                     year = 2000 + int.Parse(inDate.Substring(i, 2));
                     i++;
                  }

                  break;

               case 'h':

                  if ((i + 1) < format.Length && format[i + 1] == 'h')
                  {
                     hour = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else
                  {
                     hour = int.Parse(inDate.Substring(i, 1));
                  }

                  break;

               case 'm':

                  if ((i + 1) < format.Length && format[i + 1] == 'm')
                  {
                     minute = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else
                  {
                     minute = int.Parse(inDate.Substring(i, 1));
                  }

                  break;

               case 'M':

                  if ((i + 1) < format.Length && format[i + 1] == 'M')
                  {
                     month = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }
                  else
                  {
                     month = int.Parse(inDate.Substring(i, 1));
                  }

                  break;

               case 's':

                  if ((i + 1) < format.Length && format[i + 1] == 's')
                  {
                     second = int.Parse(inDate.Substring(i, 2));
                     i++;
                  }

                  if ((i + 1) < format.Length && format[i + 1] == '.')
                  {
                     i++;
                     if ((i + 1) < format.Length && format[i + 1] == 'f')
                     {
                        i++;
                        int decimalCount = 0;
                        do
                        {
                           short digit = short.Parse(new string(new char[] { inDate[i] }));
                           millisecond = 10 * millisecond + digit;
                           i++;
                           decimalCount++;
                        }
                        while (i < format.Length && format[i] == 'f');
                     }
                  }

                  break;
            }
         }

         try
         {
            if (isDoy)
            {
               time.dateTime = new DateTime(year, 1, 1, hour, minute, second, millisecond).AddDays(doy - 1);
            }
            else
            {
               time.dateTime = new DateTime(year, month, dom, hour, minute, second, millisecond);
            }
         }
         catch
         {
            return false;
         }

         return true;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Returns a new System.DateTime that adds the value of the specified System.TimeSpan
      /// to the value of this instance.
      /// </summary>
      /// <param name="time">A Time object to be subtraced from this instance.</param>
      /// <returns>
      ///  the elapsed seconds between the times.
      /// </returns>
      [Provide]
      public double ElapsedSeconds(Time time)
      {
         Time converted = time.ConvertTo(this.BaseStandard);
         switch(this.BaseStandard)
         {
            case TimeStandard.CoordinatedUniversalTime:
               return (this.JulianDate - converted.JulianDate) * Time.SecondsPerDay + Time.GetLeapSeconds(this.JulianDate) - Time.GetLeapSeconds(converted.JulianDate);
            default:
               return (this.JulianDate - converted.JulianDate) * Time.SecondsPerDay;
         }
      }

      /// <summary>
      /// Returns a new System.DateTime that adds the value of the specified System.TimeSpan
      /// to the value of this instance.
      /// </summary>
      /// <param name="value">A System.TimeSpan object that represents a positive or negative time interval.</param>
      /// <returns>
      ///  A System.DateTime whose value is the sum of the date and time represented
      ///  by this instance and the time interval represented by value.
      /// </returns>
      public Time Add(TimeSpan value)
      {
         return new Time(this.dateTime.Add(value), this.BaseStandard);
      }

      /// <summary>
      /// Returns a new System.DateTime that adds the specified number of days to the
      /// value of this instance.
      /// </summary>
      /// <param name="value">A number of whole and fractional days. The value parameter can be negative or positive.</param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented by this instance and the number of
      /// days represented by value.
      /// </returns>
      public Time AddDays(double value)
      {
         return new Time(this.dateTime.AddDays(value), this.BaseStandard);
      }

      /// <summary>
      /// Returns a new System.DateTime that adds the specified number of hours to
      /// the value of this instance.
      /// </summary>
      /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented
      /// by this instance and the number of hours represented by value.
      /// </returns>
      public Time AddHours(double value)
      {
         return new Time(this.dateTime.AddHours(value), this.BaseStandard);
      }

      /// <summary>
      ///  Returns a new System.DateTime that adds the specified number of milliseconds
      ///  to the value of this instance.
      /// </summary>
      /// <param name="value">
      /// A number of whole and fractional milliseconds. The value parameter can be
      /// by this instance and the number of milliseconds represented by value.
      /// </param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented
      /// by this instance and the number of milliseconds represented by value.
      /// </returns>
      public Time AddMilliseconds(double value)
      {
         return new Time(this.dateTime.AddMilliseconds(value), this.BaseStandard);
      }

      /// <summary>
      /// Returns a new System.DateTime that adds the specified number of minutes to
      ///  the value of this instance.
      /// </summary>
      /// <param name="value">
      /// A number of whole and fractional minutes. The value parameter can be negative
      /// or positive.
      /// </param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented
      /// by this instance and the number of minutes represented by value.
      /// </returns>
      public Time AddMinutes(double value)
      {
         return new Time(this.dateTime.AddMinutes(value), this.BaseStandard);
      }

      /// <summary>
      /// Returns a new System.DateTime that adds the specified number of months to
      /// the value of this instance.
      /// </summary>
      /// <param name="months">A number of months. The months parameter can be negative or positive.</param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented
      /// by this instance and months.
      /// </returns>
      public Time AddMonths(int months)
      {
         return new Time(this.dateTime.AddMonths(months), this.BaseStandard);
      }

      /// <summary>
      /// Returns a new System.DateTime that adds the specified number of seconds to
      /// the value of this instance.
      /// </summary>
      /// <param name="value">A number of whole and fractional seconds. The value parameter can be negative or positive.</param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented
      /// by this instance and the number of seconds represented by value.
      /// </returns>
      public Time AddSeconds(double value)
      {
         return new Time(this.dateTime.AddSeconds(value), this.BaseStandard);
      }

      /// <summary>
      ///  Returns a new System.DateTime that adds the specified number of ticks to
      ///  the value of this instance.
      /// </summary>
      /// <param name="value">A number of 100-nanosecond ticks. The value parameter can be positive or negative.</param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented
      /// by this instance and the time represented by value.
      /// </returns>
      public Time AddTicks(long value)
      {
         return new Time(this.dateTime.AddTicks(value), this.BaseStandard);
      }

      /// <summary>
      /// Returns a new System.DateTime that adds the specified number of years to
      /// the value of this instance.
      /// </summary>
      /// <param name="value">A number of years. The value parameter can be negative or positive.</param>
      /// <returns>
      /// A System.DateTime whose value is the sum of the date and time represented
      /// by this instance and the number of years represented by value.
      /// </returns>
      public Time AddYears(int value)
      {
         return new Time(this.dateTime.AddYears(value), this.BaseStandard);
      }

      /// <summary>
      /// Compares the value of this instance to a specified System.DateTime value
      /// and returns an integer that indicates whether this instance is earlier than,
      /// the same as, or later than the specified System.DateTime value.
      /// </summary>
      /// <param name="value">A System.DateTime object to compare.</param>
      /// <returns>
      /// A signed number indicating the relative values of this instance and the value
      /// parameter.Value Description Less than zero This instance is earlier than
      /// value. Zero This instance is the same as value. Greater than zero This instance
      /// is later than value.
      /// </returns>
      public int CompareTo(object value)
      {
         Time time = (Time)value;
         return this.dateTime.CompareTo(time.ConvertTo(this.BaseStandard).dateTime);
      }

      /// <summary>
      /// Converts time between time standards (for UT0 use ConvertTo defined below)
      /// </summary>
      /// <param name="standard">The time standard which to convert this instance</param>
      /// <returns>a Time instance representing the converted time represented by this instance</returns>
      public Time ConvertTo(TimeStandard standard)
      {
         if (standard == this.baseStandard)
         {
            return this;
         }

         if (this.baseStandard == TimeStandard.NotSpecified || standard == TimeStandard.NotSpecified)
         {
            return this;
         }

         return Time.conversions[(int)this.baseStandard, (int)standard](this);
      }

      /// <summary>
      /// Converts time between a given time standard and UT0
      /// </summary>
      /// <param name="standard">The time standard which to convert this instance</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>a Time instance representing the converted time represented by this instance</returns>
      public Time ConvertTo(TimeStandard standard, double longitude, double latitude)
      {
         if (standard == this.baseStandard)
         {
            return this;
         }

         if (standard == TimeStandard.UniversalTime0)
         {
            switch (this.baseStandard)
            {
               case TimeStandard.BarycentricDynamicalTime: TDBToUT0(this, longitude, latitude); break;
               case TimeStandard.CoordinatedUniversalTime: UTCToUT0(this, longitude, latitude); break;
               case TimeStandard.InternationalAtomicTime: TAIToUT0(this, longitude, latitude); break;
               case TimeStandard.TerrestrialDynamicalTime: TDTToUT0(this, longitude, latitude); break;
               case TimeStandard.UniversalTime1: UT1ToUT0(this, longitude, latitude); break;
               case TimeStandard.UniversalTime2: UT2ToUT0(this, longitude, latitude); break;
            }
         }
         else
         {
            switch (standard)
            {
               case TimeStandard.BarycentricDynamicalTime: UT0ToTDB(this, longitude, latitude); break;
               case TimeStandard.CoordinatedUniversalTime: UT0ToUTC(this, longitude, latitude); break;
               case TimeStandard.InternationalAtomicTime: UT0ToTAI(this, longitude, latitude); break;
               case TimeStandard.TerrestrialDynamicalTime: UT0ToTDT(this, longitude, latitude); break;
               case TimeStandard.UniversalTime1: UT0ToUT1(this, longitude, latitude); break;
               case TimeStandard.UniversalTime2: UT0ToUT2(this, longitude, latitude); break;
            }
         }

         return Time.conversions[(int)this.baseStandard, (int)standard](this);
      }

      /// <summary>
      ///  Returns a value indicating whether this instance is equal to the specified System.DateTime instance.
      /// </summary>
      /// <param name="value">A System.DateTime instance to compare to this instance.</param>
      /// <returns>
      /// true if the value parameter equals the value of this instance; otherwise,
      /// false.
      /// </returns>
      public bool Equals(Time value)
      {
         return this.dateTime.Equals(value.ConvertTo(this.BaseStandard));
      }

      /// <summary>
      /// Returns a value indicating whether this instance is equal to a specified object.
      /// </summary>
      /// <param name="value">An object to compare to this instance.</param>
      /// <returns>
      /// true if value is an instance of System.DateTime and equals the value of this
      /// instance; otherwise, false.
      /// </returns>
      public override bool Equals(object value)
      {
         return this.dateTime.Equals(value);
      }

      /// <summary>
      ///  Returns the hash code for this instance.
      /// </summary>
      /// <returns>A 32-bit signed integer hash code.</returns>
      public override int GetHashCode()
      {
         return this.dateTime.GetHashCode();
      }

      /// <summary>
      /// Indicates whether this instance of System.DateTime is within the Daylight
      /// Saving Time range for the current time zone.
      /// </summary>
      /// <returns>
      /// true if System.DateTime.Kind is System.DateTimeKind.Local or System.DateTimeKind.Unspecified
      /// and the value of this instance of System.DateTime is within the Daylight
      /// Saving Time range for the current time zone. false if System.DateTime.Kind
      /// is System.DateTimeKind.Utc.
      /// </returns>
      public bool IsDaylightSavingTime()
      {
         return this.dateTime.IsDaylightSavingTime();
      }

      /// <summary>
      /// Subtracts the specified date and time from this instance.
      /// </summary>
      /// <param name="value">An instance of System.DateTime.</param>
      /// <returns>
      /// A System.TimeSpan interval equal to the date and time represented by this
      /// instance minus the date and time represented by value.
      /// </returns>
      public TimeSpan Subtract(Time value)
      {
         return this.dateTime.Subtract(value.ConvertTo(this.BaseStandard).dateTime);
      }

      /// <summary>
      /// Subtracts the specified duration from this instance.
      /// </summary>
      /// <param name="value">An instance of System.TimeSpan.</param>
      /// <returns>
      /// A System.DateTime equal to the date and time represented by this instance
      /// minus the time interval represented by value.
      /// </returns>
      public Time Subtract(TimeSpan value)
      {
         return new Time(this.dateTime.Subtract(value), this.BaseStandard);
      }

      /// <param name="inDate">A string representing a date/time</param>
      public override string ToString()
      {
         if(this.OnLeapSecond)
         {
            DateTime temp = this.DateTime.AddSeconds(-1);
            return temp.Year.ToString("0000") + "/" + temp.DayOfYear.ToString("000") + ":" + temp.Hour.ToString("00") + ":" + temp.Minute.ToString("00") + ":60." + this.Millisecond.ToString("000");
         }

         return this.Year.ToString("0000") + "/" + this.DayOfYear.ToString("000") + ":" + this.Hour.ToString("00") + ":" + this.Minute.ToString("00") + ":" + this.Second.ToString("00") + "." + this.Millisecond.ToString("000");
      }

      #endregion
   }
}
