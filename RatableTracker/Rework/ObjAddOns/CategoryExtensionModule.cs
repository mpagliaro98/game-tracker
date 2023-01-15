using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class CategoryExtensionModule
    {
        protected IList<RatingCategory> RatingCategories => new List<RatingCategory>();

        public CategoryExtensionModule() { }

        public IList<RatingCategory> GetRatingCategoryList()
        {
            return RatingCategories;
        }
    }
}
