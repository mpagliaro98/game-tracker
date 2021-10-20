using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;

namespace GameTracker.Model
{
    public class RatingModuleGame
        : RatingModuleGameTemplate<RatableGame, ScoreRange, SettingsScore, CompletionStatus, RatingCategoryWeighted>
    {
        public RatingModuleGame()
        {
            loadSaveEngine = new LoadSaveEngineGameJson<ValueContainer>();
        }
    }
}
