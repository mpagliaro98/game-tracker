using GameTrackerMobile.Views;
using RatableTracker.Framework;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ScoreRangeDetailViewModel : BaseViewModel<ScoreRange>
    {
        private ScoreRange item = new ScoreRange();

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public ScoreRange Item
        {
            get => item;
            set => SetProperty(ref item, value);
        }

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public ScoreRangeDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
        }

        public async void LoadItemId(ObjectReference itemId)
        {
            try
            {
                Item = await DataStore.GetItemAsync(itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewScoreRangePage)}?{nameof(NewScoreRangeViewModel.ItemId)}={new ObjectReference(item)}");
        }

        async void OnDelete()
        {
            var ret = await Util.ShowPopupAsync("Attention", "Are you sure you would like to delete this score range?", PopupViewModel.EnumInputType.YesNo);
            if (ret.Item1.ToString().ToUpper() == "YES")
            {
                await DataStore.DeleteItemAsync(Item);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
