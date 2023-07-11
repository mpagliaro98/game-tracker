using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.Converters
{
    public class PopupListSelectedValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int? val = (int?)values[0];
            int? selected = (int?)values[1];
            if (!val.HasValue || !selected.HasValue)
                return FontAttributes.None;
            return val.Value == selected.Value ? FontAttributes.Bold : FontAttributes.None;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
