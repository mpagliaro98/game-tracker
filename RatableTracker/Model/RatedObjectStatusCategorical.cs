using RatableTracker.Interfaces;
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
        public CategoryExtension CategoryExtension { get; private set; }

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
        protected readonly new TrackerModuleScoreStatusCategorical module;

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module) : this(settings, module, new StatusExtension(module.StatusExtension)) { }

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module, StatusExtension statusExtension) : this(settings, module, statusExtension, new CategoryExtension(module.CategoryExtension, settings)) { }

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module, CategoryExtension categoryExtension) : this(settings, module, new StatusExtension(module.StatusExtension), categoryExtension) { }

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module, StatusExtension statusExtension, CategoryExtension categoryExtension) : base(settings, module, statusExtension)
        {
            CategoryExtension = categoryExtension;
            CategoryExtension.BaseObject = this;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            CategoryExtension.ValidateFields();
        }

        public override bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            // deliberately not using || to avoid short-circuit behavior
            return base.RemoveReferenceToObject(obj, type) | CategoryExtension.RemoveReferenceToObject(obj, type);
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            CategoryExtension.ApplySettingsChanges(settings);
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
