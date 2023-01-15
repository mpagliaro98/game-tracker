using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class CategoryValue : ISavable
    {
        private UniqueID _category = new UniqueID(false);
        public RatingCategory RatingCategory
        {
            get
            {
                if (!_category.HasValue()) return null;
                return TrackerModule.FindObjectInList(module.GetRatingCategoryList(), _category);
            }
        }

        public double PointValue { get; set; } = 0;

        private readonly CategoryExtensionModule module;

        public CategoryValue(CategoryExtensionModule module, RatingCategory ratingCategory)
        {
            this.module = module;
            _category = ratingCategory.UniqueID;
        }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation<ValueContainer> sr)
        {
            // TODO load into representation (use attributes?)
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation<ValueContainer> sr)
        {
            // TODO get from representation (use attributes?)
        }
    }
}
