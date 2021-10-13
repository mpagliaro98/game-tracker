using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
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
    }
}
