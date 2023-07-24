using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameStartedOn : FilterOptionDateBase<GameObject>
    {
        public override string Name => "Started On";

        public FilterOptionGameStartedOn() : base() { }

        protected override DateTime GetComparisonValue(GameObject obj)
        {
            return obj.StartedOn;
        }
    }
}
