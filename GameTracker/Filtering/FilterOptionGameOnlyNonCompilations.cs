using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject), InAutoList = false)]
    internal class FilterOptionGameOnlyNonCompilations : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Hide Compilations";
        public override string DisplayText => "Only individual games";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => !obj.IsCompilation;
        }
    }
}
