using Microsoft.VisualBasic;
using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    public class CategoryExtension
    {
        public IList<CategoryValue> CategoryValuesManual { get; private set; } = new List<CategoryValue>();

        public virtual double TotalScoreFromCategoryValues
        {
            get
            {
                return module.GetTotalScoreFromCategoryValues(CategoryValueList);
            }
        }

        public virtual IList<CategoryValue> CategoryValueList { get { return CategoryValuesManual; } }

        public virtual IList<CategoryValue> CategoryValuesDisplay { get { return CategoryValueList; } }

        public bool IgnoreCategories { get; set; } = false;
        public virtual bool AreCategoryValuesEditable { get { return !IgnoreCategories; } }

        protected readonly CategoryExtensionModule module;
        protected readonly SettingsScore settings;
        public RatedObject BaseObject { get; internal set; }

        public CategoryExtension(CategoryExtensionModule module, SettingsScore settings)
        {
            this.module = module;
            this.settings = settings;
            this.module.RatingCategoryDeleted += OnRatingCategoryDeleted;

            foreach (RatingCategory category in module.GetRatingCategoryList())
            {
                var categoryValue = new CategoryValue(module, settings, category);
                CategoryValuesManual.Add(categoryValue);
            }
        }

        public CategoryExtension(CategoryExtension copyFrom) : this(copyFrom.module, copyFrom.settings)
        {
            CategoryValuesManual = copyFrom.CategoryValuesManual;
            IgnoreCategories = copyFrom.IgnoreCategories;
        }

        public virtual void ValidateFields()
        {
            var categories = module.GetRatingCategoryList();
            foreach (CategoryValue categoryValue in CategoryValuesManual)
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
            if (categories.Count() > 0)
                throw new ValidationException("Category values were illegally modified - more categories are represented than exist", string.Join(", ", categories.Select(cat => cat.UniqueID.ToString())));
        }

        private void OnRatingCategoryDeleted(object sender, RatingCategoryDeleteArgs args)
        {
            ICollection<CategoryValue> toDelete = new List<CategoryValue>();
            foreach (CategoryValue cv in CategoryValuesManual)
            {
                if (cv.CategoryEquals(args.DeletedObject))
                {
                    toDelete.Add(cv);
                }
            }
            foreach (CategoryValue cv in toDelete)
            {
                CategoryValuesManual.Remove(cv);
            }
            if (toDelete.Count > 0)
                BaseObject.Save(module.BaseModule, args.Connection);
        }

        public virtual void ApplySettingsChanges(Settings settings)
        {
            if (settings is SettingsScore settingsScore)
            {
                foreach (CategoryValue cv in CategoryValuesManual)
                {
                    cv.PointValue = settingsScore.ScaleValueToNewMinMaxRange(cv.PointValue);
                }
            }
        }
        public void Dispose()
        {
            RemoveEventHandlers();
        }

        protected virtual void RemoveEventHandlers()
        {
            module.RatingCategoryDeleted -= OnRatingCategoryDeleted;
        }

        public virtual double ScoreOfCategory(RatingCategory ratingCategory)
        {
            return CategoryValueList.First(cv => cv.RatingCategory.Equals(ratingCategory)).PointValue;
        }

        public virtual double ScoreOfCategoryDisplay(RatingCategory ratingCategory)
        {
            return CategoryValuesDisplay.First(cv => cv.RatingCategory.Equals(ratingCategory)).PointValue;
        }

        protected IList<CategoryValue> CreateListOfEmptyCategoryValues()
        {
            IList<CategoryValue> list = new List<CategoryValue>();
            foreach (RatingCategory category in module.GetRatingCategoryList())
            {
                list.Add(new CategoryValue(module, settings, category));
            }
            return list;
        }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation sr)
        {
            sr.SaveValue("IgnoreCategories", new ValueContainer(IgnoreCategories));
            sr.SaveValue("CategoryValues", new ValueContainer(CategoryValuesManual));
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
                        CategoryValuesManual = sr.GetValue(key).GetRepresentationObjectList(() => new CategoryValue(module, settings)).ToList();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
