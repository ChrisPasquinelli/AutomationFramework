// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-07-01</date>
// <summary>Class representing backward chaining arguments.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System.Collections.Generic;

   #endregion

   /// <summary>
   /// Arguments for backward chaining events.
   /// </summary>
   public class GoalEventArgs
   {
      #region Fields

      /// <summary>
      /// The state variable whose goal value is to be proven through a constructed solution.
      /// </summary>
      private State goalState;

      /// <summary>
      /// The goal value for the state variable
      /// </summary>
      private object goalValue;

      /// <summary>
      /// The sub-goals leading to a potential solution
      /// </summary>
      private Stack<GoalEventArgs> subgoals = new Stack<GoalEventArgs>();

      /// <summary>
      /// The list of solutions.
      /// </summary>
      private List<Solution> solutions = new List<Solution>();

      /// <summary>
      /// A list of status messages.
      /// </summary>
      private List<string> messages = new List<string>();

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the GoalEventArgs class.
      /// </summary>
      /// <param name="goalState">The state variable whose value is to be determined</param>
      public GoalEventArgs(State goalState)
      {
         this.goalState = goalState;
         this.goalValue = null;
         this.FindOneSolution = false;
      }

      /// <summary>
      /// Initializes a new instance of the GoalEventArgs class.
      /// </summary>
      /// <param name="goalState">The state variable for which a solution will be constructed (or not) for the given value.</param>
      /// <param name="goalValue">The goal value for the state variable.</param>
      public GoalEventArgs(State goalState, object goalValue)
      {
         this.goalState = goalState;
         this.goalValue = goalValue;
         this.FindOneSolution = false;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets the state variable whose value is to be determined.
      /// </summary>
      public State GoalState
      {
         get { return this.goalState; }
      }

      /// <summary>
      /// Gets the goal value for the state variable. 
      /// </summary>
      public object GoalValue
      {
         get { return this.goalValue; }
      }

      /// <summary>
      /// Gets the sub-goals that are currently on the queue to be proven.
      /// </summary>
      public Stack<GoalEventArgs> SubGoals
      {
         get { return this.subgoals; }
      }

      /// <summary>
      /// Gets a list of current solutions
      /// </summary>
      public List<Solution> Solutions
      {
         get { return this.solutions; }
      }

      /// <summary>
      /// Gets a list of status messages for the current search.
      /// </summary>
      public List<string> Messages
      {
         get { return this.messages; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the backward chaining mechanism is to stop searching after finding a solution.
      /// </summary>
      public bool FindOneSolution
      {
         get;
         set;
      }

      #endregion
   }
}
