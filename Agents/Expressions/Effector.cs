// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Effector wraps a class method in order to integrate into the inference engine as a conclusion action.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Reflection;
   using MTI.Core;
   #endregion

   /// <summary>
   /// Effector wraps a class method in order to integrate into the logic engine as a conclusion action.
   /// </summary>
   [Provide(Categories = new string[] { "Logic" })]
   [Description("Effector wraps a class method in order to integrate into the logic engine as a conclusion action.")]
   public class Effector :
      IExpression,
      IGraphUpdateSubscriber
   {
      #region Fields

      /// <summary>
      /// The object node of the method that is wrapped in this Effector
      /// </summary>
      private ObjectNode methodNode;
      
      /// <summary>
      /// The object node of the 'Method' property referencing the connected method
      /// </summary>
      private ObjectNode referenceNode;

      /// <summary>
      /// The object node of the 'Parameters' property referencing the connected parameters
      /// </summary>
      private ObjectNode inputParameters;
      
      /// <summary>
      /// The object nodes of the connected parameters
      /// </summary>
      private List<ObjectNode> referenceParameters;
      
      /// <summary>
      /// The object housing the object containing the method, the method, and the parameters
      /// </summary>
      private MethodNode.MethodInvokeParameters methodInvokeParameters;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Effector class.
      /// </summary>
      public Effector()
      {
         this.Method = new object();
         this.Parameters = new object[0];
         this.methodNode = null;
         this.referenceNode = null;
         this.inputParameters = null;
         this.referenceParameters = new List<ObjectNode>();
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets or sets the method.
      /// </summary>
      [Require(Cut = true)]
      [Description("The provided method to invoke")]
      public object Method
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the parameters with which the method is called.
      /// </summary>
      [Require(Cut = true)]
      [Description("The provided parameters to pass into Method")]
      public object[] Parameters
      {
         get;
         set;
      }

      #endregion

      #region Internal Properties

      /// <summary>
      /// Gets the expression string which this object represents.
      /// </summary>
      [Browsable(false)]
      public string ExpressionString
      {
         get
         {
            if (this.referenceNode != null)
            {
               return string.Format("{0}.{1}", this.referenceNode.Edge.ProvideNode.ParentNode.Name, this.referenceNode.Edge.ProvideNode.Name.Substring(0, this.referenceNode.Edge.ProvideNode.Name.IndexOf('(')));
            }

            return string.Empty;
         }
      }

      /// <summary>
      /// Gets the full path of the object containing the method
      /// </summary>
      [Browsable(false)]
      internal virtual string Identifier
      {
         get
         {
            if (this.referenceNode != null)
            {
               return this.referenceNode.Edge.ProvideNode.ParentNode.FullPath;
            }
            else
            {
               return string.Empty;
            }
         }
      }

      /// <summary>
      /// Gets the object nodes corresponding to parameters passed into the function
      /// </summary>
      [Browsable(false)]
      internal virtual ObjectNode[] ParameterNodes
      {
         get { return this.inputParameters.Properties.ToArray(); }
      }

      /// <summary>
      /// Gets the states that wrap the parameter nodes.
      /// </summary>
      [Browsable(false)]
      internal virtual State[] ParameterStates
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the method node in the object graph
      /// </summary>
      [Browsable(false)]
      internal ObjectNode ReferenceNode
      {
         get
         {
            return this.referenceNode;
         }
      }

      /// <summary>
      /// Gets the return type of the method
      /// </summary>
      [Browsable(false)]
      internal Type ReturnType
      {
         get
         {
            return this.methodInvokeParameters.MethodInfo.ReturnType;
         }
      }

      /// <summary>
      /// Gets or sets the value of the function by invoking the method with the current parameters
      /// </summary>
      [Browsable(false)]
      internal object Value
      {
         get
         {
            List<object> parameters = new List<object>();
            foreach (ObjectNode parameterNode in this.ParameterNodes)
            {
               parameters.Add(parameterNode.Value);
            }

            return this.methodInvokeParameters.MethodInfo.Invoke(this.methodInvokeParameters.Instance, parameters.ToArray());
         }

         set
         {
            try
            {
               if (this.referenceNode.Value != null)
               {
                  if (this.referenceNode.Value.GetType() != value.GetType())
                  {
                     value = Convert.ChangeType(value, this.referenceNode.Value.GetType());
                  }
               }

               this.referenceNode.Value = value;
            }
            catch (Exception ex)
            {
               throw ex;
            }
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Analyzes the validity of the expression
      /// </summary>
      /// <param name="error">Set to true if a valid configuration otherwise false.</param>
      /// <returns>A string describing any invalid configurations</returns>
      public virtual string Analyze(out bool error)
      {
         string results = string.Empty;
         error = false;

         if (this.methodNode == null || this.methodNode.Edge == null)
         {
            error = true;
            return "Undefined effector method.\r\n";
         }

         this.referenceNode = this.methodNode;
         this.methodInvokeParameters = (MethodNode.MethodInvokeParameters)this.referenceNode.Value;
 
         ParameterInfo[] parameterInfos = this.methodInvokeParameters.MethodInfo.GetParameters();
         if (parameterInfos.Length != this.Parameters.Length)
         {
            string name = this.methodInvokeParameters.MethodInfo.Name;
            results += "The number of specified parameters does not equal the number of arguments defined in " + name + "\r\n";
            error = true;
            return results;
         }

         // Assign and check parameters
         this.referenceParameters.Clear();
         this.ParameterStates = new State[this.ParameterNodes.Length];
         for (int i = 0; i < this.ParameterNodes.Length; i++)
         {
            if (this.ParameterNodes[i].Edge != null)
            {
               this.referenceParameters.Add(this.ParameterNodes[i]);
            }

            this.ParameterStates[i] = new State(this.ParameterNodes[i]);

            if (parameterInfos[i].ParameterType != typeof(object))
            {
               if (this.ParameterNodes[i].Value.GetType() is IConvertible)
               {
                  object value = Convert.ChangeType(this.ParameterNodes[i].Value, parameterInfos[i].ParameterType);
                  if (value == null)
                  {
                     results += "Cannot convert " + this.ParameterNodes[i].FullPath + " to " + parameterInfos[i].Name + "\r\n";
                     error = true;
                  }
               }
            }
         }

         return string.Empty;
      }

      /// <summary>
      /// Gets the enumerator which iterates over the terms composing the expression.
      /// </summary>
      /// <returns>The enumerator which iterates over the terms composing the expression.</returns>
      public IEnumerable<IExpression> GetTermsEnumerator()
      {
         yield return this;
      }

      #endregion

      #region Framework Event Handlers

      /// <summary>
      /// Event handler that accesses the object graph and tree of this instance
      /// </summary>
      /// <param name="thisGraph">The object graph in which the object tree for this instance is a subset.</param>
      /// <param name="thisNode">The root node of this instance's object tree.</param>
      public void OnGraphUpdate(ObjectGraph thisGraph, ObjectNode thisNode)
      {
         foreach (ObjectNode property in thisNode.Properties)
         {
            switch (property.Name)
            {
               case "Method":
                  this.methodNode = property;
                  break;

               case "Parameters":
                  this.inputParameters = property;
                  break;
            }
         }
      }
      
      #endregion
   }
}
