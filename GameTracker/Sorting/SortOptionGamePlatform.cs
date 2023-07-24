using RatableTracker.ListManipulation.Sorting;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(GameObject))]
    public class SortOptionGamePlatform : SortOptionSimpleBase<GameObject>
    {
        public override string Name => "Platform";

        public SortOptionGamePlatform() : base() { }

        protected override object GetSortValue(GameObject obj)
        {
            return obj.Platform == null ? "" : obj.Platform.Name.CleanForSorting();
        }
    }
}
