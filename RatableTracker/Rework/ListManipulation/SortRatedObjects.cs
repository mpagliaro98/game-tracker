using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class SortRatedObjects : SortRankedObjects
    {
        public const int SORT_Score = 20;

        public new TrackerModuleScores Module { get; set; }
        public new SettingsScore Settings { get; set; }

        public SortRatedObjects() : base() { }

        public SortRatedObjects(TrackerModuleScores module, SettingsScore settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            switch (sortMethod)
            {
                case SORT_Score:
                    sortFunction = obj => ((RatedObject)obj).ScoreDisplay;
                    break;
            }
            return sortFunction;
        }
    }
}
