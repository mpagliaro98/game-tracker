using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation
{
    public class FilterRatedObjects : FilterRankedObjects
    {
        public new TrackerModuleScores Module { get; set; }
        public new SettingsScore Settings { get; set; }

        public FilterRatedObjects() : base() { }

        public FilterRatedObjects(TrackerModuleScores module, SettingsScore settings) : base(module, settings) { }
    }
}
