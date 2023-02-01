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
    public class SortRankedObjectStatus : SortRankedObjects
    {
        public const int SORT_Status = 10;

        [XmlIgnore][JsonIgnore] public new TrackerModuleStatuses Module { get { return (TrackerModuleStatuses)base.Module; } set { base.Module = value; } }

        public SortRankedObjectStatus() : base() { }

        public SortRankedObjectStatus(TrackerModuleStatuses module, Settings settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            switch (sortMethod)
            {
                case SORT_Status:
                    sortFunction = obj => ((RankedObjectStatus)obj).StatusExtension.Status == null ? "" : ((RankedObjectStatus)obj).StatusExtension.Status.Name;
                    break;
            }
            return sortFunction;
        }
    }
}
