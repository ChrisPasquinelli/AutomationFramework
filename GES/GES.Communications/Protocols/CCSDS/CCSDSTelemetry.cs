// <copyright file="Command.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Class defining a primitive CCSDS telemetry packet</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using MTI.Core;
   using GES.TimeSystems;
   using BKSystem.IO;

   #endregion Directives

   /// <summary>
   /// Class defining a primitive CCSDS telemetry packet
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Telemetry" })]
   [Description("Class defining a primitive CCSDS telemetry packet.")]
   public class CCSDSTelemetry:
      CCSDSPacketSerializer,
      ITelemetry
   {
      #region Fields

      private bool outputBytes;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSTelemetry"/> class
      /// </summary>
      public CCSDSTelemetry() :
         base()
      {
         this.Name = this.GetType().Name;
      }

      #endregion Constructors

      #region Properties

      /// <summary>
      /// Gets or sets the time delayed relative to the reference event
      /// </summary>
      public string Name
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the time delayed relative to the reference event
      /// </summary>
      public virtual TimeSpan StartOffset
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the time delayed relative to the reference event
      /// </summary>
      public virtual TimeSpan StopOffset
      {
         get;
         set;
      }

      #endregion Properties

      #region ITelemeetry overrides

      /// <summary>
      /// Expands the composite nodes into primitives.
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      public virtual void Expand(List<ITelemetry> primitives)
      {
         primitives.Add(this);
      }

      #endregion ITelemeetry overrides
   }
}
