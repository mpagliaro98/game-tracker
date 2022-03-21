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

namespace GameTrackerMobile.ViewModels
{
    public class GamesViewModel : BaseViewModel<RatableGame>
    {
        private RatableGame _selectedItem;

        public ObservableCollection<RatableGame> Items { get; }
        public Command LoadItemsCommand { get; }
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
    }
}