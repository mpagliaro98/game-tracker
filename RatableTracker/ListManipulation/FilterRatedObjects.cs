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
    public class FilterRatedObjects : FilterRankedObjects
    {
        [XmlIgnore][JsonIgnore] public new TrackerModuleScores Module { get { return (TrackerModuleScores)base.Module; } set { base.Module = value; } }
        [XmlIgnore][JsonIgnore] public new SettingsScore Settings { get { return (SettingsScore)base.Settings; } set { base.Settings = value; } }

        public FilterRatedObjects() : base() { }

        public FilterRatedObjects(TrackerModuleScores module, SettingsScore settings) : base(module, settings) { }
    }
}
