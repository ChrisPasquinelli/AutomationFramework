// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>State wraps a components property or user defined variable in order to integrate into the inference engine as a state variable.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using MTI.Core;
   #endregion

   /// <summary>
   /// State wraps a components property or user defined variable in order to integrate into the inference engine as a state variable. 
   /// </summary>
   public class State :
      IExpression
   {
      #region Fields

      /// <summary>
      /// The (leaf) object node that is wrapped in this State
      /// </summary>
      private ObjectNode referenceNode;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the State class.
      /// </summary>
      public State()
      {
         this.DependentClauses = new List<Clause>();
      }

      /// <summary>
      /// Initializes a new instance of the State class.
      /// </summary>
      /// <param name="referenceNode">The object node of a value type which this State wraps.</param>
      public State(ObjectNode referenceNode)
      {
         this.DependentClauses = new List<Clause>();
         this.referenceNode = referenceNode;
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets the list of clauses which are dependent on this state variable
      /// </summary>
      [Browsable(false)]
      public List<Clause> DependentClauses
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets a string representing this expression.
      /// </summary>
      public string ExpressionString
      {
         get
         {
            return string.Format("{0}", this.Identifier.Substring(this.Identifier.LastIndexOf('.') + 1));
         }
      }

      /// <summary>
      /// Gets or sets the name of the state variable
      /// </summary>
      [Browsable(false)]
      public virtual string Identifier
      {
         get
         {
            if (this.ReferenceNode != null)
            {
               return this.ReferenceNode.FullPath;
            }
            else
            {
               return string.Empty;
            }
         }

         set
         {
            throw new NotImplementedException("State.Identifier");
         }
      }

      #endregion

      #region Internal Properties

      /// <summary>
      /// Gets or sets the node which this state variable wraps
      /// </summary>
      [Browsable(false)]
      internal virtual ObjectNode ReferenceNode
      {
         get
         {
            return this.referenceNode;
         }

         set
         {
            throw new NotImplementedException("State.ReferenceNode");
         }
      }

      /// <summary>
      /// Gets or sets the value of this state variable
      /// </summary>
      [Browsable(false)]
      internal object Value
      {
         get
         {
            return this.ReferenceNode.Value;
         }

         set
         {
            try
            {
               if (this.ReferenceNode.Value != null)
               {
                  if (this.ReferenceNode.Value.GetType() != value.GetType())
                  {
                     value = Convert.ChangeType(value, this.ReferenceNode.Value.GetType());
                  }
               }

               this.ReferenceNode.Value = value;
            }
            catch (Exception ex)
            {
               throw ex;
            }
         }
      }

      #endregion

      #region IExpression Implementation

      /// <summary>
      /// Analyzes the validity of the expression
      /// </summary>
      /// <param name="error">Set to true if a valid configuration otherwise false.</param>
      /// <returns>A string describing any invalid configurations</returns>
      public virtual string Analyze(out bool error)
      {
         error = false;

         if (this.ReferenceNode == null)
         {
            error = true;
            return "State variable has not been defined.";
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
   }
}
