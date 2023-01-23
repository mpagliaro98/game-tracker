using RatableTracker.Rework.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public class CategoryExtensionGame : CategoryExtension
    {
        public override IList<CategoryValue> CategoryValuesDisplay
        {
            get
            {
                if (BaseObject.IsRemaster && BaseObject.UseOriginalGameScore)
                {
                    try
                    {
                        return BaseObject.OriginalGame == null ? CreateListOfEmptyCategoryValues() : BaseObject.OriginalGame.CategoryExtension.CategoryValuesDisplay;
                    }
                    catch (StackOverflowException e)
                    {
                        module.Logger.Log("CategoryExtensionGame CategoryValuesDisplay " + e.GetType().Name + ": OriginalGame is set to a game that references this one");
                        return CreateListOfEmptyCategoryValues();
                    }
                }
                else
                    return base.CategoryValuesDisplay;
            }
        }

        public override bool AreCategoryValuesEditable
        {
            get
            {
                if (BaseObject.IsRemaster && BaseObject.UseOriginalGameScore)
                    return false;
                else
                    return base.AreCategoryValuesEditable;
            }
        }

        protected readonly new SettingsGame settings;

        public new GameObject BaseObject { get; }

        public CategoryExtensionGame(CategoryExtensionModule module, SettingsGame settings) : base(module, settings) { }

        protected IList<CategoryValue> CreateListOfEmptyCategoryValues()
        {
            IList<CategoryValue> list = new List<CategoryValue>();
            foreach (RatingCategory category in module.GetRatingCategoryList())
            {
                list.Add(new CategoryValue(module, settings, category));
            }
            return list;
        }
    }
}
