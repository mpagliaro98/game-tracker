using System;
using System.Collections.Generic;
using System.Text;
using RatableTracker.Framework.Interfaces;
using GameTracker.Model;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using GameTrackerMobile.Services;
using System.Linq;

namespace GameTrackerMobile.ViewModels
{
    public class StatusViewModel : BaseViewModel<CompletionStatus>
    {
        private CompletionStatus _selectedItem;

        public ObservableCollection<CompletionStatus> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<CompletionStatus> ItemTapped { get; }

        public CompletionStatus SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public StatusViewModel()
        {
            Title = "Completion Statuses";
            Items = new ObservableCollection<CompletionStatus>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<CompletionStatus>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return ModuleService.GetActiveModule().Statuses.Count() < ModuleService.GetActiveModule().LimitStatuses;
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
            await Shell.Current.GoToAsync(nameof(NewStatusPage));
        }

        async void OnItemSelected(CompletionStatus item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(StatusDetailPage)}?{nameof(StatusDetailViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
