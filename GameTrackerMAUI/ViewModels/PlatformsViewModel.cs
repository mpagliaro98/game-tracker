using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GameTracker;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Sorting;

namespace GameTrackerMAUI.ViewModels
{
    public class PlatformsViewModel : BaseViewModel
    {
        private GameTracker.Platform _selectedItem;

        public ObservableCollection<GameTracker.Platform> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command SortCommand { get; }
        public Command SortDirectionCommand { get; }
        public Command SearchCommand { get; }
        public Command<GameTracker.Platform> ItemTapped { get; }

        public GameTracker.Platform SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private string _sortDirectionButtonText = "Asc";
        public string SortDirectionButtonText
        {
            get => _sortDirectionButtonText;
            set => SetProperty(ref _sortDirectionButtonText, value);
        }

        private string _sortDirectionImageName = "sort_ascending";
        public string SortDirectionImageName
        {
            get => _sortDirectionImageName;
            set => SetProperty(ref _sortDirectionImageName, value);
        }

        private string _sortImageName = "sort";
        public string SortImageName
        {
            get => _sortImageName;
            set => SetProperty(ref _sortImageName, value);
        }

        private string _searchImageName = "search";
        public string SearchImageName
        {
            get => _searchImageName;
            set => SetProperty(ref _searchImageName, value);
        }

        public PlatformsViewModel()
        {
            Title = "Platforms";
            Items = new ObservableCollection<GameTracker.Platform>();
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);

            ItemTapped = new Command<GameTracker.Platform>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            SortCommand = new Command(OnSort);
            SortDirectionCommand = new Command(OnSortDirection);
            SearchCommand = new Command(OnSearch);
            SetSortDirectionButton();
            SetSortButton();
            SetSearchButton();
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return SharedDataService.Module.GetPlatformList(SharedDataService.Settings).Count < SharedDataService.Module.LimitPlatforms;
        }

        void ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = SharedDataService.Module.GetPlatformList(SharedDataService.SavedState.FilterPlatforms,
                    SharedDataService.SavedState.SortPlatforms, SharedDataService.Settings);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                SetSearchButton();
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

        async void OnItemSelected(GameTracker.Platform item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(PlatformDetailPage)}?{nameof(PlatformDetailViewModel.ItemId)}={item.UniqueID}");
        }

        private async void OnSort()
        {
            IList<ISortOption> sortOptions = SortEngine.GetSortOptionList<GameTracker.Platform>(SharedDataService.Module, SharedDataService.Settings);
            List<PopupListOption> options = sortOptions.Select(so => new PopupListOption(so, so.Name)).ToList();

            ISortOption selectedValue = SharedDataService.SavedState.SortPlatforms.SortOption;
            var ret = await UtilMAUI.ShowPopupListAsync("Sort by", options, selectedValue);
            if (ret != null && ret.Item1 == PopupList.EnumOutputType.Selection)
            {
                if (ret.Item2 is null)
                    return;
                else if (ret.Item2.Equals(SharedDataService.SavedState.SortPlatforms.SortOption))
                    SharedDataService.SavedState.SortPlatforms.SortOption = null;
                else
                    SharedDataService.SavedState.SortPlatforms.SortOption = (SortOptionBase)ret.Item2;
                SavedState.SaveSavedState(SharedDataService.PathController, SharedDataService.SavedState);

                ExecuteLoadItemsCommand();
                SetSortButton();
            }
        }

        private void OnSortDirection()
        {
            SharedDataService.SavedState.SortPlatforms.SortMode = SharedDataService.SavedState.SortPlatforms.SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            SetSortDirectionButton();
            SavedState.SaveSavedState(SharedDataService.PathController, SharedDataService.SavedState);
            ExecuteLoadItemsCommand();
        }

        private void SetSortDirectionButton()
        {
            SortDirectionButtonText = SharedDataService.SavedState.SortPlatforms.SortMode == SortMode.Ascending ? "Asc" : "Desc";
            SortDirectionImageName = SharedDataService.SavedState.SortPlatforms.SortMode == SortMode.Ascending ? "sort_ascending" : "sort_descending";
        }

        private void SetSortButton()
        {
            SortImageName = SharedDataService.SavedState.SortPlatforms.SortOption == null ? "sort" : "sort_active";
        }

        private void SetSearchButton()
        {
            SearchImageName = SharedDataService.SavedState.FilterPlatforms.Filters.Count <= 0 ? "search" : "search_active";
        }

        private async void OnSearch()
        {
            await Shell.Current.GoToAsync($"{nameof(FilterPage)}?{nameof(FilterViewModel.FilterTypeParam)}={FilterType.Platform}");
        }
    }
}
