using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    public class RatingCategoryStatic : RatingCategory
    {
        private const double weight = 1.0;

        public RatingCategoryStatic() { }

        public override double GetWeight()
        {
            return weight;
        }
    }
}
