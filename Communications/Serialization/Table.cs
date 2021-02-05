// <copyright file="CCSDSPacket.cs" company="Genesis Engineering Services">
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
   using BKSystem.IO;

   #endregion Directives

   /// <summary>
   /// Class representing a CCSDS command packet
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications" })]
   [Description("Table.")]
   public class Table:
      MTI.Core.Component
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSPacket"/> class
      /// </summary>
      public Table()
      {
         this.FileHeader = new object();
         this.TableHeader = new object();
         this.Content = new object();
      }

      /// <summary>
      /// Gets or sets the file header data
      /// </summary>
      [Require]
      public virtual object FileHeader
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the table header data
      /// </summary>
      [Require]
      public virtual object TableHeader
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the content data
      /// </summary>
      [Require]
      public virtual object Content
      {
         get;
         set;
      }
   }
}
