using RatableTracker.Model;
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
    public class SortRatedObjectStatus : SortRatedObjects
    {
        public const int SORT_Status = 10;

        [XmlIgnore][JsonIgnore] public new TrackerModuleScoreStatuses Module { get { return (TrackerModuleScoreStatuses)base.Module; } set { base.Module = value; } }

        public SortRatedObjectStatus() : base() { }

        public SortRatedObjectStatus(TrackerModuleScoreStatuses module, SettingsScore settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            switch (sortMethod)
            {
                case SORT_Status:
                    sortFunction = obj => ((RatedObjectStatus)obj).StatusExtension.Status == null ? "" : ((RatedObjectStatus)obj).StatusExtension.Status.Name;
                    break;
            }
            return sortFunction;
        }
    }
}
