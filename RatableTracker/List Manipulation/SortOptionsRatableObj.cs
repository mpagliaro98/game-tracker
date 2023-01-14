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
    public class SortOptionsRatableObj<TListedObj, TModule, TRange, TSettings> : SortOptionsListedObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : RatableObject
        where TModule : RatingModule<TListedObj, TRange, TSettings>
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
    {
        public const int SORT_Score = 20;

        public SortOptionsRatableObj(int sortMethod, SortMode sortMode) : base(sortMethod, sortMode) { }

        protected override Func<TListedObj, object> GetSortFunction(int sortMethod, TModule rm)
        {
            Func<TListedObj, object> sortFunction = base.GetSortFunction(sortMethod, rm);
            switch (sortMethod)
            {
                case SORT_Score:
                    sortFunction = obj => rm.GetScoreOfObject(obj);
                    break;
            }
            return sortFunction;
        }
    }
}
