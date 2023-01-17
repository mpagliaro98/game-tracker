using RatableTracker.Rework.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface ILoadSaveMethodCategoryExtension : ILoadSaveMethod
    {
        void SaveOneCategory(RatingCategory ratingCategory);
        void SaveAllCategories(IList<RatingCategory> ratingCategories);
        void DeleteOneCategory(RatingCategory ratingCategory);
        IList<RatingCategory> LoadCategories();
    }
}
