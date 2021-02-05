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
   using BKSystem.IO;

   using GES.TimeSystems;

   #endregion Directives

   /// <summary>
   /// Pure interface to be implemeted by command objects
   /// </summary>
   public interface ICommand
   {
      #region Properties

      /// <summary>
      /// Gets or sets the time delayed relative to the reference event
      /// </summary>
      string Name
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the time delayed relative to the reference event
      /// </summary>
      TimeSpan StartOffset
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the time for the command to execute relative to the start offset
      /// </summary>
      TimeSpan StopOffset
      {
         get;
         set;
      }

      #endregion Properties

      #region Public Methods

      /// <summary>
      /// Sets the absolute start time
      /// </summary>
      /// <param name="startTime">the start absolute time</param>
      void SetStartTime(Time startTime);

      /// <summary>
      /// Gets the start absolute time
      /// </summary>
      /// <returns>the start absolute time</returns>
      Time GetStartTime();

      /// <summary>
      /// Expands the composite nodes into primitives.
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      void Expand(List<ICommand> primitives);

      /// <summary>
      /// Serializes the composite commands into a bitstream
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      byte[] Serialize(BitStream bitStream);

      /// <summary>
      /// Deserializes the binary stream into the instance of data type based on the binary specification configured by end-user.
      /// </summary>
      /// <param name="bitStream"></param>
      /// <param name="byteOrder"></param>
      /// <returns></returns>
      object[] Deserialize(BitStream bitStream);
      
      #endregion Public Methods
   }
}
