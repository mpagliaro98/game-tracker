using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using GameTracker.Model;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using Xamarin.Forms;
using System.Linq;
using GameTrackerMobile.Services;
using System.Collections;
using System.Collections.Generic;
using GameTracker;
using RatableTracker.List_Manipulation;

namespace GameTrackerMobile.ViewModels
{
    public class GamesViewModel : BaseViewModel<RatableGame>
    {
        private RatableGame _selectedItem;

        public ObservableCollection<RatableGame> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command SortCommand { get; }
        public Command SortDirectionCommand { get; }
        public Command AddItemCommand { get; }
        public Command<RatableGame> ItemTapped { get; }
        public Command ShowCompilations { get; }

        public RatableGame SelectedItem
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
            Items = new ObservableCollection<RatableGame>();
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);
            ShowCompilations = new Command(OnShowCompilations);

            ItemTapped = new Command<RatableGame>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            SortCommand = new Command(OnSort);
            SortDirectionCommand = new Command(OnSortDirection);
            SetSortDirectionButton();
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return ModuleService.GetActiveModule().ListedObjects.Count() < ModuleService.GetActiveModule().LimitListedObjects;
        }

        void ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = ModuleService.GetActiveModule().GetListedObjectView(GetGameFilterOptions(), GetGameSortOptions());
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

        async void OnItemSelected(RatableGame item)
        {
            if (item == null)
                return;

            if (item is GameCompilation)
                await Shell.Current.GoToAsync($"{nameof(CompilationDetailPage)}?{nameof(CompilationDetailViewModel.ItemId)}={new ObjectReference(item)}");
            else
                await Shell.Current.GoToAsync($"{nameof(GameDetailPage)}?{nameof(GameDetailViewModel.ItemId)}={new ObjectReference(item)}");
        }

        async void OnShowCompilations()
        {
            SavedState.ShowCompilations = !SavedState.ShowCompilations;
            ExecuteLoadItemsCommand();
            string msg = SavedState.ShowCompilations ?
                "Compilations are now being shown in the list, and games in compilations are hidden." :
                "Games in compilations are visible, and compilations are being hidden.";
            await Util.ShowPopupAsync("Compilations", msg, PopupViewModel.EnumInputType.Ok);
        }

        private async void OnSort()
        {
            List<PopupListOption> options = new List<PopupListOption>()
            {
                new PopupListOption(SortOptionsGame.SORT_Name, "Name"),
                new PopupListOption(SortOptionsGame.SORT_Status, "Completion Status"),
                new PopupListOption(SortOptionsGame.SORT_Platform, "Platform"),
                new PopupListOption(SortOptionsGame.SORT_PlatformPlayedOn, "Platform Played On"),
                new PopupListOption(SortOptionsGame.SORT_Score, "Final Score"),
                new PopupListOption(SortOptionsGame.SORT_HasComment, "Has Comment"),
                new PopupListOption(SortOptionsGame.SORT_ReleaseDate, "Release Date"),
                new PopupListOption(SortOptionsGame.SORT_AcquiredOn, "Acquired On"),
                new PopupListOption(SortOptionsGame.SORT_StartedOn, "Started On"),
                new PopupListOption(SortOptionsGame.SORT_FinishedOn, "Finished On")
            };

            var module = ModuleService.GetActiveModule();
            int i = SortOptionsGame.SORT_CategoryStart;
            foreach (var cat in module.RatingCategories)
            {
                options.Add(new PopupListOption(i++, cat.Name));
            }

            int? selectedValue = SavedState.GameSortMode;
            if (selectedValue.Value == SortOptionsGame.SORT_None)
                selectedValue = null;
            var ret = await Util.ShowPopupListAsync("Sort by", options, selectedValue);
            if (ret != null)
            {
                if (ret.Item1 == PopupListViewModel.EnumOutputType.Cancel)
                    return;
                else if (ret.Item2 == SavedState.GameSortMode)
                    SavedState.GameSortMode = SortOptionsGame.SORT_None;
                else
                    SavedState.GameSortMode = ret.Item2.Value;
                
                ExecuteLoadItemsCommand();
            }
        }

        private void OnSortDirection()
        {
            SavedState.GameSortDirection = SavedState.GameSortDirection == SortMode.ASCENDING ? SortMode.DESCENDING : SortMode.ASCENDING;
            SetSortDirectionButton();
            ExecuteLoadItemsCommand();
        }

        private void SetSortDirectionButton()
        {
            SortDirectionButtonText = SavedState.GameSortDirection == SortMode.ASCENDING ? "Asc" : "Desc";
            SortDirectionImageName = SavedState.GameSortDirection == SortMode.ASCENDING ? "sort_ascending" : "sort_descending";
        }

        private FilterOptionsGame GetGameFilterOptions()
        {
            return new FilterOptionsGame(SavedState.ShowCompilations);
        }

        private SortOptionsGame GetGameSortOptions()
        {
            return new SortOptionsGame(SavedState.GameSortMode, SavedState.GameSortDirection);
        }
    }
}