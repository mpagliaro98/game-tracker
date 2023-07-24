using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(GameObject))]
    public class SortOptionGameFinishedOn : SortOptionSimpleBase<GameObject>
    {
        public override string Name => "Finished On";

        public SortOptionGameFinishedOn() : base() { }

        protected override object GetSortValue(GameObject obj)
        {
            return obj.IsUnfinishable ? obj.StartedOn : obj.FinishedOn;
        }
    }
}
