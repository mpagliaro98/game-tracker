﻿using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.ObjAddOns
{
    public class CategoryValue : RepresentationObject
    {
        [Savable("Category")] private UniqueID _category = UniqueID.BlankID();
        public RatingCategory RatingCategory
        {
            get
            {
                if (!_category.HasValue()) return null;
                return Util.Util.FindObjectInList(Module.GetRatingCategoryList(), _category);
            }
        }

        [Savable()] public double PointValue { get; set; } = 0;

        private CategoryExtensionModule Module { get; init; }
        private SettingsScore Settings { get; init; }

        internal CategoryValue(CategoryExtensionModule module, SettingsScore settings) : this(module, settings, null) { }

        public CategoryValue(CategoryExtensionModule module, SettingsScore settings, RatingCategory ratingCategory)
        {
            this.Module = module;
            this.Settings = settings;
            if (ratingCategory != null) _category = ratingCategory.UniqueID;
            PointValue = this.Settings.MinScore;
        }

        internal bool CategoryEquals(IKeyable category)
        {
            return _category.Equals(category.UniqueID);
        }
    }
}
