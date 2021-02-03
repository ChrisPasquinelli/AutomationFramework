// <copyright file="SerializerUITypeEditor.cs" company="Genesis Engineering Services">
// Copyright (c) 2014 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2014-12-15</date>
// <summary>Serializes/deserializes components into a table.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.ComponentModel;
   using System.Drawing.Design;
   using System.Windows.Forms;
   using System.Windows.Forms.Design;
   #endregion

   /// <summary>
   /// Class binding remote data located outside of the Serializer instance to data local to the instance.
   /// </summary>
   /// <typeparam name="T">A type derived from the base serializer class</typeparam>
   public class SerializerUITypeEditor<T> :
      UITypeEditor,
      IDisposable
      where T : SerializerBase.SerializerDescriptor, new()
  {
      /// <summary>
      /// The singleton dialog class associated with the instance
      /// </summary>
      private SerializerDialog<T> dialog = new SerializerDialog<T>();

      /// <summary>
      /// The editor service
      /// </summary>
      private IWindowsFormsEditorService editorService;

      /// <summary>
      /// The type descriptor context
      /// </summary>
      private ITypeDescriptorContext context;
      
      /// <summary>
      /// Track whether Dispose has been called.
      /// </summary>   
      private bool disposed = false;

      /// <summary>
      /// Initializes a new instance of the SerializerUITypeEditor class.
      /// </summary>
      public SerializerUITypeEditor()
      {
         this.dialog.OkClick += this.OnOkClick;
      }

      /// <summary>
      /// Gets the type editor style
      /// </summary>
      /// <param name="context">The type descriptor context</param>
      /// <returns>An instance of the UITypeEditorStyle</returns>
      public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
      {
         if (context != null && context.Instance != null)
         {
            return UITypeEditorEditStyle.Modal;
         }

         return base.GetEditStyle(context);
      }

      /// <summary>
      /// Gets the binding configuration 
      /// </summary>
      /// <param name="context">The type descriptor context</param>
      /// <param name="provider">The service provider</param>
      /// <param name="value">The binding configuration</param>
      /// <returns>The updated value</returns>
      public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
      {
         this.context = context;
         if (context != null && context.Instance != null && provider != null)
         {
            this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            ////if (this.editorService != null )
            {
               string type = value.GetType().FullName;
               if (value is T[])
               {
                  this.dialog.SerializerBase = context.Instance as SerializerBase;
                  this.dialog.Columns = (T[])value;
                  this.dialog.Text = context.PropertyDescriptor.DisplayName;
                  if (this.editorService.ShowDialog(this.dialog) == DialogResult.OK)
                  {
                     return this.dialog.Columns;
                  }
               }
            }
         }

         return base.EditValue(context, provider, value);
      }

      #region IDispose
      /// <summary>
      /// Implement IDisposable.
      /// </summary>
      public void Dispose()
      {
         this.Dispose(true);
         GC.SuppressFinalize(this);
      }

      /// <summary>
      /// If disposing equals true, the method has been called directly
      /// or indirectly by a user's code. Managed and unmanaged resources
      /// can be disposed.
      /// If disposing equals false, the method has been called by the 
      /// runtime from inside the destructor and you should not reference 
      /// other objects. Only unmanaged resources can be disposed.
      /// </summary>
      /// <param name="disposing">indicates how this method is called</param>
      protected virtual void Dispose(bool disposing)
      {
         if (!this.disposed)
         {
            if (disposing)
            {
               if (this.dialog != null)
               {
                  this.dialog.Dispose();
                  this.dialog = null;
               }
            }
         }

         this.disposed = true;
      }

      #endregion

      /// <summary>
      /// Event handler call when the user clicks the ok button
      /// </summary>
      /// <param name="sender">The originator of the event</param>
      /// <param name="e">The event arguments</param>
      private void OnOkClick(object sender, EventArgs e)
      {
         if (this.editorService != null)
         {
            if (this.context != null && this.context.OnComponentChanging())
            {
               this.context.PropertyDescriptor.SetValue(this.context.Instance, this.dialog.Columns);
            }
         }
      }
   }
}
