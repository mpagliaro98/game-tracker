using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    public class ScoreRangeValueVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int numValuesRequired = (int)value;
            int slot = int.Parse(parameter.ToString());
            return slot <= numValuesRequired;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
