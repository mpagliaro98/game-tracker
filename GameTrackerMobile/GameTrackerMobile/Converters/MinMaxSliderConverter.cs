using GameTracker.Model;
using GameTrackerMobile.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    internal class MinMaxSliderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // input : CategoryValue
            double val = (double)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            double min = rm.Settings.MinScore;
            double max = rm.Settings.MaxScore;
            if (max - min == 0)
                return 0.0;
            double result = (val - min) / (max - min);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // input : Slider
            double val = (double)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            double min = rm.Settings.MinScore;
            double max = rm.Settings.MaxScore;
            return ((max - min) * val) + min;
        }
    }
}
