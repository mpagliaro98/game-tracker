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

        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color? color)
        {
            var colorInput = color ?? new System.Windows.Media.Color();
            return System.Drawing.Color.FromArgb(colorInput.A, colorInput.R, colorInput.G, colorInput.B);
        }

        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color? color)
        {
            var colorInput = color ?? new System.Drawing.Color();
            return System.Windows.Media.Color.FromArgb(colorInput.A, colorInput.R, colorInput.G, colorInput.B);
        }

        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
