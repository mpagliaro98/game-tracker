using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Model;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    public class PlatformsViewModel : BaseViewModel<Platform>
    {
        private Platform _selectedItem;

        public ObservableCollection<Platform> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Platform> ItemTapped { get; }

        public Platform SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public PlatformsViewModel()
        {
            Title = "Platforms";
            Items = new ObservableCollection<Platform>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Platform>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewPlatformPage));
        }

        async void OnItemSelected(Platform item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(PlatformDetailPage)}?{nameof(PlatformDetailViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
