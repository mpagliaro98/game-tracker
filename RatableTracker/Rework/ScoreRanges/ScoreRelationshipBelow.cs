using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ScoreRanges
{
    public class ScoreRelationshipBelow : ScoreRelationship
    {
        public override string Name => "Below";
        public override int NumValuesRequired => 1;
        public override UniqueID UniqueID => new UniqueID(2);

        public override bool IsValueInRange(double val, IEnumerable<double> valueList)
        {
            if (valueList.Count() != NumValuesRequired)
            {
                // TODO throw different exception
                throw new Exception(GetType().Name + " expects " + NumValuesRequired.ToString() + " values, got " + valueList.Count().ToString());
            }
            return val < valueList.ElementAt(0);
        }
    }
}
