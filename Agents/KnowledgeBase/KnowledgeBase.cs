// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Component representing a knowledgbase consisting of states, rules, and facts.  Performs goal-oriented searches utilizing a backward-chaining mechanism.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;

   #endregion

   /// <summary>
   /// The validity of search or condition
   /// </summary>
   public enum TriState
   { 
      /// <summary>
      /// The search or condition cannot be determined
      /// </summary>
      Undetermined = -1, 

      /// <summary>
      /// The search failed or the condition is not true.
      /// </summary>
      False = 0, 

      /// <summary>
      /// The search succeeded or the condition is true.
      /// </summary>
      True = 1
   }

   /// <summary>
   /// Component representing a knowledgebase consisting of states, rules, and facts.  
   /// Performs goal-oriented searches utilizing a backward-chaining mechanism.
   /// </summary>
   public class KnowledgeBase :
      MTI.Core.Component
   {
      #region Fields
      /// <summary>
      /// The single knowledge base instance
      /// </summary>
      private static KnowledgeBase singleton;
      #endregion

      #region Constructors

      /// <summary>
      /// Prevents a default instance of the KnowledgeBase class from being created.
      /// </summary>
      private KnowledgeBase()
      {
         this.States = new State[0];
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets a set of state variables composing the knowledgebase.
      /// </summary>
      private State[] States
      {
         get;
         set;
      }

      #endregion

      #region Public Static Methods

      /// <summary>
      /// Gets the singleton knowledgebase.
      /// </summary>
      /// <returns>The singleton knowledgebase.</returns>
      public static KnowledgeBase GetKnowledgeBase()
      {
         if (KnowledgeBase.singleton == null)
         {
            KnowledgeBase.singleton = new KnowledgeBase();
         }

         return KnowledgeBase.singleton;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Initiates a backward chain search for the given goal
      /// </summary>
      /// <param name="sender">The object raising the event</param>
      /// <param name="args">The goal arguments</param>
      public void BackwardChain(object sender, GoalEventArgs args)
      {
         bool found = false;

         foreach (Clause clause in args.GoalState.DependentClauses)
         {
            if (clause.Usage != Clause.Type.Conclusion)
            {
               continue;
            }

            if (args.GoalValue != null && !clause.Function.Value.Equals(args.GoalValue))
            {
               continue;
            }

            foreach (Rule rule in clause.DependentRules)
            {
               switch (this.BackwardChain(rule, args))
               {
                  case TriState.Undetermined:
                     args.Messages.Add("Rule " + rule.Identifier + " is undetermined.");
                     break;

                  case TriState.True:
                     args.GoalState.Value = clause.Function.Value;
                     args.Solutions.Add(new Solution(clause.Function.Value, new Stack<GoalEventArgs>(args.SubGoals)));
                     args.SubGoals.Clear();
                     args.Messages.Add("Rule " + clause.DependentRules[0].Identifier + " evaluated as true.");
                     args.Messages.Add("Solution is " + rule.Identifier);

                     foreach (GoalEventArgs subgoal in args.Solutions[args.Solutions.Count - 1].SubGoals)
                     {
                        this.ResetGoals(subgoal);
                     }

                     if (args.FindOneSolution == true)
                     {
                        found = true;
                     }

                     break;

                  case TriState.False:
                     args.Messages.Add("Rule " + rule.Identifier + " evaluated as false.");
                     break;
               }

               if (found)
               {
                  return;
               }
            }
         }
      }

      /// <summary>
      /// Continues a backward chain search from the given rule.
      /// </summary>
      /// <param name="rule">The rule to be explored.</param>
      /// <param name="e">The goal to prove.</param>
      /// <returns>The status of a search.</returns>
      public TriState BackwardChain(Rule rule, GoalEventArgs e)
      {
         List<GoalEventArgs> subgoals = new List<GoalEventArgs>();

         for (int i = 0; i < rule.Conditions.Length; i++)
         {
            switch (rule.Conditions[i].Evaluate())
            {
               case TriState.Undetermined:

                  GoalEventArgs subgoal = new GoalEventArgs(rule.Conditions[i].State);
                  subgoals.Add(subgoal);

                  this.GetSolutions(rule.Conditions[i], subgoal);

                  if (subgoal.Solutions.Count == 0)
                  {
                     this.ResetGoals(subgoals);
                     return TriState.Undetermined;
                  }
                  else
                  {
                     rule.Conditions[i].State.Value = subgoal.Solutions[0].Value;
                  }

                  break;

               case TriState.False:

                  this.ResetGoals(subgoals);
                  return TriState.False;
            }
         }

         foreach (GoalEventArgs subgoal in subgoals)
         {
            e.SubGoals.Push(subgoal);
         }

         return TriState.True;
      }

      /// <summary>
      /// Event handler that initializes this instance.
      /// </summary>
      /// <returns>A value indicating whether the object successfully initialized.</returns>
      public override bool OnInitialize()
      {
         return true;
      }

      #endregion

      #region Helpers

      /// <summary>
      /// Adds a state variable to the knowledgebase.
      /// </summary>
      /// <param name="state">The state variable.</param>
      public void AddState(State state)
      {
         List<State> states = new List<State>(this.States);
         states.Add(state);
         this.States = states.ToArray();
      }

      ////public State GetState(string name)
      ////{
      ////   try
      ////   {
      ////      if (!this.States.Any<State>(s => s.Identifier == name))
      ////      {
      ////         if (ObjectGraph.ObjectDictionary.Keys.Contains(name))
      ////         {
      ////            this.AddState(new State(ObjectGraph.ObjectDictionary[name]));
      ////         }
      ////      }

      ////      return this.States.Single<State>(s => s.Identifier == name);
      ////   }
      ////   catch
      ////   {
      ////   }

      ////   return null;
      ////}

      /// <summary>
      /// Removes a state variable from the knowledgebase.
      /// </summary>
      /// <param name="state">The state variable.</param>
      public void RemoveState(State state)
      {
         try
         {
            this.States = this.States.Where<State>(s => s == state).ToArray();
         }
         catch
         {
         }
      }

      /// <summary>
      /// Resets the sub-goals after a search failed while exploring a path.
      /// </summary>
      /// <param name="goal">The goal to be proven.</param>
      public void ResetGoals(GoalEventArgs goal)
      {
         goal.GoalState.Value = null;

         foreach (GoalEventArgs args in goal.SubGoals)
         {  // this should be empty but check anyway
            this.ResetGoals(args);
         }

         foreach (Solution solution in goal.Solutions)
         {
            foreach (GoalEventArgs subgoal in solution.SubGoals)
            {
               this.ResetGoals(subgoal);
            }
         }
      }

      /// <summary>
      /// Resets the states variables.
      /// </summary>
      public void Reset()
      {
         foreach (State state in this.States)
         {
            state.Value = null;
         }
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Initiates a search to prove a sub-goal
      /// </summary>
      /// <param name="condition">The condition</param>
      /// <param name="subgoal">The sub-goal parameters</param>
      private void GetSolutions(Clause condition, GoalEventArgs subgoal)
      {
         this.BackwardChain(this, subgoal);

         List<Solution> solutions = new List<Solution>();
         foreach (Solution solution in subgoal.Solutions)
         {
            condition.State.Value = solution.Value;

            if (condition.Evaluate() == TriState.True)
            {
               solutions.Add(solution);
            }
         }

         subgoal.Solutions.Clear();
         subgoal.Solutions.AddRange(solutions);
      }

      /// <summary>
      /// Resets the sub-goals due to a failed search
      /// </summary>
      /// <param name="goals">The goals to reset</param>
      private void ResetGoals(List<GoalEventArgs> goals)
      {
         foreach (GoalEventArgs goal in goals)
         {
            this.ResetGoals(goal);
         }
      }

      #endregion
   }
}
