using RatableTracker.Exceptions;
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
    public class SortRatedObjects : SortRankedObjects
    {
        public const int SORT_Score = 20;

        [XmlIgnore][JsonIgnore] public new TrackerModuleScores Module { get { return (TrackerModuleScores)base.Module; } set { base.Module = value; } }
        [XmlIgnore][JsonIgnore] public new SettingsScore Settings { get { return (SettingsScore)base.Settings; } set { base.Settings = value; } }

        public SortRatedObjects() : base() { }

        public SortRatedObjects(TrackerModuleScores module, SettingsScore settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            switch (sortMethod)
            {
                case SORT_Score:
                    sortFunction = obj => ((RatedObject)obj).ScoreDisplay;
                    break;
            }
            return sortFunction;
        }
    }
}
