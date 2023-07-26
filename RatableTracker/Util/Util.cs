using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    public static class Util
    {
        public static Encoding TextEncoding => Encoding.UTF8;

        public static VersionNumber FrameworkVersion => new VersionNumber(2, 0, 0, 0);

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

        public static string FormatUnhandledExceptionMessage(Exception e, string source = null)
        {
            string message = "\n" + new string('=', 60) + "\nUNHANDLED EXCEPTION - " + (source ?? "Unknown Source") + "\n";
            System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetAssembly(e.GetType()).GetName();
            message += "Assembly: " + assemblyName.Name + "\n";
            message += e.GetType().Name + ": " + e.Message + "\n";
            message += e.StackTrace + "\n";
            message += new string('=', 60);
            return message;
        }

        public static ScoreRange GetScoreRange(double score, TrackerModuleScores module)
        {
            foreach (ScoreRange range in module.GetScoreRangeList())
            {
                try
                {
                    if (range.ScoreRelationship.IsValueInRange(score, range.ValueList)) return range;
                }
                catch (InvalidObjectStateException e)
                {
                    module.Logger.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            return null;
        }
    }
}
