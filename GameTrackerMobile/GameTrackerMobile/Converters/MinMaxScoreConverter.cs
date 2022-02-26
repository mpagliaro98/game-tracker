using GameTracker.Model;
using GameTrackerMobile.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    internal class MinMaxScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value from slider
            double val = (double)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            double min = rm.Settings.MinScore;
            double max = rm.Settings.MaxScore;
            return ((max - min) * val) + min;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // textbox entered value
            if (((string)value).Trim() == "")
                return 0.0;
            double val = double.Parse((string)value);
            RatingModuleGame rm = ModuleService.GetActiveModule();
            double min = rm.Settings.MinScore;
            double max = rm.Settings.MaxScore;
            if (val - min == 0)
                return 0.0;
            return (val - min) / (max - min);
        }
    }
}
