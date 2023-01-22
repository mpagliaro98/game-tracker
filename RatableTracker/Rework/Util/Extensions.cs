using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public static class Extensions
    {
        public static void ThrowIfNull<T>(this T source, string message)
        {
            if (source == null)
                throw new NullReferenceException("ForEach: " + message + " type " + source.ToString() + " is null");
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.ThrowIfNull("source");
            action.ThrowIfNull("action");
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static IEnumerable<T2> ForEach<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> action)
        {
            source.ThrowIfNull("source");
            action.ThrowIfNull("action");
            List<T2> results = new List<T2>();
            foreach (T1 element in source)
            {
                results.Add(action(element));
            }
            return results;
        }

        public static T RemoveAtAndReturn<T>(this IList<T> source, int index)
        {
            source.ThrowIfNull("source");
            T obj = source.ElementAt(index);
            source.RemoveAt(index);
            return obj;
        }

        public static void Move<T>(this IList<T> source, int oldIndex, int newIndex)
        {
            source.ThrowIfNull("source");
            T obj = source.RemoveAtAndReturn(oldIndex);
            source.Insert(newIndex, obj);
        }

        public static void Move<T>(this IList<T> source, T obj, int newIndex)
        {
            source.ThrowIfNull("source");
            int oldIndex = source.IndexOf(obj);
            source.RemoveAt(oldIndex);
            source.Insert(newIndex, obj);
        }

        public static string CleanForSorting(this string input)
        {
            return input.ToLower().StartsWith("the ") ? input.Substring(4) : input;
        }
    }
}
