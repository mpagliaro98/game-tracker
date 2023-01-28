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
    public class CategoryExtension : ExtensionBase
    {
        public IList<CategoryValue> CategoryValuesManual { get; private set; } = new List<CategoryValue>();

        public virtual double TotalScoreFromCategoryValues
        {
            get
            {
                return Module.GetTotalScoreFromCategoryValues(CategoryValueList);
            }
        }

        public virtual IList<CategoryValue> CategoryValueList { get { return CategoryValuesManual; } }

        public virtual IList<CategoryValue> CategoryValuesDisplay { get { return CategoryValueList; } }

        public bool IgnoreCategories { get; set; } = false;
        public virtual bool AreCategoryValuesEditable { get { return !IgnoreCategories; } }

        protected new CategoryExtensionModule Module => (CategoryExtensionModule)base.Module;
        protected new SettingsScore Settings => (SettingsScore)base.Settings;
        public new RatedObject BaseObject { get { return (RatedObject)base.BaseObject; } internal set { base.BaseObject = value; } }

        public CategoryExtension(CategoryExtensionModule module, SettingsScore settings) : base(module, settings)
        {
            foreach (RatingCategory category in module.GetRatingCategoryList())
            {
                var categoryValue = new CategoryValue(module, settings, category);
                CategoryValuesManual.Add(categoryValue);
            }
        }

        public CategoryExtension(CategoryExtension copyFrom) : base(copyFrom)
        {
            CategoryValuesManual = copyFrom.CategoryValuesManual;
            IgnoreCategories = copyFrom.IgnoreCategories;
        }

        public override void ValidateFields()
        {
            base.ValidateFields();
            var categories = Module.GetRatingCategoryList();
            foreach (CategoryValue categoryValue in CategoryValuesManual)
            {
                if (categoryValue.PointValue < Settings.MinScore || categoryValue.PointValue > Settings.MaxScore)
                    throw new ValidationException(categoryValue.RatingCategory.Name + " score must be between " + Settings.MinScore.ToString() + " and " + Settings.MaxScore.ToString(), categoryValue.PointValue);
                if (categories.Count <= 0)
                    throw new ValidationException("Category values were illegally modified - category " + categoryValue.RatingCategory.Name + " could not be found", categoryValue.RatingCategory.UniqueID);
                int indexOfCategory = categories.IndexOf(categoryValue.RatingCategory);
                if (indexOfCategory < 0)
                    throw new ValidationException("Category values were illegally modified - category " + categoryValue.RatingCategory.Name + " is a duplicate or does not exist on the module", categoryValue.RatingCategory.UniqueID);
                categories.RemoveAt(indexOfCategory);
            }
            if (categories.Count > 0)
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
                BaseObject.Save((TrackerModuleScores)Module.BaseModule, args.Connection);
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

        protected override void AddEventHandlers()
        {
            Module.RatingCategoryDeleted += OnRatingCategoryDeleted;
        }

        protected override void RemoveEventHandlers()
        {
            Module.RatingCategoryDeleted -= OnRatingCategoryDeleted;
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
            foreach (RatingCategory category in Module.GetRatingCategoryList())
            {
                list.Add(new CategoryValue(Module, Settings, category));
            }
            return list;
        }

        public override void LoadIntoRepresentation(ref SavableRepresentation sr)
        {
            base.LoadIntoRepresentation(ref sr);
            sr.SaveValue("IgnoreCategories", new ValueContainer(IgnoreCategories));
            sr.SaveValue("CategoryValues", new ValueContainer(CategoryValuesManual));
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "IgnoreCategories":
                        IgnoreCategories = sr.GetValue(key).GetBool();
                        break;
                    case "CategoryValues":
                        CategoryValuesManual = sr.GetValue(key).GetRepresentationObjectList(() => new CategoryValue(Module, Settings)).ToList();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
