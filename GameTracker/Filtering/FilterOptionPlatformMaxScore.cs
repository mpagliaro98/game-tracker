using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformMaxScore : FilterOptionNumericBase<Platform>
    {
        public override string Name => "Highest Score";

        public FilterOptionPlatformMaxScore() : base() { }

        protected override double GetComparisonValue(Platform obj)
        {
            return obj.HighestScore;
        }
    }
}
