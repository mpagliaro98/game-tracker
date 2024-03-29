﻿using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Model
{
    public class RatedObjectStatusCategorical : RatedObjectStatus, IModelObjectCategorical
    {
        [TrackerObjectExtension]
        public CategoryExtension CategoryExtension { get; init; }

        public override double Score
        {
            get
            {
                if (CategoryExtension.IgnoreCategories)
                    return base.Score;
                else
                    return CategoryExtension.TotalScoreFromCategoryValues;
            }
        }

        // Module re-declared as a different derived type
        protected new TrackerModuleScoreStatusCategorical Module => (TrackerModuleScoreStatusCategorical)base.Module;

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module) : this(settings, module, new StatusExtension(module.StatusExtension, settings)) { }

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module, StatusExtension statusExtension) : this(settings, module, statusExtension, new CategoryExtensionWithStatus(module.CategoryExtension, settings)) { }

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module, CategoryExtensionWithStatus categoryExtension) : this(settings, module, new StatusExtension(module.StatusExtension, settings), categoryExtension) { }

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module, StatusExtension statusExtension, CategoryExtensionWithStatus categoryExtension) : base(settings, module, statusExtension)
        {
            CategoryExtension = categoryExtension;
            CategoryExtension.BaseObject = this;
        }

        public RatedObjectStatusCategorical(RatedObjectStatusCategorical copyFrom, StatusExtension statusExtension, CategoryExtensionWithStatus categoryExtension) : base(copyFrom, statusExtension)
        {
            CategoryExtension = categoryExtension;
            CategoryExtension.BaseObject = this;
        }
    }
}
