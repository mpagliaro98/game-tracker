using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public static class Util
    {
        public static T FindObjectInList<T>(IList<T> list, UniqueID uniqueID) where T : IKeyable
        {
            foreach (T item in list)
            {
                if (item.UniqueID.Equals(uniqueID)) return item;
            }
            return default; // null
        }

        public static IEnumerable<string> ConvertListToStringList<T>(IEnumerable<T> vals)
        {
            LinkedList<string> list = new LinkedList<string>();
            foreach (T value in vals)
            {
                list.AddLast(value.ToString());
            }
            return list;
        }
    }
}
