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
    public class FilterRatedObjectStatus : FilterRatedObjects
    {
        [XmlIgnore][JsonIgnore] public new TrackerModuleScoreStatuses Module { get { return (TrackerModuleScoreStatuses)base.Module; } set { base.Module = value; } }

        public FilterRatedObjectStatus() : base() { }

        public FilterRatedObjectStatus(TrackerModuleScoreStatuses module, SettingsScore settings) : base(module, settings) { }
    }
}
