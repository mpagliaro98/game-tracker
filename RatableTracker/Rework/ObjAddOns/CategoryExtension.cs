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
    public class CategoryExtension
    {
        private IList<CategoryValue> _categoryValues = new List<CategoryValue>();
        protected IList<CategoryValue> CategoryValues { get { return _categoryValues; } }

        public bool IgnoreCategories { get; set; } = false;

        private readonly CategoryExtensionModule module;
        private readonly SettingsScore settings;

        public CategoryExtension(CategoryExtensionModule module, SettingsScore settings)
        {
            this.module = module;
            this.settings = settings;

            foreach (RatingCategory category in module.GetRatingCategoryList())
            {
                var categoryValue = new CategoryValue(module, category)
                {
                    PointValue = settings.MinScore
                };
                CategoryValues.Add(categoryValue);
            }
        }

        public IList<CategoryValue> GetCategoryValues()
        {
            return CategoryValues;
        }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation sr)
        {
            sr.SaveValue("IgnoreCategories", new ValueContainer(IgnoreCategories));
            sr.SaveValue("CategoryValues", new ValueContainer(_categoryValues));
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "IgnoreCategories":
                        IgnoreCategories = sr.GetValue(key).GetBool();
                        break;
                    case "CategoryValues":
                        _categoryValues = sr.GetValue(key).GetISavableList(() => new CategoryValue(module)).ToList();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
