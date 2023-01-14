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
    public class SortOptionsRatableObjStatus<TListedObj, TModule, TRange, TSettings, TStatus> : SortOptionsRatableObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : RatableObjectStatus
        where TModule : RatingModuleStatus<TListedObj, TRange, TSettings, TStatus>
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
        where TStatus : Status
    {
        public const int SORT_Status = 10;

        public SortOptionsRatableObjStatus(int sortMethod, SortMode sortMode) : base(sortMethod, sortMode) { }

        protected override Func<TListedObj, object> GetSortFunction(int sortMethod, TModule rm)
        {
            Func<TListedObj, object> sortFunction = base.GetSortFunction(sortMethod, rm);
            switch (sortMethod)
            {
                case SORT_Status:
                    sortFunction = obj => obj.RefStatus.HasReference() ? rm.FindStatus(obj.RefStatus).Name : "";
                    break;
            }
            return sortFunction;
        }
    }
}
