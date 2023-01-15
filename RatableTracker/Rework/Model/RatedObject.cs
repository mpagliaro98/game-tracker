using RatableTracker.Rework.Util;
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
        /// <summary>
        /// The score of this ratable object.
        /// </summary>
        public double Score
        {
            get { return ManualScore; }
        }

        private double _manualScore = 0;
        public double ManualScore
        {
            get { return _manualScore; }
            set
            {
                // TODO throw specialized type of exception
                if (value < settings.MinScore || value > settings.MaxScore)
                    throw new Exception("Invalid score");
                _manualScore = value;
            }
        }

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

        // Re-declared as a different derived type
        protected readonly new SettingsScore settings;
        protected readonly new TrackerModuleScores module;

        public RatedObject(SettingsScore settings, TrackerModuleScores module) : base(settings, module) { }

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
