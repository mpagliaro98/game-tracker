using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(GameObject))]
    public class SortOptionGameDaysToFinish : SortOptionSimpleBase<GameObject>
    {
        public override string Name => "Days to Finish";

        public SortOptionGameDaysToFinish() : base() { }

        protected override object GetSortValue(GameObject obj)
        {
            if (obj.IsUnfinishable)
                return 0;
            else
                return obj.StartedOn > DateTime.MinValue && obj.FinishedOn > DateTime.MinValue ? (obj.FinishedOn - obj.StartedOn).Days : 0;
        }
    }
}
