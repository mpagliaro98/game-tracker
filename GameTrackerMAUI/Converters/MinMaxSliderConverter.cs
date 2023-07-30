using GameTrackerMAUI.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Converters
{
    public class MinMaxSliderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // input : CategoryValue
            double val = (double)value;
            var provider = Application.Current.Handler.MauiContext.Services;
            double min = provider.GetSharedDataService().Settings.MinScore;
            double max = provider.GetSharedDataService().Settings.MaxScore;
            if (max - min == 0)
                return 0.0;
            double result = (val - min) / (max - min);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // input : Slider
            double val = (double)value;
            var provider = Application.Current.Handler.MauiContext.Services;
            double min = provider.GetSharedDataService().Settings.MinScore;
            double max = provider.GetSharedDataService().Settings.MaxScore;
            return ((max - min) * val) + min;
        }
    }
}
