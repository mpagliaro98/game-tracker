using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(Platform))]
    public class SortOptionPlatformAvgScore : SortOptionSimpleBase<Platform>
    {
        public override string Name => "Average Score";

        public SortOptionPlatformAvgScore() : base() { }

        protected override object GetSortValue(Platform obj)
        {
            return obj.AverageScore;
        }
    }
}
