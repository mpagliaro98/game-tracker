using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Converters
{
    public class ShowFinalScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectReference key = (ObjectReference)value;
            RatingModuleGame rm = ModuleService.GetActiveModule();
            CompletionStatus status = rm.FindStatus(key);
            if (status == null) return false;
            return status.UseAsFinished;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
