using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Exceptions;

namespace RatableTracker.Framework.ScoreRelationships
{
    public class ScoreRelationshipAbove : ScoreRelationship
    {
        public override string Name
        {
            get { return "Above"; }
        }

        public override int NumValuesRequired
        {
            get { return 1; }
        }

        public override Guid ReferenceKey => KeyScoreRelAbove;

        public override bool IsValueInRange(double val, IEnumerable<double> valueList)
        {
            if (valueList.Count() != NumValuesRequired)
            {
                throw new IncompleteScoreRangeException("ScoreRelationshipAbove expects " + NumValuesRequired.ToString() + " values, got " + valueList.Count().ToString());
            }
            return val >= valueList.ElementAt(0);
        }
    }
}
