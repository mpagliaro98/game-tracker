using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Model;
using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    public class PlatformsViewModel : BaseViewModel<Platform>
    {
        public const int SORT_NONE = -1;
        public const int SORT_NAME = 0;
        public const int SORT_NUMGAMES = 1;
        public const int SORT_AVG = 2;
        public const int SORT_HIGHEST = 3;
        public const int SORT_LOWEST = 4;
        public const int SORT_PERCFINISHED = 5;
        public const int SORT_RELEASEYEAR = 6;
        public const int SORT_ACQUIREDYEAR = 7;

        private Platform _selectedItem;

        public ObservableCollection<Platform> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command SortCommand { get; }
        public Command SortDirectionCommand { get; }
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

        public PlatformsViewModel()
        {
            Title = "Platforms";
            Items = new ObservableCollection<Platform>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Platform>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            SortCommand = new Command(OnSort);
            SortDirectionCommand = new Command(OnSortDirection);
            SetSortDirectionButton();
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return ModuleService.GetActiveModule().Platforms.Count() < ModuleService.GetActiveModule().LimitPlatforms;
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                SortPlatformsList(ref items);
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

        private async void OnSort()
        {
            List<PopupListOption> options = new List<PopupListOption>()
            {
                new PopupListOption(SORT_NAME, "Name"),
                new PopupListOption(SORT_NUMGAMES, "# Games"),
                new PopupListOption(SORT_AVG, "Average Score"),
                new PopupListOption(SORT_HIGHEST, "Highest Score"),
                new PopupListOption(SORT_LOWEST, "Lowest Score"),
                new PopupListOption(SORT_PERCFINISHED, "% Finished"),
                new PopupListOption(SORT_RELEASEYEAR, "Release Year"),
                new PopupListOption(SORT_ACQUIREDYEAR, "Acquired Year")
            };

            int? selectedValue = SavedState.PlatformSortMode;
            if (selectedValue.Value == SORT_NONE)
                selectedValue = null;
            var ret = await Util.ShowPopupListAsync("Sort by", options, selectedValue);
            if (ret != null)
            {
                if (ret.Item1 == PopupListViewModel.EnumOutputType.Cancel)
                    return;
                else if (ret.Item2 == SavedState.PlatformSortMode)
                    SavedState.PlatformSortMode = SORT_NONE;
                else
                    SavedState.PlatformSortMode = ret.Item2.Value;

                await ExecuteLoadItemsCommand();
            }
        }

        private async void OnSortDirection()
        {
            SavedState.PlatformSortDirection = SavedState.PlatformSortDirection == SortMode.ASCENDING ? SortMode.DESCENDING : SortMode.ASCENDING;
            SetSortDirectionButton();
            await ExecuteLoadItemsCommand();
        }

        private void SetSortDirectionButton()
        {
            SortDirectionButtonText = SavedState.PlatformSortDirection == SortMode.ASCENDING ? "Asc" : "Desc";
            SortDirectionImageName = SavedState.PlatformSortDirection == SortMode.ASCENDING ? "sort_ascending" : "sort_descending";
        }

        private void SortPlatformsList(ref IEnumerable<Platform> items)
        {
            if (SavedState.PlatformSortMode == SORT_NONE)
            {
                if (SavedState.PlatformSortDirection == SortMode.DESCENDING)
                    items = items.Reverse();
                return;
            }

            var rm = ModuleService.GetActiveModule();
            Func<Platform, object> sortFunc;
            switch (SavedState.PlatformSortMode)
            {
                case SORT_NAME:
                    sortFunc = platform => platform.Name;
                    break;
                case SORT_NUMGAMES:
                    sortFunc = platform => rm.GetNumGamesByPlatform(platform);
                    break;
                case SORT_AVG:
                    sortFunc = platform => rm.GetAverageScoreOfGamesByPlatform(platform);
                    break;
                case SORT_HIGHEST:
                    sortFunc = platform => rm.GetHighestScoreFromGamesByPlatform(platform);
                    break;
                case SORT_LOWEST:
                    sortFunc = platform => rm.GetLowestScoreFromGamesByPlatform(platform);
                    break;
                case SORT_PERCFINISHED:
                    sortFunc = platform => rm.GetPercentageGamesFinishedByPlatform(platform);
                    break;
                case SORT_RELEASEYEAR:
                    sortFunc = platform => platform.ReleaseYear;
                    break;
                case SORT_ACQUIREDYEAR:
                    sortFunc = platform => platform.AcquiredYear;
                    break;
                default:
                    throw new Exception("Unknown sort mode");
            }

            if (SavedState.PlatformSortDirection == SortMode.ASCENDING)
                items = items.OrderBy(game => game.Name).OrderBy(sortFunc);
            else
                items = items.OrderBy(game => game.Name).OrderByDescending(sortFunc);
        }
    }
}
