using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    public class DecimalPlaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double) || !int.TryParse(parameter.ToString(), out _))
                return "";
            string format = "0.";
            int param = int.Parse(parameter.ToString());
            for (int i = 0; i < param; i++)
                format += "#";
            return ((double)value).ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
