using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public CategoryValue(CategoryExtensionModule module) : this(module, null) { }

        public CategoryValue(CategoryExtensionModule module, RatingCategory ratingCategory)
        {
            this.module = module;
            _category = ratingCategory.UniqueID;
        }

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("Category", new ValueContainer(_category));
            sr.SaveValue("PointValue", new ValueContainer(PointValue));
            return sr;
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "Category":
                        _category = sr.GetValue(key).GetUniqueID();
                        break;
                    case "PointValue":
                        PointValue = sr.GetValue(key).GetDouble();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
