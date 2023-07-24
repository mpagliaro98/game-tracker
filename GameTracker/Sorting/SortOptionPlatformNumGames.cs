using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Sorting
{
    [SortOption(typeof(Platform))]
    public class SortOptionPlatformNumGames : SortOptionSimpleBase<Platform>
    {
        public override string Name => "# Games";

        public SortOptionPlatformNumGames() : base() { }

        protected override object GetSortValue(Platform obj)
        {
            return obj.NumGames;
        }
    }
}
