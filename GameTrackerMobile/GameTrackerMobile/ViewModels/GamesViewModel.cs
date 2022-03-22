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

namespace GameTrackerMobile.ViewModels
{
    public class GamesViewModel : BaseViewModel<RatableGame>
    {
        public const int SORT_NONE = -1;
        public const int SORT_NAME = 0;
        public const int SORT_STATUS = 1;
        public const int SORT_PLATFORM = 2;
        public const int SORT_PLAYEDON = 3;
        public const int SORT_SCORE = 4;
        public const int SORT_HASCOMMENT = 5;
        public const int SORT_CATEGORY_START = 6;

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
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
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

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                if (SavedState.ShowCompilations)
                {
                    items = items.Where(rg => !rg.IsPartOfCompilation).Concat(await DependencyService.Get<IDataStore<GameCompilation>>().GetItemsAsync());
                }
                SortGamesList(ref items);
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
            await ExecuteLoadItemsCommand();
            string msg = SavedState.ShowCompilations ?
                "Compilations are now being shown in the list, and games in compilations are hidden." :
                "Games in compilations are visible, and compilations are being hidden.";
            await Util.ShowPopupAsync("Compilations", msg, PopupViewModel.EnumInputType.Ok);
        }

        private async void OnSort()
        {
            List<PopupListOption> options = new List<PopupListOption>()
            {
                new PopupListOption(SORT_NAME, "Name"),
                new PopupListOption(SORT_STATUS, "Completion Status"),
                new PopupListOption(SORT_PLATFORM, "Platform"),
                new PopupListOption(SORT_PLAYEDON, "Platform Played On"),
                new PopupListOption(SORT_SCORE, "Final Score"),
                new PopupListOption(SORT_HASCOMMENT, "Has Comment")
            };

            var module = ModuleService.GetActiveModule();
            int i = SORT_CATEGORY_START;
            foreach (var cat in module.RatingCategories)
            {
                options.Add(new PopupListOption(i++, cat.Name));
            }
            
            var ret = await Util.ShowPopupListAsync("Sort by", options);
            if (ret != null)
            {
                if (ret.Item1 == PopupListViewModel.EnumOutputType.Cancel)
                    return;
                else if (ret.Item1 == PopupListViewModel.EnumOutputType.Clear)
                    SavedState.GameSortMode = SORT_NONE;
                else
                    SavedState.GameSortMode = ret.Item2.Value;
                
                await ExecuteLoadItemsCommand();
            }
        }

        private async void OnSortDirection()
        {
            SavedState.GameSortDirection = SavedState.GameSortDirection == SortMode.ASCENDING ? SortMode.DESCENDING : SortMode.ASCENDING;
            SetSortDirectionButton();
            await ExecuteLoadItemsCommand();
        }

        private void SetSortDirectionButton()
        {
            SortDirectionButtonText = SavedState.GameSortDirection == SortMode.ASCENDING ? "Desc" : "Asc";
            SortDirectionImageName = SavedState.GameSortDirection == SortMode.ASCENDING ? "sort_descending" : "sort_ascending";
        }

        private void SortGamesList(ref IEnumerable<RatableGame> items)
        {
            if (SavedState.GameSortMode == SORT_NONE)
            {
                if (SavedState.GameSortDirection == SortMode.ASCENDING)
                    items = items.Reverse();
                return;
            }

            Func<RatableGame, object> sortFunc;
            switch (SavedState.GameSortMode)
            {
                case SORT_NAME:
                    sortFunc = game => game.Name;
                    break;
                case SORT_STATUS:
                    sortFunc = game => game.RefStatus.HasReference() ? ModuleService.GetActiveModule().FindStatus(game.RefStatus).Name : "";
                    break;
                case SORT_PLATFORM:
                    sortFunc = game => game.RefPlatform.HasReference() ? ModuleService.GetActiveModule().FindPlatform(game.RefPlatform).Name : "";
                    break;
                case SORT_PLAYEDON:
                    sortFunc = game => game.RefPlatformPlayedOn.HasReference() ? ModuleService.GetActiveModule().FindPlatform(game.RefPlatformPlayedOn).Name : "";
                    break;
                case SORT_SCORE:
                    sortFunc = game => ModuleService.GetActiveModule().GetScoreOfObject(game);
                    break;
                case SORT_HASCOMMENT:
                    sortFunc = game => game.Comment.Length > 0;
                    break;
                case int n when n >= SORT_CATEGORY_START:
                    RatingCategoryWeighted selectedCat = null;
                    int i = SORT_CATEGORY_START;
                    foreach (var cat in ModuleService.GetActiveModule().RatingCategories)
                    {
                        if (i++ == n)
                        {
                            selectedCat = cat;
                            break;
                        }
                    }
                    if (selectedCat == null)
                    {
                        SavedState.GameSortMode = SORT_NONE;
                        return;
                    }
                    var rm = ModuleService.GetActiveModule();
                    sortFunc = game => rm.GetScoreOfCategory(game, selectedCat);
                    break;
                default:
                    throw new Exception("Unknown sort mode");
            }

            if (SavedState.GameSortDirection == SortMode.ASCENDING)
                items = items.OrderBy(game => game.Name).OrderByDescending(sortFunc);
            else
                items = items.OrderBy(game => game.Name).OrderBy(sortFunc);
        }
    }
}