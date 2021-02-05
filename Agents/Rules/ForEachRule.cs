// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2011-06-07</date>
// <summary>ForEachRule executes a set of conclusions actions in parallel.</summary>

namespace GES.ArtificialIntelligence
{
   #region Directives

   using System.Threading.Tasks;

   #endregion

   /// <summary>
   /// ForEachRule executes a set of conclusions actions in parallel.
   /// </summary>
   public class ForEachRule : Rule
   {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the ForEachRule class.
      /// </summary>
      /// <param name="identifier">The name of the rule.</param>
      /// <param name="Conditions">The condition Conditions.</param>
      /// <param name="conclusions">The conclusion actions</param>
      public ForEachRule(string identifier, Clause[] Conditions, Clause[] conclusions) :
         base(identifier, Conditions, conclusions)
      {
      }

      #endregion

      #region Methods

      /// <summary>
      /// Executes the conclusion actions in parallel.
      /// </summary>
      public override void Execute()
      {         
         ParallelLoopResult result = Parallel.ForEach<Clause>(
                                                               this.Conclusions, 
                                                               (Clause conclusion, ParallelLoopState loopState) => 
                                                               {
                                                                  try
                                                                  {
                                                                     if (conclusion.Evaluate() != TriState.True)
                                                                     {
                                                                        loopState.Stop();
                                                                        return;
                                                                     }
                                                                  }
                                                                  catch
                                                                  {
                                                                  }
                                                               });

         this.Executed = result.IsCompleted;
      }

      #endregion
   }
}
