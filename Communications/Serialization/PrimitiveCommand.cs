// <copyright file="Command.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Class interfacing with user specified commands</summary>

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
   /// Command represents is a command that is issued based on a temporal event such as a start time
   /// for absolute and relative time events or asynchronously where the start and stop times
   /// are the times at which the event is armed and disarmed respectively and triggered by and
   /// external source.
   /// </summary>
   public abstract class PrimitiveCommand:
      ICommand
   {
      #region Fields

      private byte[] serializedBytes;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSCommand"/> class
      /// </summary>
      public PrimitiveCommand() :
         base()
      {
         this.Name = this.GetType().Name;
         this.serializedBytes = new byte[0];
      }

      #endregion Constructors

      #region Properties

      [Provide(Cut = true)]
      public byte[] SerializedBytes
      {
         get
         {
            return this.serializedBytes;
         }

         set
         {
            this.serializedBytes = value;
            this.OnPropertyChanged("SerializedBytes");
         }
      }

      #endregion Properties

      #region Command overrides

      /// <summary>
      /// Expands the composite nodes into primitives.
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      public override void Expand(List<ICommand> primitives)
      {
         primitives.Add(this);
      }

      #endregion Command overrides
   }
}
