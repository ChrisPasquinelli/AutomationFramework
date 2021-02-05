// <copyright file="CCSDSPacket.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Classes implementing a CCSDS packet</summary>

namespace GES.Communications
{
   #region Directives

   using System.ComponentModel;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Class representing a CCSDS command packet
   /// </summary>
   [Provide(Categories = new string[] { "Serialization" })]
   [Description("CCSDS packet.")]
   public class CCSDSPacket:
      MTI.Core.Component
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSPacket"/> class
      /// </summary>
      public CCSDSPacket()
      {
         this.PrimaryHeader = new CCSDSPrimaryHeader();
         this.SecondaryHeader = new object();
         this.ApplicationData = new object(); 
      }

      /// <summary>
      /// Gets or sets the CCSDS primary header
      /// </summary>
      public CCSDSPrimaryHeader PrimaryHeader
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the secondary header
      /// </summary>
      [Require]
      public virtual object SecondaryHeader
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the application data
      /// </summary>
      [Require]
      public virtual object ApplicationData
      {
         get;
         set;
      }
   }
}
