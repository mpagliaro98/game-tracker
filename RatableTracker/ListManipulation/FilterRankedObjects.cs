using RatableTracker.Exceptions;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation
{
    [Serializable]
    public class FilterRankedObjects : FilterBase<RankedObject>
    {
        public FilterRankedObjects() : base() { }

        public FilterRankedObjects(TrackerModule module, Settings settings) : base(module, settings) { }
    }
}
