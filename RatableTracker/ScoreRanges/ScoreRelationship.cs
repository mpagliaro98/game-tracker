using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ScoreRanges
{
    public abstract class ScoreRelationship : IKeyable
    {
        public abstract string Name { get; }
        public abstract int NumValuesRequired { get; }
        public abstract UniqueID UniqueID { get; }

        public abstract bool IsValueInRange(double val, IList<double> valueList);

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ScoreRange)) return false;
            ScoreRange other = (ScoreRange)obj;
            return UniqueID.Equals(other.UniqueID);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
