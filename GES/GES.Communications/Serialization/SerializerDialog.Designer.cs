namespace GES.Communications
{
   public partial class SerializerDialog<T>
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.ListView listParts;
      private System.Windows.Forms.Button buttonUp;
      private System.Windows.Forms.Button buttonDown;
      private System.Windows.Forms.Button buttonAdd;
      private System.Windows.Forms.Button buttonRemove;
      private System.Windows.Forms.PropertyGrid gridProperties;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      

      #region Windows Form Designer generated code

      private void InitializeComponent()
      {
         System.Windows.Forms.Label label1;
         System.Windows.Forms.Label label2;
         System.Windows.Forms.Label label3;
         this.treeParts = new System.Windows.Forms.TreeView();
         this.buttonOk = new System.Windows.Forms.Button();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.listParts = new System.Windows.Forms.ListView();
         this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.buttonUp = new System.Windows.Forms.Button();
         this.buttonDown = new System.Windows.Forms.Button();
         this.buttonAdd = new System.Windows.Forms.Button();
         this.buttonRemove = new System.Windows.Forms.Button();
         this.gridProperties = new System.Windows.Forms.PropertyGrid();
         this.button1 = new System.Windows.Forms.Button();
         label1 = new System.Windows.Forms.Label();
         label2 = new System.Windows.Forms.Label();
         label3 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         label1.Location = new System.Drawing.Point(9, 13);
         label1.Name = "label1";
         label1.Size = new System.Drawing.Size(36, 13);
         label1.TabIndex = 9;
         label1.Text = "Parts";
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         label2.Location = new System.Drawing.Point(237, 13);
         label2.Name = "label2";
         label2.Size = new System.Drawing.Size(60, 13);
         label2.TabIndex = 10;
         label2.Text = "Parts List";
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         label3.Location = new System.Drawing.Point(516, 13);
         label3.Name = "label3";
         label3.Size = new System.Drawing.Size(64, 13);
         label3.TabIndex = 11;
         label3.Text = "Properties";
         // 
         // treeParts
         // 
         this.treeParts.CheckBoxes = true;
         this.treeParts.Location = new System.Drawing.Point(12, 35);
         this.treeParts.Name = "treeParts";
         this.treeParts.PathSeparator = ".";
         this.treeParts.Size = new System.Drawing.Size(222, 317);
         this.treeParts.TabIndex = 0;
         this.treeParts.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.OnAfterCheck);
         // 
         // buttonOk
         // 
         this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.buttonOk.Location = new System.Drawing.Point(537, 371);
         this.buttonOk.Name = "buttonOk";
         this.buttonOk.Size = new System.Drawing.Size(75, 23);
         this.buttonOk.TabIndex = 1;
         this.buttonOk.Text = "OK";
         this.buttonOk.UseVisualStyleBackColor = true;
         this.buttonOk.Click += new System.EventHandler(this.OnOkClick);
         // 
         // buttonCancel
         // 
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Location = new System.Drawing.Point(634, 371);
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.Size = new System.Drawing.Size(75, 23);
         this.buttonCancel.TabIndex = 2;
         this.buttonCancel.Text = "Cancel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         // 
         // listParts
         // 
         this.listParts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.listParts.HideSelection = false;
         this.listParts.Location = new System.Drawing.Point(240, 35);
         this.listParts.MultiSelect = false;
         this.listParts.Name = "listParts";
         this.listParts.Size = new System.Drawing.Size(227, 288);
         this.listParts.TabIndex = 3;
         this.listParts.UseCompatibleStateImageBehavior = false;
         this.listParts.View = System.Windows.Forms.View.Details;
         this.listParts.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.OnItemSelectionChanged);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "Path";
         this.columnHeader1.Width = 129;
         // 
         // buttonUp
         // 
         this.buttonUp.Location = new System.Drawing.Point(473, 35);
         this.buttonUp.Name = "buttonUp";
         this.buttonUp.Size = new System.Drawing.Size(32, 32);
         this.buttonUp.TabIndex = 4;
         this.buttonUp.Text = "^";
         this.buttonUp.UseVisualStyleBackColor = true;
         this.buttonUp.Click += new System.EventHandler(this.OnUpClick);
         // 
         // buttonDown
         // 
         this.buttonDown.Location = new System.Drawing.Point(473, 73);
         this.buttonDown.Name = "buttonDown";
         this.buttonDown.Size = new System.Drawing.Size(32, 32);
         this.buttonDown.TabIndex = 5;
         this.buttonDown.Text = "v";
         this.buttonDown.UseVisualStyleBackColor = true;
         this.buttonDown.Click += new System.EventHandler(this.OnClickDown);
         // 
         // buttonAdd
         // 
         this.buttonAdd.Location = new System.Drawing.Point(240, 329);
         this.buttonAdd.Name = "buttonAdd";
         this.buttonAdd.Size = new System.Drawing.Size(72, 23);
         this.buttonAdd.TabIndex = 6;
         this.buttonAdd.Text = "Add";
         this.buttonAdd.UseVisualStyleBackColor = true;
         this.buttonAdd.Click += new System.EventHandler(this.OnAdd);
         // 
         // buttonRemove
         // 
         this.buttonRemove.Location = new System.Drawing.Point(317, 329);
         this.buttonRemove.Name = "buttonRemove";
         this.buttonRemove.Size = new System.Drawing.Size(72, 23);
         this.buttonRemove.TabIndex = 7;
         this.buttonRemove.Text = "Remove";
         this.buttonRemove.UseVisualStyleBackColor = true;
         this.buttonRemove.Click += new System.EventHandler(this.OnRemove);
         // 
         // gridProperties
         // 
         this.gridProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.gridProperties.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
         this.gridProperties.Location = new System.Drawing.Point(520, 35);
         this.gridProperties.Name = "gridProperties";
         this.gridProperties.Size = new System.Drawing.Size(274, 317);
         this.gridProperties.TabIndex = 8;
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(395, 329);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(72, 23);
         this.button1.TabIndex = 12;
         this.button1.Text = "Remove All";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.OnRemoveAll);
         // 
         // SerializerDialog
         // 
         this.AcceptButton = this.buttonOk;
         this.AutoSize = true;
         this.CancelButton = this.buttonCancel;
         this.ClientSize = new System.Drawing.Size(806, 406);
         this.Controls.Add(this.button1);
         this.Controls.Add(label3);
         this.Controls.Add(label2);
         this.Controls.Add(label1);
         this.Controls.Add(this.gridProperties);
         this.Controls.Add(this.buttonRemove);
         this.Controls.Add(this.buttonAdd);
         this.Controls.Add(this.buttonDown);
         this.Controls.Add(this.buttonUp);
         this.Controls.Add(this.listParts);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.buttonOk);
         this.Controls.Add(this.treeParts);
         this.Name = "SerializerDialog";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (this.components != null))
         {
            this.components.Dispose();
         }

         base.Dispose(disposing);
      }

      private System.Windows.Forms.Button button1;
   }
}