using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformMinScore : FilterOptionNumericBase<Platform>
    {
        public override string Name => "Lowest Score";

        public FilterOptionPlatformMinScore() : base() { }

        protected override double GetComparisonValue(Platform obj)
        {
            return obj.LowestScore;
        }
    }
}
