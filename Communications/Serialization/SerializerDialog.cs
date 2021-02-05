// <copyright file="SerializerDialog.cs" company="Genesis Engineering Services">
// Copyright (c) 2011 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2013-12-20</date>
// <summary>Serializes/Deserializes a string into a table of user-defined records</summary>
namespace GES.Communications
{
   #region Directives
   
   using System;
   using System.Collections.Generic;
   using System.Windows.Forms;
   using MTI.Core;

   #endregion

   /// <summary>
   /// Dialog that opens when the end-user defines which object trees (sub-trees) will be serialized
   /// </summary>
   /// <typeparam name="T">A type derived from the SerializerBase.Descriptor</typeparam>
   public partial class SerializerDialog<T> : 
      Form
      where T : SerializerBase.SerializerDescriptor, new()
   {
      /// <summary>
      /// The tree view which specifies which trees (sub-trees) to serialize.
      /// </summary>
      private TreeView treeParts;

      /// <summary>
      /// The ok button which applies the define serialization.
      /// </summary>
      private Button buttonOk;

      /// <summary>
      /// Mapping between the serialized object nodes and the corresponding tree node in the tree view.
      /// </summary>
      private Dictionary<ObjectNode, TreeNode> objectNodeToTreeNode = new Dictionary<ObjectNode, TreeNode>();

      /// <summary>
      /// The serialized nodes.
      /// </summary>
      private List<T> selectedNodes = new List<T>();

      /// <summary>
      /// Initializes a new instance of the <see cref="SerializerDialog{T}"/> class.
      /// </summary>
      public SerializerDialog()
      {
         this.InitializeComponent();
      }

      #region Events

      /// <summary>
      /// Event raised when the ok button is clicked
      /// </summary>
      public event EventHandler OkClick;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the column parameters associated with the serialized object nodes
      /// </summary>
      public T[] Columns
      {
         get
         {
            return this.selectedNodes.ToArray();
         }

         set
         {
            this.selectedNodes.Clear();
            this.selectedNodes.AddRange(value);
         }
      }

      /// <summary>
      /// Gets or sets the SerializerBase for which the dialog defines the serialization process.
      /// </summary>
      public SerializerBase SerializerBase
      {
         get;
         set;
      }

      /// <summary>
      /// Gets the selected descriptors corresponding to serialized nodes
      /// </summary>
      public List<T> SelectedNodes
      {
         get { return this.selectedNodes; }
      }

      #endregion

      #region Public Methods
     
      /// <summary>
      /// Updates list view with the selected (sub)tree.
      /// </summary>
      /// <param name="newItem">The  descriptor to be appended to the list view</param>
      public void AddRow(T newItem)
      {
         // Check to see if row exists
         ListViewItem existingItem = null;
         foreach(ListViewItem listViewItem in this.listParts.Items)
         {
            if(listViewItem.Tag == newItem.ObjectNode)
            {
               existingItem = listViewItem;
               break;
            }
         }

         if (existingItem != null)
         {
            // Move to new location in list
            this.listParts.Items.Remove(existingItem);
            this.listParts.Items.Add(existingItem);
         }
         else
         {
            ListViewItem item = new ListViewItem(new string[] { newItem.LocalPathName, typeof(T).FullName });
            item.Tag = newItem;
            item.UseItemStyleForSubItems = false;
            this.listParts.Items.Add(item);
            this.selectedNodes.Add(newItem);
         }

         this.listParts.Update();
         this.listParts.SelectedIndices.Add(this.selectedNodes.Count - 1);
         this.gridProperties.SelectedObject = newItem;
      }

      #endregion

      #region Protected Methods

      /// <summary>
      /// The event handler that populates the controls during initialization.
      /// </summary>
      /// <param name="e">The event arguments</param>
      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         this.listParts.Items.Clear();
         foreach (T selected in this.selectedNodes)
         {
            ListViewItem item = new ListViewItem(new string[] { selected.LocalPathName, typeof(T).FullName });
            item.Tag = selected;
            item.UseItemStyleForSubItems = false;
            this.listParts.Items.Add(item);
         }

         this.listParts.Update();
         this.gridProperties.SelectedObject = null;
         this.treeParts.Nodes.Clear();
         if (this.SerializerBase.TemplatePropertyNode.Value.GetType().IsArray)
         {
            if (this.SerializerBase.TemplatePropertyNode.Edge != null)
            {
               this.treeParts.Nodes.Add(this.CreateObjectTree(this.SerializerBase.TemplatePropertyNode.Edge.ProvideNode, 0));
            }
            //ProvideAttribute provide = this.SerializerBase.TemplatePropertyNode.GetAttribute<ProvideAttribute>();
            //RequireAttribute require = this.SerializerBase.TemplatePropertyNode.GetAttribute<RequireAttribute>();
            //if (provide != null && provide.Cut)
            //{
            //   ObjectNode node = this.SerializerBase.TemplatePropertyNode;
            //   ObjectGraph graph = node.Graph;
            //   Array array = node.Value as Array;
            //   for(int i = 0; i < array.Length; i++)
            //   {
            //      ObjectNode elementNode = graph.CreateObjectTree(array.GetValue(i), array.GetValue(i).GetType().Name, System.Reflection.BindingFlags.DeclaredOnly, true);
            //      this.treeParts.Nodes.Add(this.CreateObjectTree(elementNode, 0));
            //   }
            //}
            //else
            //{
            //   foreach (ObjectNode node in this.SerializerBase.TemplatePropertyNode.Properties)
            //   {
            //      this.treeParts.Nodes.Add(this.CreateObjectTree(node.Edge.ProvideNode, 0));
            //   }
            //}
         }
         else
         {
            if(this.SerializerBase.TemplatePropertyNode.Edge != null)
            {
               this.treeParts.Nodes.Add(this.CreateObjectTree(this.SerializerBase.TemplatePropertyNode.Edge.ProvideNode, 0));
            }
            else
            {
               this.treeParts.Nodes.Add(this.CreateObjectTree(this.SerializerBase.TemplatePropertyNode, 0));
            }
         }
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Creates a tree node corresponding to an object node within the configured graphs.
      /// </summary>
      /// <param name="node">The object node</param>
      /// <param name="imageIndex">The index of the image to be displayed by the tree node</param>
      /// <returns>The initialized tree node.</returns>
      private TreeNode CreateTreeNode(ObjectNode node, int imageIndex)
      {
         try
         {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = node.Name;
            treeNode.ImageIndex = imageIndex;
            treeNode.SelectedImageIndex = imageIndex;
            treeNode.Tag = node;
            if (!this.objectNodeToTreeNode.ContainsKey(node))
            {   // MethodNode
               this.objectNodeToTreeNode.Add(node, treeNode);
            }

            string localPath = this.SerializerBase.GetRelativePathName(node.FullPath);
            foreach (T selected in this.selectedNodes)
            {
               if (selected.LocalPathName == localPath)
               {
                  treeNode.Checked = true;
               }
            }

            return treeNode;
         }
         catch
         {
            return null;
         }
      }
      
      /// <summary>
      /// Creates the tree displayed in the tree view
      /// </summary>
      /// <param name="node">The root node of a graph corresponding to a configured part</param>
      /// <param name="imageIndex">The index of the image to be displayed by the tree nodes</param>
      /// <returns>The root tree node.</returns>
      private TreeNode CreateObjectTree(ObjectNode node, int imageIndex)
      {
         if (!node.IsBrowsable)
         {
            return null;
         }

         TreeNode treeNode = this.CreateTreeNode(node, imageIndex);

         if (node.Edge != null && !node.Edge.Embedded)
         {
            treeNode.Nodes.Add("Linked to " + node.Edge.ProvideNode.FullPath);
            return treeNode;
         }

         RequireAttribute requireAttribute = node.GetAttribute<RequireAttribute>();
         if (requireAttribute != null && !string.IsNullOrEmpty(requireAttribute.Alias))
         {
            return treeNode;
         }

         ProvideAttribute provideAttribute = node.GetAttribute<ProvideAttribute>();
         if (provideAttribute != null && !string.IsNullOrEmpty(provideAttribute.Alias))
         {
            return treeNode;
         }

         foreach (ObjectNode child in node.Properties)
         {
            TreeNode childNode = this.CreateObjectTree(child, imageIndex);
            if (childNode != null)
            {
               treeNode.Nodes.Add(childNode);
            }
         }

         return treeNode;
      }

      /// <summary>
      /// Gets the tree node corresponding to the given object node.
      /// </summary>
      /// <param name="targetNode">The node for which to search</param>
      /// <returns>The tree node corresponding to the given object node.</returns>
      private TreeNode GetTreeNode(ObjectNode targetNode)
      {
         if (this.objectNodeToTreeNode.ContainsKey(targetNode))
         {
            return this.objectNodeToTreeNode[targetNode];
         }

         return null;
      }

      /// <summary>
      /// The event handler executed when a tree node has been selected
      /// </summary>
      /// <param name="sender">The sender</param>
      /// <param name="e">The event arguments</param>
      private void OnAfterCheck(object sender, TreeViewEventArgs e)
      {
         ObjectNode node = null;
         if (e.Node.Tag != null && e.Node.Tag is ObjectNode)
         {
            node = e.Node.Tag as ObjectNode;
         }

         if (e.Node.Checked && node != null)
         {
            foreach (T item in this.selectedNodes)
            {
               if (item.LocalPathName == this.SerializerBase.GetRelativePathName(node.FullPath))
               {
                  return;
               }
            }

            if (e.Node.Nodes.Count > 1)
            {
               e.Node.Expand();
               foreach (TreeNode child in e.Node.Nodes)
               {
                  child.Checked = true;
               }
            }
            else
            {
               T descriptor = new T();
               descriptor.LocalPathName = this.SerializerBase.GetRelativePathName(node.FullPath);
               descriptor.SetDefaultAttributes(node);
               this.AddRow(descriptor);
            }
         }
         else if (!e.Node.Checked && node != null)
         {
            if (e.Node.Nodes.Count > 1)
            {
               foreach (TreeNode child in e.Node.Nodes)
               {
                  child.Checked = false;
               }
            }
            else
            {
               int deselected = -1;
               for (int i = 0; i < this.selectedNodes.Count; i++)
               {
                  if (this.selectedNodes[i].LocalPathName == this.SerializerBase.GetRelativePathName(node.FullPath))
                  {
                     deselected = i;
                     break;
                  }
               }

               if (deselected > -1)
               {
                  this.selectedNodes.RemoveAt(deselected);
                  this.listParts.Items.RemoveAt(deselected);
               }
            }
         }
      }
      
      /// <summary>
      /// The event handler executed in response to an Ok button click.
      /// </summary>
      /// <param name="sender">The sender</param>
      /// <param name="e">The event arguments</param>
      private void OnOkClick(object sender, EventArgs e)
      {
         if (this.OkClick != null)
         {
            this.OkClick(sender, e);
         }
      }

      /// <summary>
      /// The event handler executed in response to an item added to the list view.
      /// </summary>
      /// <param name="sender">The sender</param>
      /// <param name="e">The event arguments</param>
      private void OnAdd(object sender, EventArgs e)
      {
         this.AddRow(new T());
      }

      /// <summary>
      /// The event handler executed when an object node has been deselected.
      /// </summary>
      /// <param name="sender">The sender</param>
      /// <param name="e">The event arguments</param>
      private void OnRemove(object sender, EventArgs e)
      {
         if (this.selectedNodes.Count > this.listParts.SelectedIndices[0])
         {
            this.selectedNodes.RemoveAt(this.listParts.SelectedIndices[0]);
            this.listParts.Items.RemoveAt(this.listParts.SelectedIndices[0]);
         }
      }

      /// <summary>
      /// The event handler executed when an object node has been selected.
      /// </summary>
      /// <param name="sender">The sender</param>
      /// <param name="e">The event arguments</param>
      private void OnItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
      {
         if(e.ItemIndex < this.selectedNodes.Count)
         {
            this.gridProperties.SelectedObject = this.selectedNodes[e.ItemIndex];
         }
      }

      /// <summary>
      /// The event handler executed when the list view has been traversed upwards.
      /// </summary>
      /// <param name="sender">The sender</param>
      /// <param name="e">The event arguments</param>
      private void OnUpClick(object sender, EventArgs e)
      {
         int selectedIndex = this.listParts.SelectedIndices[0];
         T item = this.selectedNodes[selectedIndex];
         ListViewItem listViewItem = this.listParts.Items[selectedIndex];
         this.selectedNodes.RemoveAt(selectedIndex);
         this.listParts.Items.RemoveAt(selectedIndex);
         this.selectedNodes.Insert(selectedIndex - 1, item);
         this.listParts.Items.Insert(selectedIndex - 1, listViewItem);
      }

      /// <summary>
      /// The event handler executed when the list view has been traversed downwards.
      /// </summary>
      /// <param name="sender">The sender</param>
      /// <param name="e">The event arguments</param>
      private void OnClickDown(object sender, EventArgs e)
      {
         int selectedIndex = this.listParts.SelectedIndices[0];
         T item = this.selectedNodes[selectedIndex];
         ListViewItem listViewItem = this.listParts.Items[selectedIndex];
         this.selectedNodes.RemoveAt(selectedIndex);
         this.listParts.Items.RemoveAt(selectedIndex);
         this.selectedNodes.Insert(selectedIndex + 1, item);
         this.listParts.Items.Insert(selectedIndex + 1, listViewItem);
      }

      #endregion

      private void OnRemoveAll(object sender, EventArgs e)
      {
         this.selectedNodes.Clear();
         this.listParts.Items.Clear();
      }
   }
}
