using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using RatableTracker.Framework.ModuleHierarchy;

namespace GameTrackerMobile.ViewModels
{
    public class CategoryViewModel : BaseViewModel<RatingCategoryWeighted>
    {
        private RatingCategoryWeighted _selectedItem;

        public ObservableCollection<RatingCategoryWeighted> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<RatingCategoryWeighted> ItemTapped { get; }

        public RatingCategoryWeighted SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public CategoryViewModel()
        {
            Title = "Rating Categories";
            Items = new ObservableCollection<RatingCategoryWeighted>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<RatingCategoryWeighted>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return ModuleService.GetActiveModule().RatingCategories.Count() < ModuleService.GetActiveModule().LimitRatingCategories;
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
            await Shell.Current.GoToAsync(nameof(NewCategoryPage));
        }

        async void OnItemSelected(RatingCategoryWeighted item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(CategoryDetailPage)}?{nameof(CategoryDetailViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
