using GameTrackerMAUI.Views;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameTrackerMAUI.ViewModels
{
    public abstract class BaseViewModelEdit<T> : BaseViewModel where T : TrackerObjectBase
    {
        private T _item;

        public string ItemId
        {
            get => _item.UniqueID.ToString();
            set
            {
                UniqueID key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public T Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                UpdatePropertiesOnLoad();
            }
        }

        public virtual string Name
        {
            get => Item.Name;
            set => SetProperty(Item.Name, value, () => Item.Name = value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public BaseViewModelEdit(IServiceProvider provider) : base(provider)
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();

            _item = CreateNewObject();
        }

        protected abstract T CreateNewObject();

        protected abstract T CreateCopyObject(T item);

        protected abstract void UpdatePropertiesOnLoad();

        protected abstract IList<T> GetObjectList();

        protected virtual bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            try
            {
                PreSave();
                await SaveObject();
            }
            catch (Exception ex)
            {
                await AlertService.DisplayAlertAsync("Unable to Save", ex.Message);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        protected virtual void PreSave()
        {

        }

        protected virtual async Task SaveObject()
        {
            await Task.Run(() => Item.Save(Module, Settings));
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                var item = Util.FindObjectInList(GetObjectList(), itemId);
                Item = CreateCopyObject(item);
                PostLoad(item);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        protected virtual void PostLoad(T item)
        {

        }
    }
}