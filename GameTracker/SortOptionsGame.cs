using GameTracker.Model;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class SortOptionsGame : SortOptionsGameTemplate<RatableGame, RatingModuleGame, ScoreRange, SettingsScore, CompletionStatus, RatingCategoryWeighted>
    {
        public SortOptionsGame(int sortMethod, SortMode sortMode) : base(sortMethod, sortMode) { }
    }
}
