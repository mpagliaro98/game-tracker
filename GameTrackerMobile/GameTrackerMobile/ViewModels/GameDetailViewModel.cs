using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameTracker.Model;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class GameDetailViewModel : BaseViewModel<RatableGame>
    {
        private RatableGame item = new RatableGame();

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public RatableGame Item
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

        public GameDetailViewModel()
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
            await Shell.Current.GoToAsync($"{nameof(NewGamePage)}?{nameof(NewGameViewModel.ItemId)}={new ObjectReference(item)}");
        }

        async void OnDelete()
        {
            var popup = new PopupPage("Attention", "Are you sure you would like to delete this game?", PopupViewModel.EnumInputType.YesNo);
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
