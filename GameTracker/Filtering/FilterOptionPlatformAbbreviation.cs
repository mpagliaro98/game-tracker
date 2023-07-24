using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformAbbreviation : FilterOptionTextBase<Platform>
    {
        public override string Name => "Abbreviation";

        public FilterOptionPlatformAbbreviation() : base() { }

        protected override string GetComparisonValue(Platform obj)
        {
            return obj.Abbreviation;
        }
    }
}
