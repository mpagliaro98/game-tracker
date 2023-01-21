using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class SortRankedObjects
    {
        public const int SORT_None = 0;
        public const int SORT_Name = 1;
        public const int SORT_HasComment = 2;

        public int SortMethod { get; set; } = SORT_None;
        public SortMode SortMode { get; set; } = SortMode.Ascending;

        public SortRankedObjects() { }

        public IList<RankedObject> ApplySorting(IList<RankedObject> list, TrackerModule module)
        {
            Func<RankedObject, object> sortFunction = GetSortFunction(SortMethod, module);
            if (sortFunction == null)
            {
                if (SortMode == SortMode.Descending)
                    list.ToList().Reverse();
                return list;
            }

            try
            {
                if (SortMode == SortMode.Ascending)
                    return list.OrderBy(obj => obj.Name.ToLower().StartsWith("the ") ? obj.Name.Substring(4) : obj.Name).OrderBy(sortFunction).ToList();
                else if (SortMode == SortMode.Descending)
                    return list.OrderBy(obj => obj.Name.ToLower().StartsWith("the ") ? obj.Name.Substring(4) : obj.Name).OrderByDescending(sortFunction).ToList();
                else
                    throw new ListManipulationException("Unhandled sort mode", SortMode);
            }
            catch (InvalidCastException e)
            {
                throw new ListManipulationException(GetType().Name + " - Chosen sort method requires all objects in list be a more specific derived type (" + e.GetType().Name + ": " + e.Message + ")", SortMethod);
            }
        }

        protected virtual Func<RankedObject, object> GetSortFunction(int sortMethod, TrackerModule module)
        {
            Func<RankedObject, object> sortFunction = null;
            switch (sortMethod)
            {
                case SORT_Name:
                    sortFunction = obj => obj.Name.ToLower().StartsWith("the ") ? obj.Name.Substring(4) : obj.Name;
                    break;
                case SORT_HasComment:
                    sortFunction = obj => obj.Comment.Length > 0;
                    break;
            }
            return sortFunction;
        }
    }
}
