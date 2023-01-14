using RatableTracker.Framework.ModuleHierarchy;
using RatableTracker.Framework;
using RatableTracker.Framework.ObjectHierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.List_Manipulation
{
    public class SortOptionsListedObjStatus<TListedObj, TModule, TRange, TSettings, TStatus> : SortOptionsListedObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : ListedObjectStatus
        where TModule : RankingModuleStatus<TListedObj, TRange, TSettings, TStatus>
        where TRange : ScoreRange
        where TSettings : Settings, new()
        where TStatus : Status
    {
        public const int SORT_Status = 10;

        public SortOptionsListedObjStatus(int sortMethod, SortMode sortMode) : base(sortMethod, sortMode) { }

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
