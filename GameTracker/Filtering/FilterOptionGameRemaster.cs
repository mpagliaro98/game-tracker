using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameRemaster : FilterOptionBooleanBase<GameObject>
    {
        public override string Name => "Remaster/Re-release";
        public override string DisplayText => "Only remasters and re-releases";

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => obj.IsRemaster;
        }
    }
}
