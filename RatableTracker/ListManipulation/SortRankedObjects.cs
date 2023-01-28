﻿using RatableTracker.Exceptions;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation
{
    public class SortRankedObjects : SortBase<RankedObject>
    {
        public const int SORT_Name = 1;
        public const int SORT_HasComment = 2;

        public SortRankedObjects() : base() { }

        public SortRankedObjects(TrackerModule module, Settings settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            switch (sortMethod)
            {
                case SORT_Name:
                    sortFunction = obj => obj.Name.CleanForSorting();
                    break;
                case SORT_HasComment:
                    sortFunction = obj => obj.Comment.Length > 0;
                    break;
            }
            return sortFunction;
        }

        protected override Func<RankedObject, object> DefaultSort()
        {
            return obj => obj.Name.CleanForSorting();
        }
    }
}