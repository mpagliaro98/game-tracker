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
    public class FilterOptionsRatableObjStatusCategorical<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat> : FilterOptionsRatableObjStatus<TListedObj, TModule, TRange, TSettings, TStatus>
        where TListedObj : RatableObjectStatusCategorical
        where TModule : RatingModuleStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
        where TStatus : Status
        where TRatingCat : RatingCategory
    {
        public FilterOptionsRatableObjStatusCategorical() : base() { }
    }
}
