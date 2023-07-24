using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameTimeSpent : FilterOptionTextBase<GameObject>
    {
        public override string Name => "Time Spent";

        public FilterOptionGameTimeSpent() : base() { }

        protected override string GetComparisonValue(GameObject obj)
        {
            return obj.TimeSpent;
        }
    }
}
