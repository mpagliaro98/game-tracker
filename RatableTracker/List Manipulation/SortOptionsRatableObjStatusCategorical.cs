using RatableTracker.Framework.ModuleHierarchy;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.List_Manipulation
{
    public class SortOptionsRatableObjStatusCategorical<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat> : SortOptionsRatableObjStatus<TListedObj, TModule, TRange, TSettings, TStatus>
        where TListedObj : RatableObjectStatusCategorical
        where TModule : RatingModuleStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
        where TStatus : Status
        where TRatingCat : RatingCategory
    {
        public const int SORT_CategoryStart = 5100;

        public SortOptionsRatableObjStatusCategorical(int sortMethod, SortMode sortMode) : base(sortMethod, sortMode) { }

        protected override Func<TListedObj, object> GetSortFunction(int sortMethod, TModule rm)
        {
            Func<TListedObj, object> sortFunction = base.GetSortFunction(sortMethod, rm);
            if (sortMethod >= SORT_CategoryStart && sortMethod < SORT_CategoryStart + rm.LimitRatingCategories)
            {
                TRatingCat cat = rm.RatingCategories.ElementAt(sortMethod - SORT_CategoryStart);
                sortFunction = obj => rm.GetScoreOfCategory(obj, cat);
            }
            return sortFunction;
        }
    }
}
