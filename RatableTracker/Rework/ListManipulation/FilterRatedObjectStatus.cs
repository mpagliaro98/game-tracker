using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class FilterRatedObjectStatus : FilterRatedObjects
    {
        public new TrackerModuleScoreStatuses Module { get; set; }

        public FilterRatedObjectStatus() : base() { }

        public FilterRatedObjectStatus(TrackerModuleScoreStatuses module, SettingsScore settings) : base(module, settings) { }
    }
}
