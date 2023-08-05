using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameFinished : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Finished";
        public override string DisplayText => "Games that have been finished";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => obj.IsFinished;
        }
    }
}
