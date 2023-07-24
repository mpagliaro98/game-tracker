using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGamePartOfCompilation : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Part of Compilation";
        public override string DisplayText => "Only games that are part of a compilation";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => obj.IsPartOfCompilation;
        }
    }
}
