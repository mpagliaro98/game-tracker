using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Model
{
    public class RatedObjectStatus : RatedObject, IModelObjectStatus
    {
        public override double ScoreDisplay
        {
            get
            {
                if (!ShowScore)
                    return Settings.MinScore;
                else
                    return base.ScoreDisplay;
            }
        }

        public override bool ShowScore
        {
            get { return StatusExtension.Status == null ? false : !StatusExtension.Status.HideScoreOfModelObject; }
        }

        public override ScoreRange ScoreRangeDisplay
        {
            get
            {
                if (!ShowScore)
                    return null;
                else
                    return base.ScoreRangeDisplay;
            }
        }

        [TrackerObjectExtension]
        public StatusExtension StatusExtension { get; init; }

        // Module re-declared as a different derived type
        protected new TrackerModuleScoreStatuses Module => (TrackerModuleScoreStatuses)base.Module;

        public RatedObjectStatus(SettingsScore settings, TrackerModuleScoreStatuses module) : this(settings, module, new StatusExtension(module.StatusExtension, settings)) { }

        public RatedObjectStatus(SettingsScore settings, TrackerModuleScoreStatuses module, StatusExtension statusExtension) : base(settings, module)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseObject = this;
        }

        public RatedObjectStatus(RatedObjectStatus copyFrom, StatusExtension statusExtension) : base(copyFrom)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseObject = this;
        }
    }
}
