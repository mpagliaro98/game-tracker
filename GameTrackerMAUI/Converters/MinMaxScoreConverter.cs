using GameTrackerMAUI.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Converters
{
    public class MinMaxScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value from slider
            double val = (double)value;
            var provider = Application.Current.Handler.MauiContext.Services;
            double min = provider.GetSharedDataService().Settings.MinScore;
            double max = provider.GetSharedDataService().Settings.MaxScore;
            return ((max - min) * val) + min;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // textbox entered value
            if (((string)value).Trim() == "")
                return 0.0;
            double val = double.Parse((string)value);
            var provider = Application.Current.Handler.MauiContext.Services;
            double min = provider.GetSharedDataService().Settings.MinScore;
            double max = provider.GetSharedDataService().Settings.MaxScore;
            if (val - min == 0)
                return 0.0;
            return (val - min) / (max - min);
        }
    }
}
