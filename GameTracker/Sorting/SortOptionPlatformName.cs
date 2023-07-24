using RatableTracker.ListManipulation.Sorting;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(Platform))]
    public class SortOptionPlatformName : SortOptionSimpleBase<Platform>
    {
        public override string Name => "Name";

        public SortOptionPlatformName() : base() { }

        protected override object GetSortValue(Platform obj)
        {
            return obj.Name.CleanForSorting();
        }
    }
}
