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
    public class RatedObjectCategorical : RatedObject, IModelObjectCategorical
    {
        public CategoryExtension CategoryExtension { get; init; }

        public override double Score
        {
            get
            {
                if (CategoryExtension.IgnoreCategories)
                {
                    return base.Score;
                }
                else
                {
                    return CategoryExtension.TotalScoreFromCategoryValues;
                }
            }
        }

        // Module re-declared as a different derived type
        protected new TrackerModuleScoreCategorical Module => (TrackerModuleScoreCategorical)base.Module;

        public RatedObjectCategorical(SettingsScore settings, TrackerModuleScoreCategorical module) : this(settings, module, new CategoryExtension(module.CategoryExtension, settings)) { }

        public RatedObjectCategorical(SettingsScore settings, TrackerModuleScoreCategorical module, CategoryExtension categoryExtension) : base(settings, module)
        {
            CategoryExtension = categoryExtension;
            CategoryExtension.BaseObject = this;
        }

        public RatedObjectCategorical(RatedObjectCategorical copyFrom, CategoryExtension categoryExtension) : base(copyFrom)
        {
            CategoryExtension = categoryExtension;
            CategoryExtension.BaseObject = this;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            CategoryExtension.ValidateFields();
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            CategoryExtension.ApplySettingsChanges(settings);
        }

        public override void InitAdditionalResources()
        {
            base.InitAdditionalResources();
            CategoryExtension.InitAdditionalResources();
        }

        public override void Dispose()
        {
            base.Dispose();
            CategoryExtension.Dispose();
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            CategoryExtension.LoadIntoRepresentation(ref sr);
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            CategoryExtension.RestoreFromRepresentation(sr);
        }
    }
}
