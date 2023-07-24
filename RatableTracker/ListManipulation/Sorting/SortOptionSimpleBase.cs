using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Sorting
{
    public abstract class SortOptionSimpleBase<T> : SortOptionBase, ISortOptionAction<T>
    {
        public SortOptionSimpleBase() : base() { }

        public Func<T, object> GenerateSortExpression()
        {
            return obj => GetSortValue(obj);
        }

        protected abstract object GetSortValue(T obj);
    }
}
