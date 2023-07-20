using RatableTracker.Interfaces;
using RatableTracker.Model;
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
        [Savable()] public new double Weight { get => base.Weight; set => base.Weight = value; }

        public RatingCategoryWeighted(IModuleCategorical module, SettingsScore settings) : base(module, settings) { }

        public RatingCategoryWeighted(RatingCategoryWeighted copyFrom) : base(copyFrom) { }
    }
}
