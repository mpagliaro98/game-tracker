﻿using RatableTracker.Rework.LoadSave;
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
        private IList<CategoryValue> _categoryValues = new List<CategoryValue>();
        protected IList<CategoryValue> CategoryValues { get { return _categoryValues; } }

        public bool IgnoreCategories { get; set; } = false;

        private readonly CategoryExtensionModule module;
        private readonly SettingsScore settings;
        private readonly RatedObject obj;

        public CategoryExtension(CategoryExtensionModule module, SettingsScore settings, RatedObject obj)
        {
            this.module = module;
            this.settings = settings;
            this.obj = obj;

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
            // TODO unique exceptions
            foreach (CategoryValue categoryValue in GetCategoryValues())
            {
                if (categoryValue.PointValue < settings.MinScore || categoryValue.PointValue > settings.MaxScore)
                    throw new Exception(categoryValue.RatingCategory.Name + " score must be between " + settings.MinScore.ToString() + " and " + settings.MaxScore.ToString());
            }
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
                        _categoryValues = sr.GetValue(key).GetISavableList(() => new CategoryValue(module, settings)).ToList();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
