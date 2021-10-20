using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public interface IModuleCategorical<TRatingCat> where TRatingCat : RatingCategory
    {
        IEnumerable<TRatingCat> RatingCategories { get; }
        int LimitRatingCategories { get; }
        void LoadRatingCategories();
        void SaveRatingCategories();
        TRatingCat FindRatingCategory(ObjectReference objectKey);
        void AddRatingCategory(TRatingCat obj);
        void UpdateRatingCategory(TRatingCat obj, TRatingCat orig);
        void DeleteRatingCategory(TRatingCat obj);
    }
}
