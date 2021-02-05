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

   #endregion Directives

   /// <summary>
   /// Pure interface to be implemeted by telemetry objects
   /// </summary>
   public interface ITelemetry
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
      /// Gets or sets the time delayed relative to the reference event
      /// </summary>
      TimeSpan StopOffset
      {
         get;
         set;
      }

      #endregion Properties

      #region Public Methods

      /// <summary>
      /// Expands the composite nodes into primitives.
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      void Expand(List<ITelemetry> primitives);

      /// <summary>
      /// Serializes the composite commands into a bitstream
      /// </summary>
      /// <param name="bitStream">the bit stream</param>
      byte[] Serialize(BitStream bitStream);

      /// <summary>
      /// Deserializes the binary stream into the instance of data type based on the binary specification configured by end-user.
      /// </summary>
      /// <param name="bitStream">the bit stream</param>
      /// <returns></returns>
      object[] Deserialize(BitStream bitStream);
      
      #endregion Public Methods
   }
}
