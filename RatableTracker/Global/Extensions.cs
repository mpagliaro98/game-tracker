using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Global
{
    public static partial class Extensions
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
    }
}
