// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2013-12-04</date>
// <summary>Code graph representing an object.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System;
   using System.CodeDom.Compiler;
   using System.Reflection;

   #endregion

   /// <summary>
   /// C# compiler for generating user-defined classes, methods, and expressions.
   /// </summary>
   public class CSharpCompiler : MarshalByRefObject
   {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="CSharpCompiler"/> class.
      /// </summary>
      public CSharpCompiler()
      {
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets or sets the source code in the form of a string.
      /// </summary>
      public string Source
      {
         get;
         set;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Compiles a user-defined expression
      /// </summary>
      /// <param name="referencedAssemblies">Referenced assemblies</param>
      /// <param name="returnType">The return type of the expression</param>
      /// <param name="expression">The expression in the form of a string</param>
      /// <param name="target">The target object</param>
      /// <param name="methodInfo">The method node of the expression</param>
      /// <returns>The results of the the compilation</returns>
      public string CompileExpression(string[] referencedAssemblies, string returnType, string expression, ref object target, ref MethodInfo methodInfo)
      {
         CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
         CompilerParameters compilerParameters = new CompilerParameters();
         compilerParameters.CompilerOptions = "/optimize";

         compilerParameters.ReferencedAssemblies.Add("System.dll");
         foreach (string referencedAssembly in referencedAssemblies)
         {
            compilerParameters.ReferencedAssemblies.Add(referencedAssembly);
         }

         compilerParameters.GenerateInMemory = true;
         compilerParameters.TreatWarningsAsErrors = false;

         string className = "Class_" + Guid.NewGuid().ToString("N");
         string methodName = "Method_" + Guid.NewGuid().ToString("N");
         string source = @"namespace MTI.Agents
                           {{
                              using System;
                              public class {0} : MarshalByRefObject
                              {{
                                 public {1} {2}()
                                 {{
                                    return {3};
                                 }}
                              }}
                           }}";
         this.Source = string.Format(source, className, returnType, methodName, expression);

         CompilerResults results = codeDomProvider.CompileAssemblyFromSource(compilerParameters, this.Source);

         string errors = string.Empty;
         if (results.Errors.Count > 0)
         {
            errors = string.Format("Errors compiling expression: {0}\r\n", expression);
            foreach (CompilerError ce in results.Errors)
            {
               errors += string.Format("  {0}\r\n", ce.ToString());
            }
         }
         else
         {
            target = results.CompiledAssembly.CreateInstance("MTI.Agents." + className);
            if (target != null)
            {
               MethodInfo[] methods = target.GetType().GetMethods();

               string name = string.Empty;
               for (int i = 0; i < methods.Length; i++)
               {
                  if (methods[i].DeclaringType == target.GetType())
                  {
                     methodInfo = methods[i];
                     errors = string.Format("Method {0} built successfully.\n", methods[i].Name);
                     break;
                  }
               }
            }
         }

         return errors;
      }

      /// <summary>
      /// Compiles a user-defined method
      /// </summary>
      /// <param name="referencedAssemblies">Referenced assemblies</param>
      /// <param name="methods">The method definitions</param>
      /// <param name="target">The target object</param>
      /// <returns>The results of the the compilation</returns>
      public string CompileMethods(string[] referencedAssemblies, string[] methods, ref object target)
      {
         CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
         CompilerParameters compilerParameters = new CompilerParameters() { GenerateInMemory = true, CompilerOptions = "/optimize" };

         foreach (string referencedAssembly in referencedAssemblies)
         {
            compilerParameters.ReferencedAssemblies.Add(referencedAssembly);
         }

         if (!compilerParameters.ReferencedAssemblies.Contains("C:\\Windows\\Microsoft.Net\\assembly\\GAC_MSIL\\System\\v4.0_4.0.0.0__b77a5c561934e089\\System.dll"))
         {
            //compilerParameters.ReferencedAssemblies.Add("System.dll");
         }

         compilerParameters.GenerateInMemory = true;
         compilerParameters.TreatWarningsAsErrors = false;

         string className = "Class_" + Guid.NewGuid().ToString("N");
         string source = @"namespace MTI.Agents
                           {
                              using System;
                              public class " + className + ": MarshalByRefObject \r\n {";

         foreach (string method in methods)
         {
            source += "public " + method + "\r\n";
         }

         source += "  }\r\n   }";
         this.Source = source;

         CompilerResults results = codeDomProvider.CompileAssemblyFromSource(compilerParameters, this.Source);

         string errors = string.Empty;
         if (results.Errors.Count > 0)
         {
            errors = "Errors compiling methods: \r\n";
            foreach (CompilerError ce in results.Errors)
            {
               errors += string.Format("  {0}\r\n", ce.ToString());
            }
         }
         else
         {
            target = results.CompiledAssembly.CreateInstance("MTI.Agents." + className);
         }

         return errors;
      }

      /// <summary>
      /// Compiles a user-defined class
      /// </summary>
      /// <param name="referencedAssemblies">Referenced assemblies</param>
      /// <param name="className">The name of the class</param>
      /// <param name="propertyStrings">The property definitions of the class</param>
      /// <param name="methodStrings">The method definitions of the class</param>
      /// <returns>The results of the the compilation</returns>
      public CompilerResults CompileClass(string[] referencedAssemblies, string className, string[] propertyStrings, string[] methodStrings)
      {
         CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
         CompilerParameters compilerParameters = new CompilerParameters();
 
         compilerParameters.ReferencedAssemblies.Add("System.dll");
         foreach (string referencedAssembly in referencedAssemblies)
         {
            compilerParameters.ReferencedAssemblies.Add(referencedAssembly);
         }

         compilerParameters.GenerateInMemory = true;
         compilerParameters.TreatWarningsAsErrors = false;

         this.Source = "namespace MTI.Console{using System;\n using MTI.Core;\n public class " + className;
         this.Source += ": MTI.Core.Component\n{";
         foreach (string propertyString in propertyStrings)
         {
            this.Source += "public " + propertyString + "{ get; set; }\n";
         }

         this.Source += "public override bool Persist { get { return false; } }";

         foreach (string methodString in methodStrings)
         {
            this.Source += "public " + methodString + "\n";
         }

         this.Source += "public override bool Initialize(){ return true; }\n";

         this.Source += "}\n}\n";

         return codeDomProvider.CompileAssemblyFromSource(compilerParameters, this.Source);
      }

      #endregion
   }
}
