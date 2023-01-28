using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.ObjAddOns
{
    public class CategoryValue : RepresentationObject
    {
        private UniqueID _category = UniqueID.BlankID();
        public RatingCategory RatingCategory
        {
            get
            {
                if (!_category.HasValue()) return null;
                return Util.Util.FindObjectInList(Module.GetRatingCategoryList(), _category);
            }
        }

        public double PointValue { get; set; } = 0;

        private CategoryExtensionModule Module { get; set; }
        private SettingsScore Settings { get; set; }

        internal CategoryValue(CategoryExtensionModule module, SettingsScore settings) : this(module, settings, null) { }

        public CategoryValue(CategoryExtensionModule module, SettingsScore settings, RatingCategory ratingCategory)
        {
            this.Module = module;
            this.Settings = settings;
            if (ratingCategory != null) _category = ratingCategory.UniqueID;
            PointValue = this.Settings.MinScore;
        }

        internal bool CategoryEquals(IKeyable category)
        {
            return _category.Equals(category.UniqueID);
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("Category", new ValueContainer(_category));
            sr.SaveValue("PointValue", new ValueContainer(PointValue));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "Category":
                        _category = sr.GetValue(key).GetUniqueID();
                        break;
                    case "PointValue":
                        PointValue = sr.GetValue(key).GetDouble();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
