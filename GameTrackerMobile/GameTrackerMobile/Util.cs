using GameTrackerMobile.ViewModels;
using GameTrackerMobile.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMobile
{
    public static class Util
    {
        public static async Task<Tuple<PopupViewModel.EnumOutputType, string>> ShowPopupAsync(string title, string message, PopupViewModel.EnumInputType inputType)
        {
            var popup = new PopupPage(title, message, inputType);
            await PopupNavigation.Instance.PushAsync(popup);
            var ret = await popup.PopupClosedTask;
            return ret;
        }

        public static async Task<Tuple<PopupListViewModel.EnumOutputType, int?>> ShowPopupListAsync(string title, IEnumerable<PopupListOption> options, int? selectedValue)
        {
            var popup = new PopupListPage(title, options, selectedValue);
            await PopupNavigation.Instance.PushAsync(popup);
            var ret = await popup.PopupClosedTask;
            return ret;
        }
    }
}
