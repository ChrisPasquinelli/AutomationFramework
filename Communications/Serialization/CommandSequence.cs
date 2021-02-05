// <copyright file="Serializer.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Serializer for transfering an array of bytes into and from a command sequence.</summary>

namespace GES.Communications
{
   using System;
   using System.Collections.Generic;
   #region Directives

   using System.ComponentModel;

   using BKSystem.IO;

   using GES.TimeSystems;

   using MTI.Core;

   #endregion Directives

   [Provide(Categories = new string[] { "GES.Communications", "GES.Commands" })]
   [Description("Command Time Sequence")]
   public class CommandSequence :
      MTI.Core.Component,
      ICommand
   {
      #region Fields

      private ICommand[] commands;

      private Time referenceAbsoluteTime;

      private Time startTime;

      #endregion Fields

      #region Constructors

      public CommandSequence()
      {
         this.commands = new ICommand[0];
         this.DefaultCommandOffset = new TimeSpan(0, 0, 1);
         this.referenceAbsoluteTime = new Time(DateTime.UtcNow, TimeStandard.CoordinatedUniversalTime);
         this.startTime = new Time(DateTime.UtcNow, TimeStandard.CoordinatedUniversalTime);
      }

      #endregion Constructors

      #region Properties

      /// <summary>
      /// Gets or sets the command sequence
      /// </summary>
      [Require]
      public ICommand[] Commands
      {
         get
         {
            return this.commands;
         }

         set
         {
            this.commands = value;
            this.OnPropertyChanged("Commands");
         }
      }

      /// <summary>
      /// Gets or sets the time delayed relative to the reference event
      /// </summary>
      public virtual string Name
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

      /// <summary>
      /// Gets or sets the default command offset
      /// </summary>
      public TimeSpan DefaultCommandOffset
      {
         get;
         set;
      }

      #endregion Properties

      /// <summary>
      /// Sets the absolute start time
      /// </summary>
      /// <param name="startTime">the start absolute time</param>
      public void SetStartTime(Time startTime)
      {
         this.startTime = startTime;
         Time referenceTime = startTime;
         foreach(ICommand command in this.commands)
         {
            referenceTime += command.StartOffset;
            command.SetStartTime(referenceTime);
         }
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
         foreach (ICommand command in this.Commands)
         {
            command.Expand(primitives);
         }
      }

      /// <summary>
      /// Serializes the composite commands into a bitstream
      /// </summary>
      /// <param name="bitStream">the bitstream</param>
      public virtual byte[] Serialize(BitStream bitStream)
      {
         List<byte> bytes = new List<byte>();
         foreach (ICommand command in this.Commands)
         {
            bytes.AddRange(command.Serialize(bitStream));
         }

         return bytes.ToArray();
      }

      /// <summary>
      /// Deserializes the binary stream into the instance of data type based on the binary specification configured by end-user.
      /// </summary>
      /// <param name="bitStream">the bitstream</param>
      /// <returns></returns>
      public virtual object[] Deserialize(BitStream bitStream)
      {
         List<object> objects = new List<object>();
         foreach (ICommand command in this.Commands)
         {
            objects.AddRange(command.Deserialize(bitStream));
         }

         return objects.ToArray();
      }

      public override bool OnInitialize()
      {
         this.StartOffset = this.DefaultCommandOffset;
         return base.OnInitialize();
      }
   }
}
