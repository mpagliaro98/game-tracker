using GameTrackerMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Converters
{
    public class PopupTypeToIsVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter is null || !(parameter is string))
                return false;

            string[] pars = ((string)parameter).Split('|');

            if (value is PopupViewModel.EnumInputType && pars != null)
            {
                switch ((PopupViewModel.EnumInputType)value)
                {
                    case PopupViewModel.EnumInputType.Ok:
                        if (Array.IndexOf(pars, "OK") >= 0)
                            return true;

                        break;
                    case PopupViewModel.EnumInputType.YesNo:
                        if (Array.IndexOf(pars, "YESNO") >= 0)
                            return true;

                        break;
                    case PopupViewModel.EnumInputType.OkCancel:
                        if (Array.IndexOf(pars, "OKCANCEL") >= 0)
                            return true;

                        break;
                    case PopupViewModel.EnumInputType.OkCancelWithInput:
                        if (Array.IndexOf(pars, "OKCANCELWITHINPUT") >= 0)
                            return true;

                        break;
                    default:
                        return false;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
