using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Converters
{
    public class FilterTypeVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            IFilterOption option = (IFilterOption)value;
            FilterType filterType = (FilterType)int.Parse(parameter.ToString());
            return option.FilterType == filterType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
