using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace GameTracker.Global
{
    public static class ValueContainerExt
    {
        public static string ConvertValueToJSON<TValCont>(this IValueContainer<TValCont> vc) where TValCont : IValueContainer<TValCont>, new()
        {
            string jsonObj = "";
            if (vc.HasValue())
            {
                if (vc.IsValueAList())
                {
                    if (vc.IsValueObjectList())
                        jsonObj += Util.CreateJSONArray(vc.GetContentSavableRepresentationList());
                    else
                        jsonObj += Util.CreateJSONArray(vc.GetContentStringList());
                }
                else
                {
                    if (vc.IsValueObject())
                        jsonObj += vc.GetContentSavableRepresentation().ConvertToJSON();
                    else
                        jsonObj += "\"" + Util.FixJSONSpecialChars(vc.GetContentString()) + "\"";
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
