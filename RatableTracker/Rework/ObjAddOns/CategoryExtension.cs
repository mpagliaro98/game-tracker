using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
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
        protected IList<CategoryValue> CategoryValues { get; private set; } = new List<CategoryValue>();

        public bool IgnoreCategories { get; set; } = false;

        protected readonly CategoryExtensionModule module;
        protected readonly SettingsScore settings;

        public CategoryExtension(CategoryExtensionModule module, SettingsScore settings)
        {
            this.module = module;
            this.settings = settings;

            foreach (RatingCategory category in module.GetRatingCategoryList())
            {
                var categoryValue = new CategoryValue(module, settings, category);
                CategoryValues.Add(categoryValue);
            }
        }

        public IList<CategoryValue> GetCategoryValues()
        {
            return CategoryValues;
        }

        public virtual double CalculateTotalCategoryScore()
        {
            double sumOfWeights = module.GetRatingCategoryList().Select(cat => cat.Weight).Sum();
            double total = GetCategoryValues().Select(cv => (cv.RatingCategory.Weight / sumOfWeights) * cv.PointValue).Sum();
            return total;
        }

        public virtual void Validate()
        {
            foreach (CategoryValue categoryValue in GetCategoryValues())
            {
                if (categoryValue.PointValue < settings.MinScore || categoryValue.PointValue > settings.MaxScore)
                    throw new ValidationException(categoryValue.RatingCategory.Name + " score must be between " + settings.MinScore.ToString() + " and " + settings.MaxScore.ToString(), categoryValue.PointValue);
            }
        }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            if (type == typeof(RatingCategory))
            {
                ICollection<CategoryValue> toDelete = new List<CategoryValue>();
                foreach (CategoryValue cv in CategoryValues)
                {
                    if (obj.Equals(cv.RatingCategory))
                    {
                        toDelete.Add(cv);
                    }
                }
                foreach (CategoryValue cv in toDelete)
                {
                    CategoryValues.Remove(cv);
                }
                return toDelete.Count > 0;
            }
            return false;
        }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation sr)
        {
            sr.SaveValue("IgnoreCategories", new ValueContainer(IgnoreCategories));
            sr.SaveValue("CategoryValues", new ValueContainer(CategoryValues));
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
                        CategoryValues = sr.GetValue(key).GetSavableObjectList(() => new CategoryValue(module, settings)).ToList();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
