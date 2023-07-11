using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class EditCompilationViewModel : BaseViewModel
    {
        private GameCompilation _item = new GameCompilation(SharedDataService.Settings, SharedDataService.Module);

        public string ItemId
        {
            get => _item.UniqueID.ToString();
            set
            {
                UniqueID key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public GameCompilation Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                Title = "Edit Compilation";
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(Platform));
                OnPropertyChanged(nameof(PlatformPlayedOn));
            }
        }

        public string Name
        {
            get => Item.Name;
            set => SetProperty(Item.Name, value, () => Item.Name = value);
        }

        public Status Status
        {
            get => Item.StatusExtension.Status;
            set => SetProperty(Item.StatusExtension.Status, value, () => Item.StatusExtension.Status = value);
        }

        public IEnumerable<Status> CompletionStatuses
        {
            get => SharedDataService.Module.StatusExtension.GetStatusList().OrderBy(s => s.Name).ToList();
        }

        public GameTracker.Platform Platform
        {
            get => Item.Platform;
            set => SetProperty(Item.Platform, value, () => Item.Platform = value);
        }

        public GameTracker.Platform PlatformPlayedOn
        {
            get => Item.PlatformPlayedOn;
            set => SetProperty(Item.PlatformPlayedOn, value, () => Item.PlatformPlayedOn = value);
        }

        public IEnumerable<GameTracker.Platform> Platforms
        {
            get => SharedDataService.Module.GetPlatformList(new SortPlatforms() { SortMethod = SortPlatforms.SORT_Name }, SharedDataService.Settings);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public EditCompilationViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
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
                var popup = new PopupMain("Unable to Save", ex.Message, PopupMain.EnumInputType.Ok);
                await ShowPopupAsync(popup);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = (GameCompilation)SharedDataService.Module.GetModelObjectList(SharedDataService.Settings).First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
