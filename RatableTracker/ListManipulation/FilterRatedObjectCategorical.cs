using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation
{
    public class FilterRatedObjectCategorical : FilterRatedObjects
    {
        public new TrackerModuleScoreCategorical Module { get { return (TrackerModuleScoreCategorical)base.Module; } set { base.Module = value; } }

        public FilterRatedObjectCategorical() : base() { }

        public FilterRatedObjectCategorical(TrackerModuleScoreCategorical module, SettingsScore settings) : base(module, settings) { }
    }
}
