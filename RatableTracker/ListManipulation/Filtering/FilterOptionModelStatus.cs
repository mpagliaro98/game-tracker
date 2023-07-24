using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    [FilterOption(typeof(IModelObjectStatus))]
    public class FilterOptionModelStatus : FilterOptionListBase<IModelObjectStatus>
    {
        public override string Name => "Status";
        public override List<KeyValuePair<UniqueID, string>> ListValues => new List<KeyValuePair<UniqueID, string>>()
        {
            new KeyValuePair<UniqueID, string>(UniqueID.BlankID(), "No Status")
        }.Concat(((IModuleStatus)Module).StatusExtension.GetStatusList().OrderBy(p => p.Name.CleanForSorting()).Select(p => new KeyValuePair<UniqueID, string>(p.UniqueID, p.Name))).ToList();

        public override Func<IModelObjectStatus, bool> GenerateFilterExpression()
        {
            return (obj) => FilterID.HasValue() ? obj.StatusExtension.Status != null && obj.StatusExtension.Status.UniqueID.Equals(FilterID) : obj.StatusExtension.Status == null;
        }
    }
}
