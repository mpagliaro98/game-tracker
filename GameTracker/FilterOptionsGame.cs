using GameTracker.Model;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class FilterOptionsGame : FilterOptionsGameTemplate<RatableGame, RatingModuleGame, ScoreRange, SettingsScore, CompletionStatus, RatingCategoryWeighted>
    {
        public FilterOptionsGame(bool showCompilations) : base(showCompilations) { }
    }
}
