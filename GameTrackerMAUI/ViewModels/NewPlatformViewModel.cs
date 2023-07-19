using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    class NewPlatformViewModel : BaseViewModel
    {
        private GameTracker.Platform _item = new GameTracker.Platform(SharedDataService.Module, SharedDataService.Settings);

        public string ItemId
        {
            get => _item.UniqueID.ToString();
            set
            {
                UniqueID key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public GameTracker.Platform Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                Title = "Edit Platform";
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Abbreviation));
                OnPropertyChanged(nameof(ReleaseYear));
                OnPropertyChanged(nameof(AcquiredYear));
                OnPropertyChanged(nameof(Color));
            }
        }

        public string Name
        {
            get => Item.Name;
            set => SetProperty(Item.Name, value, () => Item.Name = value);
        }

        public string Abbreviation
        {
            get => Item.Abbreviation;
            set => SetProperty(Item.Abbreviation, value, () => Item.Abbreviation = value);
        }

        public int ReleaseYear
        {
            get => Item.ReleaseYear;
            set => SetProperty(Item.ReleaseYear, value, () => Item.ReleaseYear = value);
        }

        public int AcquiredYear
        {
            get => Item.AcquiredYear;
            set => SetProperty(Item.AcquiredYear, value, () => Item.AcquiredYear = value);
        }

        public Microsoft.Maui.Graphics.Color Color
        {
            get => Item.Color.ToMAUIColor();
            set => SetProperty(Item.Color.ToMAUIColor(), value, () => Item.Color = value.ToFrameworkColor());
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
            return !String.IsNullOrWhiteSpace(Name);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            try
            {
                Item.Save(SharedDataService.Module, SharedDataService.Settings);
            }
            catch (Exception ex)
            {
                await UtilMAUI.ShowPopupMainAsync("Unable to Save", ex.Message, PopupMain.EnumInputType.Ok);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = RatableTracker.Util.Util.FindObjectInList(SharedDataService.Module.GetPlatformList(SharedDataService.Settings), itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
