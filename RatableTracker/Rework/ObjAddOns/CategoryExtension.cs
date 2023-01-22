using Microsoft.VisualBasic;
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
        public IList<CategoryValue> CategoryValues { get; private set; } = new List<CategoryValue>();

        public virtual double TotalScoreFromCategoryValues
        {
            get
            {
                double sumOfWeights = module.GetRatingCategoryList().Select(cat => cat.Weight).Sum();
                double total = CategoryValuesDisplay.Select(cv => (cv.RatingCategory.Weight / sumOfWeights) * cv.PointValue).Sum();
                return total;
            }
        }

        public virtual IList<CategoryValue> CategoryValuesDisplay { get { return CategoryValues; } }

        public bool IgnoreCategories { get; set; } = false;
        public virtual bool AreCategoryValuesEditable { get { return !IgnoreCategories; } }

        protected readonly CategoryExtensionModule module;
        protected readonly SettingsScore settings;
        public IModelObjectCategorical BaseObject { get; internal set; }

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

        public virtual void ValidateFields()
        {
            var categories = module.GetRatingCategoryList();
            foreach (CategoryValue categoryValue in CategoryValues)
            {
                if (categoryValue.PointValue < settings.MinScore || categoryValue.PointValue > settings.MaxScore)
                    throw new ValidationException(categoryValue.RatingCategory.Name + " score must be between " + settings.MinScore.ToString() + " and " + settings.MaxScore.ToString(), categoryValue.PointValue);
                if (categories.Count <= 0)
                    throw new ValidationException("Category values were illegally modified - category " + categoryValue.RatingCategory.Name + " could not be found", categoryValue.RatingCategory.UniqueID);
                int indexOfCategory = categories.IndexOf(categoryValue.RatingCategory);
                if (indexOfCategory < 0)
                    throw new ValidationException("Category values were illegally modified - category " + categoryValue.RatingCategory.Name + " is a duplicate or does not exist on the module", categoryValue.RatingCategory.UniqueID);
                categories.RemoveAt(indexOfCategory);
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

        public virtual void ApplySettingsChanges(Settings settings)
        {
            if (settings is SettingsScore settingsScore)
            {
                foreach (CategoryValue cv in CategoryValues)
                {
                    cv.PointValue = settingsScore.ScaleValueToNewMinMaxRange(cv.PointValue);
                }
            }
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
                        CategoryValues = sr.GetValue(key).GetRepresentationObjectList(() => new CategoryValue(module, settings)).ToList();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
