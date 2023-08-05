using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    [FilterOption(typeof(IModelObjectStatus))]
    public class FilterOptionModelShowScore : FilterOptionBooleanBase<IModelObjectStatus>
    {
        public override string Name => "Showing Score";
        public override string DisplayText => "Items that have a visible score";

        public override Func<IModelObjectStatus, bool> GenerateFilterExpression()
        {
            return (obj) => obj.ShowScore;
        }
    }
}
