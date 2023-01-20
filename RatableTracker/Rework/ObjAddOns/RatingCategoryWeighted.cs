using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class RatingCategoryWeighted : RatingCategory
    {
        public RatingCategoryWeighted(CategoryExtensionModule module) : base(module) { }

        public void SetWeight(double val)
        {
            Weight = val;
        }
    }
}
