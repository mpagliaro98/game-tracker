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
    public class RatedObjectCategorical : RatedObject
    {
        private readonly CategoryExtension _categoryExtension;
        public CategoryExtension CategoryExtension { get { return _categoryExtension; } }

        // Module re-declared as a different derived type
        protected readonly new TrackerModuleScoreCategorical module;

        public RatedObjectCategorical(SettingsScore settings, TrackerModuleScoreCategorical module) : base(settings, module)
        {
            _categoryExtension = new CategoryExtension(module.CategoryExtension, settings);
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
