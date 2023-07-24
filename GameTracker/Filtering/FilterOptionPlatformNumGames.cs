using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformNumGames : FilterOptionNumericBase<Platform>
    {
        public override string Name => "# Games";
        public override FilterNumberFormat NumberFormat => FilterNumberFormat.Integer;

        public FilterOptionPlatformNumGames() : base() { }

        protected override double GetComparisonValue(Platform obj)
        {
            return obj.NumGames;
        }
    }
}
