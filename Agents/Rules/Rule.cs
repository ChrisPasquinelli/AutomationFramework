// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>Rule executes a set of conclusion actions upon a set of condition Conditions evaluating to true.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using MTI.Core;

   #endregion

   /// <summary>
   /// Rule executes a set of actions upon a set of Conditions evaluating to true.
   /// </summary>
   [Provide(Categories = new string[] { "Logic" })]
   [Description("Rule executes a set of actions upon a set Conditions evaluating to true")]
   public class Rule :
      MTI.Core.Component,
      MTI.Core.IGraphUpdateSubscriber
   {
      #region Fields

      /// <summary>
      /// The states of the signals triggering the rule.
      /// </summary>
      private List<bool> signals = new List<bool>();

      /// <summary>
      /// The object nodes of the state variables whose value changed event will trigger the rule to be evaluated.
      /// </summary>
      private List<ObjectNode> triggersNodes = new List<ObjectNode>();

      /// <summary>
      /// The object node of the property referencing the state variables whose value changed event will trigger the rule to be evaluated.
      /// </summary>
      private ObjectNode triggersNode;

      /// <summary>
      /// The object nodes of the state variables whose value changed event will trigger the rule to be evaluated.
      /// </summary>
      private object[] triggers = new object[0];

      /// <summary>
      /// The synchronizing object which is to be locked.
      /// </summary>
      private object synchronizationObject = new object();

      /// <summary>
      /// Value indicating whether the rule complete execution
      /// </summary>
      protected bool executed;

      /// <summary>
      /// The conclusion actions
      /// </summary>
      protected List<Clause> conclusions;

      /// <summary>
      /// The condition Conditions.
      /// </summary>
      protected List<Clause> conditions;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the Rule class.
      /// </summary>
      public Rule()
      {
         this.Enable = true;
         this.AutoReset = true;
         this.TriggerType = Trigger.AnySignal;
         this.conditions = new List<Clause>();
         this.conclusions = new List<Clause>();
         this.triggers = new object[0];
      }

      /// <summary>
      /// Initializes a new instance of the Rule class.
      /// </summary>
      /// <param name="identifier">The name of the rule.</param>
      /// <param name="conditions">The condition clauses.</param>
      /// <param name="conclusions">The conclusion clauses.</param>
      public Rule(string identifier, Clause[] conditions, Clause[] conclusions) :
         this()
      {
         this.Identifier = identifier;
         this.AutoReset = true;
         this.TriggerType = Trigger.AnySignal;
         this.Conditions = conditions;
         this.Conclusions = conclusions;
         this.triggers = new object[0];
      }

      #endregion

      #region Enumerations

      /// <summary>
      /// The types of triggers for which the rule can be configured.
      /// </summary>
      public enum Trigger
      {
         /// <summary>
         /// Trigger rule evaluation after all signal have gone high (Join).
         /// </summary>
         AllSignals,

         /// <summary>
         /// Trigger rule evaluation after any signal goes high.
         /// </summary>
         AnySignal
      }

      #endregion

      #region Public Properties

      /// <summary>
      /// Gets or sets the condition clauses
      /// </summary>
      [Require(Cut = true)]
      [Description("The condition Conditions to be evaluated")]
      public Clause[] Conditions
      {
         get 
         { 
            return this.conditions.ToArray(); 
         }

         set
         {
            foreach (Clause condition in this.conditions)
            {
               condition.DependentRules.Remove(this);
            }

            this.conditions.Clear();
            this.conditions.AddRange(value);
            
            foreach (Clause condition in this.conditions)
            {
               condition.DependentRules.Add(this);
               condition.Usage = Clause.Type.Condition;
            }

            this.OnPropertyChanged("Conditions");
         }
      }
      
      /// <summary>
      /// Gets or sets a value indicating whether the signals should automatically be reset to low after the rule evaluation.
      /// </summary>
      [Description("If true, automatically resets signals to low after the rule evaluation, otherwise manually reset using Reset().")]
      public bool AutoReset
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the conclusion clauses.
      /// </summary>
      [Require(Cut = true)]
      [Description("The conclusion actions to be executed.")]
      public Clause[] Conclusions
      {
         get
         { 
            return this.conclusions.ToArray(); 
         }

         set
         {
            foreach (Clause conclusion in this.conclusions)
            {
               conclusion.DependentRules.Remove(this);
            }

            this.conclusions.Clear();
            this.conclusions.AddRange(value);
            
            foreach (Clause conclusion in this.conclusions)
            {
               conclusion.DependentRules.Add(this);
               conclusion.Usage = Clause.Type.Conclusion;
            }

            this.OnPropertyChanged("Conclusions");
         }
      }

      /// <summary>
      /// Gets or sets the list of trigger sources.
      /// </summary>
      [Require(Cut = true)]
      public object[] Triggers
      {
         get
         {
            return this.triggers;
         }

         set
         {
            this.triggers = value;
            this.OnPropertyChanged("Triggers");
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the rule executed.
      /// </summary>
      [Description("Indicates whether the rule executed.")]
      [Provide]
      public virtual bool Executed
      {
         get
         {
            return this.executed;
         }

         set
         {
            this.executed = value;
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the rule is enabled.
      /// </summary>
      [Description("Enables or disables the rule.")]
      public bool Enable
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the priority of the rule.
      /// </summary>
      [Description("The priority of the rule.")]
      public int Priority
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the type of trigger mechanism.
      /// </summary>
      [Description("The type of trigger mechanism based on raised signals")]
      public Trigger TriggerType
      {
         get;
         set;
      }

      #endregion

      #region Protected Properties
      /// <summary>
      /// Gets the states of the signals triggering the rule.
      /// </summary>
      protected List<bool> Signals
      {
         get
         {
            return this.signals;
         }
      }

      /// <summary>
      ///  Gets the object nodes of the state variables whose value changed event will trigger the rule to be evaluated.
      /// </summary>
      protected List<ObjectNode> TriggersNodes 
      {
         get
         {
            return this.triggersNodes;
         }
      }

      #endregion

      #region Methods

      /// <summary>
      /// Evaluates the Conditions of the rule
      /// </summary>
      /// <returns>A tri-state indicating the result</returns>
      [Provide]
      [Description("Evaluates the Conditions of the rule.")]
      public TriState Evaluate()
      {
         for (int i = 0; i < this.conditions.Count; i++)
         {
            try
            {
               switch (this.conditions[i].Evaluate())
               {
                  case TriState.Undetermined: return TriState.Undetermined;
                  case TriState.False: return TriState.False;
               }
            }
            catch (Exception ex)
            {
               MTI.Core.Component.SetState(this, ComponentState.Error);
               this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, this.Identifier, this));
            }
         }

         return TriState.True;
      }

      /// <summary>
      /// Executes the conclusion actions of the rule.
      /// </summary>
      [Provide]
      [Description("Executes the actions of the rule.")]
      public virtual void Execute()
      {
         if (!this.Enable)
         {
            return;
         }

         this.executed = false;
         try
         {
            for (int i = 0; i < this.conclusions.Count; i++)
            {
               this.conclusions[i].Evaluate();
            }

            this.Executed = true;
         }
         catch (Exception ex)
         {
            MTI.Core.Component.SetState(this, ComponentState.Error);
            this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message + ": " + ex.InnerException.Message, this.Identifier, this));
         }
      }

      /// <summary>
      /// Resets the rule and signals.
      /// </summary>
      [Provide]
      [Description("Resets the rule and signals.")]
      public void Reset()
      {
         this.executed = false;
         for (int i = 0; i < this.signals.Count; i++)
         {
            this.signals[i] = false;
         }
      }

      #endregion

      #region Framework Event Handlers

      /// <summary>
      /// Event handler that accesses the containing component's object graph and this instance's sub-tree.
      /// </summary>
      /// <param name="thisGraph">The object graph in which the object tree for this instance is a subset.</param>
      /// <param name="thisNode">The root node of this instance's object tree.</param>
      public void OnGraphUpdate(ObjectGraph thisGraph, ObjectNode thisNode)
      {
         foreach (ObjectNode property in thisNode.Properties)
         {
            switch (property.Name)
            {
               case "Triggers":
                  this.triggersNode = property;
                  break;
            }
         }
      }

      /// <summary>
      /// Event handler that initializes this instance.
      /// </summary>
      /// <returns>A value indicating whether the object successfully initialized.</returns>
      public override bool OnInitialize()
      {
         this.executed = false;

        foreach (Clause antecendent in this.Conditions)
         {
            if (!antecendent.OnInitialize(this.Identifier))
            {
               return false;
            }
         }

         foreach (Clause conclusion in this.Conclusions)
         {
            if (!conclusion.OnInitialize(this.Identifier))
            {
               return false;
            }
         }

         this.signals.Clear();
         this.triggersNodes.Clear();
         foreach (ObjectNode valueChangedEventSourceNode in this.triggersNode.Properties)
         {
            if (valueChangedEventSourceNode.Edge != null)
            {
               valueChangedEventSourceNode.RemoveValueChangedEventHandler(this.OnSignalRaised);
               valueChangedEventSourceNode.AddValueChangedEventHandler(this.OnSignalRaised);
               this.triggersNodes.Add(valueChangedEventSourceNode);
               this.signals.Add(false);
            }
         }

         return true;
      }

      /// <summary>
      /// Event handler that determines which signal went high
      /// </summary>
      /// <param name="sender">The signal source.</param>
      /// <param name="args">The value event changed arguments.</param>
      protected virtual void OnSignalRaised(object sender, ValueChangedEventArgs args)
      {
         if (args.UpdateEvent != ValueChangedEventArgs.Update.Assign)
         {
            return;
         }

         int index = this.triggersNodes.IndexOf((ObjectNode)sender);
         this.signals[index] = true;
         if (this.TriggerType == Trigger.AllSignals)
         {
            for (int i = 0; i < this.signals.Count; i++)
            {
               if (!this.signals[i])
               {
                  return;
               }
            }
         }

         if (this.Enable && this.Evaluate() == TriState.True)
         {
            MTI.Core.Component.SetState(this, ComponentState.Processing);
            this.Execute();
            MTI.Core.Component.ClearState(this, ComponentState.Processing);
         }

         if (this.AutoReset)
         {
            this.Reset();
         }
      }

      #endregion
   }
}
