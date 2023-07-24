using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformReleaseYear : FilterOptionNumericBase<Platform>
    {
        public override string Name => "Release Year";
        public override FilterNumberFormat NumberFormat => FilterNumberFormat.Integer;

        public FilterOptionPlatformReleaseYear() : base() { }

        protected override double GetComparisonValue(Platform obj)
        {
            return obj.ReleaseYear;
        }
    }
}
