using RatableTracker.Exceptions;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.ListManipulation.Filtering
{
    public abstract class FilterOptionListBase<T> : FilterOptionBase, IFilterOptionList, IFilterOptionAction<T>
    {
        public override FilterType FilterType => FilterType.List;
        public abstract List<KeyValuePair<UniqueID, string>> ListValues { get; }

        protected UniqueID FilterID { get; private set; }

        public FilterOptionListBase() : base() { }

        protected internal override void ValidateFilterValues(object filterValues)
        {
            // validate filter values, throw exception if invalid, then fill protected properties with filter values
            if (filterValues == null)
                FilterID = UniqueID.BlankID();
            else if (filterValues is not string stringValue || !UniqueID.TryParse(stringValue, out UniqueID uniqueID))
                throw new ListManipulationException("Invalid filter value, list filters expect one unique ID", filterValues);
            else
                FilterID = uniqueID;
        }

        public abstract Func<T, bool> GenerateFilterExpression();
    }
}
