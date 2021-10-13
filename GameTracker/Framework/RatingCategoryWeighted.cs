using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework
{
    public class RatingCategoryWeighted : RatingCategory
    {
        public RatingCategoryWeighted() : base() { }

        public RatingCategoryWeighted(string name, string comment, double weight) : base(name, comment)
        {
            this.weight = weight;
        }

        public void SetWeight(double val)
        {
            weight = val;
        }
    }
}
