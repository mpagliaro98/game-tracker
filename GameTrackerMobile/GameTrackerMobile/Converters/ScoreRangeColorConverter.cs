using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GameTracker.Model;
using GameTrackerMobile.Services;
using GameTrackerMobile.ViewModels;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using Xamarin.Forms;

namespace GameTrackerMobile.Converters
{
    public class ScoreRangeColorConverter : IValueConverter
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
            if (game == null) return new Xamarin.Forms.Color();
            RatableTracker.Framework.Color color;
            switch (SavedState.GameSortMode)
            {
                case int n when n >= GamesViewModel.SORT_CATEGORY_START:
                    RatingCategoryWeighted selectedCat = null;
                    int i = GamesViewModel.SORT_CATEGORY_START;
                    foreach (var cat in ModuleService.GetActiveModule().RatingCategories)
                    {
                        if (i++ == n)
                        {
                            selectedCat = cat;
                            break;
                        }
                    }
                    if (selectedCat == null)
                        color = rm.GetRangeColorFromObject(game);
                    else
                        color = rm.GetRangeColorFromValue(rm.GetScoreOfCategory(game, selectedCat));
                    break;
                default:
                    color = rm.GetRangeColorFromObject(game);
                    break;
            }
            return color.ToXamarinColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
