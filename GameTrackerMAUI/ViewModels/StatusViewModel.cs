using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
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
    public class StatusViewModel : BaseViewModel
    {
        private StatusGame _selectedItem;

        public ObservableCollection<StatusGame> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<StatusGame> ItemTapped { get; }

        public StatusGame SelectedItem
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
            Title = "Statuses";
            Items = new ObservableCollection<StatusGame>();
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);

            ItemTapped = new Command<StatusGame>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem, ShowAddButton);
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private bool ShowAddButton(object o)
        {
            return SharedDataService.Module.StatusExtension.GetStatusList().Count < SharedDataService.Module.StatusExtension.LimitStatuses;
        }

        void ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = SharedDataService.Module.StatusExtension.GetStatusList().OfType<StatusGame>().ToList();
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

        async void OnItemSelected(StatusGame item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(StatusDetailPage)}?{nameof(StatusDetailViewModel.ItemId)}={item.UniqueID}");
        }
    }
}
