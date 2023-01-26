using RatableTracker.Util;
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
                foreach (ScoreRange range in module.GetScoreRangeList())
                {
                    try
                    {
                        if (range.ScoreRelationship.IsValueInRange(ScoreDisplay, range.ValueList)) return range;
                    }
                    catch (InvalidObjectStateException e)
                    {
                        module.Logger.Log(e.GetType().Name + ": " + e.Message);
                        throw;
                    }
                }
                return null;
            }
        }

        public override int Rank
        {
            get
            {
                IList<RankedObject> rankedObjects = module.GetModelObjectList().OrderByDescending(obj => obj.ScoreDisplay).ToList();
                return rankedObjects.IndexOf(this) + 1;
            }
        }

        // Re-declared as a different derived type
        protected new SettingsScore settings => (SettingsScore)base.settings;
        protected new TrackerModuleScores module => (TrackerModuleScores)base.module;

        public RatedObject(SettingsScore settings, TrackerModuleScores module) : base(settings, module)
        {
            ManualScore = settings.MinScore;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (ManualScore < settings.MinScore || ManualScore > settings.MaxScore)
                throw new ValidationException("Score must be between " + settings.MinScore.ToString() + " and " + settings.MaxScore.ToString(), ManualScore);
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
