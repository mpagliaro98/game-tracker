using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    public class GamePlatformNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectReference key = (ObjectReference)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            RatableGame game = rm.FindListedObject(key);
            if (game == null) return "";
            Platform platform = null;
            if (game.RefPlatform.HasReference())
                platform = rm.FindPlatform(game.RefPlatform);
            return game.Name + (platform != null ? " (" + (platform.Abbreviation != "" ? platform.Abbreviation : platform.Name) + ")" : "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
