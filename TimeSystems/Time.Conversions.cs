
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2012-07-17</date>
// <summary>Class representing time in a specified standard</summary>

namespace GES.TimeSystems
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.Configuration;
   using System.IO;

   #endregion

   /// <summary>
   /// Time standards
   /// </summary>
   public enum TimeStandard : int
   {
      /// <summary>
      /// Identifies the barycentric dynamical time system
      /// </summary>
      BarycentricDynamicalTime = 0,

      /// <summary>
      /// Identifies the coordinated universal time system
      /// </summary>
      CoordinatedUniversalTime = 1,

      /// <summary>
      /// Identifies the terrestrial dynamical time system
      /// </summary>
      InternationalAtomicTime = 2,

      /// <summary>
      /// Identifies the terrestrial dynamical time system
      /// </summary>
      TerrestrialDynamicalTime = 3,

      /// <summary>
      /// Identifies the universal time (UT1) system
      /// </summary>
      UniversalTime1 = 4,

      /// <summary>
      /// Identifies the universal time (UT2) system
      /// </summary>
      UniversalTime2 = 5,

      /// <summary>
      /// Standard not specified
      /// </summary>
      UniversalTime0 = 6,
      
      /// <summary>
      /// Standard not specified
      /// </summary>
      NotSpecified = 7,
   }

   /// <summary>
   /// Class representing a time standard.
   /// </summary>
   public partial class Time
   {
      #region Fields
      /// <summary>
      /// Conversion delegates indexed by time standard identifiers
      /// </summary>
      private static TimeConversion[,] conversions = new TimeConversion[,]
      {
         { null,     TDBToUTC, TDBToTAI, TDBToTDT, TDBToUT1, TDBToUT2 },
         { UTCToTDB, null,     UTCToTAI, UTCToTDT, UTCToUT1, UTCToUT2 },
         { TAIToTDB, TAIToUTC, null,     TAIToTDT, TAIToUT1, TAIToUT2 },
         { TDTToTDB, TDTToUTC, TDTToTAI, null,     TDTToUT1, TDTToUT2 },
         { UT1ToTDB, UT1ToUTC, UT1ToTAI, UT1ToTDT, null,     UT1ToUT2 },
         { UT2ToTDB, UT2ToUTC, UT2ToTAI, UT2ToTDT, UT2ToUT1, null     },
      };

      /// <summary>
      /// Conversion parameters relating international atomic time with coordinated universal time, see http://maia.usno.navy.mil/ser7/tai-utc.dat
      /// </summary>
      private static List<KeyValuePair<double, double[]>> atomicTimeCorrections = Time.ParseTAI_UTCFile(Time.GetConfigurationPath("Time.TAI-UTC", "SystemManager"));

      /// <summary>
      /// A function table with time as the abscissa and coordinated universal time corrections as the ordinates 
      /// </summary>
      private static List<KeyValuePair<double, double[]>> utcCorrections = Time.ParseIERSBulletinAFile(Time.GetConfigurationPath("Time.UTCCorrections", "SystemManager"));

      #endregion

      #region Delegates

      /// <summary>
      /// Signature to which all time conversion delegates are defined.
      /// </summary>
      /// <param name="fromTime">The time to be converted</param>
      /// <returns>The converted time corresponding to the given time</returns>
      public delegate Time TimeConversion(Time fromTime);

      #endregion

      #region Public Methods

      /// <summary>
      /// Normalizes an angular quantity within the range of -PI to PI
      /// </summary>
      /// <param name="angle">angle to be normalized in radians</param>
      /// <returns>the normalized angle</returns>
      public static double Normalize(double angle)
      {
         if (angle > 0)
         {
            return angle - (2.0 * Math.PI * Math.Truncate((angle + Math.PI) / (2.0 * Math.PI)));
         }
         else
         {
            return angle + (2.0 * Math.PI * Math.Truncate((-angle + Math.PI) / (2.0 * Math.PI)));
         }
      }

      /// <summary>
      /// Normalizes an angular quantity within the range of 0 degrees to 2 * PI
      /// </summary>
      /// <param name="angle">angle to be normalized in radians</param>
      /// <returns>the normalized angle</returns>
      public static double NormalizePositive(double angle)
      {
         if (angle > 0)
         {
            return angle - (2.0 * Math.PI * Math.Truncate(angle / (2.0 * Math.PI)));
         }
         else
         {
            return 2.0 * Math.PI + angle + (2.0 * Math.PI * Math.Truncate(-angle / (2.0 * Math.PI)));
         }
      }

      /// <summary>
      /// Converts the Julian date in seconds to year, month, and day.
      /// </summary>
      /// <param name="julianDate">the julian date in seconds</param>
      /// <returns>The DateTime instance corresponding the julian date</returns>
      public static DateTime ToDateTime(double julianDate)
      {
         int year, month, day, hour, minute;
         double seconds;
         Time.ToYearMonthDayHourMinuteSeconds(julianDate, out year, out month, out day, out hour, out minute, out seconds);
         try
         {
            return new DateTime(year, month, day, hour, minute, (int)seconds, (int)Time.ToMilliseconds(seconds - (int)seconds), DateTimeKind.Utc);
         }
         catch
         {
            return new DateTime();
         }
      }

      /// <summary>
      /// Converts the day into day, hour, minutes, and seconds.
      /// </summary>
      /// <param name="dbl_day">The input day</param>
      /// <param name="day">The output day</param>
      /// <param name="hours">The output hours</param>
      /// <param name="minutes">The output minutes</param>
      /// <param name="seconds">The output seconds</param>
      public static void ToDayHourMinuteSeconds(double dbl_day, out int day, out int hours, out int minutes, out double seconds)
      {
         double hms, mm;
         double dayFraction;
         double hourFraction;
         double minFraction;

         if (dbl_day == 0.0)
         {
            day = 0;
            hours = 0;
            minutes = 0;
            seconds = 0.0;
         }
         else
         {
            dayFraction = dbl_day % 1.0; 
            day = (int)dbl_day; 
            hms = dayFraction * 24.0;
            hourFraction = hms % 1.0;
            hours = (int)hms;

            mm = hourFraction * 60.0;
            minFraction = mm % 1.0;
            minutes = (int)mm;
            seconds = minFraction * 60.0;
            seconds += 1e-04;
            if (seconds >= 60.0)
            {
               minutes += 1;
               seconds -= 60.0;
               if (minutes >= 60)
               {
                  minutes = 0;
                  hours++;
                  if (hours >= 24)
                  {
                     hours = 0;
                     day++;
                  }
               }
            }
         }
      }

      /// <summary>
      /// Converts the hours into integral hours, minutes, and seconds
      /// </summary>
      /// <param name="hours">The output day</param>
      /// <param name="hour">The output hours</param>
      /// <param name="minutes">The output minutes</param>
      /// <param name="seconds">The output seconds</param>
      public static void ToHourMinuteSeconds(double hours, out int hour, out int minutes, out double seconds)
      {
         hour = (int)Math.Truncate(hours);
         minutes = (int)Math.Truncate((hours - hour) * 60);
         seconds = (((hours - hour) * 60) - minutes) * 60;
      }

      /// <summary>
      /// Converts the hours, minutes, and seconds to hours.
      /// </summary>
      /// <param name="hours">The input hours</param>
      /// <param name="minutes">The input minutes</param>
      /// <param name="seconds">The input seconds</param>
      /// <returns>The floating point hours corresponding to the given hours, minutes, and seconds.</returns>
      public static double ToHours(int hours, int minutes, double seconds)
      {
         return hours + (minutes + seconds / 60d) / 60d;
      }

      /// <summary>
      /// Converts date and time to Julian date in days
      /// </summary>
      /// <param name="dateTime">The date and time</param>
      /// <returns>The Julian date in days</returns>
      public static double ToJulianDate(DateTime dateTime)
      {
         return ToJulianDate(dateTime.Year, dateTime.Month, dateTime.Day)
               + (  dateTime.Hour * ArcsecondsPerHourAngle 
                  + dateTime.Minute * ArcsecondsPerArcminute 
                  + dateTime.Second + dateTime.Millisecond / 1000.0
                  ) / SecondsPerDay;
      }

      /// <summary>
      /// Converts the time to Julian date
      /// </summary>
      /// <param name="time">The input time time</param>
      /// <returns>The Julian date in days</returns>
      public static double ToJulianDate(Time time)
      {
         return ToJulianDate(time.DateTime);
      }

      /// <summary>
      /// Converts year, month, and day to the Julian date in days.
      /// </summary>
      /// <param name="year">The Gregorian calendar year</param>
      /// <param name="month">The month (1-12)</param>
      /// <param name="day">The day (1-31)</param>
      /// <returns>The Julian date in days</returns>
      public static double ToJulianDate(int year, int month, int day)
      {  // Astronomical Algorithms by Jean Meeus 1991 pg 60-61
         int y, m, d;
         int a, b;
         double jd;

         d = day;

         if ((month == 2) || (month == 1))
         {  // if Jan or Feb, it is the 13th or 14th month of the preceeding Year
            y = year - 1;   
            m = month + 12; 
         }
         else
         {
            y = year;
            m = month;
         }

         a = (int)(y / 100.0);
         b = 2 - a + (int)(a / 4.0);

         jd = (double)((int)(365.25 * (y + 4716)) + (int)(30.6001 * (m + 1)) + d + b) - 1524.5;

         if (jd < 2299191.5)
         { // if we want Julian Calendar, take b = 0 (October 15,1582)
            jd -= b;
         }

         return jd;
      }

      /// <summary>
      /// Gets the fraction of the second in milliseconds
      /// </summary>
      /// <param name="seconds">the input seconds</param>
      /// <returns>The output milliseconds</returns>
      public static double ToMilliseconds(double seconds)
      {
         return (seconds - (int)seconds) * 1000;
      }

      /// <summary>
      /// Converts the given Julian date to a Time object
      /// </summary>
      /// <param name="julianDate">The julian date</param>
      /// <param name="standard">The time standard associated with the Julian date.</param>
      /// <returns>A time object initialized to the time/date associated with the given Julian date</returns>
      public static Time ToTime(double julianDate, TimeStandard standard)
      {
         int year, month, day, hour, minute;
         double seconds;
         Time.ToYearMonthDayHourMinuteSeconds(julianDate, out year, out month, out day, out hour, out minute, out seconds);
         return new Time(new DateTime(year, month, day, hour, minute, (int)seconds, (int)Time.ToMilliseconds(seconds - (int)seconds)), standard);
      }

      /// <summary>
      /// Converts the Julian date in seconds to year, month, and day.
      /// </summary>
      /// <param name="julianDate">The julian date in seconds</param>
      /// <param name="year">The output year</param>
      /// <param name="month">The output month(1-12)</param>
      /// <param name="day">The output day(1-31)</param>
      public static void ToYearMonthDay(double julianDate, out int year, out int month, out double day)
      { // Astronomical Algorithms by Jean Meeus 1991, pg 63
         double jd, z, f;
         int a, b, c, d, e, alpha;

         jd = julianDate + 0.5;

         f = jd % 1.0;
         z = (int)jd; 
         if (z < 2299161)
         {
            a = (int)z;
         }
         else
         {
            alpha = (int)((z - 1867216.25) / 36524.25);
            a = (int)z + 1 + alpha - (int)(alpha / 4.0);
         }

         b = a + 1524;
         c = (int)(((double)b - 122.1) / 365.25);
         d = (int)(365.25 * (double)c);
         e = (int)(((double)(b - d)) / 30.6001);

         day = (double)(b - d - (int)(30.6001 * (double)e) + f);

         if (e < 14)
         {
            month = e - 1;
         }
         else if ((e == 14) || (e == 15))
         {
            month = e - 13;
         }
         else
         {
            month = 0;
         }

         if (month > 2)
         {
            year = c - 4716;
         }
         else if ((month == 1) || (month == 2))
         {
            year = c - 4715;
         }
         else
         {
            year = 0;
         }
      }

      /// <summary>
      /// Converts the Julian date in seconds to year, month, and day.
      /// </summary>
      /// <param name="julianDate">The julian date in seconds</param>
      /// <param name="year">The output year</param>
      /// <param name="month">The output month(1-12)</param>
      /// <param name="day">The output day(1-31)</param>
      /// <param name="hour">The output hours</param>
      /// <param name="minute">The output minutes</param>
      /// <param name="seconds">The output seconds</param>
      public static void ToYearMonthDayHourMinuteSeconds(double julianDate, out int year, out int month, out int day, out int hour, out int minute, out double seconds)
      {
         double days;
         Time.ToYearMonthDay(julianDate, out year, out month, out days);
         Time.ToDayHourMinuteSeconds(days, out day, out hour, out minute, out seconds);
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Enables specifying relative paths for directories in the AppConfig file
      /// </summary>
      /// <param name="key">the key of the directory path</param>
      /// <returns>the directory path</returns>
      private static string GetConfigurationPath(string key, string executableName)
      {
         ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
         fileMap.ExeConfigFilename = System.Environment.CurrentDirectory + @"\" + executableName + ".exe.config";
         if (!File.Exists(fileMap.ExeConfigFilename))
         {
            fileMap.ExeConfigFilename = System.Environment.CurrentDirectory + @"\" + executableName + ".dll.config";
            if (!File.Exists(fileMap.ExeConfigFilename))
            {
               if ((new List<string>(System.Configuration.ConfigurationManager.AppSettings.AllKeys)).Contains(key))
               {
                  return ConfigurationManager.AppSettings[key].Trim();
               }
               else
               {
                  throw new Exception("Configuration file required :" + fileMap.ExeConfigFilename);
               }
            }
         }

         Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
         string path = configuration.AppSettings.Settings[key].Value.Trim();
         if (string.IsNullOrEmpty(path))
         {
            return null;
         }
         else if (path.StartsWith(".."))
         {
            string exePath = configuration.FilePath;
            while (path.StartsWith(".."))
            {
               path = path.Substring(2, path.Length - 2);
               while (path.StartsWith("\\") || path.StartsWith("/"))
               {
                  path = path.Substring(1, path.Length - 1);
               }

               exePath = exePath.Substring(0, exePath.LastIndexOf("\\"));
            }

            path = exePath + "\\" + path;
         }
         else if (path.StartsWith("."))
         {
            return Path.GetDirectoryName(configuration.FilePath) + "\\" + Path.GetFileName(path);
         }

         return path;
      }

      /// <summary>
      /// Extracts the correction data from the given function table associated with the given julian date.
      /// </summary>
      /// <param name="table">The function table of ordinate corrections corresponding to abscissa julian date</param>
      /// <param name="julianDate">The abscissa associated with the target ordinate corrections</param>
      /// <returns>The ordinate corrections corresponding to the given julian date</returns>
      public static int GetLeapSeconds(double julianDate)
      {
         int index = Search(julianDate, atomicTimeCorrections);
         if (index > 0)
         {
            return (int)atomicTimeCorrections[index].Value[0];
         }

         return (julianDate > atomicTimeCorrections[atomicTimeCorrections.Count - 1].Key) ? (int)atomicTimeCorrections[atomicTimeCorrections.Count - 1].Value[0] : (int)atomicTimeCorrections[0].Value[0];
      }

      /// <summary>
      /// Return a value indicating whether the given time occurs at a leap second epoch
      /// </summary>
      /// <param name="utc">The time to be tested</param>
      /// <returns>true if the given time occurs at within a leap second interval false</returns>
      public static bool InLeapSecondInterval(Time utc)
      {
         Time truncateSubseconds = new Time(utc.Year, utc.DayOfYear, utc.Hour, utc.Minute, utc.Second, 0, TimeStandard.CoordinatedUniversalTime);
         double julianDate = truncateSubseconds.JulianDate;
         for (int i = atomicTimeCorrections.Count - 1; i >= 0; i--)
         {
            if(julianDate == atomicTimeCorrections[i].Key)
            {
               return true;
            }
         }

         return false;
      }

      /// <summary>
      /// Extracts the correction data from the given function table associated with the given julian date.
      /// </summary>
      /// <param name="table">The function table of ordinate corrections corresponding to abscissa julian date</param>
      /// <param name="julianDate">The abscissa associated with the target ordinate corrections</param>
      /// <returns>The ordinate corrections corresponding to the given julian date</returns>
      private static double[] GetCorrections(List<KeyValuePair<double, double[]>> table, double julianDate)
      {
         int index = Search(julianDate, table);
         if (index > 0)
         {
            return (double[])table[index].Value;
         }

         return (julianDate > table[table.Count - 1].Key) ? table[table.Count - 1].Value : table[0].Value;
      }

      /// <summary>
      /// Bisection search that locates closest abscissa less than given value.
      /// </summary>
      /// <param name="x">The independent value.</param>
      /// <returns>The index of the closest abscissa in the total Table less than the independent value.</returns>
      private static int Search(double x, List<KeyValuePair<double, double[]>> entries)
      {
         if (x < entries[0].Key || x > entries[entries.Count - 1].Key)
         {
            return -1;
         }

         int high = entries.Count, low = 0;

         while ((high - low) > 1)
         {
            int mid = (high + low) / 2;
            if (x >= entries[mid].Key)
            {
               low = mid;
            }
            else
            {
               high = mid;
            }
         }

         return low;
      }

      /// <summary>
      /// Parses the tai-utc.dat file into a sorted list of dates and conversion parameters (http://maia.usno.navy.mil/)
      /// </summary>
      /// <param name="filePath">The fully qualified path to the IERS bulletin file</param>
      /// <returns>A sorted list of dates and conversion parameters</returns>
      private static List<KeyValuePair<double, double[]>> ParseTAI_UTCFile(string filePath)
      {
         List<KeyValuePair<double, double[]>> utcCorrections = new List<KeyValuePair<double, double[]>>();
         try
         {
            using (StreamReader reader = new StreamReader(filePath))
            {
               string line;
               while ((line = reader.ReadLine()) != null)
               {
                  try
                  {
                     string[] tokens = line.Split(new string[] { "=JD", "TAI-UTC=", "S + (MJD - ", ") X" }, StringSplitOptions.RemoveEmptyEntries);
                     double julianDate = double.Parse(tokens[1].Trim());
                     double c0 = double.Parse(tokens[2].Trim());
                     double c1 = double.Parse(tokens[3].Trim());
                     double c2 = double.Parse(tokens[4].Split(new string[] { "S" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim());

                     utcCorrections.Add(new KeyValuePair<double, double[]>(julianDate, new double[] { c0, c1, c2 }));
                  }
                  catch(Exception ex)
                  {
                     return utcCorrections;
                  }
               }
            }
         }
         catch
         {
            return utcCorrections;
         }

         return utcCorrections;
      }

      /// <summary>
      /// Parses the IERS bulletin file into a sorted list of dates and conversion parameters (http://maia.usno.navy.mil/)
      /// </summary>
      /// <param name="filePath">The fully qualified path to the IERS bulletin file</param>
      /// <returns>A sorted list of dates and conversion parameters</returns>
      private static List<KeyValuePair<double, double[]>> ParseIERSBulletinAFile(string filePath)
      {
         // The format of the finals2000A.data, finals2000A.daily, and finals2000A.all files is:
         // Col.#    Format  Quantity
         // -------  ------  -------------------------------------------------------------
         // 1-2      I2      year (to get true calendar year, add 1900 for MJD<=51543 or add 2000 for MJD>=51544)
         // 3-4      I2      month number
         // 5-6      I2      day of month
         // 7        X       [blank]
         // 8-15     F8.2    fractional Modified Julian Date (MJD UTC)
         // 16       X       [blank]
         // 17       A1      IERS (I) or Prediction (P) flag for Bull. A polar motion values
         // 18       X       [blank]
         // 19-27    F9.6    Bull. A PM-x (sec. of arc)
         // 28-36    F9.6    error in PM-x (sec. of arc)
         // 37       X       [blank]
         // 38-46    F9.6    Bull. A PM-y (sec. of arc)
         // 47-55    F9.6    error in PM-y (sec. of arc)
         // 56-57    2X      [blanks]
         // 58       A1      IERS (I) or Prediction (P) flag for Bull. A UT1-UTC values
         // 59-68    F10.7   Bull. A UT1-UTC (sec. of time)
         // 69-78    F10.7   error in UT1-UTC (sec. of time)
         // 79       X       [blank]
         // 80-86    F7.4    Bull. A LOD (msec. of time) -- NOT ALWAYS FILLED
         // 87-93    F7.4    error in LOD (msec. of time) -- NOT ALWAYS FILLED
         // 94-95    2X      [blanks]
         // 96       A1      IERS (I) or Prediction (P) flag for Bull. A nutation values
         // 97       X       [blank]
         // 98-106   F9.3    Bull. A dX wrt IAU2000A Nutation (msec. of arc), Free Core Nutation NOT Removed
         // 107-115  F9.3    error in dX (msec. of arc)
         // 116      X       [blank]
         // 117-125  F9.3    Bull. A dY wrt IAU2000A Nutation (msec. of arc), Free Core Nutation NOT Removed
         // 126-134  F9.3    error in dY (msec. of arc)
         // 135-144  F10.6   Bull. B PM-x (sec. of arc)
         // 145-154  F10.6   Bull. B PM-y (sec. of arc)
         // 155-165  F11.7   Bull. B UT1-UTC (sec. of time)
         // 166-175  F10.3   Bull. B dX wrt IAU2000A Nutation (msec. of arc)
         // 176-185  F10.3   Bull. B dY wrt IAU2000A Nutation (msec. of arc)
         List<KeyValuePair<double, double[]>> utcCorrections = new List<KeyValuePair<double, double[]>>();
         try
         {
            using (StreamReader reader = new StreamReader(filePath))
            {
               string line;
               while ((line = reader.ReadLine()) != null)
               {
                  try
                  {
                     int y = int.Parse(line.Substring(0, 2).Trim());
                     int m = int.Parse(line.Substring(2, 2).Trim());
                     int dom = int.Parse(line.Substring(4, 2).Trim());
                     double mJD = double.Parse(line.Substring(7, 8).Trim());
                     double xp = double.Parse(line.Substring(18, 9).Trim());
                     double yp = double.Parse(line.Substring(37, 9).Trim());
                     double utcCorrection = double.Parse(line.Substring(155, 10).Trim());
                     int year = mJD <= 51543 ? 1900 + y : 2000 + y;
                     utcCorrections.Add(new KeyValuePair<double, double[]>(Time.ToJulianDate(year, m, dom), new double[] { utcCorrection, xp, yp }));
                  }
                  catch
                  {
                     return utcCorrections;
                  }
               }
            }
         }
         catch
         {
            return utcCorrections;
         }

         return utcCorrections;
      }

      /// <summary>
      /// Converts international atomic time to barycentric dynamical time.
      /// </summary>
      /// <param name="tai">The international atomic time to be converted to barycentric dynamical time</param>
      /// <returns>The barycentric dynamical time corresponding to the given international atomic time</returns>
      private static Time TAIToTDB(Time tai)
      {
         double tdtJD = tai.JulianDate + 32.184 / Time.SecondsPerDay;
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdtJD - Time.JulianDate2000Jan1) / 36525d);
         double dt = 32.184 + 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         return new Time(tai.DateTime.AddTicks((long)(dt * Time.TicksPerSecond)), TimeStandard.BarycentricDynamicalTime);
      }

      /// <summary>
      /// Converts international atomic time to terrestrial dynamical time.
      /// </summary>
      /// <param name="tai">The international atomic time to be converted to terrestrial dynamical time</param>
      /// <returns>The terrestrial dynamical time corresponding to the given international atomic time</returns>
      private static Time TAIToTDT(Time tai)
      {
         return new Time(tai.DateTime.AddTicks((long)(32.184 * TicksPerSecond)), TimeStandard.TerrestrialDynamicalTime);
      }

      /// <summary>
      /// Converts international atomic time to universal time.
      /// </summary>
      /// <param name="tai">The international atomic time to be converted to universal time</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time corresponding to the given international atomic time</returns>
      private static Time TAIToUT0(Time tai, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts international atomic time to universal time.
      /// </summary>
      /// <param name="tai">The international atomic time to be converted to universal time</param>
      /// <returns>The universal time corresponding to the given international atomic time</returns>
      private static Time TAIToUT1(Time tai)
      {
         double taiJD = tai.JulianDate;
         double dt = GetCorrections(Time.atomicTimeCorrections, taiJD)[0];
         dt -= GetCorrections(Time.utcCorrections, taiJD - dt / Time.SecondsPerDay)[0];
         return new Time(tai.DateTime.AddTicks((long)(-dt * Time.TicksPerSecond)), TimeStandard.UniversalTime1);
      }

      /// <summary>
      /// Converts international atomic time to universal time.
      /// </summary>
      /// <param name="tai">The international atomic time to be converted to universal time</param>
      /// <returns>The universal time corresponding to the given international atomic time</returns>
      private static Time TAIToUT2(Time tai)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts international atomic time to universal coordinated time.
      /// </summary>
      /// <param name="tai">The international atomic time to be converted to universal coordinated time.</param>
      /// <returns>The universal coordinated time corresponding to the given international atomic time</returns>
      private static Time TAIToUTC(Time tai)
      {
         double taiJD = Time.ToJulianDate(tai);
         return new Time(tai.DateTime.AddTicks((long)(-GetCorrections(Time.atomicTimeCorrections, taiJD)[0] * TicksPerSecond)), TimeStandard.CoordinatedUniversalTime);
      }

      /// <summary>
      /// Converts barycentric dynamical time to international atomic time.
      /// </summary>
      /// <param name="tdb">The barycentric dynamical time to be converted to international atomic time</param>
      /// <returns>The international atomic time corresponding to the given barycentric dynamical time</returns>
      private static Time TDBToTAI(Time tdb)
      {
         double tdbJD = tdb.JulianDate;
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdbJD - Time.JulianDate2000Jan1) / 36525d);
         double dt = 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         return new Time(tdb.DateTime.AddTicks((long)(-(dt + 32.184) * TicksPerSecond)), TimeStandard.InternationalAtomicTime);
      }

      /// <summary>
      /// Converts barycentric dynamical time to terrestrial dynamical time.
      /// </summary>
      /// <param name="tdb">The barycentric dynamical time to be converted to terrestrial dynamical time</param>
      /// <returns>The terrestrial dynamical time corresponding to the given barycentric dynamical time</returns>
      private static Time TDBToTDT(Time tdb)
      {
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdb.JulianDate - Time.JulianDate2000Jan1) / 36525d); // Vallado 1-53/1-54
         double dt = 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         return new Time(tdb.DateTime.AddTicks((long)(-dt * TicksPerSecond)), TimeStandard.TerrestrialDynamicalTime);
      }

      /// <summary>
      /// Converts barycentic dynamical time to universal time.
      /// </summary>
      /// <param name="tdb">The barycentic dynamical time to be converted to universal time</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time corresponding to the given barycentic dynamical time</returns>
      private static Time TDBToUT0(Time tdb, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts barycentic dynamical time to universal time.
      /// </summary>
      /// <param name="tdb">The barycentic dynamical time to be converted to universal time</param>
      /// <returns>The universal time corresponding to the given barycentic dynamical time</returns>
      private static Time TDBToUT1(Time tdb)
      {
         double tdbJD = tdb.JulianDate;
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdbJD - Time.JulianDate2000Jan1) / 36525d);
         double dt = 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         dt += 32.184 + GetCorrections(Time.atomicTimeCorrections, tdbJD - dt / Time.SecondsPerDay)[0];
         dt -= GetCorrections(Time.utcCorrections, tdbJD - dt / Time.SecondsPerDay)[0];
         return new Time(tdb.DateTime.AddTicks((long)(-dt * TicksPerSecond)), TimeStandard.UniversalTime1);
      }

      /// <summary>
      /// Converts barycentic dynamical time to universal time.
      /// </summary>
      /// <param name="tdb">The barycentic dynamical time to be converted to universal time</param>
      /// <returns>The universal time corresponding to the given barycentic dynamical time</returns>
      private static Time TDBToUT2(Time tdb)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts barycentric dynamical time to universal coordinated time.
      /// </summary>
      /// <param name="tdb">The barycentric dynamical time to be converted to universal coordinated time.</param>
      /// <returns>The universal coordinated time corresponding to the given barycentric dynamical time</returns>
      private static Time TDBToUTC(Time tdb)
      {
         double tdbJD = tdb.JulianDate;
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdbJD - Time.JulianDate2000Jan1) / 36525d);
         double dt = 32.184 + 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         double taiJD = tdbJD - dt / Time.SecondsPerDay;
         dt += GetCorrections(Time.atomicTimeCorrections, taiJD)[0];
         return new Time(tdb.DateTime.AddTicks((long)(-dt * TicksPerSecond)), TimeStandard.CoordinatedUniversalTime);
      }

      /// <summary>
      /// Converts terrestrial dynamical time to international atomic time.
      /// </summary>
      /// <param name="tdt">The terrestrial dynamical time to be converted to international atomic time</param>
      /// <returns>The international atomic time corresponding to the given terrestrial dynamical time</returns>
      private static Time TDTToTAI(Time tdt)
      {
         return new Time(tdt.DateTime.AddTicks((long)(-32.184 * TicksPerSecond)), TimeStandard.InternationalAtomicTime);
      }

      /// <summary>
      /// Converts terrestrial dynamical time to barycentric dynamical time.
      /// </summary>
      /// <param name="tdt">The terrestrial dynamical time to be converted to barycentric dynamical time</param>
      /// <returns>The barycentric dynamical time corresponding to the given terrestrial dynamical time</returns>
      private static Time TDTToTDB(Time tdt)
      {
         // Vallado 1-53/1-54
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdt.JulianDate - Time.JulianDate2000Jan1) / 36525d);
         double dt = 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         return new Time(tdt.DateTime.AddTicks((long)(dt * TicksPerSecond)), TimeStandard.BarycentricDynamicalTime);
      }

      /// <summary>
      /// Converts terrestrial dynamical time to universal time.
      /// </summary>
      /// <param name="tdt">The terrestrial dynamical time to be converted to universal time</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time corresponding to the given terrestrial dynamical time</returns>
      private static Time TDTToUT0(Time tdt, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts terrestrial dynamical time to universal time.
      /// </summary>
      /// <param name="tdt">The terrestrial dynamical time to be converted to universal time</param>
      /// <returns>The universal time corresponding to the given terrestrial dynamical time</returns>
      private static Time TDTToUT1(Time tdt) 
      {
         double tdtJD = tdt.JulianDate;
         double dt = 32.184 + GetCorrections(Time.atomicTimeCorrections, tdtJD - 32.184 / Time.SecondsPerDay)[0];
         dt -= GetCorrections(Time.utcCorrections, tdtJD - dt / Time.SecondsPerDay)[0];
         return new Time(tdt.DateTime.AddTicks((long)(-dt * TicksPerSecond)), TimeStandard.UniversalTime1);
      }

      /// <summary>
      /// Converts terrestrial dynamical time to universal time.
      /// </summary>
      /// <param name="tdt">The terrestrial dynamical time to be converted to universal time</param>
      /// <returns>The universal time corresponding to the given terrestrial dynamical time</returns>
      private static Time TDTToUT2(Time tdt)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts terrestrial dynamical time to universal coordinated time.
      /// </summary>
      /// <param name="tdt">The terrestrial dynamical time to be converted to universal coordinated time.</param>
      /// <returns>The universal coordinated time corresponding to the given terrestrial dynamical time</returns>
      private static Time TDTToUTC(Time tdt)
      {
         double tdtJD = tdt.JulianDate;
         double taiJD = tdtJD - 32.184 / Time.SecondsPerDay;
         double dt = 32.184 + GetCorrections(Time.atomicTimeCorrections, taiJD)[0];
         return new Time(tdt.DateTime.AddTicks((long)(-dt * TicksPerSecond)), TimeStandard.CoordinatedUniversalTime);
      }

      /// <summary>
      /// Converts universal time to international atomic time.
      /// </summary>
      /// <param name="ut0">The universal time to be converted to international atomic time</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The international atomic time corresponding to the given universal time</returns>
      private static Time UT0ToTAI(Time ut0, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to barycentric dynamical time.
      /// </summary>
      /// <param name="ut0">The universal time to be converted to barycentric dynamical time</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The barycentric dynamical time corresponding to the given universal time</returns>
      private static Time UT0ToTDB(Time ut0, double longitude, double latitude) 
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to terrestrial dynamical time.
      /// </summary>
      /// <param name="ut0">The universal time to be converted to terrestrial dynamical time</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The terrestrial dynamical time corresponding to the given universal time</returns>
      private static Time UT0ToTDT(Time ut0, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time 0 to universal time 1.
      /// </summary>
      /// <param name="ut0">The universal time 0 to be converted to universal time 1</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time 1 corresponding to the given universal time 0</returns>
      private static Time UT0ToUT1(Time ut0, double longitudeRadians, double latitudeRadians)
      {
         double ut0JD = ut0.JulianDate;
         double[] corrections =  GetCorrections(Time.atomicTimeCorrections, ut0JD);
         double xp = corrections[1];
         double yp = corrections[2];
         //double longitudeRadians = longitude.ConvertTo(Units.Radians);
         double dt = (xp * Math.Sin(longitudeRadians) + yp * Math.Cos(longitudeRadians)) * Math.Tan(latitudeRadians);
         return new Time(ut0.DateTime.AddTicks((long)(-dt * TicksPerSecond)), TimeStandard.UniversalTime1);
      }

      /// <summary>
      /// Converts universal time 0 to universal time 2.
      /// </summary>
      /// <param name="ut0">The universal time 0 to be converted to universal time 2</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time 2 corresponding to the given universal time 0</returns>
      private static Time UT0ToUT2(Time ut0, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to universal coordinated time.
      /// </summary>
      /// <param name="ut0">The universal time to be converted to universal coordinated time</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal coordinated time corresponding to the given universal time</returns>
      private static Time UT0ToUTC(Time ut0, double longitude, double latitude) 
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to international atomic time.
      /// </summary>
      /// <param name="ut1">The universal time to be converted to international atomic time</param>
      /// <returns>The international atomic time corresponding to the given universal time</returns>
      private static Time UT1ToTAI(Time ut1)
      {
         double ut1JD = Time.ToJulianDate(ut1);
         double dt = -GetCorrections(Time.utcCorrections, ut1JD)[0];
         dt += GetCorrections(Time.atomicTimeCorrections, ut1JD + dt / Time.SecondsPerDay)[0];
         return new Time(ut1.DateTime.AddTicks((long)(dt * TicksPerSecond)), TimeStandard.InternationalAtomicTime);
      }

      /// <summary>
      /// Converts universal time to barycentric dynamical time.
      /// </summary>
      /// <param name="ut1">The universal time to be converted to barycentric dynamical time</param>
      /// <returns>The barycentric dynamical time corresponding to the given universal time</returns>
      private static Time UT1ToTDB(Time ut1)
      {
         double ut1JD = Time.ToJulianDate(ut1);
         double dt = -GetCorrections(Time.utcCorrections, ut1JD)[0];
         double dt2 = GetCorrections(Time.atomicTimeCorrections, ut1JD + dt / Time.SecondsPerDay)[0];
         dt += 32.184 + GetCorrections(Time.atomicTimeCorrections, ut1JD + dt / Time.SecondsPerDay)[0];
         double tdtJD = ut1JD + dt / Time.SecondsPerDay;
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdtJD - Time.JulianDate2000Jan1) / 36525d);
         dt += 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         return new Time(ut1.DateTime.AddTicks((long)(dt * TicksPerSecond)), TimeStandard.BarycentricDynamicalTime);
      }

      /// <summary>
      /// Converts universal time to terrestrial dynamical time.
      /// </summary>
      /// <param name="ut1">The universal time to be converted to terrestrial dynamical time</param>
      /// <returns>The terrestrial dynamical time corresponding to the given universal time</returns>
      private static Time UT1ToTDT(Time ut1)
      {
         double ut1JD = Time.ToJulianDate(ut1);
         double dt = -GetCorrections(Time.utcCorrections, ut1JD)[0];
         dt += 32.184 + GetCorrections(Time.atomicTimeCorrections, ut1JD + dt / Time.SecondsPerDay)[0];
         return new Time(ut1.DateTime.AddTicks((long)(dt * TicksPerSecond)), TimeStandard.TerrestrialDynamicalTime);
      }

      /// <summary>
      /// Converts universal time 1 to universal time 0.
      /// </summary>
      /// <param name="ut1">The universal time 1 to be converted to universal time 0</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time 0 corresponding to the given universal time 1</returns>
      private static Time UT1ToUT0(Time ut1, double longitude, double latitude)
      {
         double ut1JD = ut1.JulianDate;
         double[] corrections = GetCorrections(Time.atomicTimeCorrections, ut1JD);
         double xp = corrections[1];
         double yp = corrections[2];
         //double longitudeRadians = longitude.ConvertTo(Units.Radians);
         double dt = (xp * Math.Sin(longitude) + yp * Math.Cos(longitude)) * Math.Tan(latitude);
         return new Time(ut1.DateTime.AddTicks((long)(dt * TicksPerSecond)), TimeStandard.UniversalTime0);
      }

      /// <summary>
      /// Converts universal time 1 to universal time 2.
      /// </summary>
      /// <param name="ut1">The universal time 1 to be converted to universal time 2</param>
      /// <returns>The universal time 2 corresponding to the given universal time 1</returns>
      private static Time UT1ToUT2(Time ut1)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to universal coordinated time.
      /// </summary>
      /// <param name="ut1">The universal time to be converted to universal coordinated time</param>
      /// <returns>The universal coordinated time corresponding to the given universal time</returns>
      private static Time UT1ToUTC(Time ut1)
      {
         double ut1JD = Time.ToJulianDate(ut1);
         double delta = -GetCorrections(Time.utcCorrections, ut1JD)[0];
         DateTime dt = ut1.DateTime.AddTicks((long)(-GetCorrections(Time.utcCorrections, ut1JD)[0] * TicksPerSecond));
         double deltaTicks = -GetCorrections(Time.utcCorrections, ut1JD)[0] * TicksPerSecond;
         return new Time(ut1.DateTime.AddTicks((long)(-GetCorrections(Time.utcCorrections, ut1JD)[0] * TicksPerSecond)), TimeStandard.CoordinatedUniversalTime);
      }

      /// <summary>
      /// Converts universal time to international atomic time.
      /// </summary>
      /// <param name="ut2">The universal time to be converted to international atomic time</param>
      /// <returns>The international atomic time corresponding to the given universal time</returns>
      private static Time UT2ToTAI(Time ut2)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to barycentric dynamical time.
      /// </summary>
      /// <param name="ut2">The universal time to be converted to barycentric dynamical time</param>
      /// <returns>The barycentric dynamical time corresponding to the given universal time</returns>
      private static Time UT2ToTDB(Time ut2) 
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to terrestrial dynamical time.
      /// </summary>
      /// <param name="ut2">The universal time to be converted to terrestrial dynamical time</param>
      /// <returns>The terrestrial dynamical time corresponding to the given universal time</returns>
      private static Time UT2ToTDT(Time ut2)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time 2 to universal time 0.
      /// </summary>
      /// <param name="ut2">The universal time 2 to be converted to universal time 0</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time 0 corresponding to the given universal time 2</returns>
      private static Time UT2ToUT0(Time ut2, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time 2 to universal time 1.
      /// </summary>
      /// <param name="ut2">The universal time 2 to be converted to universal time 1</param>
      /// <returns>The universal time 1 corresponding to the given universal time 2</returns>
      private static Time UT2ToUT1(Time ut2)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal time to universal coordinated time.
      /// </summary>
      /// <param name="ut2">The universal time to be converted to universal coordinated time</param>
      /// <returns>The universal coordinated time corresponding to the given universal time</returns>
      private static Time UT2ToUTC(Time ut2)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal coordinated time  to international atomic time.
      /// </summary>
      /// <param name="utc">The universal coordinated time to be converted to international atomic time.</param>
      /// <returns>The international atomic time corresponding to the given universal coordinated time</returns>
      private static Time UTCToTAI(Time utc)
      {
         double utcJD = Time.ToJulianDate(utc);
         return new Time(utc.DateTime.AddTicks((long)(GetCorrections(Time.atomicTimeCorrections, utcJD)[0] * TicksPerSecond)), TimeStandard.InternationalAtomicTime);
      }

      /// <summary>
      /// Converts universal coordinated time  to barycentric dynamical time.
      /// </summary>
      /// <param name="utc">The universal coordinated time to be converted to barycentric dynamical time.</param>
      /// <returns>The barycentric dynamical time corresponding to the given universal coordinated time</returns>
      private static Time UTCToTDB(Time utc)
      {
         double utcJD = Time.ToJulianDate(utc);
         double dt = 32.184 + GetCorrections(Time.atomicTimeCorrections, utcJD)[0];
         double tdtJD = utcJD + dt / Time.SecondsPerDay;
         double m = Time.NormalizePositive(6.240035939 + 628.301956 * (tdtJD - Time.JulianDate2000Jan1) / 36525d);
         dt += 0.001658 * Math.Sin(m) + 0.00001385 * Math.Sin(2d * m);
         return new Time(utc.DateTime.AddTicks((long)(dt* TicksPerSecond)), TimeStandard.BarycentricDynamicalTime);
      }

      /// <summary>
      /// Converts universal coordinated time  to terrestrial dynamical time.
      /// </summary>
      /// <param name="utc">The universal coordinated time to be converted to terrestrial dynamical time.</param>
      /// <returns>The terrestrial dynamical time corresponding to the given universal coordinated time</returns>
      private static Time UTCToTDT(Time utc)
      {
         double utcJD = Time.ToJulianDate(utc);
         return new Time(utc.DateTime.AddTicks((long)(TicksPerSecond* (32.184 + GetCorrections(Time.atomicTimeCorrections, utcJD)[0]))), TimeStandard.TerrestrialDynamicalTime);
      }

      /// <summary>
      /// Converts universal coordinated time to universal time.
      /// </summary>
      /// <param name="utc">The universal coordinated time to be converted to universal time.</param>
      /// <param name="longitude">The geocentric longitude of the observer</param>
      /// <param name="latitude">The geocentric latitude of the observer</param>
      /// <returns>The universal time corresponding to the given universal coordinated time</returns>
      private static Time UTCToUT0(Time utc, double longitude, double latitude)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Converts universal coordinated time to universal time.
      /// </summary>
      /// <param name="utc">The universal coordinated time to be converted to universal time.</param>
      /// <returns>The universal time corresponding to the given universal coordinated time</returns>
      private static Time UTCToUT1(Time utc)
      {
         double utcJD = Time.ToJulianDate(utc);
         return new Time(utc.DateTime.AddTicks((long)(GetCorrections(Time.utcCorrections, utcJD)[0] * TicksPerSecond)), TimeStandard.UniversalTime1);
      }

      /// <summary>
      /// Converts universal coordinated time to universal time.
      /// </summary>
      /// <param name="utc">The universal coordinated time to be converted to universal time.</param>
      /// <returns>The universal time corresponding to the given universal coordinated time</returns>
      private static Time UTCToUT2(Time utc)
      {
         throw new NotImplementedException();
      }

      #endregion
   }
}
