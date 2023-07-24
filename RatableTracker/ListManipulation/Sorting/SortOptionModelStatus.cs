using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Sorting
{
    [SortOption(typeof(IModelObjectStatus))]
    public class SortOptionModelStatus : SortOptionSimpleBase<IModelObjectStatus>
    {
        public override string Name => "Status";

        public SortOptionModelStatus() : base() { }

        protected override object GetSortValue(IModelObjectStatus obj)
        {
            return obj.StatusExtension.Status == null ? "" : obj.StatusExtension.Status.Name.CleanForSorting();
        }
    }
}
