using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    public class CategoryExtensionWithStatus : CategoryExtension
    {
        public override IList<CategoryValue> CategoryValuesDisplay
        {
            get
            {
                if (BaseObject.StatusExtension.Status == null || BaseObject.StatusExtension.Status.HideScoreOfModelObject)
                    return CreateListOfEmptyCategoryValues();
                else
                    return base.CategoryValuesDisplay;
            }
        }

        public new IModelObjectStatus BaseObject { get { return (IModelObjectStatus)base.BaseObject; } }

        public CategoryExtensionWithStatus(CategoryExtensionModule module, SettingsScore settings) : base(module, settings) { }

        public CategoryExtensionWithStatus(CategoryExtensionWithStatus copyFrom) : base(copyFrom) { }
    }
}
