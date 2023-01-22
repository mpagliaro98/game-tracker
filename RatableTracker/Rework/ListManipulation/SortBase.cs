using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class SortBase<T>
    {
        public const int SORT_None = 0;

        public int SortMethod { get; set; } = SORT_None;
        public SortMode SortMode { get; set; } = SortMode.Ascending;

        public TrackerModule Module { get; set; }
        public Settings Settings { get; set; }

        public SortBase()
        {
            Module = null;
            Settings = null;
        }

        public SortBase(TrackerModule module, Settings settings)
        {
            Module = module;
            Settings = settings;
        }

        public IList<T> ApplySorting(IList<T> list)
        {
            Func<T, object> sortFunction = GetSortFunction(SortMethod);
            if (sortFunction == null)
            {
                if (SortMode == SortMode.Descending)
                    list.ToList().Reverse();
                return list;
            }

            try
            {
                Func<T, object> defaultSort = DefaultSort();
                if (SortMode == SortMode.Ascending)
                    return (defaultSort == null ? list : list.OrderBy(defaultSort).ToList()).OrderBy(sortFunction).ToList();
                else if (SortMode == SortMode.Descending)
                    return (defaultSort == null ? list : list.OrderBy(defaultSort).ToList()).OrderByDescending(sortFunction).ToList();
                else
                    throw new ListManipulationException("Unhandled sort mode", SortMode);
            }
            catch (InvalidCastException e)
            {
                throw new ListManipulationException(GetType().Name + " - Chosen sort method requires all objects in list be a more specific derived type (" + e.GetType().Name + ": " + e.Message + ")", SortMethod);
            }
        }

        protected virtual Func<T, object> GetSortFunction(int sortMethod)
        {
            return null;
        }

        protected virtual Func<T, object> DefaultSort()
        {
            return null;
        }
    }
}
