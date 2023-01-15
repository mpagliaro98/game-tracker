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
        protected IList<CategoryValue> CategoryValues => new List<CategoryValue>();
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
    }
}
