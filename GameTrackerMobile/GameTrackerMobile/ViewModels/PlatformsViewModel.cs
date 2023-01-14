using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTracker;
using GameTracker.Model;
using GameTrackerMobile.Services;
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
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);

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

        void ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = ModuleService.GetActiveModule().GetPlatformView(GetPlatformFilterOptions(), GetPlatformSortOptions());
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
                new PopupListOption(SortOptionsPlatform.SORT_Name, "Name"),
                new PopupListOption(SortOptionsPlatform.SORT_NumGames, "# Games"),
                new PopupListOption(SortOptionsPlatform.SORT_Average, "Average Score"),
                new PopupListOption(SortOptionsPlatform.SORT_Highest, "Highest Score"),
                new PopupListOption(SortOptionsPlatform.SORT_Lowest, "Lowest Score"),
                new PopupListOption(SortOptionsPlatform.SORT_PercentFinished, "% Finished"),
                new PopupListOption(SortOptionsPlatform.SORT_Release, "Release Year"),
                new PopupListOption(SortOptionsPlatform.SORT_Acquired, "Acquired Year")
            };

            int? selectedValue = SavedState.PlatformSortMode;
            if (selectedValue.Value == SortOptionsPlatform.SORT_None)
                selectedValue = null;
            var ret = await Util.ShowPopupListAsync("Sort by", options, selectedValue);
            if (ret != null)
            {
                if (ret.Item1 == PopupListViewModel.EnumOutputType.Cancel)
                    return;
                else if (ret.Item2 == SavedState.PlatformSortMode)
                    SavedState.PlatformSortMode = SortOptionsPlatform.SORT_None;
                else
                    SavedState.PlatformSortMode = ret.Item2.Value;

                ExecuteLoadItemsCommand();
            }
        }

        private void OnSortDirection()
        {
            SavedState.PlatformSortDirection = SavedState.PlatformSortDirection == SortMode.ASCENDING ? SortMode.DESCENDING : SortMode.ASCENDING;
            SetSortDirectionButton();
            ExecuteLoadItemsCommand();
        }

        private void SetSortDirectionButton()
        {
            SortDirectionButtonText = SavedState.PlatformSortDirection == SortMode.ASCENDING ? "Asc" : "Desc";
            SortDirectionImageName = SavedState.PlatformSortDirection == SortMode.ASCENDING ? "sort_ascending" : "sort_descending";
        }

        private FilterOptionsPlatform GetPlatformFilterOptions()
        {
            return new FilterOptionsPlatform();
        }

        private SortOptionsPlatform GetPlatformSortOptions()
        {
            return new SortOptionsPlatform(SavedState.PlatformSortMode, SavedState.PlatformSortDirection);
        }
    }
}
