using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject), InAutoList = false)]
    public class FilterOptionGameShowCompilations : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Show Compilations";
        public override string DisplayText => "Compilations & games not in a compilation";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => obj.IsCompilation || !obj.IsCompilation && !obj.IsPartOfCompilation;
        }
    }
}
