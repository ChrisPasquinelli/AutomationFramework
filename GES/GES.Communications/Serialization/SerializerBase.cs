// <copyright file="SerializerBase.cs" company="Genesis Engineering Services">
// Copyright (c) 2011 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Base serializer class for flattening components into a list </summary>
namespace GES.Communications
{
   #region Directives
   
   using System.ComponentModel;
   using MTI.Core;
   
   #endregion

   /// <summary>
   /// Serializes/Deserializes a file into a table of user-defined records.
   /// </summary>
   public abstract class SerializerBase :
      MTI.Core.Component,
      IGraphUpdateSubscriber
   {
      #region Public Properties

      /// <summary>
      /// Gets or sets the object tree of the record template.
      /// </summary>
      [Browsable(false)]
      public ObjectNode TemplatePropertyNode
      {
         get;
         set;
      }

      #endregion

      #region Protected Properties

      /// <summary>
      /// Gets the name of the property referencing the templates
      /// </summary>
      protected abstract string TemplatePropertyName
      {
         get;
      }

      /// <summary>
      /// Gets or sets the object tree of the record template.
      /// </summary>
      protected ObjectNode ThisNode
      {
         get;
         set;
      }

      #endregion

      #region Public Methods
      
      /// <summary>
      /// Get the path name of the property relative to this instance
      /// </summary>
      /// <param name="fullPathName">The full path name of the property from the root container of this instance</param>
      /// <returns>The relative path of the specified property</returns>
      public string GetRelativePathName(string fullPathName)
      {
         if (this.ThisNode.ParentNode != null)
         {
            if (fullPathName.StartsWith(this.ThisNode.ParentNode.FullPath))
            {
               return fullPathName.Substring(this.ThisNode.ParentNode.FullPath.Length + 1);
            }
            else
            {
               return fullPathName;
            }
         }
         else if (fullPathName.StartsWith(this.ThisNode.FullPath))
         {
            return fullPathName.Substring(this.ThisNode.FullPath.Length + 1);
         }
         else
         {
            return fullPathName;
         }
      }

      /// <summary>
      /// Get the absolute path name of the specified property
      /// </summary>
      /// <param name="relativePathName">The path name of the property relative to this instance</param>
      /// <returns>The absolute path of the specified property</returns>
      public string GetFullPathName(string relativePathName)
      {
         //if (this.ThisNode.ParentNode != null)
         //{
         //   return this.ThisNode.ParentNode.FullPath + '.' + relativePathName; 
         //}
         string name = relativePathName;
         int index = relativePathName.IndexOf('.');
         if(index > -1)
         {
            name = relativePathName.Substring(0, index);
         }
          if(this.ThisNode.PropertiesByName.ContainsKey(name))
         {
            return this.ThisNode.FullPath + '.' + relativePathName;
         }
         else if (this.ThisNode.ParentNode != null && this.ThisNode.ParentNode.PropertiesByName.ContainsKey(name))
         {
            return this.ThisNode.ParentNode.FullPath + '.' + relativePathName;
         }
         else
         {
            return relativePathName;
         }
      }

      #endregion

      #region Framework Event Handlers

      /// <summary>
      /// Gets the object node of this component.
      /// </summary>
      /// <param name="thisGraph">The graph of the component in which this subcomponent belongs</param>
      /// <param name="thisNode">The object node of this component</param>
      public virtual void OnGraphUpdate(ObjectGraph thisGraph, ObjectNode thisNode)
      {
         foreach (ObjectNode property in thisNode.Properties)
         {
            if (property.Name == this.TemplatePropertyName)
            {
               this.ThisNode = thisNode;
               this.TemplatePropertyNode = property;
               return;
            }
         }
      }

      /// <summary>
      /// Event handler that initializes this instance.
      /// </summary>
      /// <returns>A value indicating whether the object successfully initialized.</returns>
      public override bool OnInitialize()
      {
         return base.OnInitialize();
      }

      #endregion

      #region Nested Types

      /// <summary>
      /// The base descriptor class mapping an object node.
      /// </summary>
      public class SerializerDescriptor
      {
         /// <summary>
         /// Initializes a new instance of the <see cref="SerializerDescriptor"/> class.
         /// </summary>
         public SerializerDescriptor()
         {
            this.LocalPathName = string.Empty;
         }

         /// <summary>
         /// Gets or sets the local path name
         /// </summary>
         public string LocalPathName
         {
            get;
            set;
         }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="node"></param>
         [Browsable(false)]
         public ObjectNode ObjectNode
         {
            get;
            set;
         }


         /// <summary>
         /// 
         /// </summary>
         /// <param name="node">the object node to derive default attributes</param>
         public virtual bool SetDefaultAttributes(ObjectNode node)
         {
            return true;
         }
      }

      #endregion
   }
}
