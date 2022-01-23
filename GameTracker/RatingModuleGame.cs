using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.IO;

namespace GameTracker.Model
{
    public class RatingModuleGame
        : RatingModuleGameTemplate<RatableGame, ScoreRange, SettingsScore, CompletionStatus, RatingCategoryWeighted>
    {
        public RatingModuleGame(LoadSaveEngineGame<RatableGame, ScoreRange, SettingsScore, CompletionStatus, RatingCategoryWeighted> engine)
        {
            loadSaveEngine = engine;
        }
    }
}
