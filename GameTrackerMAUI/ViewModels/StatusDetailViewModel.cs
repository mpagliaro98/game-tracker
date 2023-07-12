using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class StatusDetailViewModel : BaseViewModel
    {
        private StatusGame _item = new StatusGame(SharedDataService.Module, SharedDataService.Settings);

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public StatusGame Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        public string ItemId
        {
            get => Item.UniqueID.ToString();
            set
            {
                var key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public StatusDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = (StatusGame)SharedDataService.Module.StatusExtension.GetStatusList().First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewStatusPage)}?{nameof(NewStatusViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnDelete()
        {
            Tuple<PopupMain.EnumOutputType, string> ret = (Tuple<PopupMain.EnumOutputType, string>)await ShowPopupAsync(new PopupMain("Attention", "Are you sure you would like to delete this status?", PopupMain.EnumInputType.YesNo));
            if (ret.Item1 == PopupMain.EnumOutputType.Yes)
            {
                Item.Delete(SharedDataService.Module, SharedDataService.Settings);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
