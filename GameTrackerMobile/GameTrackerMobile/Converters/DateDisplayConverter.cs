using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Converters
{
    public class DateDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is DateTime) ? ((DateTime)value).ToShortDateString() : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
