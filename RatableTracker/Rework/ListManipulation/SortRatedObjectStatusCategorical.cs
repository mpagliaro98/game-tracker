﻿using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.ObjAddOns;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class SortRatedObjectStatusCategorical : SortRatedObjectStatus
    {
        public const int SORT_CategoryStart = 5100;

        public SortRatedObjectStatusCategorical(SettingsScore settings) : base(settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod, TrackerModule module)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod, module);
            TrackerModuleScoreCategorical moduleCast = module as TrackerModuleScoreCategorical;
            if (sortMethod >= SORT_CategoryStart && sortMethod < SORT_CategoryStart + moduleCast.CategoryExtension.LimitRatingCategories)
            {
                RatingCategory cat = moduleCast.CategoryExtension.GetRatingCategoryList().ElementAt(sortMethod - SORT_CategoryStart);
                sortFunction = obj => ((RatedObjectCategorical)obj).CategoryExtension.GetCategoryValues().First((cv) => cv.RatingCategory.Equals(cat)).PointValue;
            }
            return sortFunction;
        }
    }
}