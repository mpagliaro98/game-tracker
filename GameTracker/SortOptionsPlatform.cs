using GameTracker.Model;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class SortOptionsPlatform : SortOptionsPlatformTemplate<RatableGame, RatingModuleGame, ScoreRange, SettingsScore, CompletionStatus, RatingCategoryWeighted>
    {
        public SortOptionsPlatform(int sortMethod, SortMode sortMode) : base(sortMethod, sortMode) { }
    }
}
