using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Interfaces;
using Newtonsoft.Json.Linq;

namespace RatableTracker.Framework.LoadSave
{
    public partial class SavableRepresentation<TValCont>
        where TValCont : IValueContainer<TValCont>, new()
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

        public static SavableRepresentation<TValCont> LoadFromJSON(string json)
        {
            if (json == "")
            {
                return new SavableRepresentation<TValCont>();
            }
            if (!Util.IsValidJSONObject(json))
            {
                throw new Exception("SavableRepresentation LoadFromJSON: object is not valid json: " + json);
            }
            SavableRepresentation<TValCont> sr = new SavableRepresentation<TValCont>();
            JObject root = JObject.Parse(json);
            foreach (KeyValuePair<string, JToken> node in root)
            {
                if (node.Value is JArray)
                {
                    // Node is an array
                    if (((JArray)node.Value).First is JValue)
                    {
                        // Array is all single values (assumes all values are same type)
                        LinkedList<string> stringList = new LinkedList<string>();
                        var objects = node.Value;
                        foreach (JValue childRoot in objects)
                        {
                            stringList.AddLast(childRoot.Value.ToString());
                        }
                        sr.SaveList(node.Key, stringList);
                    }
                    else if (((JArray)node.Value).First is JObject)
                    {
                        // Array is all json objects (assumes all values are same type)
                        LinkedList<SavableRepresentation<TValCont>> srList = new LinkedList<SavableRepresentation<TValCont>>();
                        var objects = node.Value;
                        foreach (JObject childRoot in objects)
                        {
                            string jsonObj = childRoot.ToString();
                            SavableRepresentation<TValCont> srChild = LoadFromJSON(jsonObj);
                            srList.AddLast(srChild);
                        }
                        sr.SaveList(node.Key, srList);
                    }
                }
                else if (node.Value is JValue)
                {
                    // Node is a single value
                    sr.SaveValue(node.Key, node.Value.ToString());
                }
                else if (node.Value is JObject)
                {
                    // Node is a json array
                    sr.SaveValue(node.Key, LoadFromJSON(node.Value.ToString()));
                }
            }
            return sr;
        }
    }
}
