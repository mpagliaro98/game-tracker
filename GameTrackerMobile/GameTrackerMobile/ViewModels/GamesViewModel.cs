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
        private RatableGame _selectedItem;

        public ObservableCollection<RatableGame> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command SortCommand { get; }
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

        public GamesViewModel()
        {
            Title = "Games";
            Items = new ObservableCollection<RatableGame>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ShowCompilations = new Command(OnShowCompilations);

            ItemTapped = new Command<RatableGame>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            SortCommand = new Command(OnSort);
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
                new PopupListOption(-1, SavedState.GameSortDirection == SortMode.ASCENDING ? "Switch to descending" : "Switch to ascending"),
                new PopupListOption(0, "Name"),
                new PopupListOption(1, "Completion Status"),
                new PopupListOption(2, "Platform"),
                new PopupListOption(3, "Platform Played On"),
                new PopupListOption(4, "Final Score"),
                new PopupListOption(5, "Has Comment")
            };

            var module = ModuleService.GetActiveModule();
            int i = 6;
            foreach (var cat in module.RatingCategories)
            {
                options.Add(new PopupListOption(i++, cat.Name));
            }
            
            var ret = await Util.ShowPopupListAsync("Sort", options);
            if (ret != null)
            {
                if (ret.Item1 == PopupListViewModel.EnumOutputType.Cancel)
                    return;
                else if (ret.Item1 == PopupListViewModel.EnumOutputType.Clear)
                    SavedState.GameSortMode = -1;
                else
                {
                    if (ret.Item2 == -1)
                        SavedState.GameSortDirection = SavedState.GameSortDirection == SortMode.ASCENDING ? SortMode.DESCENDING : SortMode.ASCENDING;
                    else
                        SavedState.GameSortMode = ret.Item2.Value;
                }
                
                await ExecuteLoadItemsCommand();
            }
        }

        private void SortGamesList(ref IEnumerable<RatableGame> items)
        {
            if (SavedState.GameSortMode < 0) return;

            Func<RatableGame, object> sortFunc;
            switch (SavedState.GameSortMode)
            {
                case 0:
                    sortFunc = game => game.Name;
                    break;
                case 1:
                    sortFunc = game => game.RefStatus.HasReference() ? ModuleService.GetActiveModule().FindStatus(game.RefStatus).Name : "";
                    break;
                case 2:
                    sortFunc = game => game.RefPlatform.HasReference() ? ModuleService.GetActiveModule().FindPlatform(game.RefPlatform).Name : "";
                    break;
                case 3:
                    sortFunc = game => game.RefPlatformPlayedOn.HasReference() ? ModuleService.GetActiveModule().FindPlatform(game.RefPlatformPlayedOn).Name : "";
                    break;
                case 4:
                    sortFunc = game => ModuleService.GetActiveModule().GetScoreOfObject(game);
                    break;
                case 5:
                    sortFunc = game => game.Comment.Length > 0;
                    break;
                case int n when n >= 6:
                    RatingCategoryWeighted selectedCat = null;
                    int i = 6;
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
                        SavedState.GameSortMode = -1;
                        return;
                    }
                    var rm = ModuleService.GetActiveModule();
                    sortFunc = game => rm.GetScoreOfCategory(game, selectedCat);
                    break;
                default:
                    throw new Exception("Unknown sort mode");
            }

            if (SavedState.GameSortDirection == SortMode.ASCENDING)
                items = items.OrderBy(game => game.Name).OrderBy(sortFunc);
            else
                items = items.OrderBy(game => game.Name).OrderByDescending(sortFunc);
        }
    }
}