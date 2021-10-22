using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;

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
        bool ValidateCategoryScores(IEnumerable<RatingCategoryValue> vals);
        void SetCategoryValuesAndBoundsCheck(TListedObj obj, IEnumerable<RatingCategoryValue> vals);
        TRatingCat FindRatingCategory(ObjectReference objectKey);
        void AddRatingCategory(TRatingCat obj);
        void UpdateRatingCategory(TRatingCat obj, TRatingCat orig);
        void DeleteRatingCategory(TRatingCat obj);
    }
}
