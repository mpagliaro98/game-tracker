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
                // TODO different behavior if using original game
                return base.CategoryValuesDisplay;
            }
        }

        protected readonly new SettingsGame settings;

        public new GameObject BaseObject { get; }

        public CategoryExtensionGame(CategoryExtensionModule module, SettingsGame settings) : base(module, settings) { }
    }
}
