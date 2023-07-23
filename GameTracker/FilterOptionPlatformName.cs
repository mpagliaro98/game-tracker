using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformName : FilterOptionTextBase<Platform>
    {
        public override string Name => "Name";

        public FilterOptionPlatformName() : base() { }

        protected override string GetComparisonValue(Platform obj)
        {
            return obj.Name;
        }
    }
}
