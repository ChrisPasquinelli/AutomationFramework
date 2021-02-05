// <copyright file="Serializer.cs" company="Genesis Engineering Services">
// Copyright (c) 2020 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2020-12-20</date>
// <summary>Class defining a table based on a user configuration.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.ComponentModel;

   using BKSystem.IO;

   using MTI.Core;

   #endregion

   /// <summary>
   /// Class defining a table based on a user configuration
   /// </summary>
   [Provide(Categories = new string[] { "Serialization", "GES.Tables"})]
   [Description("Serializes the table members into a bit stream and deserializes a bit stream into the members of a table")]
   public class TableDefinition :
      SerializerBase
   {
      #region Fields

      /// <summary>
      /// The file header to be serialized/deserialized
      /// </summary>
      private object fileHeader;

      /// <summary>
      /// The table header to be serialized/deserialized
      /// </summary>
      private object tableHeader;

      /// <summary>
      /// The table header to be serialized/deserialized
      /// </summary>
      private object content;

      /// <summary>
      /// Information describing the application data
      /// </summary>
      private DataAttributes[] fileHeaderAttributes;

      /// <summary>
      /// Information describing the application data
      /// </summary>
      private DataAttributes[] tableHeaderAttributes;

      /// <summary>
      /// Information describing the application data
      /// </summary>
      private DataAttributes[] contentAttributes;

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
      private ObjectNode fileHeaderNode;

      /// <summary>
      /// Supports SerializerBase interface
      /// </summary>
      private ObjectNode tableHeaderNode;

      /// <summary>
      /// Supports SerializerBase interface
      /// </summary>
      private ObjectNode contentNode;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="Serializer"/> class.
      /// </summary>
      public TableDefinition() :
         base()
      {
         this.fileHeader = new object();
         this.tableHeader = new object();
         this.content = new object();
         this.fileHeaderAttributes = new DataAttributes[0];
         this.tableHeaderAttributes = new DataAttributes[0];
         this.contentAttributes = new DataAttributes[0];
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
      public object FileHeader
      {
         get
         {
            return this.fileHeader;
         }

         set
         {
            this.fileHeader = value;
            this.OnPropertyChanged("FileHeader");
         }
      }

      /// <summary>
      /// Gets or sets the table header definition
      /// </summary>
      [Require]
      public object TableHeader
      {
         get
         {
            return this.tableHeader;
         }

         set
         {
            this.tableHeader = value;
            this.OnPropertyChanged("TableHeader");
         }
      }

      /// <summary>
      /// Gets or sets the table content definition
      /// </summary>
      [Require]
      public object Content
      {
         get
         {
            return this.content;
         }

         set
         {
            this.content = value;
            this.OnPropertyChanged("Content");
         }
      }

      #endregion Connection Point Properties

      #region Public Properties

      /// <summary>
      /// Gets or sets the properties to use as columns as determined by user.
      /// </summary>
      [Editor(typeof(GES.Communications.SerializerUITypeEditor<DataAttributes>), typeof(System.Drawing.Design.UITypeEditor))]
      [Description("Properties that are used as columns in the the table")]
      public virtual DataAttributes[] FileHeaderAttributes
      {
         get
         {
            this.templatePropertyName = "FileHeader";
            this.TemplatePropertyNode = this.fileHeaderNode;
            return this.fileHeaderAttributes;
         }

         set
         {
            this.fileHeaderAttributes = value;
            if (this.ThisNode != null) 
            {
               this.OnPropertyChanged("FileHeaderAttributes");
            }
            else
            {
               int bitSize = 0;
               for (int i = 0; i < this.fileHeaderAttributes.Length; i++)
               {
                  bitSize += this.fileHeaderAttributes[i].BitLength;
               }

               this.FileHeaderSize = bitSize / 8;
               this.TotalFileSize = this.FileHeaderSize + this.TableHeaderSize + this.ContentSize;
               this.OnPropertyChanged("FileHeaderAttributes");
            }
         }
      }

      /// <summary>
      /// Gets or sets the properties to use as columns as determined by user.
      /// </summary>
      [Editor(typeof(GES.Communications.SerializerUITypeEditor<DataAttributes>), typeof(System.Drawing.Design.UITypeEditor))]
      [Description("Properties that are used as columns in the the table")]
      public virtual DataAttributes[] TableHeaderAttributes
      {
         get
         {
            this.templatePropertyName = "TableHeader";
            this.TemplatePropertyNode = this.tableHeaderNode;
            return this.tableHeaderAttributes;
         }

         set
         {
            this.tableHeaderAttributes = value;
            if (this.ThisNode != null)
            {
               this.OnPropertyChanged("TableHeaderAttributes");
            }
            else
            {
               int bitSize = 0;
               for (int i = 0; i < this.tableHeaderAttributes.Length; i++)
               {
                  bitSize += this.tableHeaderAttributes[i].BitLength;
               }

               this.TableHeaderSize = bitSize / 8;
               this.TotalFileSize = this.FileHeaderSize + this.TableHeaderSize + this.ContentSize;
               this.OnPropertyChanged("TableHeaderAttributes");
            }
         }
      }

      /// <summary>
      /// Gets or sets the properties to use as columns as determined by user.
      /// </summary>
      [Editor(typeof(GES.Communications.SerializerUITypeEditor<DataAttributes>), typeof(System.Drawing.Design.UITypeEditor))]
      [Description("Properties that are used as columns in the the table")]
      public virtual DataAttributes[] ContentAttributes
      {
         get
         {
            this.templatePropertyName = "Content";
            this.TemplatePropertyNode = this.contentNode;
            return this.contentAttributes;
         }

         set
         {
            this.contentAttributes = value;
            if (this.ThisNode != null)
            {
               this.OnPropertyChanged("ContentAttributes");
            }
            else
            {
               int bitSize = 0;
               for (int i = 0; i < this.contentAttributes.Length; i++)
               {
                  bitSize += this.contentAttributes[i].BitLength;
               }

               this.ContentSize = bitSize / 8;
               this.TotalFileSize = this.FileHeaderSize + this.TableHeaderSize + this.ContentSize;
               this.OnPropertyChanged("ContentAttributes");
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

      public int ContentSize
      {
         get;
         set;
      }

      public int TotalFileSize
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
            if (property.Name == "FileHeader")
            {
               this.ThisNode = thisNode;
               this.fileHeaderNode = property;
            }
            else if (property.Name == "TableHeader")
            {
               this.ThisNode = thisNode;
               this.tableHeaderNode = property;
            }
            else if (property.Name == "Content")
            {
               this.ThisNode = thisNode;
               this.contentNode = property;
            }
         }
      }

      public override bool OnInitialize()
      {
         foreach (DataAttributes attributes in this.fileHeaderAttributes)
         {
            attributes.ObjectNode = (KnowledgeBase.GetObjectNode(this.GetFullPathName(attributes.LocalPathName)));
            if(attributes.ObjectNode == null)
            {
               return false;
            }
         }

         foreach (DataAttributes attributes in this.tableHeaderAttributes)
         {
            attributes.ObjectNode = (KnowledgeBase.GetObjectNode(this.GetFullPathName(attributes.LocalPathName)));
            if (attributes.ObjectNode == null)
            {
               return false;
            }
         }

         foreach (DataAttributes attributes in this.contentAttributes)
         {
            attributes.ObjectNode = (KnowledgeBase.GetObjectNode(this.GetFullPathName(attributes.LocalPathName)));
            if (attributes.ObjectNode == null)
            {
               return false;
            }
         }

         return base.OnInitialize();
      }

      #endregion Public Methods
   }
}
