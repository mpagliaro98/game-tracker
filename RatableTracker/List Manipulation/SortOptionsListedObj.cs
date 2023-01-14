using RatableTracker.Framework;
using RatableTracker.Framework.ModuleHierarchy;
using RatableTracker.Framework.ObjectHierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.List_Manipulation
{
    public class SortOptionsListedObj<TListedObj, TModule, TRange, TSettings>
        where TListedObj : ListedObject
        where TModule : RankingModule<TListedObj, TRange, TSettings>
        where TRange : ScoreRange
        where TSettings : Settings, new()
    {
        public const int SORT_None = 0;
        public const int SORT_Name = 1;
        public const int SORT_HasComment = 2;

        private readonly int SortMethod = SORT_None;
        private readonly SortMode SortMode = SortMode.ASCENDING;

        public SortOptionsListedObj(int sortMethod, SortMode sortMode)
        {
            SortMethod = sortMethod;
            SortMode = sortMode;
        }

        public IEnumerable<TListedObj> ApplySorting(IEnumerable<TListedObj> list, TModule rm)
        {
            Func<TListedObj, object> sortFunction = GetSortFunction(SortMethod, rm);
            if (sortFunction == null)
            {
                if (SortMode == SortMode.DESCENDING)
                    list = list.Reverse();
                return list;
            }

            if (SortMode == SortMode.ASCENDING)
                return list.OrderBy(obj => obj.Name.ToLower().StartsWith("the ") ? obj.Name.Substring(4) : obj.Name).OrderBy(sortFunction);
            else if (SortMode == SortMode.DESCENDING)
                return list.OrderBy(obj => obj.Name.ToLower().StartsWith("the ") ? obj.Name.Substring(4) : obj.Name).OrderByDescending(sortFunction);
            else
                throw new Exception("Unhandled sort mode");
        }

        protected virtual Func<TListedObj, object> GetSortFunction(int sortMethod, TModule rm)
        {
            Func<TListedObj, object> sortFunction = null;
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
