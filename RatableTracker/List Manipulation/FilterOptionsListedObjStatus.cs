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
    public class FilterOptionsListedObjStatus<TListedObj, TModule, TRange, TSettings, TStatus> : FilterOptionsListedObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : ListedObjectStatus
        where TModule : RankingModuleStatus<TListedObj, TRange, TSettings, TStatus>
        where TRange : ScoreRange
        where TSettings : Settings, new()
        where TStatus : Status
    {
        public FilterOptionsListedObjStatus() : base() { }
    }
}
