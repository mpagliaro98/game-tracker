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
    public class RankedObjectStatus : RankedObject, IModelObjectStatus
    {
        private readonly StatusExtension _statusExtension;
        public StatusExtension StatusExtension { get { return _statusExtension; } }

        // Module re-declared as a different derived type
        protected readonly new TrackerModuleStatuses module;

        public RankedObjectStatus(Settings settings, TrackerModuleStatuses module) : this(settings, module, new StatusExtension(module.StatusExtension)) { }

        public RankedObjectStatus(Settings settings, TrackerModuleStatuses module, StatusExtension statusExtension) : base(settings, module)
        {
            _statusExtension = statusExtension;
        }

        public override void Validate()
        {
            base.Validate();
            StatusExtension.Validate();
        }

        public override bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            // deliberately not using || to avoid short-circuit behavior
            return base.RemoveReferenceToObject(obj, type) | StatusExtension.RemoveReferenceToObject(obj, type);
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
