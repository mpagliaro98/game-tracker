using RatableTracker.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Converters
{
    public class ScoreRelationshipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            ScoreRange range = (ScoreRange)value;
            if (range == null) return "";
            if (range.ScoreRelationship == null) return "";
            string result = range.ScoreRelationship.Name + " " + GetRelationshipValues(range.ValueList);
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
