using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformPercFinished : FilterOptionNumericBase<Platform>
    {
        public override string Name => "% Games Finished";
        public override FilterNumberFormat NumberFormat => FilterNumberFormat.Percentage;

        public FilterOptionPlatformPercFinished() : base() { }

        protected override double GetComparisonValue(Platform obj)
        {
            return obj.FinishPercent;
        }
    }
}
