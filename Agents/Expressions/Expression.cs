// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Expression dynamically creates an expression from an expression tree configured by the user and evaluates the expression with current parameter values.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives
   
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using MTI.Core;
   
   #endregion

   /// <summary>
   /// Expression dynamically creates an expression from a hierarchy configured by the user and 
   /// evaluates the expression with current parameter values.
   /// </summary>
   public class Expression :
      Function,
      IExpression
   {
      #region Fields

      /// <summary>
      /// The object nodes of the variables configured in the expression
      /// </summary>
      private ObjectNode[] parameterNodes = new ObjectNode[0];

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Expression class.
      /// </summary>
      public Expression()
      {
         this.Result = new State();
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets a string representing this expression.
      /// </summary>
      [Browsable(false)]
      public string ExpressionString
      {
         get { return this.Result.ExpressionString; }
      }

      /// <summary>
      /// Gets or sets the root node of the expression tree that is to be compiled and executes.
      /// </summary>
      public IExpression Result
      {
         get;
         set;
      }

      #endregion

      #region Internal Properties

      /// <summary>
      /// Gets the parameters configured in the expression tree.
      /// </summary>
      internal override ObjectNode[] ParameterNodes
      {
         get
         {
            return this.parameterNodes;
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Analyzes the validity of the expression
      /// </summary>
      /// <param name="errors">Set to true if a valid configuration otherwise false.</param>
      /// <returns>A string describing any invalid configurations</returns>
      public override string Analyze(out bool errors)
      {
         string result = this.Result.Analyze(out errors);
         if (errors)
         {
            return result;
         }

         List<ArgumentBinding> argumentBindings = new List<ArgumentBinding>();
         List<ObjectNode> parameterNodes = new List<ObjectNode>();
         foreach (IExpression expression in this.GetTermsEnumerator())
         {
            if (expression is State)
            {
               State state = (State)expression;
               ArgumentBinding argumentBinding = new ArgumentBinding();
               argumentBinding.ObjectNode = state.ReferenceNode;
               argumentBinding.ActualIdentifier = state.Identifier;
               argumentBinding.FormalIdentifier = state.Identifier.Substring(state.Identifier.LastIndexOf('.') + 1);
               argumentBindings.Add(argumentBinding);
               parameterNodes.Add(argumentBinding.ObjectNode);
            }
            else if (expression is Effector)
            {
               Effector effector = (Effector)expression;
               string results = effector.Analyze(out errors);
               if (errors)
               {
                  return results;
               }

               ArgumentBinding argumentBinding = new ArgumentBinding();
               argumentBinding.ObjectNode = effector.ReferenceNode.Edge.ProvideNode.ParentNode;
               argumentBinding.ActualIdentifier = effector.Identifier;
               argumentBinding.FormalIdentifier = effector.Identifier.Substring(effector.Identifier.LastIndexOf('.') + 1);
               argumentBindings.Add(argumentBinding);
               parameterNodes.Add(argumentBinding.ObjectNode);
               foreach (ObjectNode parameterNode in effector.ParameterNodes)
               {
                  argumentBinding = new ArgumentBinding();
                  argumentBinding.ObjectNode = parameterNode.Edge.ProvideNode;
                  argumentBinding.ActualIdentifier = parameterNode.Edge.ProvideNode.FullPath;
                  argumentBinding.FormalIdentifier = parameterNode.Edge.ProvideNode.FullPath.Substring(parameterNode.Edge.ProvideNode.FullPath.LastIndexOf('.') + 1);
                  argumentBindings.Add(argumentBinding);
                  parameterNodes.Add(argumentBinding.ObjectNode);
               }
            }
            else if (expression is UserDefinedFunction)
            {
               UserDefinedFunction function = (UserDefinedFunction)expression;
               string results = function.Analyze(out errors);
               if (errors)
               {
                  return results;
               }

               foreach (ObjectNode parameterNode in function.ParameterNodes)
               {
                  ArgumentBinding argumentBinding = new ArgumentBinding();
                  argumentBinding.ObjectNode = parameterNode.Edge.ProvideNode;
                  argumentBinding.ActualIdentifier = parameterNode.Edge.ProvideNode.FullPath;
                  string aliasedName = parameterNode.Edge.AliasedProvideNode.FullPath;
                  argumentBinding.FormalIdentifier = aliasedName.Substring(aliasedName.LastIndexOf('.') + 1);
                  argumentBindings.Add(argumentBinding);
                  parameterNodes.Add(argumentBinding.ObjectNode);
               }
            }
         }

         argumentBindings.ToArray();
         this.parameterNodes = parameterNodes.ToArray();

         this.FunctionString = "object " + this.RegisterFunctionIdentifier("F") + "(";
         for (int i = 0; i < argumentBindings.Count; i++)
         {
            string argumentTypeName = string.Empty;
            argumentTypeName = this.GetTypeName(argumentBindings[i].ObjectNode.Value.GetType().ToString());
            if (i == argumentBindings.Count - 1)
            {
               this.FunctionString += string.Format("\r\n   {0} {1}", argumentTypeName, argumentBindings[i].FormalIdentifier);
            }
            else
            {
               this.FunctionString += string.Format("\r\n   {0} {1},", argumentTypeName, argumentBindings[i].FormalIdentifier);
            }
         }

         if (this.Result is Effector)
         {
            Effector effector = this.Result as Effector;

            if (effector.ReturnType == typeof(void))
            {
               this.FunctionString += ")\r\n{ " + this.ExpressionString + "(";
               for (int i = 1; i < argumentBindings.Count - 1; i++)
               {
                  this.FunctionString += argumentBindings[i].FormalIdentifier + ",";
               }

               if (argumentBindings.Count > 1)
               {
                  this.FunctionString += argumentBindings[argumentBindings.Count - 1].FormalIdentifier;
               }

               this.FunctionString += ");";
               this.FunctionString += "return true; }";
            }
            else
            {
               this.FunctionString += ")\r\n{ return " + this.ExpressionString + "(";
               for (int i = 1; i < argumentBindings.Count - 1; i++)
               {
                  this.FunctionString += argumentBindings[i].FormalIdentifier + ",";
               }

               if (argumentBindings.Count > 1)
               {
                  this.FunctionString += argumentBindings[argumentBindings.Count - 1].FormalIdentifier;
               }

               this.FunctionString += "); }";
            }

            return base.Analyze(out errors);
         }

         if (this.Result is UserDefinedFunction)
         {
            UserDefinedFunction function = this.Result as UserDefinedFunction;

            this.FunctionString += ")\r\n{ " + function.ExpressionString + "}";

            return base.Analyze(out errors);
         }

         this.FunctionString += ")\r\n{ return " + this.ExpressionString + "; }";
         return base.Analyze(out errors);
      }

      /// <summary>
      /// Gets the enumerator which iterates over the terms composing the expression.
      /// </summary>
      /// <returns>The enumerator which iterates over the terms composing the expression.</returns>
      public IEnumerable<IExpression> GetTermsEnumerator()
      {
         foreach (IExpression expression in this.Result.GetTermsEnumerator())
         {
            yield return expression;
         }
      }

      #endregion

      #region Protected Methods
      
      /// <summary>
      /// Gets the length of the next type name in the string
      /// </summary>
      /// <param name="text">The type name</param>
      /// <returns>The length of the next type name</returns>
      protected int GetTypeNameLength(string text)
      {
         if (text.Contains("`"))
         {
            int genericBracketIndex = text.IndexOf('[');
            int length = genericBracketIndex + 1;

            for (int depth = 1; length < text.Length; length++)
            {
               if (text[length] == '[')
               {
                  depth++;
               }
               else if (text[length] == ']')
               {
                  depth--;
                  if (depth == 0)
                  {
                     length++;
                     break;
                  }
               }
            }

            return length;
         }
         else if (text.Contains(","))
         {
            return text.IndexOf(",");
         }

         return text.Length;
      }

      /// <summary>
      /// Gets the next type name from the string
      /// </summary>
      /// <param name="text">The text from which to extract the type name</param>
      /// <returns>the type name</returns>
      protected string GetTypeName(string text)
      {
         int i = 0;
         while (!char.IsLetter(text[i]))
         {
            i++;
         }

         text = text.Substring(i);

         if (text.Contains("`"))
         {
            int genericBracketIndex = text.IndexOf('[');
            int typeNameLength = this.GetTypeNameLength(text);

            int genericBackTickIndex = text.IndexOf('`');
            string typeName = text.Substring(0, genericBackTickIndex);
            string arrityString = text.Substring(genericBackTickIndex + 1, genericBracketIndex - genericBackTickIndex - 1);
            int arrity = int.Parse(arrityString);
            text = text.Substring(genericBracketIndex + 1, typeNameLength - 1 - genericBracketIndex - 1);

            string argument = this.GetTypeName(text);
            typeName += "< " + argument;
            text = text.Substring(this.GetTypeNameLength(text));

            for (int j = 1; j < arrity; j++)
            {
               argument = this.GetTypeName(text);
               typeName += ", " + argument;
               int start = 0;
               while (!char.IsLetter(text[start]))
               {
                  start++;
               }

               text = text.Substring(start);

               text = text.Substring(this.GetTypeNameLength(text));
            }

            typeName += " >";
            return typeName;
         }

         return text.Substring(0, this.GetTypeNameLength(text));
      }

      #endregion

      #region Nested Types

      /// <summary>
      /// Maps the object nodes to formal parameters defining the expression.
      /// </summary>
      public class ArgumentBinding
      {
         /// <summary>
         /// Gets or sets the object node of the configured method
         /// </summary>
         public ObjectNode ObjectNode
         {
            get;
            set;
         }

         /// <summary>
         /// Gets or sets the identifier string of the actual parameter
         /// </summary>
         public string ActualIdentifier
         {
            get;
            set;
         }

         /// <summary>
         /// Gets or sets the identifier string of the actual parameter
         /// </summary>
         public string FormalIdentifier
         {
            get;
            set;
         }
      }

      #endregion
   }
}