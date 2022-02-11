using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    public class ScoreRelationshipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectReference key = (ObjectReference)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            ScoreRange range = rm.FindRange(key);
            if (range == null) return "";
            string result = rm.FindScoreRelationship(range.RefScoreRelationship).Name + " " + GetRelationshipValues(range.ValueList);
            return result;
        }

        private string GetRelationshipValues(IEnumerable<double> valueList)
        {
            List<double> list = valueList.ToList();
            string result = "";
            for (int i = 0; i < valueList.Count(); i += 1)
            {
                if (result != "" && valueList.Count() > 2) result += ", ";
                if (result != "" && i == valueList.Count() - 1) result += " and ";
                result += list[i].ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
