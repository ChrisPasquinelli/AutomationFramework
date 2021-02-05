// <copyright file="Command.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Serializes a table into a binary file format</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using MTI.Core;
   using GES.TimeSystems;
   using BKSystem.IO;
   using System.IO;

   #endregion Directives

   /// <summary>
   /// Serializes a table into a binary file format
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Tables" })]
   [Description("Serializes a table into a binary file format.")]
   public class TableSerializer :
      MTI.Core.Component
   {
      #region Fields

      /// <summary>
      /// Start the processing
      /// </summary>
      private bool start;

      /// <summary>
      /// The flag that indicates that processing has completed.
      /// </summary>
      private bool ready;

      /// <summary>
      /// The table definition
      /// </summary>
      private TableDefinition tableDefinition;

      /// <summary>
      /// .The file header serializer
      /// </summary>
      private Serializer fileHeaderSerializer;

      /// <summary>
      /// .The table header serializer
      /// </summary>
      private Serializer tableHeaderSerializer;

      /// <summary>
      /// .The content serializer
      /// </summary>
      private Serializer contentSerializer;

      /// <summary>
      /// The serialized bytes
      /// </summary>
      private byte[] serializedBytes;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSCommand"/> class
      /// </summary>
      public TableSerializer() :
         base()
      {
         this.Mode = OperationMode.Undefined;
         this.tableDefinition = new TableDefinition();
         this.fileHeaderSerializer = new Serializer();
         this.tableHeaderSerializer = new Serializer();
         this.contentSerializer = new Serializer();
      }

      #endregion Constructors

      #region Enumerations

      public enum OperationMode
      {
         Undefined,
         Read,
         Write
      }

      #endregion Enumerations

      #region Public Properties

      [Require]
      public bool Start
      {
         get
         {
            return this.start;
         }
         set
         {
            this.Ready = false;
            if (value && !this.start)
            {
               try
               {
                  switch (this.Mode)
                  {
                     case OperationMode.Read:
                        {
                           byte[] bytes = File.ReadAllBytes(this.Filename);
                           this.FileSize = bytes.Length;
                           BitStream bitStream = new BitStream();
                           bitStream.Write(bytes);
                           bitStream.Position = 0;
                           this.Deserialize(bitStream, ByteOrder.ByteReverse);
                           this.Ready = true;
                           break;
                        }

                     case OperationMode.Write:
                        {
                           BitStream bitStream = new BitStream();
                           this.Serialize(bitStream, ByteOrder.ByteReverse);
                           this.Ready = true;
                           break;
                        }
                  }
               }
               catch(Exception ex)
               {
                  this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, this.Identifier, this, MessageType.Error));
               }
            }

            this.start = value;
         }
      }

      [Require]
      public TableDefinition TableDefinition
      {
         get
         {
            return this.tableDefinition;
         }
         set
         {
            this.tableDefinition = value;
            this.OnPropertyChanged("TableDefinition");
         }
      }

      [Provide]
      public bool Ready
      {
         get
         {
            return this.ready;
         }

         set
         {
            this.ready = value;
            this.OnPropertyChanged("Ready");
         }
      }

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

      public OperationMode Mode
      {
         get;
         set;
      }

      public string Filename
      {
         get;
         set;
      }

      public int PadToSize
      {
         get;
         set;
      }
      public int FileHeaderSize
      {
         get;
         set;
      }
      public int TableHeaderSize
      {
         get;
         set;
      }
      public int UnpaddedContentSize
      {
         get;
         set;
      }
      public int ContentSize
      {
         get;
         set;
      }
      public int FileSize
      {
         get;
         set;
      }

      #endregion Public Properties

      #region Public Methods

      /// <summary>
      /// Serializes the composite commands into a bitstream
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      public virtual byte[] Serialize(BitStream bitStream, ByteOrder byteOrder)
      {
         this.fileHeaderSerializer.DataAttributes = this.tableDefinition.FileHeaderAttributes;
         this.tableHeaderSerializer.DataAttributes = this.tableDefinition.TableHeaderAttributes;
         this.contentSerializer.DataAttributes = this.tableDefinition.ContentAttributes;

         long startPosition = bitStream.Position;
         this.fileHeaderSerializer.Serialize(bitStream);
         this.FileHeaderSize = (int)(bitStream.Position - startPosition) / 8;
         this.tableHeaderSerializer.Serialize(bitStream);
         this.TableHeaderSize = (int)(bitStream.Position - startPosition) / 8 - this.FileHeaderSize;
         this.contentSerializer.Serialize(bitStream);
         this.UnpaddedContentSize = (int)(bitStream.Position - startPosition) / 8 - this.TableHeaderSize - this.FileHeaderSize;
         if(this.PadToSize > this.FileHeaderSize + this.TableHeaderSize + this.UnpaddedContentSize)
         {
            byte[] padding = new byte[this.PadToSize - this.UnpaddedContentSize - this.TableHeaderSize - this.FileHeaderSize];
            bitStream.Write(padding);
         }

         this.ContentSize = (int)(bitStream.Position - startPosition) / 8 - this.TableHeaderSize - this.FileHeaderSize;
         byte[] bytes = bitStream.ToByteArray(startPosition, bitStream.Position - startPosition);
         this.tableDefinition.Checksum.Compute(bytes);

         this.SerializedBytes = bytes;
         this.FileSize = bytes.Length;
         File.WriteAllBytes(this.Filename, bytes);

         return bytes;
      }

      /// <summary>
      /// Deserializes the binary stream into the instance of data type based on the binary specification configured by end-user.
      /// </summary>
      /// <param name="bitStream"></param>
      /// <param name="byteOrder"></param>
      /// <returns></returns>
      public virtual object[] Deserialize(BitStream bitStream, ByteOrder byteOrder)
      {
         this.fileHeaderSerializer.DataAttributes = this.tableDefinition.FileHeaderAttributes;
         this.tableHeaderSerializer.DataAttributes = this.tableDefinition.TableHeaderAttributes;
         this.contentSerializer.DataAttributes = this.tableDefinition.ContentAttributes;


         Table table = new Table();

         table.FileHeader = this.fileHeaderSerializer.Deserialize(bitStream, byteOrder, this.tableDefinition.FileHeader)[0];
         table.TableHeader = this.tableHeaderSerializer.Deserialize(bitStream, byteOrder, this.tableDefinition.TableHeader)[0];
         table.Content = this.contentSerializer.Deserialize(bitStream, byteOrder, this.tableDefinition.Content)[0];

         return new object[] { table };
      }

      #endregion Public Methods
   }
}
