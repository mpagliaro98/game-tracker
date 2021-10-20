using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;

namespace GameTracker.Model
{
    public class RatingModuleGame
        : RatingModuleCompletableGame<RatableGame, RatingCategoryWeighted, CompletionStatusGame>
    {
        public RatingModuleGame()
        {
            loadSaveEngine = new LoadSaveEngineGameJson();
        }
    }
}
