using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ScoreRanges
{
    public class ScoreRelationshipBetween : ScoreRelationship
    {
        public override string Name => "Between";
        public override int NumValuesRequired => 2;
        public override UniqueID UniqueID => new UniqueID(3);

        public override bool IsValueInRange(double val, IEnumerable<double> valueList)
        {
            if (valueList.Count() != NumValuesRequired)
            {
                // TODO throw different exception
                throw new Exception(GetType().Name + " expects " + NumValuesRequired.ToString() + " values, got " + valueList.Count().ToString());
            }
            return val >= valueList.ElementAt(0) && val < valueList.ElementAt(1);
        }
    }
}
