using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    public abstract class FilterOptionBooleanBase<T> : FilterOptionBase, IFilterOptionBoolean, IFilterOptionAction<T>
    {
        public override FilterType FilterType => FilterType.Boolean;
        public abstract string DisplayText { get; }

        public FilterOptionBooleanBase() : base() { }

        protected internal override void ValidateFilterValues(object filterValues)
        {
            // no validation or filter values
        }

        public abstract Func<T, bool> GenerateFilterExpression();
    }
}
