﻿using RatableTracker.Exceptions;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ScoreRanges
{
    public class ScoreRelationshipBelow : ScoreRelationship
    {
        public override string Name => "Below";
        public override int NumValuesRequired => 1;
        public override UniqueID UniqueID => UniqueID.Parse(2);

        public override bool IsValueInRange(double val, IList<double> valueList)
        {
            if (valueList.Count() != NumValuesRequired)
                throw new InvalidObjectStateException(GetType().Name + " expects " + NumValuesRequired.ToString() + " values, got " + valueList.Count().ToString());
            return val < valueList[0];
        }
    }
}
