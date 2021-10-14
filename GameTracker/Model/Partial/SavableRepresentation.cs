using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Global;
using Newtonsoft.Json.Linq;

namespace RatableTracker.Framework.LoadSave
{
    public partial class SavableRepresentation
    {
        public string ConvertToJSON()
        {
            string json = "";
            foreach (string key in GetAllSavedKeys())
            {
                if (json != "") json += ",";
                string jsonObj = "\"" + Util.FixJSONSpecialChars(key) + "\":" + ConvertValueToJSON(key);
                json += jsonObj;
            }
            json = "{" + json + "}";
            return json;
        }

        public string ConvertValueToJSON(string key)
        {
            return values[key].ConvertValueToJSON();
        }

        public static SavableRepresentation LoadFromJSON(string json)
        {
            if (json == "")
            {
                return new SavableRepresentation();
            }
            if (!Util.IsValidJSONObject(json))
            {
                throw new Exception("SavableRepresentation LoadFromJSON: object is not valid json: " + json);
            }
            SavableRepresentation sr = new SavableRepresentation();
            JObject root = JObject.Parse(json);
            foreach (KeyValuePair<string, JToken> node in root)
            {
                if (node.Value is JArray)
                {
                    IEnumerable<SavableRepresentation> srList = new LinkedList<SavableRepresentation>();
                    var objects = node.Value;
                    foreach (JObject childRoot in objects)
                    {
                        string jsonObj = childRoot.ToString();
                        SavableRepresentation srChild = LoadFromJSON(jsonObj);
                        srList = srList.Append(srChild).ToList();
                    }
                    sr.SaveList(node.Key, srList);
                }
                else if (node.Value is JValue)
                {
                    sr.SaveValue(node.Key, node.Value.ToString());
                }
                else if (node.Value is JObject)
                {
                    sr.SaveValue(node.Key, LoadFromJSON(node.Value.ToString()));
                }
            }
            return sr;
        }
    }
}
