using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Converters
{
    public class PlatformColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectReference key = (ObjectReference)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            Platform platform = rm.FindPlatform(key);
            if (platform == null) return new Color();
            RatableTracker.Framework.Color color = platform.Color;
            return color.ToXamarinColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
