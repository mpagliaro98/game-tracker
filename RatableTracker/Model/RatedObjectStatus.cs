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
    public class RatedObjectStatus : RatedObject, IModelObjectStatus
    {
        public override double ScoreDisplay
        {
            get
            {
                if (StatusExtension.Status == null ? true : StatusExtension.Status.HideScoreOfModelObject)
                    return settings.MinScore;
                else
                    return base.ScoreDisplay;
            }
        }

        public override bool ShowScore
        {
            get { return StatusExtension.Status == null ? false : !StatusExtension.Status.HideScoreOfModelObject; }
        }

        public StatusExtension StatusExtension { get; private set; }

        // Module re-declared as a different derived type
        protected new TrackerModuleScoreStatuses module => (TrackerModuleScoreStatuses)base.module;

        public RatedObjectStatus(SettingsScore settings, TrackerModuleScoreStatuses module) : this(settings, module, new StatusExtension(module.StatusExtension)) { }

        public RatedObjectStatus(SettingsScore settings, TrackerModuleScoreStatuses module, StatusExtension statusExtension) : base(settings, module)
        {
            StatusExtension = statusExtension;
            StatusExtension.BaseObject = this;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            StatusExtension.ValidateFields();
        }

        public override bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            // deliberately not using || to avoid short-circuit behavior
            return base.RemoveReferenceToObject(obj, type) | StatusExtension.RemoveReferenceToObject(obj, type);
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            StatusExtension.ApplySettingsChanges(settings);
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
