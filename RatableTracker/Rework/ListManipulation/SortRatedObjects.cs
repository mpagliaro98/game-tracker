using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
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

        public SortRatedObjects() : base() { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod, TrackerModule module)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod, module);
            switch (sortMethod)
            {
                case SORT_Score:
                    sortFunction = obj => ((RatedObject)obj).Score;
                    break;
            }
            return sortFunction;
        }
    }
}
