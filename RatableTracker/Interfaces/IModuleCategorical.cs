using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IModuleCategorical<TListedObj, TRatingCat>
        where TListedObj : IObjectCategorical
        where TRatingCat : RatingCategory
    {
        IEnumerable<TRatingCat> RatingCategories { get; }
        int LimitRatingCategories { get; }
        void LoadRatingCategories();
        Task LoadRatingCategoriesAsync();
        void SaveRatingCategories();
        Task SaveRatingCategoriesAsync();
        double GetScoreOfCategory(TListedObj obj, TRatingCat cat);
        double GetScoreOfCategoryValues(IEnumerable<RatingCategoryValue> categoryValues);
        TRatingCat FindRatingCategory(ObjectReference objectKey);
        void AddRatingCategory(TRatingCat obj);
        void UpdateRatingCategory(TRatingCat obj, TRatingCat orig);
        void DeleteRatingCategory(TRatingCat obj);
        void ValidateCategoryScores(IEnumerable<RatingCategoryValue> vals);
        void ValidateRatingCategory(TRatingCat obj);
    }
}
