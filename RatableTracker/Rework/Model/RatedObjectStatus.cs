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
    public class RatedObjectStatus : RatedObject
    {
        private readonly StatusExtension _statusExtension;
        public StatusExtension StatusExtension { get { return _statusExtension; } }

        // Module re-declared as a different derived type
        protected readonly new TrackerModuleScoreStatuses module;

        public RatedObjectStatus(SettingsScore settings, TrackerModuleScoreStatuses module) : base(settings, module)
        {
            _statusExtension = new StatusExtension(module.StatusExtension);
        }
    }
}
