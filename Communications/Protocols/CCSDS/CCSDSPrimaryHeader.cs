// <copyright file="CCSDSPrimaryHeader.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Class implementing a CCSDS command header</summary>

namespace GES.Communications
{
   #region Directives

   using System.ComponentModel;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Class implementing a CCSDS command header
   /// </summary>
   [Provide(Categories = new string[] { "Communications" })]
   [Description("Base Task Component")]
   public class CCSDSPrimaryHeader:
      MTI.Core.Component
   {
      #region Constants

      /// <summary>
      /// The byte length of the CCSDSD primary header
      /// </summary>
      public const int PrimaryHeaderLength = 6;

      #endregion Constants

      #region Fields
      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSPrimaryHeader"/> class
      /// </summary>
      public CCSDSPrimaryHeader() 
      {
      }

      #endregion Constructors

      #region Enumerations

      /// <summary>
      /// The sequence flag indicating where packet is within a sequence of packets
      /// </summary>
      public enum SequenceFlags : byte
      {
         /// <summary>
         /// First packet of in a sequence
         /// </summary>
         FirstSegment = 0B01,

         /// <summary>
         /// Intermediate packet
         /// </summary>
         MiddleSegment = 0B00,

         /// <summary>
         /// Last packet
         /// </summary>
         LastSegment = 0B10,

         /// <summary>
         /// Single packet
         /// </summary>
         NoSegment = 0B11,
      }

      /// <summary>
      /// The packet types
      /// </summary>
      public enum PacketType : byte
      {
         /// <summary>
         /// Telemetry packet
         /// </summary>
         Telemetry = 0,

         /// <summary>
         /// Command packet
         /// </summary>
         Telecommand = 1
      }

      #endregion Enumerations

      #region Public Properties

      /// <summary>
      /// Gets or sets the packet version
      /// </summary>
      public byte Version
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the packet type
      /// </summary>
      public byte Type
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the secondary flag
      /// </summary>
      public byte SecondaryFlag
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the application identifier
      /// </summary>
      public ushort ApplicationId
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the grouping
      /// </summary>
      public byte Grouping
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the sequence count
      /// </summary>
      public ushort SequenceCount
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the packet data length
      /// </summary>
      public virtual ushort PacketDataLength
      {
         get;
         set;
      }

      #endregion Public Properties

      #region Public Methods
      #endregion Public Methods
   }
}
