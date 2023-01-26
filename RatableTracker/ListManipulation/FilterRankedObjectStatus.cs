using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation
{
    public class FilterRankedObjectStatus : FilterRankedObjects
    {
        public new TrackerModuleStatuses Module { get { return (TrackerModuleStatuses)base.Module; } set { base.Module = value; } }

        public FilterRankedObjectStatus() : base() { }

        public FilterRankedObjectStatus(TrackerModuleStatuses module, Settings settings) : base(module, settings) { }
    }
}
