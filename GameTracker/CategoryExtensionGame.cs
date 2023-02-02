using RatableTracker.Exceptions;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class CategoryExtensionGame : CategoryExtensionWithStatus
    {
        public override IList<CategoryValue> CategoryValueList
        {
            get
            {
                return CalculateCategoryValuesRecursive(new List<UniqueID>());
            }
        }

        public IList<CategoryValue> CategoryValueListMinIfCyclical
        {
            get
            {
                try
                {
                    return CategoryValueList;
                }
                catch (CyclicalReferenceException)
                {
                    return CreateListOfEmptyCategoryValues();
                }
            }
        }

        public override IList<CategoryValue> CategoryValuesDisplay
        {
            get
            {
                try
                {
                    return base.CategoryValuesDisplay;
                }
                catch (CyclicalReferenceException ex)
                {
                    Module.Logger.Log(ex.GetType().Name + " in CategoryValuesDisplay: " + ex.Message);
                    return CreateListOfEmptyCategoryValues();
                }
            }
        }

        public override bool AreCategoryValuesEditable
        {
            get
            {
                if (BaseObject.IsUsingOriginalGameScore)
                    return false;
                else
                    return base.AreCategoryValuesEditable;
            }
        }

        protected new SettingsGame Settings => (SettingsGame)base.Settings;

        public new GameObject BaseObject => (GameObject)base.BaseObject;

        public CategoryExtensionGame(CategoryExtensionModule module, SettingsGame settings) : base(module, settings) { }

        public CategoryExtensionGame(CategoryExtensionGame copyFrom) : base(copyFrom) { }

        public override double ScoreOfCategory(RatingCategory ratingCategory)
        {
            try
            {
                return base.ScoreOfCategory(ratingCategory);
            }
            catch (CyclicalReferenceException)
            {
                return Settings.MinScore;
            }
        }

        private IList<CategoryValue> CalculateCategoryValuesRecursive(IList<UniqueID> path)
        {
            if (path.Contains(BaseObject.UniqueID))
                throw new CyclicalReferenceException("Cyclical reference in OriginalGame field: " + string.Join(" -> ", path));
            if (BaseObject.IsUsingOriginalGameScore)
            {
                path.Add(BaseObject.UniqueID);
                return BaseObject.OriginalGame == null ? CreateListOfEmptyCategoryValues() : BaseObject.OriginalGame.CategoryExtension.CalculateCategoryValuesRecursive(path);
            }
            else
                return base.CategoryValueList;
        }
    }
}
