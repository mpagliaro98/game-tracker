using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameFinishable : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Finishable";
        public override string DisplayText => "Games with a start/end";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => !obj.IsUnfinishable;
        }
    }
}
