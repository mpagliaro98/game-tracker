using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface ILoadSaveCategorical<TRatingCat>
        where TRatingCat : RatingCategory
    {
        IEnumerable<TRatingCat> LoadRatingCategories();
        void SaveRatingCategories(IEnumerable<TRatingCat> ratingCategories);
    }
}
