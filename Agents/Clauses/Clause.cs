// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Clause implememnts an condition condition or conclusion action.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Globalization;
   using MTI.Core;
   
   #endregion

   /// <summary>
   /// Clause implements an condition condition or conclusion action.
   /// </summary>
   [Provide(Categories = new string[] { "Logic" })]
   [Description("Clause implements an condition condition or conclusion action.")]
   public class Clause :
      NotifyPropertyChange,
      IGraphUpdateSubscriber
   {
      #region Fields

      /// <summary>
      /// The object node of the state variable that is the left side of this clause.
      /// </summary>
      private ObjectNode variableNode;
      
      /// <summary>
      /// The object node of the expression tree that is the right side of this clause.
      /// </summary>
      private ObjectNode expressionNode;
      
      /// <summary>
      /// The state object that wraps the left side variable.
      /// </summary>
      private State state;

      /// <summary>
      /// The expression object that compiles and executes the right side expression tree.
      /// </summary>
      private Expression expression = new Expression();
      
      /// <summary>
      /// The list of rules containing the is clause.
      /// </summary>
      private List<Rule> dependentRules = new List<Rule>();

      /// <summary>
      /// The comparer for checking type integrity between the state variable and expression
      /// </summary>
      private Comparer comparer = new Comparer(CultureInfo.CurrentCulture);

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Clause class.
      /// </summary>
      public Clause()
      {
         this.Variable = new object();
         this.Expression = new object();
      }

      /// <summary>
      /// Initializes a new instance of the Clause class.
      /// </summary>
      /// <param name="condition">The conditional operator</param>
      public Clause(Conditional condition) :
         this()
      {
         this.Operator = condition;
         this.Variable = new object();
         this.Expression = new object();
      }

      /// <summary>
      /// Initializes a new instance of the Clause class.
      /// </summary>
      /// <param name="leftHandSide">The left hand state variable.</param>
      /// <param name="condition">The conditional operator</param>
      public Clause(State leftHandSide, Conditional condition) :
         this(condition)
      {
         this.State = leftHandSide;
         this.Operator = condition;
         this.Usage = Type.Undefined;
         this.Variable = new object();
         this.Expression = new object();
      }

      #endregion 

      #region Enumerations

      /// <summary>
      /// The conditional operator
      /// </summary>
      public enum Conditional 
      { 
         /// <summary>
         /// Identifies the less-than operator
         /// </summary>
         Less,
         
         /// <summary>
         /// Identifies the less-than-or-equal operator
         /// </summary>
         LessEqual,
         
         /// <summary>
         /// Identifies the equal operator
         /// </summary>
         Equal,
         
         /// <summary>
         /// Identifies the unequal operator
         /// </summary>
         NotEqual,
         
         /// <summary>
         /// Identifies the greater-than-or-equal operator
         /// </summary>
         GreaterEqual,
         
         /// <summary>
         /// Identifies the greater-than operator
         /// </summary>
         Greater, 

         /// <summary>
         /// Identifies a void clause (a conclusion action that returns void)
         /// </summary>
         Void
      }

      /// <summary>
      /// The type of clause
      /// </summary>
      public enum Type 
      { 
         /// <summary>
         /// The hypothesis or condition clause
         /// </summary>
         Condition, 

         /// <summary>
         /// The action or conclusion clause
         /// </summary>
         Conclusion, 

         /// <summary>
         /// Unreferenced clause
         /// </summary>
         Undefined 
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets or sets the left side variable.
      /// </summary>
      [Require(Cut = true)]
      [Description("The left side variable to be tested or assigned.")]
      public virtual object Variable
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the conditional operator.
      /// </summary>
      [Description("The conditional operator or assignment operator.")]
      public Conditional Operator
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the right side expression.
      /// </summary>
      [Require(Cut = true)]
      [Description("The right side expression of the condition or assignment.")]
      public virtual object Expression
      {
         get;
         set;
      }

      #endregion

      #region Internal Properties

      /// <summary>
      /// Gets the rules referencing this clause for backward chaining.
      /// </summary>
      internal List<Rule> DependentRules
      {
         get { return this.dependentRules; }
      }

      /// <summary>
      /// Gets or sets the function representing the left side expression
      /// </summary>
      internal virtual Expression Function
      {
         get;
         set;
      }

      /// <summary>
      /// Gets the State object wrapping the connected variable.
      /// </summary>
      internal virtual State State
      {
         get
         {
            return this.state;
         }

         private set
         {
            if (this.state != null)
            {
               this.state.DependentClauses.Remove(this);
            }

            this.state = value;
            this.state.DependentClauses.Add(this);
         }
      }

      /// <summary>
      /// Gets or sets how the clause is being used in the referencing rule.
      /// </summary>
      internal virtual Type Usage
      {
         get;
         set;
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
         string results = string.Empty;

         ObjectNode referenceNode = null;
         if (this.variableNode.Edge == null)
         {
            referenceNode = this.variableNode;
         }
         else
         {
            referenceNode = this.variableNode.Edge.ProvideNode;
         }

         if ((this.Usage == Clause.Type.Conclusion) || (this.Usage == Clause.Type.Condition && referenceNode.Value is IComparable))
         {
            this.State = new State(referenceNode);
         }
         else
         {
            error = true;
            results += "Variable does not implement IComparablee.\n";
         }

         referenceNode = this.expressionNode;
         if (this.expressionNode.Edge != null)
         {
            referenceNode = this.expressionNode.Edge.ProvideNode;
         }

         if (referenceNode.Value is IExpression)
         {
            this.expression.Result = referenceNode.Value as IExpression;
         }
         else if ((this.Usage == Clause.Type.Conclusion) || (this.Usage == Clause.Type.Condition && referenceNode.Value is IComparable))
         {
            this.expression.Result = (new State(referenceNode)) as IExpression;
         }
         else
         {
            error = true;
            results += "Clause expression is not an expression.\n";
         }

         if (this.Operator != Conditional.Void)
         {
            bool stateError;
            results += this.State.Analyze(out stateError);
            if (stateError)
            {
               error = true;
            }
         }

         bool functionError;
         results += this.expression.Analyze(out functionError);
         if (functionError)
         {
            error = true;
         }

         return results;
      }

      /// <summary>
      /// Evaluates the clause
      /// </summary>
      /// <returns>A value indication the validity of the clause - true, false, undetermined.</returns>
      public virtual TriState Evaluate()
      {
         return this.Evaluate(this.State.Value, this.expression.Value);
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
         this.expression.OnGraphUpdate(thisGraph, thisNode);

         foreach (ObjectNode property in thisNode.Properties)
         {
            if (property.Name == "Variable")
            {
               this.variableNode = property;
            }
            else if (property.Name == "Expression")
            {
               this.expressionNode = property;
            }
         }
      }

      /// <summary>
      /// Event handler that initializes this instance.
      /// </summary>
      /// <param name="ruleIdentifier">The identifier of the rule referencing this clause for reporting errors.</param>
      /// <returns>A value indicating whether the object successfully initialized.</returns>
      public virtual bool OnInitialize(string ruleIdentifier)
      {
         try
         {
            bool error;
            string results = this.Analyze(out error);
            if (error)
            {
               this.dependentRules[0].OnSystemNotification(this, new SystemEventArgs<object>(results, "Clause.OnInitialize", this, MessageType.Error));
               return false;
            }
         }
         catch (Exception ex)
         {
            this.dependentRules[0].OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, "Clause.OnInitialize", this, MessageType.Error));
            return false;
         }

         return true;
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Evaluates the clause with the given values.
      /// </summary>
      /// <param name="value1">The left side value</param>
      /// <param name="value2">The right side value</param>
      /// <returns>The validity of the clause.</returns>
      private TriState Evaluate(object value1, object value2)
      {
         if (this.Usage == Clause.Type.Conclusion)
         {
            if (this.Operator != Conditional.Void)
            {
               if (value1.GetType() != value2.GetType())
               {
                  try
                  {
                     value2 = Convert.ChangeType(value2, value1.GetType());
                  }
                  catch
                  {
                     return TriState.False;
                  }
               }

               this.State.Value = value2;
            }

            return TriState.True;
         }

         if (value1.GetType() != value2.GetType())
         {
            value2 = Convert.ChangeType(value2, value1.GetType());
         }

         int comparison = 0;

         try
         {
            comparison = this.comparer.Compare(value1, value2);
         }
         catch (Exception ex)
         {
            this.dependentRules[0].OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, "Clause.Evaluate", this, MessageType.Error));
         }

         switch (this.Operator)
         {
            case Conditional.Equal:
               if (comparison == 0)
               {
                  return TriState.True;
               }

               break;

            case Conditional.Greater:
               if (comparison > 0)
               {
                  return TriState.True;
               }

               break;

            case Conditional.GreaterEqual:
               if (comparison >= 0)
               {
                  return TriState.True;
               }

               break;

            case Conditional.Less:
               if (comparison < 0)
               {
                  return TriState.True;
               }

               break;

            case Conditional.LessEqual:
               if (comparison <= 0)
               {
                  return TriState.True;
               }

               break;

            case Conditional.NotEqual:
               if (comparison != 0)
               {
                  return TriState.True;
               }

               break;
         }

         return TriState.False;
      }

      #endregion
   }
}