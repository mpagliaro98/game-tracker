using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public interface IModuleCategorical<TListedObj, TRatingCat>
        where TListedObj : ICategorical
        where TRatingCat : RatingCategory
    {
        IEnumerable<TRatingCat> RatingCategories { get; }
        int LimitRatingCategories { get; }
        void LoadRatingCategories();
        void SaveRatingCategories();
        double GetScoreOfCategoryValues(IEnumerable<RatingCategoryValue> categoryValues);
        TRatingCat FindRatingCategory(ObjectReference objectKey);
        void AddRatingCategory(TRatingCat obj);
        void UpdateRatingCategory(TRatingCat obj, TRatingCat orig);
        void DeleteRatingCategory(TRatingCat obj);
        IEnumerable<TRatingCat> SortRatingCategories<TField>(Func<TRatingCat, TField> keySelector, SortMode mode = SortMode.ASCENDING);
        void ValidateCategoryScores(IEnumerable<RatingCategoryValue> vals);
        void ValidateRatingCategory(TRatingCat obj);
    }
}
