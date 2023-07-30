using Amazon.S3.Model;
using GameTrackerMAUI.Views;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public abstract class BaseViewModelDetail<T> : BaseViewModel where T : TrackerObjectBase
    {
        private T _item;

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public T Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                UpdatePropertiesOnLoad();
            }
        }

        public string ItemId
        {
            get => Item.UniqueID.ToString();
            set
            {
                var key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public BaseViewModelDetail(IServiceProvider provider) : base(provider)
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);

            _item = CreateNewObject();
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = GetObjectList().First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        protected abstract T CreateNewObject();

        protected abstract void UpdatePropertiesOnLoad();

        protected abstract IList<T> GetObjectList();

        public async void OnEdit()
        {
            await GoToEditPageAsync();
        }

        protected abstract Task GoToEditPageAsync();

        public async void OnDelete()
        {
            if (await AlertService.DisplayConfirmationAsync("Attention", "Are you sure you would like to delete this?"))
            {
                PreDelete();
                Item.Delete(Module, Settings);
                await Shell.Current.GoToAsync("..");
            }
        }

        protected virtual void PreDelete()
        {

        }
    }
}
