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
    public class FilterRankedObjectStatus : FilterRankedObjects
    {
        [XmlIgnore][JsonIgnore] public new TrackerModuleStatuses Module { get { return (TrackerModuleStatuses)base.Module; } set { base.Module = value; } }

        public FilterRankedObjectStatus() : base() { }

        public FilterRankedObjectStatus(TrackerModuleStatuses module, Settings settings) : base(module, settings) { }
    }
}
