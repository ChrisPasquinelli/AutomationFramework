// <copyright file="TimeSequenceSerializer.cs" company="Genesis Engineering Services">
// Copyright (c) 2021 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2021-01-17</date>
// <summary>Generates the binary stream of an absolute/relative sequence of commands ordered with respect to time</summary>

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
   /// TimeSequenceSerializer generates the binary stream of an absolute/relative sequence of commands ordered with respect to time
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Tables" })]
   [Description("Generates the binary absolute/relative sequence of commands ordered with respect to time.")]
   public class TimeSequenceSerializer :
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
      /// .The content serializer
      /// </summary>
      private ICommand[] commands;

      /// <summary>
      /// The serialized bytes
      /// </summary>
      private byte[] serializedBytes;


      /// <summary>
      /// The mission epoch time for an absolute time sequence only
      /// </summary>
      private Time missionEpochTime;

      /// <summary>
      /// The absolute start time for an absolute time sequence only
      /// </summary>
      private Time startTime;
 
      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CCSDSCommand"/> class
      /// </summary>
      public TimeSequenceSerializer() :
         base()
      {
         this.commands = new ICommand[0];
         this.serializedBytes = new byte[0];
         this.missionEpochTime = new Time(DateTime.UtcNow, TimeStandard.CoordinatedUniversalTime);
         this.startTime = new Time(DateTime.UtcNow, TimeStandard.CoordinatedUniversalTime);
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
                           BitStream bitStream = new BitStream();
                           bitStream.Write(this.SerializedBytes);
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
               catch (Exception ex)
               {
                  this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, this.Identifier, this, MessageType.Error));
               }
            }

            this.start = value;
         }
      }

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

      public Time MissionEpochTime
      {
         get
         {
            return this.missionEpochTime;
         }

         set
         {
            this.missionEpochTime.BaseStandard = value.BaseStandard;
            this.missionEpochTime.DateTime = value.DateTime;
            this.OnPropertyChanged("MissionEpochTime");
         }
      }
      public Time StartTime
      {
         get
         {
            return this.startTime;
         }

         set
         {
            this.startTime.BaseStandard = value.BaseStandard;
            this.startTime.DateTime = value.DateTime;
            this.OnPropertyChanged("StartTime");
         }
      }

      #endregion Properties

      #region Command overrides

      /// <summary>
      /// Serializes the composite commands into a bitstream
      /// </summary>
      /// <param name="primitives">list of primitive commands</param>
      public virtual byte[] Serialize(BitStream bitStream, ByteOrder byteOrder)
      {
         Time referenceTime = this.startTime;
         foreach (ICommand command in this.Commands)
         {
            referenceTime += command.StartOffset;
            command.SetStartTime(referenceTime);
         }

         List<ICommand> primitives = new List<ICommand>();
         foreach (ICommand command in this.Commands)
         {
            command.Expand(primitives);
         }

         if (primitives.Count == 0)
         {
            return new byte[0];
         }

         List<ICommand> timeSequence = new List<ICommand>();
         timeSequence.Add(primitives[0]);
         for(int i = 1; i < primitives.Count; i++)
         {
            for(int j = 0; j < timeSequence.Count; j++)
            {
               if(primitives[i].GetStartTime() < timeSequence[j].GetStartTime())
               {
                  timeSequence.Insert(j, primitives[i]);
                  break;
               }
            }
         }

         long startPosition = bitStream.Position;
         ushort commandNumber = 1;
         foreach (ICommand command in timeSequence)
         {
            byte[] u16Bytes = BitConverter.GetBytes(commandNumber);
            ushort commandNumberLE = BitConverter.ToUInt16(u16Bytes, 0);
            bitStream.Write(commandNumberLE);
            commandNumber++;
            uint time = (uint)((command.GetStartTime() - this.missionEpochTime).TotalSeconds);
            byte[] u32Bytes = BitConverter.GetBytes(time);
            uint timeLE = BitConverter.ToUInt32(u32Bytes, 0);
            bitStream.Write(timeLE);
            command.Serialize(bitStream);
         }

         byte[] bytes = bitStream.ToByteArray(startPosition, bitStream.Position - startPosition);

         this.SerializedBytes = bytes;

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
         List<ICommand> timeSequence = new List<ICommand>();
         long startPosition = bitStream.Position;
         int commandNumber = 0;
         foreach (ICommand command in timeSequence)
         {
            bitStream.Write(commandNumber++);
            bitStream.Write((uint)((command.GetStartTime() - this.missionEpochTime).TotalSeconds));
            command.Serialize(bitStream);
         }



         byte[] bytes = bitStream.ToByteArray(startPosition, bitStream.Position - startPosition);

         this.SerializedBytes = bytes;

         return new object[0];
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
