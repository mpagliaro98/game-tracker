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
    }
}
