using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    public class FinalScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectReference key = (ObjectReference)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            RatableGame game;
            try
            {
                game = rm.FindListedObject(key);
            }
            catch (ReferenceNotFoundException)
            {
                game = rm.FindGameCompilation(key);
            }
            double score = rm.GetScoreOfObject(game);
            return score;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
