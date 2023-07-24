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
    public class SortOptionGamePlatformPlayedOn : SortOptionSimpleBase<GameObject>
    {
        public override string Name => "Platform Played On";

        public SortOptionGamePlatformPlayedOn() : base() { }

        protected override object GetSortValue(GameObject obj)
        {
            return obj.PlatformPlayedOn == null ? "" : obj.PlatformPlayedOn.Name.CleanForSorting();
        }
    }
}
