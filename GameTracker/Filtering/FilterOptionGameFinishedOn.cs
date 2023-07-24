using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameFinishedOn : FilterOptionDateBase<GameObject>
    {
        public override string Name => "Finished On";

        public FilterOptionGameFinishedOn() : base() { }

        protected override DateTime GetComparisonValue(GameObject obj)
        {
            return obj.IsUnfinishable ? obj.StartedOn : obj.FinishedOn;
        }
    }
}
