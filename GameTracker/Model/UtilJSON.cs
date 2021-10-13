using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public static partial class Util
    {
        public static string FixJSONSpecialChars(string str)
        {
            return str.Replace("\"", "\\\"");
        }

        public static string CreateJSONArray(IEnumerable<SavableRepresentation> objs)
        {
            IEnumerable<string> jsonObjs = new LinkedList<string>();
            foreach (SavableRepresentation obj in objs)
            {
                string json = obj.ConvertToJSON();
                jsonObjs = jsonObjs.Append(json).ToList();
            }
            string jsonArray = string.Join(",", jsonObjs);
            return "[" + jsonArray + "]";
        }

        public static string CreateJSONArray(IEnumerable<ISavable> objs)
        {
            IEnumerable<string> jsonObjs = new LinkedList<string>();
            foreach (ISavable obj in objs)
            {
                SavableRepresentation sr = obj.LoadIntoRepresentation();
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
    }
}
