﻿using RatableTracker.Util;
using RatableTracker.Modules;
using RatableTracker.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.LoadSave;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;

namespace RatableTracker.Model
{
    public class RatedObject : RankedObject
    {
        public override double Score
        {
            get { return ManualScore; }
        }

        public double ManualScore { get; set; } = 0;

        public ScoreRange ScoreRange
        {
            get
            {
                foreach (ScoreRange range in Module.GetScoreRangeList())
                {
                    try
                    {
                        if (range.ScoreRelationship.IsValueInRange(ScoreDisplay, range.ValueList)) return range;
                    }
                    catch (InvalidObjectStateException e)
                    {
                        Module.Logger.Log(e.GetType().Name + ": " + e.Message);
                        throw;
                    }
                }
                return null;
            }
        }

        // Re-declared as a different derived type
        protected new SettingsScore Settings => (SettingsScore)base.Settings;
        protected new TrackerModuleScores Module => (TrackerModuleScores)base.Module;

        public RatedObject(SettingsScore settings, TrackerModuleScores module) : base(settings, module)
        {
            ManualScore = settings.MinScore;
        }

        public RatedObject(RatedObject copyFrom) : base(copyFrom)
        {
            ManualScore = copyFrom.ManualScore;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (ManualScore < Settings.MinScore || ManualScore > Settings.MaxScore)
                throw new ValidationException("Score must be between " + Settings.MinScore.ToString() + " and " + Settings.MaxScore.ToString(), ManualScore);
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            if (settings is SettingsScore settingsScore)
            {
                ManualScore = settingsScore.ScaleValueToNewMinMaxRange(ManualScore);
            }
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("ManualScore", new ValueContainer(ManualScore));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "ManualScore":
                        ManualScore = sr.GetValue(key).GetDouble();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
