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
        public new TrackerModuleScores Module { get { return (TrackerModuleScores)base.Module; } set { base.Module = value; } }
        public new SettingsScore Settings { get { return (SettingsScore)base.Settings; } set { base.Settings = value; } }

        public FilterRatedObjects() : base() { }

        public FilterRatedObjects(TrackerModuleScores module, SettingsScore settings) : base(module, settings) { }
    }
}
