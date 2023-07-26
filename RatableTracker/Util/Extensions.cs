using RatableTracker.Exceptions;
using RatableTracker.Modules;
using RatableTracker.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Util
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

        public static ScoreRange GetScoreRange(this double input, TrackerModuleScores module)
        {
            foreach (ScoreRange range in module.GetScoreRangeList())
            {
                try
                {
                    if (range.ScoreRelationship.IsValueInRange(input, range.ValueList)) return range;
                }
                catch (InvalidObjectStateException e)
                {
                    module.Logger.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            return null;
        }

        public static double AverageIfEmpty(this IEnumerable<double> source, double useIfEmpty)
        {
            try
            {
                source.ThrowIfNull("source");
                return source.Average();
            }
            catch (InvalidOperationException)
            {
                return useIfEmpty;
            }
        }

        public static T Replace<T>(this IList<T> source, T value)
        {
            source.ThrowIfNull("source");
            int indexReplace = -1;
            for (int i = 0; i < source.Count; i++)
            {
                var find = source[i];
                if (find.Equals(value))
                {
                    indexReplace = i;
                    break;
                }
            }
            if (indexReplace >= 0)
            {
                var temp = source[indexReplace];
                source[indexReplace] = value;
                return temp;
            }
            else
                throw new InvalidObjectStateException("Object " + value.ToString() + " was not found in list");
        }

        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            type.ThrowIfNull("type");

            PropertyInfo[] props = null;
            do
            {
                var newProps = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (props == null)
                    props = newProps;
                else
                    props = props.Union(newProps).ToArray();
                type = type.BaseType;
            } while (type != null);

            return props ?? Array.Empty<PropertyInfo>();
        }

        public static FieldInfo[] GetAllFields(this Type type)
        {
            type.ThrowIfNull("type");

            FieldInfo[] fields = null;
            do
            {
                var newFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (fields == null)
                    fields = newFields;
                else
                    fields = fields.Union(newFields).ToArray();
                type = type.BaseType;
            } while (type != null);

            return fields ?? Array.Empty<FieldInfo>();
        }

        public static string ToDisplayString(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            EnumDisplayAttribute attr = (EnumDisplayAttribute)Attribute.GetCustomAttribute(field, typeof(EnumDisplayAttribute));
            return attr == null ? value.ToString() : attr.Name;
        }

        public static DateTime StartOfWeek(this DateTime value)
        {
            return value.Date.AddDays(-(int)(value.DayOfWeek));
        }

        public static DateTime EndOfWeek(this DateTime value)
        {
            return value.StartOfWeek().AddDays(7).AddTicks(-1);
        }

        public static DateTime StartOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime value)
        {
            return value.StartOfMonth().AddMonths(1).AddTicks(-1);
        }

        public static DateTime StartOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1);
        }

        public static DateTime EndOfYear(this DateTime value)
        {
            return value.StartOfYear().AddYears(1).AddTicks(-1);
        }
    }
}
