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
    public class FilterOptionsRatableObjCategorical<TListedObj, TModule, TRange, TSettings, TRatingCat> : FilterOptionsRatableObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : RatableObjectCategorical
        where TModule : RatingModuleCategorical<TListedObj, TRange, TSettings, TRatingCat>
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
        where TRatingCat : RatingCategory
    {
        public FilterOptionsRatableObjCategorical() : base() { }
    }
}
