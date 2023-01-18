﻿using RatableTracker.Rework.Util;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Rework.LoadSave;

namespace RatableTracker.Rework.Model
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
                    if (range.ScoreRelationship.IsValueInRange(Score, range.ValueList)) return range;
                }
                return null;
            }
        }

        public override int Rank
        {
            get
            {
                IList<RankedObject> rankedObjects = module.GetModelObjectList().OrderByDescending(obj => obj.Score).ToList();
                return rankedObjects.IndexOf(this) + 1;
            }
        }

        // Re-declared as a different derived type
        protected readonly new SettingsScore settings;
        protected readonly new TrackerModuleScores module;

        public RatedObject(SettingsScore settings, TrackerModuleScores module) : base(settings, module)
        {
            ManualScore = settings.MinScore;
        }

        public override void Validate()
        {
            base.Validate();
            // TODO throw specialized type of exception
            if (ManualScore < settings.MinScore || ManualScore > settings.MaxScore)
                throw new Exception("Score must be between " + settings.MinScore.ToString() + " and " + settings.MaxScore.ToString());
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