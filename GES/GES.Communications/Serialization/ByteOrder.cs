// <copyright file="ByteOrder.cs" company="Genesis Engineering Services">
// Copyright (c) 2019 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2019-04-19</date>
// <summary>Enunmeration specifying the byte ordering.</summary>

using MTI.Core;

using System.ComponentModel;

namespace GES.Communications
{
   #region Directives
   #endregion Directives

   /// <summary>
   /// Enumeration of various types of byte orderings supported including
   /// mixed type of orderings 
   /// </summary>
   [Provide(Categories = new string[] { "GES.Serialization" })]
   [Description("Byte ordering of serialization/deserialization")]
   public enum ByteOrder : byte
   {
      /// <summary>
      /// Uninitialized byte ordering
      /// </summary>
      Undefined = 0,

      /// <summary>
      /// Bytes ordered from most significant byte to least significant byte
      /// </summary>
      Sequential = 1,

      /// <summary>
      /// Bytes ordered from least significant byte to most significant byte
      /// </summary>
      ByteReverse = 2,

      /// <summary>
      /// Bytes ordered from least significant word to most significant word
      /// </summary>
      WordReverse = 4,

      /// <summary>
      /// Bytes ordered from least significant double word to most significant double word
      /// N+6|N+7|N+4|N+5|N+2|N+3|N|N+1
      /// </summary>
      DoubleWordReverse = 8,
   }
}