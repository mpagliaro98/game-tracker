using RatableTracker.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class CategoryExtensionGameCompilation : CategoryExtensionGame
    {
        public override IList<CategoryValue> CategoryValuesDisplay
        {
            get
            {
                IList<RatingCategory> ratingCategories = module.GetRatingCategoryList();
                IList<GameObject> gamesInComp = BaseObject.GamesInCompilation();
                IList<CategoryValue> categoryValues = new List<CategoryValue>();
                foreach (RatingCategory category in ratingCategories)
                {
                    double score = gamesInComp.Select(obj => obj.CategoryExtension.CategoryValuesDisplay.First(cv => cv.RatingCategory.Equals(category)).PointValue).Average();
                    categoryValues.Add(new CategoryValue(module, settings, category) { PointValue = score });
                }
                return categoryValues;
            }
        }

        public override bool AreCategoryValuesEditable { get { return false; } }

        public new GameCompilation BaseObject { get; }

        public CategoryExtensionGameCompilation(CategoryExtensionModule module, SettingsGame settings) : base(module, settings) { }
    }
}
