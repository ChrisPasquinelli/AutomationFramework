// <copyright file="Serializer.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Serializer for transfering an array of bytes into and from selected data.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.ComponentModel;
   using MTI.Core;
   using BKSystem.IO;
   using System.Collections.Generic;

   #endregion

   /// <summary>
   /// Serializer for transfering an array of bytes into selected members of an instance
   /// </summary>
   public class Serializer
   {
      #region Fields

      /// <summary>
      /// The properties to use as columns as determined by end user.
      /// </summary>
      private DataAttributes[] dataAttributes;

      /// <summary>
      /// The serialized bytes
      /// </summary>
      public byte[] serializedOutput;

      /// <summary>
      /// The deserialized objects
      /// </summary>
      public object[] deserializedOutput;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="Serializer"/> class.
      /// </summary>
      public Serializer() :
         base()
      {
         this.dataAttributes = new DataAttributes[0];
         this.deserializedOutput = new object[0];
         this.serializedOutput = new byte[0];
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Serializer"/> class.
      /// </summary>
      public Serializer(DataAttributes[] dataAttributes) :
         base()
      {
         this.dataAttributes = dataAttributes;
         this.deserializedOutput = new object[0];
         this.serializedOutput = new byte[0];
      }

      #endregion Constructors

      #region Enumerations
      #endregion Enumerations

      #region Public Properties

      /// <summary>
      /// Gets or sets the properties to use as columns as determined by user.
      /// </summary>
      public virtual DataAttributes[] DataAttributes
      {
         get
         {
            return this.dataAttributes;
         }

         set
         {
            this.dataAttributes = value;
         }
      }

      #endregion Public Properties

      #region Public Methods

      public void Initialize()
      {

      }

      /// <summary>
      /// Serializes the data type based on the binary specification configured by end-user.
      /// </summary>
      /// <param name="bitStream"></param>
      /// <param name="byteOrder"></param>
      /// <returns></returns>
      public byte[] Serialize(BitStream bitStream)
      {
         long start = bitStream.Position;

         foreach (DataAttributes attributes in this.dataAttributes)
         {
            ObjectNode node = attributes.ObjectNode;

            if (node == null)
            {
               continue;
            }

            Type type = node.Value.GetType();
            switch (type.FullName)
            {
               case "System.Boolean": bitStream.Write((bool)node.Value); break;
               case "System.Boolean[]": bitStream.Write((bool[])node.Value); break;
               case "System.Byte": bitStream.Write((byte)node.Value, 0, attributes.BitLength); break;
               case "System.Byte[]": bitStream.Write((byte[])node.Value, 0, attributes.BitLength / 8); break;
               case "System.SByte": bitStream.Write((sbyte)node.Value, 0, attributes.BitLength); break;
               case "System.SByte[]": bitStream.Write((sbyte)node.Value, 0, attributes.BitLength); break;
               case "System.Char": bitStream.Write((char)node.Value, 0, attributes.BitLength); break;
               case "System.Char[]": bitStream.Write((char[])node.Value, 0, attributes.BitLength); break;
               case "System.String":
                  string str = (string)node.Value;
                  char[] charStr = new char[attributes.BitLength / 8];
                  for (int i = 0; i < str.Length; i++)
                  {
                     charStr[i] = str[i];
                  }

                  for (int i = str.Length; i < charStr.Length; i++)
                  {
                     charStr[i] = '\0';
                  }

                  bitStream.Write(charStr, 0, charStr.Length);
                  break;
               default:
                  if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian)
                  {
                     switch (type.FullName)
                     {
                        case "System.UInt16": bitStream.Write(ReverseBytes((ushort)node.Value), 0, attributes.BitLength); break;
                        case "System.UInt16[]": bitStream.Write(ReverseBytes((ushort[])node.Value), 0, attributes.BitLength); break;
                        case "System.UInt32": bitStream.Write(ReverseBytes((uint)node.Value), 0, attributes.BitLength); break;
                        case "System.UInt32[]": bitStream.Write(ReverseBytes((uint[])node.Value), 0, attributes.BitLength); break;
                        case "System.UInt64": bitStream.Write(ReverseBytes((ulong)node.Value), 0, attributes.BitLength); break;
                        case "System.UInt64[]": bitStream.Write(ReverseBytes((ulong[])node.Value), 0, attributes.BitLength); break;
                        case "System.Int16": bitStream.Write(ReverseBytes((ushort)(short)node.Value), 0, attributes.BitLength); break;
                        case "System.Int16[]": bitStream.Write(ReverseBytes((short[])node.Value), 0, attributes.BitLength); break;
                        case "System.Int32": bitStream.Write(ReverseBytes((uint)(int)node.Value), 0, attributes.BitLength); break;
                        case "System.Int32[]": bitStream.Write(ReverseBytes((int[])node.Value), 0, attributes.BitLength); break;
                        case "System.DateTime": bitStream.Write(ReverseBytes((ulong)((DateTime)node.Value).Ticks), 0, attributes.BitLength); break;
                        case "System.Int64": bitStream.Write(ReverseBytes((ulong)(long)node.Value), 0, attributes.BitLength); break;
                        case "System.Int64[]": bitStream.Write(ReverseBytes((long[])node.Value), 0, attributes.BitLength); break;
                        case "System.Single": bitStream.Write(ReverseBytes((uint)(float)node.Value), 0, attributes.BitLength); break;
                        case "System.Single[]": bitStream.Write(ReverseBytes((float[])node.Value), 0, attributes.BitLength); break;
                        case "System.Double": bitStream.Write(ReverseBytes((ulong)(double)node.Value), 0, attributes.BitLength); break;
                        case "System.Double[]": bitStream.Write(ReverseBytes((double[])node.Value), 0, attributes.BitLength); break;
                     }
                  }
                  else
                  {
                     switch (type.FullName)
                     {
                        case "System.UInt16": bitStream.Write((ushort)node.Value, 0, attributes.BitLength); break;
                        case "System.UInt16[]": bitStream.Write((ushort[])node.Value, 0, attributes.BitLength); break;
                        case "System.UInt32": bitStream.Write((uint)node.Value, 0, attributes.BitLength); break;
                        case "System.UInt32[]": bitStream.Write((uint[])node.Value, 0, attributes.BitLength); break;
                        case "System.UInt64": bitStream.Write((ulong)node.Value, 0, attributes.BitLength); break;
                        case "System.UInt64[]": bitStream.Write((ulong[])node.Value, 0, attributes.BitLength); break;
                        case "System.Int16": bitStream.Write((short)node.Value, 0, attributes.BitLength); break;
                        case "System.Int16[]": bitStream.Write((short[])node.Value, 0, attributes.BitLength); break;
                        case "System.Int32": bitStream.Write((int)node.Value, 0, attributes.BitLength); break;
                        case "System.Int32[]": bitStream.Write((int[])node.Value, 0, attributes.BitLength); break;
                        case "System.DateTime": bitStream.Write((long)((DateTime)node.Value).Ticks, 0, attributes.BitLength); break;
                        case "System.Int64": bitStream.Write((long)node.Value, 0, attributes.BitLength); break;
                        case "System.Int64[]": bitStream.Write((long[])node.Value, 0, attributes.BitLength); break;
                        case "System.Single": bitStream.Write((float)node.Value, 0, attributes.BitLength); break;
                        case "System.Single[]": bitStream.Write((float[])node.Value, 0, attributes.BitLength); break;
                        case "System.Double": bitStream.Write((double)node.Value, 0, attributes.BitLength); break;
                        case "System.Double[]": bitStream.Write((double[])node.Value, 0, attributes.BitLength); break;
                     }
                  }

               break;
            }
         }

         return bitStream.ToByteArray(start, bitStream.Position - start);
      }

      /// <summary>
      /// Deserializes the binary stream into the instance of data type based on the binary specification configured by end-user.
      /// </summary>
      /// <param name="bitStream"></param>
      /// <param name="byteOrder"></param>
      /// <returns></returns>
      public object[] Deserialize(BitStream bitStream, params object[] objects)
      {
         foreach (DataAttributes attributes in this.dataAttributes)
         {
            ObjectNode node = attributes.ObjectNode;
            if (node == null)
            {
               continue;
            }

            Type type = node.Value.GetType();
            switch (type.FullName)
            {
               case "System.Boolean":
                  {
                     bool value;
                     bitStream.Read(out value);
                     node.Value = value;
                     break;
                  }
               case "System.Boolean[]":
                  {
                     bool[] value = (bool[])node.Value;
                     bitStream.Read(value);
                     break;
                  }
               case "System.Byte":
                  {
                     byte value;
                     bitStream.Read(out value, 0, attributes.BitLength);
                     node.Value = value;
                     break;
                  }
               case "System.Byte[]":
                  {
                     byte[] value = (byte[])node.Value;
                     bitStream.Read(value, 0, attributes.BitLength / 8);
                     break;
                  }
               case "System.SByte":
                  {
                     sbyte value;
                     bitStream.Read(out value, 0, attributes.BitLength);
                     node.Value = value;
                     break;
                  }
               case "System.SByte[]":
                  {
                     sbyte[] value = (sbyte[])node.Value;
                     bitStream.Read(value, 0, attributes.BitLength);
                     break;
                  }
               case "System.Char":
                  {
                     char value;
                     bitStream.Read(out value, 0, attributes.BitLength);
                     node.Value = value;
                     break;
                  }
               case "System.Char[]":
                  {
                     char[] value = (char[])node.Value;
                     bitStream.Read(value, 0, attributes.BitLength);
                     break;
                  }
               case "System.String":
                  {
                     char[] chStr = new char[attributes.BitLength / 8];
                     bitStream.Read(chStr, 0, chStr.Length);
                     node.Value = new string(chStr);
                     break;
                  }

               default:
                  switch (type.FullName)
                     {
                        case "System.UInt16":
                           ushort ushortValue;
                           bitStream.Read(out ushortValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(ushortValue);
                           else node.Value = ushortValue;
                           break;
                        case "System.UInt16[]":
                           ushort[] ushortArray = (ushort[])node.Value;
                           bitStream.Read(ushortArray, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(ushortArray);
                           else node.Value = ushortArray;
                           break;
                        case "System.UInt32":
                           uint uintValue;
                           bitStream.Read(out uintValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(uintValue);
                           else node.Value = uintValue;
                           break;
                        case "System.UInt32[]":
                           uint[] uintArray = (uint[])node.Value;
                           bitStream.Read(uintArray, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(uintArray);
                           else node.Value = uintArray;
                           break;
                        case "System.UInt64":
                           ulong ulongValue;
                           bitStream.Read(out ulongValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(ulongValue);
                           else node.Value = ulongValue;
                        break;
                        case "System.UInt64[]":
                           ulong[] ulongArray = (ulong[])node.Value;
                           bitStream.Read(ulongArray, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(ulongArray);
                           else node.Value = ulongArray;
                           break;
                        case "System.Int16":
                           short shortValue;
                           bitStream.Read(out shortValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes((ushort)shortValue);
                           else node.Value = shortValue;
                           break;
                        case "System.Int16[]":
                           short[] shortArray = (short[])node.Value;
                           bitStream.Read(shortArray, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(shortArray);
                           else node.Value = shortArray;
                           break;
                        case "System.Int32":
                           int intValue;
                           bitStream.Read(out intValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes((uint)intValue);
                           else node.Value = intValue;
                           break;
                        case "System.Int32[]":
                           int[] intArray = (int[])node.Value;
                           bitStream.Read(intArray, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(intArray);
                           else node.Value = intArray;
                           break;
                        case "System.DateTime":
                           long dateTime;
                           bitStream.Read(out dateTime);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = new DateTime(ReverseBytes((uint)dateTime));
                           else node.Value = new DateTime(dateTime);
                           break;
                        case "System.Int64":
                           long longValue;
                           bitStream.Read(out longValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes((ulong)longValue);
                           else node.Value = longValue;
                           break;
                        case "System.Int64[]":
                           long[] longArray = (long[])node.Value;
                           bitStream.Read(longArray, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(longArray);
                           else node.Value = longArray;
                           break;
                        case "System.Single":
                           float floatValue;
                           bitStream.Read(out floatValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes((uint)floatValue);
                           else node.Value = floatValue;
                           break;
                        case "System.Single[]":
                           float[] floatArray = (float[])node.Value;
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(floatArray);
                           else node.Value = floatArray;
                           break;
                        case "System.Double":
                           double doubleValue;
                           bitStream.Read(out doubleValue, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes((ulong)doubleValue);
                           else node.Value = doubleValue;
                           break;
                        case "System.Double[]":
                           double[] doubleArray = (double[])node.Value;
                           bitStream.Read(doubleArray, 0, attributes.BitLength);
                           if (attributes.Endian == Communications.DataAttributes.EndianType.LittleEndian) node.Value = ReverseBytes(doubleArray);
                           else node.Value = doubleArray;
                           break;
                     }
               break;
            }
         }

         List<object> dataObjects = new List<object>();
         foreach (object dataObject in objects)
         {
            dataObjects.Add(ObjectExtensions.Copy(dataObject));
         }

         this.deserializedOutput = dataObjects.ToArray();
         return this.deserializedOutput;
      }

      #endregion Public Methods

      #region Private Methods

      private static ushort ReverseBytes(ushort value)
      {
         return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
      }
      public static uint ReverseBytes(uint value)
      {
         return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
      }
      private static ulong ReverseBytes(ulong value)
      {
         return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
      }
      private ushort[] ReverseBytes(ushort[] values)
      {
         ushort[] reversed = new ushort[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = ReverseBytes(values[i]);
         }

         return reversed;
      }

      private short[] ReverseBytes(short[] values)
      {
         short[] reversed = new short[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = (short)ReverseBytes((ushort)values[i]);
         }

         return reversed;
      }

      private uint[] ReverseBytes(uint[] values)
      {
         uint[] reversed = new uint[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = ReverseBytes(values[i]);
         }

         return reversed;
      }

      private int[] ReverseBytes(int[] values)
      {
         int[] reversed = new int[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = (int)ReverseBytes((uint)values[i]);
         }

         return reversed;
      }

      private ulong[] ReverseBytes(ulong[] values)
      {
         ulong[] reversed = new ulong[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = ReverseBytes(values[i]);
         }

         return reversed;
      }

      private long[] ReverseBytes(long[] values)
      {
         long[] reversed = new long[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = (long)ReverseBytes((ulong)values[i]);

         }

         return reversed;
      }

      private float[] ReverseBytes(float[] values)
      {
         float[] reversed = new float[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = (float)ReverseBytes((uint)values[i]);
         }

         return reversed;
      }

      private double[] ReverseBytes(double[] values)
      {
         double[] reversed = new double[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            reversed[i] = (double)ReverseBytes((ulong)values[i]);
         }

         return reversed;
      }

      #endregion Private Methods
   }
}
