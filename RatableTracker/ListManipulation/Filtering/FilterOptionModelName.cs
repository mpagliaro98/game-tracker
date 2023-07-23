using RatableTracker.Exceptions;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    [FilterOption(typeof(RankedObject))]
    public class FilterOptionModelName : FilterOptionTextBase<RankedObject>
    {
        public override string Name => "Name";

        public FilterOptionModelName() : base() { }

        protected override string GetComparisonValue(RankedObject obj)
        {
            return obj.Name;
        }
    }
}
