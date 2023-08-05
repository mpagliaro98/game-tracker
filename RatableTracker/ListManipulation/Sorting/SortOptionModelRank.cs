using RatableTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Sorting
{
    [SortOption(typeof(RankedObject))]
    public class SortOptionModelRank : SortOptionSimpleBase<RankedObject>
    {
        public override string Name => "Rank";

        public SortOptionModelRank() : base() { }

        protected override object GetSortValue(RankedObject obj)
        {
            return obj.Rank;
        }
    }
}
