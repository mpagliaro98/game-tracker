using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameAcquiredOn : FilterOptionDateBase<GameObject>
    {
        public override string Name => "Acquired On";

        public FilterOptionGameAcquiredOn() : base() { }

        protected override DateTime GetComparisonValue(GameObject obj)
        {
            return obj.AcquiredOn;
        }
    }
}
