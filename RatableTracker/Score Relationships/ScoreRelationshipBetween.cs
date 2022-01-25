using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ScoreRelationships
{
    public class ScoreRelationshipBetween : ScoreRelationship
    {
        public override string Name => "Between";

        public override int NumValuesRequired => 2;

        public override ObjectReference ReferenceKey => KeyScoreRelBetween;

        public override bool IsValueInRange(double val, IEnumerable<double> valueList)
        {
            if (valueList.Count() != NumValuesRequired)
            {
                throw new IncompleteScoreRangeException("ScoreRelationshipBetween expects " + NumValuesRequired.ToString() + " values, got " + valueList.Count().ToString());
            }
            return val >= valueList.ElementAt(0) && val < valueList.ElementAt(1);
        }

        public override void OverwriteReferenceKey(IReferable orig)
        {
            if (orig is ScoreRelationship origRel)
                referenceKey = origRel.ReferenceKey;
        }
    }
}
