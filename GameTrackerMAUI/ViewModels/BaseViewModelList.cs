using GameTrackerMAUI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Util;
using System.ComponentModel;

namespace GameTrackerMAUI.ViewModels
{
    public abstract class BaseViewModelList<T> : BaseViewModel
    {
        private T _selectedItem;

        private ObservableCollection<T> _items;
        public ObservableCollection<T> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<T> ItemTapped { get; }

        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public abstract int ListLimit { get; }

        public BaseViewModelList(IServiceProvider provider) : base(provider)
        {
            Items = new ObservableCollection<T>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<T>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem, ShowAddButton);
        }

        public bool ShowAddButton(object o)
        {
            return GetObjectList().Count < ListLimit;
        }

        public async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                var items = await Task.Run(GetObjectList);
                items.RemoveAll(SkipItemOnLoadList);
                Items = new ObservableCollection<T>(items);
                PostLoad();
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

        protected abstract IList<T> GetObjectList();

        protected virtual bool SkipItemOnLoadList(T item)
        {
            return false;
        }

        protected virtual void PostLoad()
        {

        }

        public override void OnAppearing()
        {
            this.PropertyChanged -= CheckLimit;
            IsBusy = true;
            SelectedItem = default;
            this.PropertyChanged += CheckLimit;
        }

        private void CheckLimit(object sender, PropertyChangedEventArgs args)
        {
            AddItemCommand.ChangeCanExecute();
        }

        public async void OnAddItem(object obj)
        {
            await GoToNewItemAsync();
        }

        protected abstract Task GoToNewItemAsync();

        public async void OnItemSelected(T item)
        {
            if (item == null)
                return;

            await GoToSelectedItemAsync(item);
        }

        protected abstract Task GoToSelectedItemAsync(T item);
    }
}
