using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    class ScoreRelationshipBetween : ScoreRelationship
    {
        public override string Name
        {
            get { return "Between"; }
        }

        public override int NumValuesRequired
        {
            get { return 2; }
        }

        public override bool IsValueInRange(double val, IEnumerable<double> valueList)
        {
            if (valueList.Count() != NumValuesRequired)
            {
                throw new IncompleteScoreRangeException("ScoreRelationshipBetween expects " + NumValuesRequired.ToString() + " values, got " + valueList.Count().ToString());
            }
            return val >= valueList.ElementAt(0) && val < valueList.ElementAt(1);
        }
    }
}
