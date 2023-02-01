using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
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
    public class SortRatedObjectCategorical : SortRatedObjects
    {
        public const int SORT_CategoryStart = 5100;

        [XmlIgnore][JsonIgnore] public new TrackerModuleScoreCategorical Module { get { return (TrackerModuleScoreCategorical)base.Module; } set { base.Module = value; } }

        public SortRatedObjectCategorical() : base() { }

        public SortRatedObjectCategorical(TrackerModuleScoreCategorical module, SettingsScore settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            if (sortMethod >= SORT_CategoryStart && sortMethod < SORT_CategoryStart + Module.CategoryExtension.LimitRatingCategories)
            {
                RatingCategory cat = Module.CategoryExtension.GetRatingCategoryList().ElementAt(sortMethod - SORT_CategoryStart);
                sortFunction = obj => ((RatedObjectCategorical)obj).CategoryExtension.ScoreOfCategoryDisplay(cat);
            }
            return sortFunction;
        }
    }
}
