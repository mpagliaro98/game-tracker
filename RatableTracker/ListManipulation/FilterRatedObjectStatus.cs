using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation
{
    public class FilterRatedObjectStatus : FilterRatedObjects
    {
        public new TrackerModuleScoreStatuses Module { get { return (TrackerModuleScoreStatuses)base.Module; } set { base.Module = value; } }

        public FilterRatedObjectStatus() : base() { }

        public FilterRatedObjectStatus(TrackerModuleScoreStatuses module, SettingsScore settings) : base(module, settings) { }
    }
}
