// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Interface defining methods implemented by expression classes.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives
   using System;
   using System.CodeDom;
   using System.Collections.Generic;
   #endregion

   /// <summary>
   /// Interface defining methods implemented by expression classes.
   /// </summary>
   public interface IExpression
   {
      #region Properties

      /// <summary>
      /// Gets a string representing this expression.
      /// </summary>
      string ExpressionString
      {
         get;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Analyzes the validity of the expression
      /// </summary>
      /// <param name="errors">Set to true if a valid configuration otherwise false.</param>
      /// <returns>A string describing any invalid configurations</returns>
      string Analyze(out bool errors);

      /// <summary>
      /// Gets the enumerator which iterates over the terms composing the expression.
      /// </summary>
      /// <returns>The enumerator which iterates over the terms composing the expression.</returns>
      IEnumerable<IExpression> GetTermsEnumerator();

      #endregion
   }
}