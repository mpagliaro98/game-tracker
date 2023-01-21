using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class SortRatedObjectStatus : SortRatedObjects
    {
        public const int SORT_Status = 10;

        public SortRatedObjectStatus() : base() { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod, TrackerModule module)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod, module);
            switch (sortMethod)
            {
                case SORT_Status:
                    sortFunction = obj => ((RankedObjectStatus)obj).StatusExtension.Status == null ? "" : ((RankedObjectStatus)obj).StatusExtension.Status.Name;
                    break;
            }
            return sortFunction;
        }
    }
}
