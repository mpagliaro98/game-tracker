using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RatableTracker.ListManipulation
{
    [Serializable]
    public class FilterRatedObjectStatusCategorical : FilterRatedObjectStatus
    {
        [XmlIgnore][JsonIgnore] public new TrackerModuleScoreStatusCategorical Module { get { return (TrackerModuleScoreStatusCategorical)base.Module; } set { base.Module = value; } }

        public FilterRatedObjectStatusCategorical() : base() { }

        public FilterRatedObjectStatusCategorical(TrackerModuleScoreStatusCategorical module, SettingsScore settings) : base(module, settings) { }
    }
}
