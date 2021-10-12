using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public abstract class ScoreRelationship
    {
        public abstract string Name
        {
            get;
        }

        public abstract int NumValuesRequired
        {
            get;
        }

        public abstract bool IsValueInRange(double val, IEnumerable<double> valueList);
    }
}
