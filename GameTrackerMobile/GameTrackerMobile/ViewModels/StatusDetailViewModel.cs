using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameTracker.Model;
using RatableTracker.Framework;
using Xamarin.Forms;
using GameTrackerMobile.Views;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class StatusDetailViewModel : BaseViewModel<CompletionStatus>
    {
        private CompletionStatus item = new CompletionStatus();

        public Command EditCommand { get; }

        public CompletionStatus Item
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

        public StatusDetailViewModel()
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
            await Shell.Current.GoToAsync($"{nameof(NewStatusPage)}?{nameof(NewStatusViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
