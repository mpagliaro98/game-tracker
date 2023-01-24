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
        public RatingCategoryWeighted(CategoryExtensionModule module, SettingsScore settings) : base(module, settings) { }

        public void SetWeight(double val)
        {
            Weight = val;
        }
    }
}
