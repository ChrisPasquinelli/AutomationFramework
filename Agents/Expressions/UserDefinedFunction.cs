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
   [Provide(Categories = new string[] { "System.Utilities" })]
   [Description("Effector wraps a class method in order to integrate into the logic engine as a conclusion action.")]
   public class UserDefinedFunction :
      MTI.Core.Component,
      IExpression,
      IGraphUpdateSubscriber
   {
      #region Fields

      /// <summary>
      /// The object node of the 'Parameters' property referencing the connected parameters
      /// </summary>
      private ObjectNode inputParameters;

      /// <summary>
      /// The object node of the 'Parameters' property referencing the connected parameters
      /// </summary>
      private object[] parameters;

      /// <summary>
      /// The object nodes of the connected parameters
      /// </summary>
      private List<ObjectNode> referenceParameters;

      private string[] functionString;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Effector class.
      /// </summary>
      public UserDefinedFunction()
      {
         this.parameters = new object[0];
         this.functionString = new string[0];
         this.inputParameters = null;
         this.referenceParameters = new List<ObjectNode>();
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets or sets the function text.
      /// </summary>
      [Description("The provided method to invoke")]
      public string[] FunctionString
      {
         get
         {
            return this.functionString;
         }

         set
         {
            this.functionString = value;
            this.OnPropertyChanged("FunctionString");
         }
      }

      /// <summary>
      /// Gets or sets the parameters with which the method is called.
      /// </summary>
      [Require(Cut = true)]
      [Description("The provided parameters to pass into Method")]
      public object[] Parameters
      {
         get
         {
            return this.parameters;
         }

         set
         {
            this.parameters = value;
            this.OnPropertyChanged("Parameters");
         }
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
            string expression = string.Empty;
            for (int i = 0; i < this.FunctionString.Length; i++) expression += this.FunctionString[i];
            return expression;
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
            return string.Empty;
         }
      }

      /// <summary>
      /// Gets the object nodes corresponding to parameters passed into the function
      /// </summary>
      [Browsable(false)]
      internal virtual ObjectNode[] ParameterNodes
      {
         get
         {
            if(this.inputParameters.Edge != null)
            {
               return new ObjectNode[] { this.inputParameters };
            }
            return this.inputParameters.Properties.ToArray();
         }
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

      #endregion

      #region Public Methods

      /// <summary>
      /// Analyzes the validity of the expression
      /// </summary>
      /// <param name="error">Set to true if a valid configuration otherwise false.</param>
      /// <returns>A string describing any invalid configurations</returns>
      public virtual string Analyze(out bool error)
      {
         error = false;
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
               case "Parameters":
                  this.inputParameters = property;
                  break;
            }
         }
      }

      #endregion
   }
}
