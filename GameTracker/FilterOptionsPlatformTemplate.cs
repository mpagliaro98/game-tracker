using GameTracker.Model;
using RatableTracker.Framework;
using RatableTracker.List_Manipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class FilterOptionsPlatformTemplate<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat>
        where TListedObj : RatableGame, new()
        where TModule : RatingModuleGameTemplate<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TRange : ScoreRange, new()
        where TSettings : SettingsScore, new()
        where TStatus : CompletionStatus, new()
        where TRatingCat : RatingCategory, new()
    {
        public FilterOptionsPlatformTemplate() { }

        public virtual IEnumerable<Platform> ApplyFilters(IEnumerable<Platform> list, TModule rm)
        {
            return list;
        }
    }
}
