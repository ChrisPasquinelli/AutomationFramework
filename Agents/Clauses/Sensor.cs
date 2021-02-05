// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Represents a general condition clause with a method that returns a boolean indicating the result of the function.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System;
   using System.Reflection;

   #endregion

   /// <summary>
   /// Represents a general condition clause with a method that returns a boolean indicating the result of the function.
   /// </summary>
   public class Sensor :
      Clause
   {
      #region Fields

      /// <summary>
      /// The object containing the method.
      /// </summary>
      private object owner;

      /// <summary>
      /// The methodInfo describing the method.
      /// </summary>
      private MethodInfo methodInfo;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Sensor class.
      /// </summary>
      /// <param name="owner">The object containing the method.</param>
      /// <param name="methodInfo">The structure describing the method.</param>
      public Sensor(object owner, MethodInfo methodInfo) :
         base(Conditional.Equal)
      {
         this.owner = owner;
         this.methodInfo = methodInfo;
         if (methodInfo.ReturnType != typeof(bool))
         {
            throw new InvalidOperationException();
         }
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the objects corresponding to parameters passed into the function
      /// </summary>
      internal virtual object[] Parameters
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets how the clause is being used in the referencing rule.
      /// </summary>
      internal override Type Usage
      {
         get { return Type.Conclusion; }
         set { throw new InvalidOperationException("Sensors can only be configured as a condition clause."); }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Evaluates the clause
      /// </summary>
      /// <returns>A value indication the validity of the clause - true, false, undetermined.</returns>
      public override TriState Evaluate()
      {
         if ((bool)this.methodInfo.Invoke(this.owner, this.Parameters))
         {
            return TriState.True;
         }
         else
         {
            return TriState.False;
         }
      }

      #endregion
   }
}
