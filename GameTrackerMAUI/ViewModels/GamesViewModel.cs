using GameTracker;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ListManipulation;
using RatableTracker.Util;
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
    public class GamesViewModel : BaseViewModel
    {
        private GameObject _selectedItem;

        public ObservableCollection<GameObject> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command SortCommand { get; }
        public Command SortDirectionCommand { get; }
        public Command AddItemCommand { get; }
        public Command<GameObject> ItemTapped { get; }
        public Command ShowCompilations { get; }

        public GameObject SelectedItem
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

        public GamesViewModel()
        {
            Title = "Games";
            Items = new ObservableCollection<GameObject>();
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);
            ShowCompilations = new Command(OnShowCompilations);

            ItemTapped = new Command<GameObject>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            SortCommand = new Command(OnSort);
            SortDirectionCommand = new Command(OnSortDirection);
            SetSortDirectionButton();
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return SharedDataService.Module.TotalNumModelObjects() < SharedDataService.Module.LimitModelObjects;
        }

        void ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = SharedDataService.Module.GetModelObjectList(SharedDataService.SavedState.FilterGames,
                    SharedDataService.SavedState.SortGames, SharedDataService.Settings).OfType<GameObject>().ToList();
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
            await Shell.Current.GoToAsync(nameof(NewGamePage));
        }

        async void OnItemSelected(GameObject item)
        {
            if (item == null)
                return;

            if (item is GameCompilation)
                await Shell.Current.GoToAsync($"{nameof(CompilationDetailPage)}?{nameof(CompilationDetailViewModel.ItemId)}={item.UniqueID}");
            else
                await Shell.Current.GoToAsync($"{nameof(GameDetailPage)}?{nameof(GameDetailViewModel.ItemId)}={item.UniqueID}");
        }

        async void OnShowCompilations()
        {
            SharedDataService.SavedState.FilterGames.ShowCompilations = !SharedDataService.SavedState.FilterGames.ShowCompilations;
            SavedState.SaveSavedState(SharedDataService.PathController, SharedDataService.SavedState);
            ExecuteLoadItemsCommand();
            string msg = SharedDataService.SavedState.FilterGames.ShowCompilations ?
                "Compilations are now being shown in the list, and games in compilations are hidden." :
                "Games in compilations are visible, and compilations are being hidden.";
            var popup = new PopupMain("Compilations", msg, PopupMain.EnumInputType.Ok)
            {
                Size = new Size(300, 200)
            };
            await UtilMAUI.ShowPopupAsync(popup);
        }

        private async void OnSort()
        {
            List<PopupListOption> options = new List<PopupListOption>()
            {
                new PopupListOption(SortGames.SORT_Name, "Name"),
                new PopupListOption(SortGames.SORT_Status, "Completion Status"),
                new PopupListOption(SortGames.SORT_Platform, "Platform"),
                new PopupListOption(SortGames.SORT_PlatformPlayedOn, "Platform Played On"),
                new PopupListOption(SortGames.SORT_Score, "Final Score"),
                new PopupListOption(SortGames.SORT_HasComment, "Has Comment"),
                new PopupListOption(SortGames.SORT_ReleaseDate, "Release Date"),
                new PopupListOption(SortGames.SORT_AcquiredOn, "Acquired On"),
                new PopupListOption(SortGames.SORT_StartedOn, "Started On"),
                new PopupListOption(SortGames.SORT_FinishedOn, "Finished On")
            };

            var module = SharedDataService.Module;
            int i = SortGames.SORT_CategoryStart;
            foreach (var cat in module.CategoryExtension.GetRatingCategoryList())
            {
                options.Add(new PopupListOption(i++, cat.Name));
            }

            int? selectedValue = SharedDataService.SavedState.SortGames.SortMethod;
            if (selectedValue.Value == SortGames.SORT_None)
                selectedValue = null;
            var ret = await UtilMAUI.ShowPopupListAsync("Sort by", options, selectedValue);
            if (ret.Item1 == PopupList.EnumOutputType.Selection)
            {
                if (ret.Item2 is null)
                    return;
                else if (ret.Item2 == SharedDataService.SavedState.SortGames.SortMethod)
                    SharedDataService.SavedState.SortGames.SortMethod = SortGames.SORT_None;
                else
                    SharedDataService.SavedState.SortGames.SortMethod = ret.Item2.Value;
                SavedState.SaveSavedState(SharedDataService.PathController, SharedDataService.SavedState);

                ExecuteLoadItemsCommand();
            }
        }

        private void OnSortDirection()
        {
            SharedDataService.SavedState.SortGames.SortMode = SharedDataService.SavedState.SortGames.SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            SetSortDirectionButton();
            SavedState.SaveSavedState(SharedDataService.PathController, SharedDataService.SavedState);
            ExecuteLoadItemsCommand();
        }

        private void SetSortDirectionButton()
        {
            SortDirectionButtonText = SharedDataService.SavedState.SortGames.SortMode == SortMode.Ascending ? "Asc" : "Desc";
            SortDirectionImageName = SharedDataService.SavedState.SortGames.SortMode == SortMode.Ascending ? "sort_ascending" : "sort_descending";
        }
    }
}
