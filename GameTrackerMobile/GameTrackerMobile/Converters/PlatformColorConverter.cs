using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    public class PlatformColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectReference key = (ObjectReference)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            Platform platform = rm.FindPlatform(key);
            var color = platform.Color;
            return "Red";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
