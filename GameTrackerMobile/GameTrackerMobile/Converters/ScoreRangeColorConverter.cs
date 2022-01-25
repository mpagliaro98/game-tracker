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
    public class ScoreRangeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectReference key = (ObjectReference)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            RatableGame game = rm.FindListedObject(key);
            if (game == null) return new Xamarin.Forms.Color();
            RatableTracker.Framework.Color color = rm.GetRangeColorFromObject(game);
            return color.ToXamarinColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
