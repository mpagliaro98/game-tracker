using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Sorting
{
    [SortOption(typeof(RankedObject))]
    public class SortOptionModelName : SortOptionSimpleBase<RankedObject>
    {
        public override string Name => "Name";

        public SortOptionModelName() : base() { }

        protected override object GetSortValue(RankedObject obj)
        {
            return obj.Name.CleanForSorting();
        }
    }
}
