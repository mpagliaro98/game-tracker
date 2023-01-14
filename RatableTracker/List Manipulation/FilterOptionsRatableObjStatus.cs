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
    public class FilterOptionsRatableObjStatus<TListedObj, TModule, TRange, TSettings, TStatus> : FilterOptionsRatableObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : RatableObjectStatus
        where TModule : RatingModuleStatus<TListedObj, TRange, TSettings, TStatus>
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
        where TStatus : Status
    {
        public FilterOptionsRatableObjStatus() : base() { }
    }
}
