using RatableTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    [FilterOption(typeof(RankedObject))]
    public class FilterOptionModelScore : FilterOptionNumericBase<RankedObject>
    {
        public override string Name => "Score";

        public FilterOptionModelScore() : base() { }

        protected override double GetComparisonValue(RankedObject obj)
        {
            return obj.ScoreDisplay;
        }
    }
}
