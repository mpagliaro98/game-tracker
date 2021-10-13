using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    partial class ValueContainer
    {
        public string ConvertValueToJSON()
        {
            string jsonObj = "";
            if (HasValue())
            {
                if (IsValueAList())
                {
                    if (IsValueObjectList())
                        jsonObj += Util.CreateJSONArray(GetContentSavableRepresentationList());
                    else
                        jsonObj += Util.CreateJSONArray(GetContentStringList());
                }
                else
                {
                    if (IsValueObject())
                        jsonObj += GetContentSavableRepresentation().ConvertToJSON();
                    else
                        jsonObj += "\"" + Util.FixJSONSpecialChars(GetContentString()) + "\"";
                }
            }
            else
            {
                jsonObj += "\"\"";
            }
            return jsonObj;
        }
    }
}
