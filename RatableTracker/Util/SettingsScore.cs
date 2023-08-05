using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    public class SettingsScore : Settings
    {
        public event SettingsChangeHandler SettingsMinMaxScoreChanged;

        // save min/max score changes to temporary variables until Save is called
        private double _minScore = 0;
        private double _tempMinScore = 0;
        [Savable(HandleRestoreManually = true)] public double MinScore
        {
            get { return _minScore; }
            set { _tempMinScore = value; }
        }

        private double _maxScore = 10;
        private double _tempMaxScore = 10;
        [Savable(HandleRestoreManually = true)] public double MaxScore
        {
            get { return _maxScore; }
            set { _tempMaxScore = value; }
        }

        [Savable] public bool ShowScoreWhenNullStatus { get; set; } = false;

        public SettingsScore() : base() { }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (_tempMinScore >= _tempMaxScore)
                throw new ValidationException("Minimum score must be less than maximum score", _tempMinScore.ToString() + ", " + _tempMaxScore.ToString());
        }

        protected override void PreSave(TrackerModule module, ILoadSaveMethod conn)
        {
            base.PreSave(module, conn);

            // new min and max are valid, so swap them into the main variables
            (_tempMinScore, _minScore) = (_minScore, _tempMinScore);
            (_tempMaxScore, _maxScore) = (_maxScore, _tempMaxScore);
        }

        protected override void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn)
        {
            base.PostSave(module, isNew, conn);

            if (_tempMinScore != _minScore || _tempMaxScore != _maxScore)
                SettingsMinMaxScoreChanged?.Invoke(this, new SettingsChangeArgs(GetType(), conn));

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

        protected override void RestoreHandleManually(SavableRepresentation sr, string key)
        {
            base.RestoreHandleManually(sr, key);
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
            }
        }
    }
}
