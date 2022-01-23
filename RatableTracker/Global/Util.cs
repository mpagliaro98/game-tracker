using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Global
{
    public static partial class Util
    {
        public static IEnumerable<string> ConvertListToStringList<T>(IEnumerable<T> vals)
        {
            LinkedList<string> list = new LinkedList<string>();
            foreach (T value in vals)
            {
                list.AddLast(value.ToString());
            }
            return list;
        }

        public static void UpdateInListOnCondition<T>(IEnumerable<T> list, Func<T, bool> where, Action<T> action)
        {
            List<T> temp = list.ToList();
            temp.Where(where).ForEach(action);
        }

        public static void DeleteFromListOnCondition<T>(ref IEnumerable<T> list, Predicate<T> where)
        {
            List<T> temp = list.ToList();
            temp.RemoveAll(where);
            list = temp.AsEnumerable();
        }
    }
}
