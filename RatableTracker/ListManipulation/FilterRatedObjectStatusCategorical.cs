using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation
{
    public class FilterRatedObjectStatusCategorical : FilterRatedObjectStatus
    {
        public new TrackerModuleScoreStatusCategorical Module { get; set; }

        public FilterRatedObjectStatusCategorical() : base() { }

        public FilterRatedObjectStatusCategorical(TrackerModuleScoreStatusCategorical module, SettingsScore settings) : base(module, settings) { }
    }
}
