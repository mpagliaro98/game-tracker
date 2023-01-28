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
    public class RankedObjectStatus : RankedObject, IModelObjectStatus
    {
        public override double ScoreDisplay
        {
            get
            {
                if (StatusExtension.Status == null ? true : StatusExtension.Status.HideScoreOfModelObject)
                    return 0;
                else
                    return base.ScoreDisplay;
            }
        }

        public override bool ShowScore
        {
            get { return StatusExtension.Status == null ? false : !StatusExtension.Status.HideScoreOfModelObject; }
        }

        public StatusExtension StatusExtension { get; init; }

        // Module re-declared as a different derived type
        protected new TrackerModuleStatuses Module => (TrackerModuleStatuses)base.Module;

        public RankedObjectStatus(Settings settings, TrackerModuleStatuses module) : this(settings, module, new StatusExtension(module.StatusExtension, settings)) { }

        public RankedObjectStatus(Settings settings, TrackerModuleStatuses module, StatusExtension statusExtension) : base(settings, module)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseObject = this;
        }

        public RankedObjectStatus(RankedObjectStatus copyFrom, StatusExtension statusExtension) : base(copyFrom)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseObject = this;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            StatusExtension.ValidateFields();
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            StatusExtension.ApplySettingsChanges(settings);
        }

        public override void InitAdditionalResources()
        {
            base.InitAdditionalResources();
            StatusExtension.InitAdditionalResources();
        }

        public override void Dispose()
        {
            base.Dispose();
            StatusExtension.Dispose();
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            StatusExtension.LoadIntoRepresentation(ref sr);
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            StatusExtension.RestoreFromRepresentation(sr);
        }
    }
}
