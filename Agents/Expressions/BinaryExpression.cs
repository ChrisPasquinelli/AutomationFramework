// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>BinaryExpression is a node structure used for constructing a binary expression tree configured by the user.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System.Collections.Generic;
   using System.ComponentModel;
   using MTI.Core;

   #endregion

   /// <summary>
   /// BinaryExpression is a node structure used for constructing a binary expression tree configured by the user.
   /// </summary>
   [Provide(Categories = new string[] { "Logic" })]
   [Description("BinaryExpression is a node structure used for constructing a binary expression tree.")]
   public class BinaryExpression :
      IExpression,
      IGraphUpdateSubscriber
   {
      #region Fields

      /// <summary>
      /// String representation of binary operators
      /// </summary>
      private static string[] binaryOperators = new string[] { "+", "=", "/", "*", "-", "%" };

      /// <summary>
      /// The object node of the left side expression
      /// </summary>
      private ObjectNode leftSideNode;
     
      /// <summary>
      /// The object node of the right side expression
      /// </summary>
      private ObjectNode rightSideNode;
      
      /// <summary>
      /// The left side expression
      /// </summary>
      private IExpression leftSideExpression;
      
      /// <summary>
      /// The right side expression
      /// </summary>
      private IExpression rightSideExpression;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the BinaryExpression class.
      /// </summary>
      public BinaryExpression()
      {
         this.LeftHandSide = new object();
         this.RightHandSide = new object();
      }

      /// <summary>
      /// Initializes a new instance of the BinaryExpression class.
      /// </summary>
      /// <param name="binaryOperator">The binary operator to be executed.</param>
      public BinaryExpression(BinaryOperator binaryOperator) :
         this()
      {
         this.Operator = binaryOperator;
      }

      #endregion

      #region Enumerations

      /// <summary>
      /// Binary operator types
      /// </summary>
      public enum BinaryOperator : int
      {
         /// <summary>
         /// Identifies an addition operator
         /// </summary>
         Add = 0,

         /// <summary>
         /// Identifies an assignment operator
         /// </summary>
         Assign = 1,

         /// <summary>
         /// Identifies a division operator
         /// </summary>
         Divide = 2,
         
         /// <summary>
         /// Identifies a multiplication operator
         /// </summary>
         Multiply = 3,

         /// <summary>
         /// Identifies a subtraction operator
         /// </summary>
         Subtract = 4,
         
            /// <summary>
         /// Identifies a remainder operator
         /// </summary>
         Remainder = 5
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the left side expression.
      /// </summary>
      [Require(Cut = true)]
      [Description("The left side expression")]
      public object LeftHandSide
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the binary operator.
      /// </summary>
      [Description("The binary operator")]
      public BinaryOperator Operator
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the right side expression.
      /// </summary>
      [Require(Cut = true)]
      [Description("The right side expression")]
      public object RightHandSide
      {
         get;
         set;
      }

      /// <summary>
      /// Gets the expression string which this object represents.
      /// </summary>
      [Browsable(false)]
      public string ExpressionString
      {
         get
         {
            return string.Format("({0} {1} {2})", this.leftSideExpression.ExpressionString, BinaryExpression.binaryOperators[(int)this.Operator], this.rightSideExpression.ExpressionString);
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Analyzes the validity of the expression
      /// </summary>
      /// <param name="error">Set to true if a valid configuration otherwise false.</param>
      /// <returns>A string describing any invalid configurations</returns>
      public string Analyze(out bool error)
      {
         error = false;
         if (this.leftSideNode.Edge == null)
         {
            error = true;
            return "Left-hand side of binary expression is null.";
         }
         else
         {
            ObjectNode referenceNode = this.leftSideNode.Edge.ProvideNode;

            if (referenceNode.Value is IExpression)
            {
               this.leftSideExpression = referenceNode.Value as IExpression;
               this.leftSideExpression.Analyze(out error);
            }
            else if (ObjectNode.IsValueType(referenceNode.Value.GetType()))
            {
               this.leftSideExpression = new State(referenceNode);
            }
            else
            {
               error = true;
               return "Left-hand side of binary expression is not an expression.";
            }
         }

         if (this.rightSideNode.Edge == null)
         {
            error = true;
            return "Left-hand side of binary expression is null.";
         }
         else
         {
            ObjectNode referenceNode = this.rightSideNode.Edge.ProvideNode;
            
            if (referenceNode.Value is IExpression)
            {
               this.rightSideExpression = referenceNode.Value as IExpression;
               this.rightSideExpression.Analyze(out error);
            }
            else if (ObjectNode.IsValueType(referenceNode.Value.GetType()))
            {
               this.rightSideExpression = new State(referenceNode);
            }
            else
            {
               error = true;
               return "Right-hand side of binary expression is not an expression.";
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
         foreach (IExpression expression in this.leftSideExpression.GetTermsEnumerator())
         {
            yield return expression;
         }

         foreach (IExpression expression in this.rightSideExpression.GetTermsEnumerator())
         {
            yield return expression;
         }
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
            if (property.Name == "LeftHandSide")
            {
               this.leftSideNode = property;
            }
            else if (property.Name == "RightHandSide")
            {
               this.rightSideNode = property;
            }
         }
      }
      
      #endregion
   }
}