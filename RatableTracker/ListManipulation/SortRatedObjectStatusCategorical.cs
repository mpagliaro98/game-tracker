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
    public class SortRatedObjectStatusCategorical : SortRatedObjectStatus
    {
        public const int SORT_CategoryStart = 5100;

        [XmlIgnore][JsonIgnore] public new TrackerModuleScoreStatusCategorical Module { get { return (TrackerModuleScoreStatusCategorical)base.Module; } set { base.Module = value; } }

        public SortRatedObjectStatusCategorical() : base() { }

        public SortRatedObjectStatusCategorical(TrackerModuleScoreStatusCategorical module, SettingsScore settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            if (sortMethod >= SORT_CategoryStart && sortMethod < SORT_CategoryStart + Module.CategoryExtension.LimitRatingCategories)
            {
                RatingCategory cat = Module.CategoryExtension.GetRatingCategoryList().ElementAt(sortMethod - SORT_CategoryStart);
                sortFunction = obj => ((RatedObjectStatusCategorical)obj).CategoryExtension.ScoreOfCategoryDisplay(cat);
            }
            return sortFunction;
        }
    }
}
