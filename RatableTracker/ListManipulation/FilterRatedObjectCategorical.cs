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
    public class FilterRatedObjectCategorical : FilterRatedObjects
    {
        [XmlIgnore][JsonIgnore] public new TrackerModuleScoreCategorical Module { get { return (TrackerModuleScoreCategorical)base.Module; } set { base.Module = value; } }

        public FilterRatedObjectCategorical() : base() { }

        public FilterRatedObjectCategorical(TrackerModuleScoreCategorical module, SettingsScore settings) : base(module, settings) { }
    }
}
