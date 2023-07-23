﻿using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RatableTracker.ListManipulation.Filtering
{
    [FilterOption(typeof(IModelObjectCategorical), InstantiateManually = true)]
    public class FilterOptionModelCategory : FilterOptionNumericBase<IModelObjectCategorical>
    {
        private UniqueID _uniqueID;
        private RatingCategory Category
        {
            get => Util.Util.FindObjectInList(((IModuleCategorical)Module).CategoryExtension.GetRatingCategoryList(), _uniqueID);
            set => _uniqueID = value.UniqueID;
        }
        public override string Name => Category.Name;

        public FilterOptionModelCategory() : base() { }

        protected override double GetComparisonValue(IModelObjectCategorical obj)
        {
            return obj.CategoryExtension.ScoreOfCategoryDisplay(Category);
        }

        public override IList<IFilterOption> InstantiateManually()
        {
            var listOptions = new List<IFilterOption>();
            var categories = ((IModuleCategorical)Module).CategoryExtension.GetRatingCategoryList();
            foreach (var category in categories)
            {
                var newOption = new FilterOptionModelCategory();
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