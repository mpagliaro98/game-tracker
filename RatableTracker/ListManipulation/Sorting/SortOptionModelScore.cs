using RatableTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Sorting
{
    [SortOption(typeof(RankedObject))]
    public class SortOptionModelScore : SortOptionSimpleBase<RankedObject>
    {
        public override string Name => "Score";

        public SortOptionModelScore() : base() { }

        protected override object GetSortValue(RankedObject obj)
        {
            return obj.ScoreDisplay;
        }
    }
}
