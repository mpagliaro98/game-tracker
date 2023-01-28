using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

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
                Title = "Edit Platform";
                Name = item.Name;
                Abbreviation = item.Abbreviation;
                ReleaseYear = item.ReleaseYear;
                AcquiredYear = item.AcquiredYear;
                Color = item.Color.ToXamarinColor();
            }
        }

        private string name = "";
        private string abbreviation = "";
        private int releaseYear;
        private int acquiredYear;
        private Color color;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Abbreviation
        {
            get => abbreviation;
            set => SetProperty(ref abbreviation, value);
        }

        public int ReleaseYear
        {
            get => releaseYear;
            set => SetProperty(ref releaseYear, value);
        }

        public int AcquiredYear
        {
            get => acquiredYear;
            set => SetProperty(ref acquiredYear, value);
        }

        public Color Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewPlatformViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            Title = "New Platform";
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
                Name = Name,
                Abbreviation = abbreviation,
                ReleaseYear = ReleaseYear,
                AcquiredYear = AcquiredYear,
                Color = Color.ToFrameworkColor()
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
