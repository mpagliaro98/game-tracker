using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(Platform))]
    public class FilterOptionPlatformAcquiredYear : FilterOptionNumericBase<Platform>
    {
        public override string Name => "Acquired Year";
        public override FilterNumberFormat NumberFormat => FilterNumberFormat.Integer;

        public FilterOptionPlatformAcquiredYear() : base() { }

        protected override double GetComparisonValue(Platform obj)
        {
            return obj.AcquiredYear;
        }
    }
}
