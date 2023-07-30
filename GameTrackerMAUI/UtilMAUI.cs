using CommunityToolkit.Maui.Views;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace GameTrackerMAUI
{
    public static class UtilMAUI
    {
        public static RatableTracker.Util.Color ToFrameworkColor(this Color? color)
        {
            var colorInput = color ?? new Color();
            return RatableTracker.Util.Color.FromArgb(Convert.ToByte(colorInput.Alpha * 255), Convert.ToByte(colorInput.Red * 255),
                Convert.ToByte(colorInput.Green * 255), Convert.ToByte(colorInput.Blue * 255));
        }

        public static Color ToMAUIColor(this RatableTracker.Util.Color? color)
        {
            var colorInput = color ?? new RatableTracker.Util.Color();
            return Color.FromRgba(colorInput.R, colorInput.G, colorInput.B, colorInput.A);
        }

        public static Color ToMAUIColor(this RatableTracker.Util.Color color)
        {
            return Color.FromRgba(color.R, color.G, color.B, color.A);
        }

#nullable enable
        public static async Task<object?> ShowPopupAsync<T>(T popup) where T : Popup
        {
            return await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }

        public static async Task<Tuple<PopupList.EnumOutputType, object>?> ShowPopupListAsync(string title, IEnumerable<PopupListOption> options, object selectedValue)
        {
            return (Tuple<PopupList.EnumOutputType, object>?)await ShowPopupAsync(new PopupList(title, options, selectedValue));
        }
#nullable disable
    }
}
