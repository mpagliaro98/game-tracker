using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(GameObject))]
    public class SortOptionGameStartedOn : SortOptionSimpleBase<GameObject>
    {
        public override string Name => "Started On";

        public SortOptionGameStartedOn() : base() { }

        protected override object GetSortValue(GameObject obj)
        {
            return obj.StartedOn;
        }
    }
}
