using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    class NewPlatformViewModel : BaseViewModel<Platform>
    {
        private Platform item;

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public Platform Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                Name = item.Name;
                // add each field here
            }
        }

        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewPlatformViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Platform newItem = new Platform()
            {
                Name = Name
            };

            try
            {
                ModuleService.GetActiveModule().ValidatePlatform(newItem);
            }
            catch (ValidationException e)
            {
                await Util.ShowPopupAsync("Error", e.Message, PopupViewModel.EnumInputType.Ok);
                return;
            }

            if (Item == null)
                await DataStore.AddItemAsync(newItem);
            else
            {
                newItem.OverwriteReferenceKey(Item);
                await DataStore.UpdateItemAsync(newItem);
            }
            
            await Shell.Current.GoToAsync("..");
        }

        public async void LoadItemId(ObjectReference itemId)
        {
            try
            {
                Item = await DataStore.GetItemAsync(itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
