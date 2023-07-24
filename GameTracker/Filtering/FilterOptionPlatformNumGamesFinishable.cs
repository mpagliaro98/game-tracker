using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformNumGamesFinishable : FilterOptionNumericBase<Platform>
    {
        public override string Name => "# Games with Start/End";
        public override FilterNumberFormat NumberFormat => FilterNumberFormat.Integer;

        public FilterOptionPlatformNumGamesFinishable() : base() { }

        protected override double GetComparisonValue(Platform obj)
        {
            return obj.NumGamesFinishable;
        }
    }
}
