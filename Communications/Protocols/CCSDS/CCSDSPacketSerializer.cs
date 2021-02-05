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
   /// CCSDS packet serializer/deserializer
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Commands" })]
   [Description("Command event whose start time is specified as a duration relative to the given time.")]
   public class CCSDSPacketSerializer :
      MTI.Core.Component
   {
      #region Fields

      /// <summary>
      /// The packet definition
      /// </summary>
      private CCSDSPacketDefinition packetDefinition;

      /// <summary>
      /// .The secondary header serializer
      /// </summary>
      private Serializer secondaryHeaderSerializer;

      /// <summary>
      /// .The secondary header serializer
      /// </summary>
      private Serializer applicationDataSerializer;

      /// <summary>
      /// The serialized bytes
      /// </summary>
      private byte[] serializedBytes;

      private bool readBytes;
      private bool writeBytes;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSCommand"/> class
      /// </summary>
      public CCSDSPacketSerializer() :
         base()
      {
         this.packetDefinition = new CCSDSPacketDefinition();
         this.secondaryHeaderSerializer = new Serializer();
         this.applicationDataSerializer = new Serializer();
      }

      #endregion Constructors

      #region Properties

      [Require]
      public virtual CCSDSPacketDefinition PacketDefinition
      {
         get
         {
            return this.packetDefinition;
         }
         set
         {
            this.packetDefinition = value;
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

      #endregion Properties

      #region Public Methods

      /// <summary>
      /// Serializes the composite commands into a bitstream
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      public virtual byte[] Serialize(BitStream bitStream)
      {
         this.secondaryHeaderSerializer.DataAttributes = this.packetDefinition.SecondaryHeaderAttributes;
         this.applicationDataSerializer.DataAttributes = this.packetDefinition.ApplicationDataAttributes;

         long startPosition = bitStream.Position;
         bitStream.Write(this.packetDefinition.PrimaryHeader.Version, 0, 3);
         bitStream.Write(this.packetDefinition.PrimaryHeader.Type, 0, 1);
         bitStream.Write(this.packetDefinition.PrimaryHeader.SecondaryFlag, 0, 1);
         bitStream.Write(this.packetDefinition.PrimaryHeader.ApplicationId, 0, 11);
         bitStream.Write(this.packetDefinition.PrimaryHeader.Grouping, 0, 2);
         bitStream.Write(this.packetDefinition.PrimaryHeader.SequenceCount, 0, 14);
         bitStream.Write(this.packetDefinition.PrimaryHeader.PacketDataLength, 0, 16);
         this.secondaryHeaderSerializer.Serialize(bitStream);
         this.applicationDataSerializer.Serialize(bitStream);
         
         long endPosition = bitStream.Position;
         byte[] bytes = bitStream.ToByteArray(startPosition, bitStream.Position - startPosition);
         this.packetDefinition.Checksum.Compute(bytes);
         bitStream.Position = startPosition + this.packetDefinition.Checksum.ChecksumIndex * 8;
         switch (this.packetDefinition.Checksum.BitLength)
         {
            case 8: bytes = new byte[1] { (byte)this.packetDefinition.Checksum.ComputedChecksum }; break;
            case 16: bytes = BitConverter.GetBytes((ushort)this.packetDefinition.Checksum.ComputedChecksum); break;
            case 32: bytes = BitConverter.GetBytes((uint)this.packetDefinition.Checksum.ComputedChecksum); break;
            case 64: bytes = BitConverter.GetBytes((ulong)this.packetDefinition.Checksum.ComputedChecksum); break;
         }

         if (BitConverter.IsLittleEndian)
         {
            Array.Reverse(bytes);
         }

         bitStream.Write(bytes);

         bitStream.Position = endPosition;
         this.SerializedBytes = bitStream.ToByteArray(startPosition, bitStream.Position - startPosition);

         return this.SerializedBytes;
      }

      /// <summary>
      /// Deserializes the binary stream into the instance of data type based on the binary specification configured by end-user.
      /// </summary>
      /// <param name="bitStream"></param>
      /// <param name="byteOrder"></param>
      /// <returns></returns>
      public virtual object[] Deserialize(BitStream bitStream)
      {
         this.secondaryHeaderSerializer.DataAttributes = this.packetDefinition.SecondaryHeaderAttributes;
         this.applicationDataSerializer.DataAttributes = this.packetDefinition.ApplicationDataAttributes;

         CCSDSPacket packet = new CCSDSPacket();
        
         CCSDSPacketSerializer.DeserializePrimaryHeader(bitStream, this.packetDefinition.PrimaryHeader);
         packet.PrimaryHeader = ObjectExtensions.Copy<CCSDSPrimaryHeader>(this.packetDefinition.PrimaryHeader);
         packet.SecondaryHeader = this.secondaryHeaderSerializer.Deserialize(bitStream, this.packetDefinition.SecondaryHeader)[0];
         packet.ApplicationData = this.applicationDataSerializer.Deserialize(bitStream, this.packetDefinition.ApplicationData)[0];

         return new object[] { packet };
      }

      /// <summary>
      /// Deserializes the binary stream into the instance of data type based on the binary specification configured by end-user.
      /// </summary>a
      /// <param name="bitStream"></param>
      /// <param name="byteOrder"></param>
      /// <returns></returns>
      public static void DeserializePrimaryHeader(BitStream bitStream, CCSDSPrimaryHeader primaryHeader)
      {
         byte b;
         bitStream.Read(out b, 0, 3);
         primaryHeader.Version = b;

         bitStream.Read(out b, 0, 1);
         primaryHeader.Type = b;

         bitStream.Read(out b, 0, 1);
         primaryHeader.SecondaryFlag = b;

         ushort us;
         bitStream.Read(out us, 0, 11);
         primaryHeader.ApplicationId = us;

         bitStream.Read(out b, 0, 2);
         primaryHeader.Grouping = b;

         bitStream.Read(out us, 0, 14);
         primaryHeader.SequenceCount = us;

         bitStream.Read(out us, 0, 16);
         primaryHeader.PacketDataLength = us;
      }

      #endregion Command overrides

      #region Component overrides

      public override bool OnInitialize()
      {
         return base.OnInitialize();
      }

      #endregion Component overrides
   }
}
