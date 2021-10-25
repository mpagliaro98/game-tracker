using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IObjectCategorical
    {
        IEnumerable<RatingCategoryValue> CategoryValues { get; set; }
        bool IgnoreCategories { get; set; }
        void UpdateRatingCategoryValues(Func<RatingCategoryValue, bool> where, Action<RatingCategoryValue> action);
        void DeleteRatingCategoryValues(Predicate<RatingCategoryValue> where);
    }
}
