using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ScoreRelationships
{
    public class ScoreRelationshipAbove : ScoreRelationship
    {
        public override string Name => "Above";

        public override int NumValuesRequired => 1;

        public override ObjectReference ReferenceKey => KeyScoreRelAbove;

        public override bool IsValueInRange(double val, IEnumerable<double> valueList)
        {
            if (valueList.Count() != NumValuesRequired)
            {
                throw new IncompleteScoreRangeException("ScoreRelationshipAbove expects " + NumValuesRequired.ToString() + " values, got " + valueList.Count().ToString());
            }
            return val >= valueList.ElementAt(0);
        }

        public override void OverwriteReferenceKey(IReferable orig)
        {
            if (orig is ScoreRelationship origRel)
                referenceKey = origRel.ReferenceKey;
        }
    }
}
