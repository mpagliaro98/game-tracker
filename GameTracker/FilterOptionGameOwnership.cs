using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameOwnership : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Ownership";
        public override string DisplayText => "Games that you own";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => !obj.IsNotOwned;
        }
    }
}
