using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
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
                if (BaseObject.IsUsingOriginalGameScore)
                {
                    try
                    {
                        return BaseObject.OriginalGame == null ? CreateListOfEmptyCategoryValues() : BaseObject.OriginalGame.CategoryExtension.CategoryValueList;
                    }
                    catch (StackOverflowException e)
                    {
                        Module.Logger.Log("CategoryExtensionGame CategoryValuesDisplay " + e.GetType().Name + ": OriginalGame is set to a game that references this one");
                        return CreateListOfEmptyCategoryValues();
                    }
                }
                else
                    return base.CategoryValueList;
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
    }
}
