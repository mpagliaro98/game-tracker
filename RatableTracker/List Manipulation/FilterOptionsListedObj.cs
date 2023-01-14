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
    public class FilterOptionsListedObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : ListedObject
        where TModule : RankingModule<TListedObj, TRange, TSettings>
        where TRange : ScoreRange
        where TSettings : Settings, new()
    {
        public FilterOptionsListedObj() { }

        public virtual IEnumerable<TListedObj> ApplyFilters(IEnumerable<TListedObj> list, TModule rm)
        {
            return list;
        }
    }
}
