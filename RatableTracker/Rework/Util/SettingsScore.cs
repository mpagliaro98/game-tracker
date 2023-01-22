using RatableTracker.Rework.Exceptions;
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
        // save min/max score changes to temporary variables until Save is called
        private double _minScore = 0;
        private double _tempMinScore = 0;
        public double MinScore
        {
            get { return _minScore; }
            set { _tempMinScore = value; }
        }

        private double _maxScore = 10;
        private double _tempMaxScore = 10;
        public double MaxScore
        {
            get { return _maxScore; }
            set { _tempMaxScore = value; }
        }

        public SettingsScore() : base() { }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (_tempMinScore >= _tempMaxScore)
                throw new ValidationException("Minimum score must be less than maximum score", _tempMinScore.ToString() + ", " + _tempMaxScore.ToString());
        }

        protected override void PreSave(TrackerModule module)
        {
            base.PreSave(module);

            // new min and max are valid, so swap them into the main variables
            (_tempMinScore, _minScore) = (_minScore, _tempMinScore);
            (_tempMaxScore, _maxScore) = (_maxScore, _tempMaxScore);
        }

        protected override void PostSave(TrackerModule module, bool isNew)
        {
            base.PostSave(module, isNew);

            // set temp values back equal to the real values
            _tempMinScore = _minScore;
            _tempMaxScore = _maxScore;
        }

        public double ScaleValueToNewMinMaxRange(double value)
        {
            if (_tempMinScore == _minScore && _tempMaxScore == _maxScore) return value;
            double newValue;
            double oldRange = _tempMaxScore - _tempMinScore;
            double newRange = _maxScore - _minScore;
            if (oldRange == 0)
                newValue = _minScore;
            else
                newValue = ((value - _tempMinScore) * newRange / oldRange) + _minScore;
            return newValue;
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
                        _tempMinScore = sr.GetValue(key).GetDouble();
                        break;
                    case "MaxScore":
                        _maxScore = sr.GetValue(key).GetDouble();
                        _tempMaxScore = sr.GetValue(key).GetDouble();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
