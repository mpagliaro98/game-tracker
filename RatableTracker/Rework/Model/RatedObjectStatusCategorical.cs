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
    public class RatedObjectStatusCategorical : RatedObjectStatus
    {
        private readonly CategoryExtension _categoryExtension;
        public CategoryExtension CategoryExtension { get { return _categoryExtension; } }

        // Module re-declared as a different derived type
        protected readonly new TrackerModuleScoreStatusCategorical module;

        public RatedObjectStatusCategorical(SettingsScore settings, TrackerModuleScoreStatusCategorical module) : base(settings, module)
        {
            _categoryExtension = new CategoryExtension(module.CategoryExtension, settings);
        }
    }
}
