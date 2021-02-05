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

   using BKSystem.IO;

   using MTI.Core;

   #endregion

   /// <summary>
   /// Class defining a CCSDS packet based on a user configuration
   /// </summary>
   [Provide(Categories = new string[] { "Serialization" })]
   [Description("Serializes the CCSDS packet members into a bit stream and deserializes a bit stream into the members of a CCSDS packet")]
   public class CCSDSPacketDefinition :
      SerializerBase
   {
      #region Fields

      /// <summary>
      /// The primary header to be serialized/deserialized
      /// </summary>
      private CCSDSPrimaryHeader primaryHeader;

      /// <summary>
      /// The secondary header serializer.
      /// </summary>
      private object secondaryHeader;

      /// <summary>
      /// The application data serializer.
      /// </summary>
      private object applicationData;

      /// Information describing the secondary header
      /// </summary>
      private DataAttributes[] secondaryHeaderAttributes;

      /// <summary>
      /// Information describing the application data
      /// </summary>
      private DataAttributes[] applicationDataAttributes;

      /// <summary>
      /// Information describing the checksum
      /// </summary>
      private Checksum checksum;

      /// <summary>
      /// Supports SerializerBase interface
      /// </summary>
      private string templatePropertyName;

      /// <summary>
      /// Supports SerializerBase interface
      /// </summary>
      private ObjectNode secondaryHeaderNode;

      /// <summary>
      /// Supports SerializerBase interface
      /// </summary>
      private ObjectNode applicationDataNode;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="Serializer"/> class.
      /// </summary>
      public CCSDSPacketDefinition() :
         base()
      {
         this.primaryHeader = new CCSDSPrimaryHeader();
         this.secondaryHeader = new object();
         this.applicationData = new object();
         this.applicationDataAttributes = new DataAttributes[0];
         this.secondaryHeaderAttributes = new DataAttributes[0];
         this.checksum = new EmptyChecksum();
      }

      #endregion

      #region Enumerations
      #endregion Enumerations

      #region Connection Point Properties

      /// <summary>
      /// Gets or sets the secondary header data applicationDataAttributes
      /// </summary>
      [Require]
      public object SecondaryHeader
      {
         get
         {
            return this.secondaryHeader;
         }

         set
         {
            this.secondaryHeader = value;
            this.OnPropertyChanged("SecondaryHeader");
         }
      }

      /// <summary>
      /// Gets or sets the application data data applicationDataAttributes
      /// </summary>
      [Require]
      public object ApplicationData
      {
         get
         {
            return this.applicationData;
         }

         set
         {
            this.applicationData = value;
            this.OnPropertyChanged("ApplicationData");
         }
      }

      #endregion Connection Point Properties

      #region Public Properties

      /// <summary>
      /// Gets or sets the properties to use as columns as determined by user.
      /// </summary>
      [Editor(typeof(GES.Communications.SerializerUITypeEditor<DataAttributes>), typeof(System.Drawing.Design.UITypeEditor))]
      [Description("Properties that are used as columns in the the table")]
      public virtual DataAttributes[] SecondaryHeaderAttributes
      {
         get
         {
            this.templatePropertyName = "SecondaryHeader";
            this.TemplatePropertyNode = this.secondaryHeaderNode;
            return this.secondaryHeaderAttributes;
         }

         set
         {
            this.secondaryHeaderAttributes = value;
            if (this.ThisNode != null)
            {
               this.OnPropertyChanged("SecondaryHeaderAttributes");
            }
            else
            {
               int bitSize = 0;
               for (int i = 0; i < this.secondaryHeaderAttributes.Length; i++)
               {
                  bitSize += this.secondaryHeaderAttributes[i].BitLength;
               }

               this.SecondaryHeaderSize = bitSize / 8;
               this.TotalPacketSize = this.ApplicationDataSize + this.SecondaryHeaderSize + 6;
               this.OnPropertyChanged("SecondaryHeaderAttributes");
            }
         }
      }
      /// <summary>
      /// Gets or sets the properties to use as columns as determined by user.
      /// </summary>
      [Editor(typeof(GES.Communications.SerializerUITypeEditor<DataAttributes>), typeof(System.Drawing.Design.UITypeEditor))]
      [Description("Properties that are used as columns in the the table")]
      public virtual DataAttributes[] ApplicationDataAttributes
      {
         get
         {
            this.templatePropertyName = "ApplicationData";
            this.TemplatePropertyNode = this.applicationDataNode;
            return this.applicationDataAttributes;
         }

         set
         {
            this.applicationDataAttributes = value;
            if (this.ThisNode != null)
            {
               this.OnPropertyChanged("ApplicationDataAttributes");
            }
            else
            {
               int bitSize = 0;
               for(int i = 0; i < this.applicationDataAttributes.Length; i++)
               {
                  bitSize += this.applicationDataAttributes[i].BitLength;
               }

               this.ApplicationDataSize = bitSize / 8;
               this.TotalPacketSize = this.ApplicationDataSize + this.SecondaryHeaderSize + 6;
               this.OnPropertyChanged("ApplicationDataAttributes");
            }
         }
      }

      /// <summary>
      /// Gets or sets the attributes describing the checksum
      /// </summary>
      [Require]
      public Checksum Checksum
      {
         get
         {
            return this.checksum;
         }

         set
         {
            this.checksum = value;
            this.OnPropertyChanged("Checksum");
         }
      }

      /// <summary>
      /// Gets or sets the Primary Header template
      /// </summary>
      public CCSDSPrimaryHeader PrimaryHeader
      {
         get
         {
            return this.primaryHeader;
         }

         set
         {
            this.primaryHeader = value;
            this.OnPropertyChanged("PrimaryHeader");
         }
      }

      public int SecondaryHeaderSize
      {
         get;
         set;
      }

      public int ApplicationDataSize
      {
         get;
         set;
      }

      public int TotalPacketSize
      {
         get;
         set;
      }

      #endregion Public Properties

      #region Protected Properties
      /// <summary>
      /// Gets or sets the property name where the template is found
      /// </summary>
      protected override string TemplatePropertyName
      {
         get
         {
            return this.templatePropertyName;
         }
      }

      #endregion

      #region Framework Overrides

      /// <summary>
      /// Gets the object node of this component.
      /// </summary>
      /// <param name="thisGraph">The graph of the component in which this subcomponent belongs</param>
      /// <param name="thisNode">The object node of this component</param>
      public override void OnGraphUpdate(ObjectGraph thisGraph, ObjectNode thisNode)
      {
         foreach (ObjectNode property in thisNode.Properties)
         {
            if (property.Name == "ApplicationData")
            {
               this.ThisNode = thisNode;
               this.applicationDataNode = property;
            }
            else if(property.Name == "SecondaryHeader")
            {
               this.ThisNode = thisNode;
               this.secondaryHeaderNode = property;
            }
         }
      }

      public override bool OnInitialize()
      {
         int bitLength = 0;
         foreach (DataAttributes attributes in this.secondaryHeaderAttributes)
         {
            bitLength += attributes.BitLength;
            attributes.ObjectNode = (KnowledgeBase.GetObjectNode(this.GetFullPathName(attributes.LocalPathName)));
            if(attributes.ObjectNode == null)
            {
               return false;
            }
         }

         foreach (DataAttributes attributes in this.applicationDataAttributes)
         {
            bitLength += attributes.BitLength;
            attributes.ObjectNode = (KnowledgeBase.GetObjectNode(this.GetFullPathName(attributes.LocalPathName)));
            if (attributes.ObjectNode == null)
            {
               return false;
            }
         }

         this.PrimaryHeader.PacketDataLength = (ushort)(bitLength / 8.0 - 1);
         return base.OnInitialize();
      }

      #endregion Framework Overrides
   }
}
