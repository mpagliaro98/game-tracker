using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class RatingCategoryWeighted : RatingCategory
    {
        public RatingCategoryWeighted(CategoryExtensionModule module, SettingsScore settings) : base(module, settings) { }

        public void SetWeight(double val)
        {
            Weight = val;
        }
    }
}
