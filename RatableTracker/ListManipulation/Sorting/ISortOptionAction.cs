using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Sorting
{
    public interface ISortOptionAction<in T>
    {
        Func<T, object> GenerateSortExpression();
    }
}
