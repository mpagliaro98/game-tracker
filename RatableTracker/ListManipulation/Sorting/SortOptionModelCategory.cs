﻿using RatableTracker.Interfaces;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RatableTracker.ListManipulation.Sorting
{
    [SortOption(typeof(IModelObjectCategorical), InstantiateManually = true)]
    public class SortOptionModelCategory : SortOptionSimpleBase<IModelObjectCategorical>
    {
        private UniqueID _uniqueID;
        public RatingCategory Category
        {
            get => Util.Util.FindObjectInList(((IModuleCategorical)Module).CategoryExtension.GetRatingCategoryList(), _uniqueID) ?? new RatingCategory((IModuleCategorical)Module, (SettingsScore)Settings);
            private set => _uniqueID = value.UniqueID;
        }
        public override string Name => Category.Name;

        public SortOptionModelCategory() : base() { }

        protected override object GetSortValue(IModelObjectCategorical obj)
        {
            try
            {
                return obj.CategoryExtension.IgnoreCategories ? ((SettingsScore)Settings).MinScore : obj.CategoryExtension.ScoreOfCategoryDisplay(Category);
            }
            catch
            {
                Debug.WriteLine("Error when sorting on category - category does not exist");
                return ((SettingsScore)Settings).MinScore;
            }
        }

        public override bool Equals(object obj)
        {
            bool result = base.Equals(obj);
            if (!result) return false;
            if (obj is not SortOptionModelCategory category) return false;
            return _uniqueID.Equals(category._uniqueID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IList<ISortOption> InstantiateManually()
        {
            var listOptions = new List<ISortOption>();
            var categories = ((IModuleCategorical)Module).CategoryExtension.GetRatingCategoryList();
            foreach (var category in categories)
            {
                var newOption = new SortOptionModelCategory();
                newOption.Module = Module;
                newOption.Settings = Settings;
                newOption.Category = category;
                listOptions.Add(newOption);
            }
            return listOptions;
        }

        protected internal override void SerializeExtraInformation(XmlWriter writer)
        {
            base.SerializeExtraInformation(writer);
            writer.WriteAttributeString("UniqueID", Category.UniqueID.ToString());
        }

        protected internal override void DeserializeExtraInformation(XmlReader reader)
        {
            base.DeserializeExtraInformation(reader);
            _uniqueID = UniqueID.Parse(reader.GetAttribute("UniqueID"));
        }
    }
}
