using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class FilterRankedObjectStatus : FilterRankedObjects
    {
        public new TrackerModuleStatuses Module { get; set; }

        public FilterRankedObjectStatus() : base() { }

        public FilterRankedObjectStatus(TrackerModuleStatuses module, Settings settings) : base(module, settings) { }
    }
}
