// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Function dynamically creates a function given configured parameters and a string representing the body.</summary>

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
   /// Function dynamically creates a function given a string representing the body and configured parameters. Because this
   /// component is only used internally it is not necessary to construct an abstract syntax tree as the core
   /// system does for generating code.  It is sufficient to simply script a c# string.
   /// </summary>
   [Provide(Categories = new string[] { "Logic" })]
   [Description("Dynamically creates a function given a string representing the body and configured parameters")]
   public class Function :
      IGraphUpdateSubscriber
   { 
      #region Fields

      /// <summary>
      /// The system graph containing all function definitions
      /// </summary>
      private static ObjectGraph graph;

      /// <summary>
      /// Global functions to be compiled once altogether in order to alleviate loading time due to compilation of many small modules
      /// </summary>
      private static Dictionary<string, Function> functions = new Dictionary<string, Function>();

      /// <summary>
      /// The target used to invoke the function
      /// </summary>
      private static object target;

      /// <summary>
      /// The function identifier
      /// </summary>
      private string identifier;

      /// <summary>
      /// The methodInfo used to invoke the function
      /// </summary>
      private MethodInfo methodInfo;

      /// <summary>
      /// The object node of the property "Parameters" containing values to be used as parameters when invoking the function
      /// </summary>
      private ObjectNode parametersArrayNode;

      /// <summary>
      /// The values to be used as parameters when invoking the function
      /// </summary>
      private object[] parameters;

      /// <summary>
      /// The object nodes of the values to be used as parameters when invoking the function
      /// </summary>
      private List<ObjectNode> referenceParameters;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Function class.
      /// </summary>
      public Function()
      {
         this.FunctionString = string.Empty;
         this.parameters = new object[0];
         this.parametersArrayNode = null;
         this.referenceParameters = new List<ObjectNode>();
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets or sets the string representing the function body.
      /// </summary>
      [Require(Cut = true)]
      public virtual string FunctionString
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the objects corresponding to parameters passed into the function
      /// </summary>
      [Require(Cut = true)]
      public object[] Parameters
      {
         get
         {
            return this.parameters;
         }

         set
         {
            this.parameters = value;
         }
      }

      #endregion

      #region Internal Properties

      /// <summary>
      /// Gets the object nodes corresponding to parameters passed into the function
      /// </summary>
      [Browsable(false)]
      internal virtual ObjectNode[] ParameterNodes
      {
         get { return this.parametersArrayNode.Properties.ToArray(); }
      }

      /// <summary>
      /// Gets the value of the function by invoking the method with the current parameters
      /// </summary>
      [Browsable(false)]
      internal virtual object Value
      {
         get
         {
            if (this.methodInfo != null)
            {
               ParameterInfo[] parameterInfos = this.methodInfo.GetParameters();
               List<object> parameters = new List<object>();
               for (int i = 0; i < parameterInfos.Length; i++)
               {
                  if (this.ParameterNodes[i].Value is IConvertible)
                  {
                     object value = Convert.ChangeType(this.ParameterNodes[i].Value, parameterInfos[i].ParameterType);
                     if (value != null)
                     {
                        parameters.Add(value);
                     }
                     else
                     {
                        parameters.Add(this.ParameterNodes[i].Value);
                     }
                  }
                  else
                  {
                     parameters.Add(this.ParameterNodes[i].Value);
                  }
               }

               object result = this.methodInfo.Invoke(target, parameters.ToArray());
               return result;
            }
            else
            {
               return null;
            }
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Event handler called for system notifications
      /// </summary>
      /// <param name="sender">The originator of the event</param>
      /// <param name="e">The event arguments</param>
      public static void OnSystemNotification(object sender, SystemEventArgs<object> e)
      {
         if (e.Message == "INITIALIZE_END")
         {
            CSharpCompiler compiler = new CSharpCompiler();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            List<string> referenceAssemblies = new List<string>(new string[] { baseDirectory + "Core.dll", baseDirectory + "Mathematics.dll" });

            List<string> methods = new List<string>();
            foreach (Function function in Function.functions.Values)
            {
               function.PreCompileMethods(referenceAssemblies);
               methods.Add(function.FunctionString);
            }

            List<string> assemblyLocations = new List<string>();
            foreach (KeyValuePair<string, string> assemblyAndLocation in MTI.Core.KnowledgeBase.LocalLoader.GetAssembliesAndLocations())
            {
               assemblyLocations.Add(assemblyAndLocation.Value);
            }

            string errors = compiler.CompileMethods(assemblyLocations.ToArray(), methods.ToArray(), ref Function.target);
            if (!string.IsNullOrEmpty(errors))
            {
               ((ObjectGraph)sender).Notify(null, new SystemEventArgs<object>("INITIALIZE FAILED: " + errors, "Function.OnSystemNotification", null, MessageType.Error), true);
            }

            foreach (Function function in Function.functions.Values)
            {
               errors += function.PostCompileMethods(Function.target);
            }

            if (!string.IsNullOrEmpty(errors))
            {
               ((ObjectGraph)sender).Notify(null, new SystemEventArgs<object>("INITIALIZE FAILED: " + errors, "Function.OnSystemNotification", null, MessageType.Error), true);
            }
         }
      }

      /// <summary>
      /// Analyzes the validity of the function.
      /// </summary>
      /// <param name="errors">Set to true if function is syntactically correct otherwise false.</param>
      /// <returns>A string describing any syntax errors.</returns>
      public virtual string Analyze(out bool errors)
      {
         string results = string.Empty;
         errors = false;

         if (string.IsNullOrEmpty(this.FunctionString))
         {
            errors = true;
            results += "Unspecified function.\r\n";
         }

         return results;
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
         if (Function.graph == null)
         {
            Function.graph = thisGraph;
            Function.graph.SystemNotification += Function.OnSystemNotification;
         }
         else if (thisGraph != Function.graph)
         {
            throw new Exception("Unable to register function with system graph");
         }

         foreach (ObjectNode property in thisNode.Properties)
         {
            switch (property.Name)
            {
               case "Parameters":
                  this.parametersArrayNode = property;
                  break;
            }
         }
      }

      #endregion

      #region Internal Functions

      /// <summary>
      /// Gets the object nodes corresponding to parameters passed into the function
      /// </summary>
      /// <param name="baseName">The base name for the function to be declared</param>
      /// <returns>A unique identifier for the function starting with the base name</returns>
      [Browsable(false)]
      internal string RegisterFunctionIdentifier(string baseName)
      {
         if (string.IsNullOrEmpty(this.identifier))
         {
            List<string> identifiers = new List<string>(functions.Keys);
            this.identifier = MTI.Core.KnowledgeBase.GetUniqueIdentifier(baseName, identifiers);
            Function.functions.Add(this.identifier, this);
         }

         return this.identifier;
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Compiles the right-hand side expression
      /// </summary>
      /// <param name="referenceAssemblies">The referenced assemblies</param>
      private void PreCompileMethods(List<string> referenceAssemblies)
      {
         try
         {
            foreach (ObjectNode node in this.ParameterNodes)
            {
               MTI.Core.KnowledgeBase.LocalLoader.GetReferences(node.Value.GetType().Assembly.GetName().FullName, referenceAssemblies);
            }
         }
         catch (Exception exception)
         {
            throw exception;
         }
      }

      /// <summary>
      /// Compiles the right-hand side expression
      /// </summary>
      /// <param name="target">The target object on which to call members</param>
      /// <returns>A string describing any syntax errors.</returns>
      private string PostCompileMethods(object target)
      {
         string results = string.Empty;

         if (target != null)
         {
            this.methodInfo = target.GetType().GetMethod(this.identifier);

            ParameterInfo[] parameterInfos = this.methodInfo.GetParameters();
            if (parameterInfos.Length != this.ParameterNodes.Length)
            {
               results += "The number of specified parameters does not equal the number of arguments defined in " + this.FunctionString + "\r\n";
               return results;
            }

            // Assign and check parameters
            this.referenceParameters.Clear();
            if (parameterInfos.Length > 0)
            {
               for (int i = 0; i < this.ParameterNodes.Length; i++)
               {
                  if (this.ParameterNodes[i].Edge != null)
                  {
                     this.referenceParameters.Add(this.ParameterNodes[i]);
                  }

                  if (this.ParameterNodes[i].Value.GetType().FullName != parameterInfos[i].ParameterType.FullName)
                  {
                     object value = Convert.ChangeType(this.ParameterNodes[i].Value, parameterInfos[i].ParameterType);
                     if (value == null)
                     {
                        results += "Cannot convert " + this.ParameterNodes[i].FullPath + " to " + parameterInfos[i].Name + "\r\n";
                     }
                  }
               }
            }
         }
         else
         {
            results = "Cannot find function: " + this.identifier;
         }

         return results;
      }

      #endregion
   }
}