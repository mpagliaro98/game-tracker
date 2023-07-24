using RatableTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    [FilterOption(typeof(RankedObject))]
    public class FilterOptionModelComment : FilterOptionTextBase<RankedObject>
    {
        public override string Name => "Comment";

        public FilterOptionModelComment() : base() { }

        protected override string GetComparisonValue(RankedObject obj)
        {
            return obj.Comment;
        }
    }
}
