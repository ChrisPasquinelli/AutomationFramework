// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Base class for agents.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System.Collections.Generic;
   using System.ComponentModel;
   using MTI.Core;

   #endregion

   /// <summary>
   /// Base class for agents.  By default implements a conflict resolution strategy based on priority.
   /// </summary>
   [Provide(Categories = new string[] { "Agents" })]
   [Description("Base class Agent that implements a priority-based strategy.")]
   public partial class Agent : MTI.Core.Component
   {
      #region Fields

      /// <summary>
      /// The configured rules.
      /// </summary>
      private List<Rule> rules = new List<Rule>();

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Agent class.
      /// </summary>
      public Agent()
      {
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the set of rules.
      /// </summary>
      [Require(Cut = true)]
      [Description("Actions based on conditions that are carried out by the agent.")]
      public virtual Rule[] Rules
      {
         get
         {
            return this.rules.ToArray();
         }

         set
         {
            this.rules = new List<Rule>(value);
            this.OnPropertyChanged("Rules");
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Adds a rule to the rule set.
      /// </summary>
      /// <param name="rule">The appended rule</param>
      public void AddRule(Rule rule)
      {
         this.rules.Add(rule);
      }

      /// <summary>
      /// Executes a forward chain process based on the (overridden) conflict resolution strategy
      /// </summary>
      [Provide]
      [Description("Executes a forward chain process based on the priority of the rules.")]
      public virtual void ForwardChain()
      {
         Rule[] activeRules = this.ActiveRuleSet(this.Rules);
         if (activeRules.Length > 0)
         {
            this.SelectRule(activeRules).Execute();
         }
         else
         {
            ;
         }
         //for (Rule[] activeRules = this.ActiveRuleSet(this.Rules); activeRules.Length > 0; activeRules = this.ActiveRuleSet(this.Rules))
         //{
         //   this.SelectRule(activeRules).Execute();
         //}

         foreach (Rule rule in this.Rules)
         {
            if (rule.Executed)
            {
               rule.Reset();
            }
         }
      }

      /// <summary>
      /// Executes a backward chain process based on the (overridden) conflict resolution strategy
      /// </summary>
      [Provide]
      [Description("Executes a forward chain process based on the priority of the rules.")]
      public virtual void BackwardChain()
      {
         // Clean this up and recommission
      }

      /// <summary>
      /// Executes a forward chain process based on the (overridden) conflict resolution strategy
      /// </summary>
      /// <param name="rules">The active rules that implement the forward chaining</param>
      public void ForwardChain(Rule[] rules)
      {
         for (Rule[] activeRules = this.ActiveRuleSet(rules); activeRules.Length > 0; activeRules = this.ActiveRuleSet(rules))
         {
            this.SelectRule(activeRules).Execute();
         }

         foreach (Rule rule in rules)
         {
            if (rule.Executed)
            {
               rule.Reset();
            }
         }
      }

      /// <summary>
      /// Event handler that initializes this instance.
      /// </summary>
      /// <returns>A value indicating whether the object successfully initialized.</returns>
      public override bool OnInitialize()
      {
         foreach (Rule rule in this.Rules)
         {
            if (!rule.OnInitialize())
            {
               return false;
            }
         }

         return true;
      }

      /// <summary>
      /// Removes a rule for the set of rules
      /// </summary>
      /// <param name="rule">The to be removed</param>
      public void RemoveRule(Rule rule)
      {
         try
         {
            this.rules.Remove(rule);
         }
         catch
         {
         }
      }

      #endregion

      #region Protected Methods

      /// <summary>
      /// The default conflict resolution strategy
      /// </summary>
      /// <param name="conflictSet">The set of active rules.</param>
      /// <returns>The rule to execute.</returns>
      protected virtual Rule SelectRule(Rule[] conflictSet)
      {
         if (conflictSet.Length == 0)
         {
            return null;
         }

         Rule selectedRule = conflictSet[0];

         for (int i = 1; i < conflictSet.Length; i++)
         {
            if (conflictSet[i].Priority > selectedRule.Priority)
            {
               selectedRule = conflictSet[i];
            }
         }

         return selectedRule;
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Determines the active rules
      /// </summary>
      /// <param name="rules">the rules to be tested</param>
      /// <returns>The active rules set</returns>
      private Rule[] ActiveRuleSet(Rule[] rules)
      {
         List<Rule> activeRules = new List<Rule>();

         foreach (Rule rule in rules)
         {
            if (rule.Enable && !rule.Executed && rule.Evaluate() == TriState.True)
            {
               activeRules.Add(rule);
            }
         }

         return activeRules.ToArray();
      }

      /// <summary>
      /// Resets the rules for the next cycle of the forward chaining
      /// </summary>
      private void Reset()
      {
         foreach (Rule rule in this.Rules)
         {
            rule.Reset();
         }
      }

      #endregion
   }
}
