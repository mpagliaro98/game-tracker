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
    public class FilterRankedObjects : FilterBase<RankedObject>
    {
        public FilterRankedObjects() : base() { }

        public FilterRankedObjects(TrackerModule module, Settings settings) : base(module, settings) { }
    }
}
