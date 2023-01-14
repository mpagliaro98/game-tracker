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
    public class FilterOptionsGameTemplate<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat> : FilterOptionsRatableObjStatusCategorical<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat>
        where TListedObj : RatableGame, new()
        where TModule : RatingModuleGameTemplate<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TRange : ScoreRange, new()
        where TSettings : SettingsScore, new()
        where TStatus : CompletionStatus, new()
        where TRatingCat : RatingCategory, new()
    {
        protected bool showCompilations = false;

        public FilterOptionsGameTemplate(bool showCompilations) : base()
        {
            this.showCompilations = showCompilations;
        }

        public override IEnumerable<TListedObj> ApplyFilters(IEnumerable<TListedObj> list, TModule rm)
        {
            // need to use RatableGame as type instead of TListedObj, otherwise overload for Concat fails
            IEnumerable<RatableGame> newList = base.ApplyFilters(list, rm);
            if (showCompilations)
            {
                newList = newList.Where(rg => !rg.IsPartOfCompilation).Concat(rm.GameCompilations);
            }
            return newList.Cast<TListedObj>();
        }
    }
}
