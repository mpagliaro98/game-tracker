using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ScoreRanges;
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
    public class ScoreRangeDetailViewModel : BaseViewModel
    {
        private ScoreRange _item = new ScoreRange(SharedDataService.Module, SharedDataService.Settings);

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public ScoreRange Item
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

        public ScoreRangeDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = SharedDataService.Module.GetScoreRangeList().First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewScoreRangePage)}?{nameof(NewScoreRangeViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnDelete()
        {
            var ret = await UtilMAUI.ShowPopupMainAsync("Attention", "Are you sure you would like to delete this score range?", PopupMain.EnumInputType.YesNo);
            if (ret.Item1 == PopupMain.EnumOutputType.Yes)
            {
                Item.Delete(SharedDataService.Module, SharedDataService.Settings);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
