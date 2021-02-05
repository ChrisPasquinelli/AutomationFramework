// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>ForRule executes a set of conclusions actions from a starting index to final index.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System.ComponentModel;
   using System.Threading.Tasks;
   using MTI.Core;
   using System;

   #endregion

   /// <summary>
   /// ForRule executes a set of conclusions actions from a starting index to final index.
   /// </summary>
   [Provide(Categories = new string[] { "Logic" })]
   [Description("ParallelForRule executes a set of actions from a starting index to a final index in parallel")]
   public class ParallelForRule : Rule
   {
      #region Fields

      /// <summary>
      /// The current index.
      /// </summary>
      private int index;

      /// <summary>
      /// The lower bound of the iteration
      /// </summary>
      private int fromInclusive;

      /// <summary>
      /// The upper bound (exclusive) of iteration
      /// </summary>
      private int toExclusive;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the ParallelForRule class.
      /// </summary>
      public ParallelForRule() :
         base()
      {
      }
      
      /// <summary>
      /// Initializes a new instance of the ParallelForRule class.
      /// </summary>
      /// <param name="identifier">The name of the rule.</param>
      /// <param name="Conditions">The condition Conditions.</param>
      /// <param name="conclusions">The conclusion actions</param>
      public ParallelForRule(string identifier, Clause[] Conditions, Clause[] conclusions) :
         base(identifier, Conditions, conclusions)
      {
      }
   
      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the starting index.
      /// </summary>
      [Require(Cut = true)]
      [Description("The starting index.")]
      public int FromInclusive
      {
         get
         {
            return this.fromInclusive;
         }

         set
         {
            lock (this)
            {
               this.fromInclusive = value;
            }
         }
      }

      /// <summary>
      /// Gets or sets the current index.
      /// </summary>
      [Provide]
      [Description("The current index.")]
      public int Index
      {
         get
         {
            return this.index;
         }

         set
         {
            this.index = value;
            this.OnPropertyChanged("Index");
         }
      }

      /// <summary>
      /// Gets or sets the final index.
      /// </summary>
      [Require(Cut = true)]
      [Description("The final index.")]
      public int ToExclusive
      {
         get
         {
            return this.toExclusive;
         }

         set
         {
            lock (this)
            {
               this.toExclusive = value;
            }
         }
      }

      #endregion

      #region Methods

      /// <summary>
      /// Event handler that determines which signal went high
      /// </summary>
      /// <param name="sender">The signal source.</param>
      /// <param name="args">The value event changed arguments.</param>
      protected override void OnSignalRaised(object sender, ValueChangedEventArgs args)
      {
         if (args.UpdateEvent != ValueChangedEventArgs.Update.Assign)
         {
            return;
         }

         int index = this.TriggersNodes.IndexOf((ObjectNode)sender);
         this.Signals[index] = true;
         if (this.TriggerType == Trigger.AllSignals)
         {
            for (int i = 0; i < this.Signals.Count; i++)
            {
               if (!this.Signals[i])
               {
                  return;
               }
            }
         }

         MTI.Core.Component.SetState(this, ComponentState.Processing);
         ParallelLoopResult result = Parallel.For(
                                                   this.FromInclusive, 
                                                   this.ToExclusive, 
                                                   (int i) =>
         {
            this.Index = i;
            foreach (Clause conclusion in this.Conclusions)
            {
               if (this.Enable && conclusion.Evaluate() != TriState.True)
               {
                  return;
               }
            }
         });
         
         for (index = this.FromInclusive; index < this.ToExclusive; index++)
         {
            this.Index = index;
            if (this.Enable && this.Evaluate() == TriState.True)
            {
               this.executed = false;
               try
               {
                  for (int i = 0; i < this.conclusions.Count; i++)
                  {
                     this.conclusions[i].Evaluate();
                  }

               }
               catch (Exception ex)
               {
                  MTI.Core.Component.SetState(this, ComponentState.Error);
                  this.OnSystemNotification(this, new SystemEventArgs<object>(ex.Message, this.Identifier, this));
               }
            }
         }

         MTI.Core.Component.ClearState(this, ComponentState.Processing);

         if (this.AutoReset)
         {
            this.Reset();
         }
      }

      #endregion
   }
}
