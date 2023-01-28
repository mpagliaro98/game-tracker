using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class CategoryExtensionGameCompilation : CategoryExtensionGame
    {
        public override IList<CategoryValue> CategoryValueList
        {
            get
            {
                IList<RatingCategory> ratingCategories = Module.GetRatingCategoryList();
                IList<GameObject> gamesInComp = BaseObject.GamesInCompilation();
                IList<CategoryValue> categoryValues = new List<CategoryValue>();
                foreach (RatingCategory category in ratingCategories)
                {
                    double score = gamesInComp.Select(obj => obj.CategoryExtension.ScoreOfCategory(category)).AverageIfEmpty(Settings.MinScore);
                    categoryValues.Add(new CategoryValue(Module, Settings, category) { PointValue = score });
                }
                return categoryValues;
            }
        }

        public override bool AreCategoryValuesEditable { get { return false; } }

        public new GameCompilation BaseObject { get { return (GameCompilation)base.BaseObject; } }

        public CategoryExtensionGameCompilation(CategoryExtensionModule module, SettingsGame settings) : base(module, settings) { }

        public CategoryExtensionGameCompilation(CategoryExtensionGameCompilation copyFrom) : base(copyFrom) { }
    }
}
