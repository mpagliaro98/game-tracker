using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(Platform))]
    public class SortOptionPlatformReleaseYear : SortOptionSimpleBase<Platform>
    {
        public override string Name => "Release Year";

        public SortOptionPlatformReleaseYear() : base() { }

        protected override object GetSortValue(Platform obj)
        {
            return obj.ReleaseYear;
        }
    }
}
