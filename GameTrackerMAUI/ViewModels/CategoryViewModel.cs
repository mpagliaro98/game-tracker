using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public class CategoryViewModel : BaseViewModel
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
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);

            ItemTapped = new Command<RatingCategoryWeighted>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return SharedDataService.Module.CategoryExtension.GetRatingCategoryList().Count < SharedDataService.Module.CategoryExtension.LimitRatingCategories;
        }

        void ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = SharedDataService.Module.CategoryExtension.GetRatingCategoryList().OfType<RatingCategoryWeighted>().ToList();
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

            await Shell.Current.GoToAsync($"{nameof(CategoryDetailPage)}?{nameof(CategoryDetailViewModel.ItemId)}={item.UniqueID}");
        }
    }
}
