using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ScoreRelationships
{
    public abstract class ScoreRelationship : IReferable
    {
        public static ObjectReference KeyScoreRelAbove = new ObjectReference(new Guid("11111111111111111111111111111111"));
        public static ObjectReference KeyScoreRelBelow = new ObjectReference(new Guid("22222222222222222222222222222222"));
        public static ObjectReference KeyScoreRelBetween = new ObjectReference(new Guid("33333333333333333333333333333333"));

        public abstract string Name { get; }

        public abstract int NumValuesRequired { get; }

        protected ObjectReference referenceKey;
        public abstract ObjectReference ReferenceKey { get; }

        public abstract bool IsValueInRange(double val, IEnumerable<double> valueList);

        public abstract void OverwriteReferenceKey(IReferable orig);

        public override string ToString()
        {
            return Name;
        }
    }
}
