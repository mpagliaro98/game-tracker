using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameTracker.Model;
using RatableTracker.Framework;
using Xamarin.Forms;
using GameTrackerMobile.Views;
using Rg.Plugins.Popup.Services;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class StatusDetailViewModel : BaseViewModel<CompletionStatus>
    {
        private CompletionStatus item = new CompletionStatus();

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

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
            await Shell.Current.GoToAsync($"{nameof(NewStatusPage)}?{nameof(NewStatusViewModel.ItemId)}={new ObjectReference(item)}");
        }

        async void OnDelete()
        {
            var popup = new PopupPage("Attention", "Are you sure you would like to delete this status?", PopupViewModel.EnumInputType.YesNo);
            await PopupNavigation.Instance.PushAsync(popup);
            var ret = await popup.PopupClosedTask;
            if (ret.Item1.ToString().ToUpper() == "YES")
            {
                await DataStore.DeleteItemAsync(Item);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
