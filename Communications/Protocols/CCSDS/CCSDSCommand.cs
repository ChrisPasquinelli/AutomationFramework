// <copyright file="Command.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Class defining a primitive CCSDS command based on the user configuration</summary>

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
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Commands" })]
   [Description("Command event whose start time is specified as a duration relative to the given time.")]
   public class CCSDSCommand:
      CCSDSPacketSerializer,
      ICommand
   {
      #region Fields

      private Time startTime;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSCommand"/> class
      /// </summary>
      public CCSDSCommand() :
         base()
      {
         this.Name = this.GetType().Name;
         this.startTime = new Time(DateTime.UtcNow, TimeStandard.CoordinatedUniversalTime);
         this.PacketDefinition.PrimaryHeader.Version = 0;
         this.PacketDefinition.PrimaryHeader.Type = 1;
         this.PacketDefinition.PrimaryHeader.Grouping = 2;
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

      #region Command overrides

      /// <summary>
      /// Sets the absolute start time
      /// </summary>
      /// <param name="startTime">the start absolute time</param>
      public void SetStartTime(Time startTime)
      {
         this.startTime = startTime;
      }

      /// <summary>
      /// Gets the start absolute time
      /// </summary>
      /// <returns>the start absolute time</returns>
      public Time GetStartTime()
      {
         return this.startTime;
      }

      /// <summary>
      /// Expands the composite nodes into primitives.
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      public virtual void Expand(List<ICommand> primitives)
      {
         primitives.Add(this);
      }

      #endregion Command overrides
   }
}
