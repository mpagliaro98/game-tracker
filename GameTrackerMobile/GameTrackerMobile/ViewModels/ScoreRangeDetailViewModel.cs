using GameTrackerMobile.Views;
using RatableTracker.Framework;
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
    }
}
