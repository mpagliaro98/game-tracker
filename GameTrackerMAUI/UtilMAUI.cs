using CommunityToolkit.Maui.Views;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTrackerMAUI.Services;
#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
#endif

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

        public static async Task<Tuple<PopupList.EnumOutputType, object>?> ShowPopupListAsync(string title, IEnumerable<PopupListOption> options, object selectedValue, Action<PopupListOption, int>? doubleTap = null)
        {
            return (Tuple<PopupList.EnumOutputType, object>?)await ShowPopupAsync(new PopupList(title, options, selectedValue, doubleTap));
        }
#nullable disable

#if ANDROID
    // fix for MAUI bug where certain storage permissions don't work on Android 13 (API 33)
    public static async Task<bool> RequestPermissionAsync(IToastService toastService)
    {
        var activity = Platform.CurrentActivity ?? throw new NullReferenceException("Current activity is null");

        if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.ReadExternalStorage) == Permission.Granted)
        {
            return true;
        }

        if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.ReadExternalStorage))
        {
            await toastService.ShowToastAsync("Please grant access to external storage");
        }

        ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.ReadExternalStorage }, 1);

        return false;
    }
#endif
    }
}
