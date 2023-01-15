using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class CategoryExtensionModule
    {
        public virtual int LimitRatingCategories => 10;

        protected IList<RatingCategory> RatingCategories => new List<RatingCategory>();

        protected readonly TrackerModuleScores module;

        public CategoryExtensionModule(TrackerModuleScores module)
        {
            this.module = module;
        }

        public IList<RatingCategory> GetRatingCategoryList()
        {
            return RatingCategories;
        }

        public void AddRatingCategory(RatingCategory ratingCategory)
        {
            // TODO validate, add, save (limit)
        }

        public void DeleteRatingCategory(RatingCategory ratingCategory)
        {
            // TODO delete, save
        }
    }
}
