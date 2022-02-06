using GameTrackerMobile.Views;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    public class ScoreRangeViewModel : BaseViewModel<ScoreRange>
    {
        private ScoreRange _selectedItem;

        public ObservableCollection<ScoreRange> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<ScoreRange> ItemTapped { get; }

        public ScoreRange SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public ScoreRangeViewModel()
        {
            Title = "Score Ranges";
            Items = new ObservableCollection<ScoreRange>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<ScoreRange>(OnItemSelected);

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
            await Shell.Current.GoToAsync(nameof(NewScoreRangePage));
        }

        async void OnItemSelected(ScoreRange item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(ScoreRangeDetailPage)}?{nameof(ScoreRangeDetailViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
