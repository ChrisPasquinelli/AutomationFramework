// <copyright file="DataAttributes.cs" company="Genesis Engineering Servies">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>The data attributes defining the binary specifications of a data object.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.ComponentModel;

   using MTI.Core;
 
   #endregion

   public class  DataAttributes:
      SerializerBase.SerializerDescriptor
   {
      public class Enumeration
      {
         public string Name { get; set; }
         public string Value { get; set; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="DataAttributes"/> class.
      /// </summary>
      public DataAttributes()
      {
         this.Name = string.Empty;
         this.DefaultValue = string.Empty;
         this.MinValue = string.Empty;
         this.MaxValue = string.Empty;
         this.DataType = string.Empty;
         this.BitLength = 0;
         this.Endian = EndianType.LittleEndian;
         this.Equation = string.Empty;
         this.EnumeratedValues = new Enumeration[0];
      }

      public enum EndianType
      {
         BigEndian,
         LittleEndian,
      }

      /// <summary>
      /// Gets or sets the name of the column
      /// </summary>
      public string Name
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the whether the value is static
      /// </summary>
      public bool Static
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the lowest threshold
      /// </summary>
      public string DefaultValue
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the lowest threshold
      /// </summary>
      public string MinValue
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the highest threshold
      /// </summary>
      public string MaxValue
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the data type
      /// </summary>
      public string DataType
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the bit length.
      /// </summary>
      public int BitLength
      {
         get;
         set;
      }

      public EndianType Endian
      {
         get;
         set;
      }

      public string Equation
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the assignable discrete values
      /// </summary>
      public Enumeration[] EnumeratedValues
      {
         get;
         set;
      }

      public override bool SetDefaultAttributes(ObjectNode node)
      {
         if (node == null)
         {
            return false;
         }

         this.ObjectNode = node;
         Type type = node.Value.GetType();
         this.DataType = type.Name;
         if (node.ParentNode != null && node.ParentNode is ArrayElementNode)
         {
            this.Name = node.ParentNode.Name + "." + node.Name;
         }
         else
         {
            this.Name = node.Name;
         }

         if (type.IsPrimitive || type == typeof(DateTime))
         {
            this.DefaultValue = node.Value.ToString();
            this.MaxValue = DataAttributes.GetMaxValue(type.FullName);
            this.MinValue = DataAttributes.GetMinValue(type.FullName);
            this.BitLength = DataAttributes.GetSize(type.FullName) * 8;
         }
         else if (type.IsEnum)
         {
            this.DefaultValue = node.Value.ToString();
            this.MaxValue = DataAttributes.GetMaxValue(Enum.GetUnderlyingType(type).FullName);
            this.MinValue = DataAttributes.GetMinValue(Enum.GetUnderlyingType(type).FullName);
            this.BitLength = DataAttributes.GetSize(Enum.GetUnderlyingType(type).FullName) * 8;

            List<Enumeration> enumerations = new List<Enumeration>();
            foreach (string name in Enum.GetNames(type))
            {
               Enumeration enumeration = new Enumeration();
               enumeration.Name = name;
               enumeration.Value = Enum.Parse(type, name).ToString();
            }

            this.EnumeratedValues = enumerations.ToArray();
         }
         else if (type.IsArray && ((Array)node.Value).Length > 0)
         {
            Array array = (Array)node.Value;
            this.MaxValue = DataAttributes.GetMaxValue(type.GetElementType().FullName);
            this.MinValue = DataAttributes.GetMinValue(type.GetElementType().FullName);
            this.BitLength = array.Length * DataAttributes.GetSize(type.GetElementType().FullName) * 8;
         }

         return true;
      }

      /// <summary>
      /// Gets the byte size of the type specified by U
      /// </summary>
      /// <param name="instance">the instance whose byte size is queried</param>
      /// <returns>The byte size</returns>
      public static int GetSize(string typeFullName)
      {
         switch (typeFullName)
         {
            case "System.Boolean": return 1;
            case "System.Byte": return 1;
            case "System.SByte": return 1;
            case "System.UInt16": return 2;
            case "System.UInt32": return 4;
            case "System.UInt64": return 8;
            case "System.Int16": return 2;
            case "System.Int32": return 4;
            case "System.DateTime": return 8;
            case "System.Int64": return 8;
            case "System.Char": return 2;
            case "System.Single": return 4;
            case "System.Double": return 8;
            default:
               return 0;
         }
      }

      /// <summary>
      /// Gets the byte size of the type specified by U
      /// </summary>
      /// <param name="instance">the instance whose byte size is queried</param>
      /// <returns>The byte size</returns>
      public static string GetMaxValue(string typeFullName)
      {
         switch (typeFullName)
         {
            case "System.Boolean": return "1";
            case "System.Byte": return byte.MaxValue.ToString();
            case "System.SByte": return sbyte.MaxValue.ToString();
            case "System.UInt16": return ushort.MaxValue.ToString();
            case "System.UInt32": return uint.MaxValue.ToString();
            case "System.UInt64": return ulong.MaxValue.ToString();
            case "System.Int16": return short.MaxValue.ToString();
            case "System.Int32": return int.MaxValue.ToString();
            case "System.DateTime": return DateTime.MaxValue.ToString();
            case "System.Int64": return long.MaxValue.ToString();
            case "System.Char": return char.MaxValue.ToString();
            case "System.Single": return float.MaxValue.ToString();
            case "System.Double": return double.MaxValue.ToString();
            default:
               return "0";
         }
      }

      /// <summary>
      /// Gets the byte size of the type specified by U
      /// </summary>
      /// <param name="instance">the instance whose byte size is queried</param>
      /// <returns>The byte size</returns>
      public static string GetMinValue(string typeFullName)
      {
         switch (typeFullName)
         {
            case "System.Boolean": return "0";
            case "System.Byte": return byte.MinValue.ToString();
            case "System.SByte": return sbyte.MinValue.ToString();
            case "System.UInt16": return ushort.MinValue.ToString();
            case "System.UInt32": return uint.MinValue.ToString();
            case "System.UInt64": return ulong.MinValue.ToString();
            case "System.Int16": return short.MinValue.ToString();
            case "System.Int32": return int.MaxValue.ToString();
            case "System.DateTime": return DateTime.MinValue.ToString();
            case "System.Int64": return long.MinValue.ToString();
            case "System.Char": return char.MinValue.ToString();
            case "System.Single": return float.MinValue.ToString();
            case "System.Double": return double.MinValue.ToString();
            default:
               return "0";
         }
      }
   }
}
