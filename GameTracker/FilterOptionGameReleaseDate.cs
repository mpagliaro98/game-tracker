using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameReleaseDate : FilterOptionDateBase<GameObject>
    {
        public override string Name => "Release Date";

        public FilterOptionGameReleaseDate() : base() { }

        protected override DateTime GetComparisonValue(GameObject obj)
        {
            return obj.ReleaseDate;
        }
    }
}
