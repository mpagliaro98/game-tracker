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

        public void ChangeMinMaxScore(double newMin, double newMax)
        {
            double oldMin = MinScore;
            double oldMax = MaxScore;
            _minScore = newMin;
            _maxScore = newMax;
            // TODO scale existing scores to new min/max range
        }
    }
}
