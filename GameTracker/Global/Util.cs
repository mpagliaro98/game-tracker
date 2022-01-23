using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Interfaces;
using Newtonsoft.Json.Linq;

namespace GameTracker.Global
{
    public static class Util
    {
        public static string FixJSONSpecialChars(string str)
        {
            return str.Replace("\"", "\\\"");
        }

        public static string CreateJSONArray<T>(IEnumerable<SavableRepresentation<T>> objs) where T : IValueContainer<T>, new()
        {
            IEnumerable<string> jsonObjs = new LinkedList<string>();
            foreach (SavableRepresentation<T> obj in objs)
            {
                string json = obj.ConvertToJSON();
                jsonObjs = jsonObjs.Append(json).ToList();
            }
            string jsonArray = string.Join(",", jsonObjs);
            return "[" + jsonArray + "]";
        }

        public static string CreateJSONArray<T>(IEnumerable<ISavable> objs) where T : IValueContainer<T>, new()
        {
            IEnumerable<string> jsonObjs = new LinkedList<string>();
            foreach (ISavable obj in objs)
            {
                SavableRepresentation<T> sr = obj.LoadIntoRepresentation<T>();
                string json = sr.ConvertToJSON();
                jsonObjs = jsonObjs.Append(json).ToList();
            }
            string jsonArray = string.Join(",", jsonObjs);
            return "[" + jsonArray + "]";
        }

        public static string CreateJSONArray(IEnumerable<string> objs)
        {
            IEnumerable<string> jsonObjs = new LinkedList<string>();
            foreach (string obj in objs)
            {
                string json = "\"" + FixJSONSpecialChars(obj) + "\"";
                jsonObjs = jsonObjs.Append(json).ToList();
            }
            string jsonArray = string.Join(",", jsonObjs);
            return "[" + jsonArray + "]";
        }

        public static bool IsValidJSONObject(string json)
        {
            try
            {
                JToken root = JToken.Parse(json);
                return root is JObject;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidJSONArray(string json)
        {
            try
            {
                JToken root = JToken.Parse(json);
                return root is JArray;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
