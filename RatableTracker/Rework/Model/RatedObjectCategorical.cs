using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.ObjAddOns;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Model
{
    public class RatedObjectCategorical : RatedObject, IModelObjectCategorical
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
                    return CategoryExtension.CalculateTotalCategoryScore();
                }
            }
        }

        // Module re-declared as a different derived type
        protected readonly new TrackerModuleScoreCategorical module;

        public RatedObjectCategorical(SettingsScore settings, TrackerModuleScoreCategorical module) : this(settings, module, new CategoryExtension(module.CategoryExtension, settings)) { }

        public RatedObjectCategorical(SettingsScore settings, TrackerModuleScoreCategorical module, CategoryExtension categoryExtension) : base(settings, module)
        {
            CategoryExtension = categoryExtension;
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
