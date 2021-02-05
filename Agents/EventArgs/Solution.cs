// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-07-01</date>
// <summary>Class housing solutions for a goal determined by the backward chaining mechanism.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System.Collections.Generic;

   #endregion

   /// <summary>
   /// Solution maintains the actions that lead up to a goal.
   /// </summary>
   public class Solution
   {
      #region Fields

      /// <summary>
      /// The goal value.
      /// </summary>
      private object value;

      /// <summary>
      /// The sub-goals that lead up to the solution
      /// </summary>
      private Stack<GoalEventArgs> subgoals = new Stack<GoalEventArgs>();

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Solution class.
      /// </summary>
      /// <param name="value">The goal value.</param>
      /// <param name="subgoals">The stack of sub-goals leading to the solution</param>
      public Solution(object value, Stack<GoalEventArgs> subgoals)
      {
         this.value = value;
         this.subgoals = subgoals;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets the goal value
      /// </summary>
      public object Value
      {
         get { return this.value; }
      }

      /// <summary>
      /// Gets the stack of sub-goals leading to the solution.
      /// </summary>
      public Stack<GoalEventArgs> SubGoals
      {
         get { return this.subgoals; }
      }

      #endregion
   }
}
