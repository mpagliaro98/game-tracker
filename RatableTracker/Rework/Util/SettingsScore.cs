using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public class SettingsScore : Settings
    {
        private double _minScore = 0;
        public double MinScore { get { return _minScore; } }

        private double _maxScore = 10;
        public double MaxScore { get { return _maxScore; } }

        public SettingsScore() : base() { }

        public void ChangeMinMaxScore(double newMin, double newMax, TrackerModuleScores module)
        {
            double oldMin = MinScore;
            double oldMax = MaxScore;
            _minScore = newMin;
            _maxScore = newMax;
            module.ScaleScoresToNewRange(oldMin, oldMax, newMin, newMax);
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("MinScore", new ValueContainer(MinScore));
            sr.SaveValue("MaxScore", new ValueContainer(MaxScore));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "MinScore":
                        _minScore = sr.GetValue(key).GetDouble();
                        break;
                    case "MaxScore":
                        _maxScore = sr.GetValue(key).GetDouble();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
