// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-07-01</date>
// <summary>Class representing time related constants.</summary>

namespace GES.TimeSystems
{
   #region Directives
   #endregion

   /// <summary>
   /// Time parametrizations and conversions.
   /// </summary>
   public partial class Time
   {
      #region Time related constants

      /// <summary>
      /// Degrees per hour angle
      /// </summary>
      public const double DegreesPerHourAngle = 15.0;

      /// <summary>
      /// Arcminutes per hour angle
      /// </summary>
      public const double ArcminutesPerHourAngle = 60.0;

      /// <summary>
      /// Arcseconds per hour angle
      /// </summary>
      public const double ArcsecondsPerHourAngle = 3600.0;

      /// <summary>
      /// Arcseconds per arcminute
      /// </summary>
      public const double ArcsecondsPerArcminute = 60.0;

      /// <summary>
      /// Degrees per arcminute
      /// </summary>
      public const double DegreesPerArcminute = 0.25;

      /// <summary>
      /// Degrees per arcsecond
      /// </summary>
      public const double DegreesPerArcsecond = 0.004166666666667;

      /// <summary>
      /// Seconds per sidereal day
      /// </summary>
      public const double SecondsPerSiderealDay = 8.61640918E+04;

      /// <summary>
      /// Seconds per ephemeris day
      /// </summary>
      public const double SecondsPerEphemerisDay = 8.64E+04;           // Ephemeris day to seconds

      /// <summary>
      /// Earth day per ephemeris day
      /// </summary>
      public const double EarthDaysPerEphemerisDay = 1.00273789;         // Ephemeris day to earth day

      /// <summary>
      /// Seconds per Vanguard unit
      /// </summary>
      public const double SecondsPerVanguard = 8.0681242E+02;

      /// <summary>
      /// Seconds per tropical year (ref aries)
      /// </summary>
      public const double SecondsPerTropicalYear = 3.1556925551E+07;

      /// <summary>
      /// Days per tropical year (ref aries)
      /// </summary>
      public const double DaysPerTropicalYear = 3.6524219388E+02;

      /// <summary>
      /// Seconds per sidereal year
      /// </summary>
      public const double SecondsPerSiderealYear = 3.1558149548E+07;

      /// <summary>
      /// Days per siderial year
      /// </summary>
      public const double DaysPerSiderialYear = 3.6525636051E+02;

      /// <summary>
      /// Seconds per year
      /// </summary>
      public const double SecondsPerYear = 3.1536E+07;

      /// <summary>
      /// Days per Julian century
      /// </summary>
      public const double DaysPerJulianCentury = 3.6525E+04;

      /// <summary>
      /// Days per Gregorian calendar century
      /// </summary>
      public const double DaysPerGregorianCentury = 3.652425E+04;

      /// <summary>
      /// Julian date, May 24, 1968 
      /// </summary>
      public const double JulianDate1968May24 = 2440000.5;

      /// <summary>
      /// Julian date,  March 21, 1990
      /// </summary>
      public const double JulianDate1990March21 = 2447972.0034832893;

      /// <summary>
      /// Julian date, January 1, 2000 (12h, noon)
      /// </summary>
      public const double JulianDate2000Jan1 = 2451545.0;

      /// <summary>
      /// Julian date GPS epoch, January 6, 1980 (0h, midnight Jan 5)
      /// </summary>
      public const double JulianDateGPSEpoch1980Jan6 = 2444244.5;

      /// <summary>
      /// Seconds per .NET tick
      /// </summary>
      public const double SecondsPerTick = 1d / 1e7;

      /// <summary>
      /// Seconds per hour
      /// </summary>
      public const double SecondsPerHour = 3600.0;

      /// <summary>
      /// Seconds per day
      /// </summary>
      public const double SecondsPerDay = 86400.0;

      /// <summary>
      /// Days per century
      /// </summary>
      public const double DaysPerCentury = 36525.0;

      /// <summary>
      /// Days per year
      /// </summary>
      public const double DaysPerYear = 365.25;

      /// <summary>
      /// Seconds per century
      /// </summary>
      public const double SecondsPerCentury = 3155760000.0;

      /// <summary>
      /// Seconds per millennia
      /// </summary>
      public const double SecondsPerMillennia = 31557600000.0;

      /// <summary>
      /// Earth angular rate (radians/second)
      /// </summary>
      public const double EarthAngularRateRadiansPerSecond = 0.000072921158553;

      /// <summary>
      /// Earth angular rate (degrees/second)
      /// </summary>
      public const double EarthAngularRateDegreesPerSecond = 0.004178074622291;

      /// <summary>
      /// .NET Ticks per second
      /// </summary>
      public const long TicksPerSecond = (long)1e7;

      #endregion
   }        
}
