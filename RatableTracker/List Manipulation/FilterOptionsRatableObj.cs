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
    public class FilterOptionsRatableObj<TListedObj, TModule, TRange, TSettings> : FilterOptionsListedObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : RatableObject
        where TModule : RatingModule<TListedObj, TRange, TSettings>
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
    {
        public FilterOptionsRatableObj() : base() { }
    }
}
