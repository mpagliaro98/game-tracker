using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class FilterRankedObjects
    {
        public FilterRankedObjects() { }

        public virtual IList<RankedObject> ApplyFilters(IList<RankedObject> list, TrackerModule module)
        {
            return list;
        }
    }
}
