using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameCompilations : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Compilations";
        public override string DisplayText => "Only compilations";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => obj.IsCompilation;
        }
    }
}
