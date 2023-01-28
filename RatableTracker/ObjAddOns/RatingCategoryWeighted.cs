﻿using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    public class RatingCategoryWeighted : RatingCategory
    {
        public RatingCategoryWeighted(IModuleCategorical module, SettingsScore settings) : base(module, settings) { }

        public RatingCategoryWeighted(RatingCategoryWeighted copyFrom) : base(copyFrom) { }

        public void SetWeight(double val)
        {
            Weight = val;
        }
    }
}