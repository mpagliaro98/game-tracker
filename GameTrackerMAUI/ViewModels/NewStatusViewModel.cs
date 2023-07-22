using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewStatusViewModel : BaseViewModel
    {
        private StatusGame _item = new StatusGame(SharedDataService.Module, SharedDataService.Settings);

        public string ItemId
        {
            get => _item.UniqueID.ToString();
            set
            {
                UniqueID key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public StatusGame Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                Title = "Edit Status";
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(UseAsFinished));
                OnPropertyChanged(nameof(HideScoreFromList));
                OnPropertyChanged(nameof(Color));
                OnPropertyChanged(nameof(StatusUsage));
                OnPropertyChanged(nameof(ShowMarkAsFinishedOption));
            }
        }

        public string Name
        {
            get => Item.Name;
            set => SetProperty(Item.Name, value, () => Item.Name = value);
        }

        public bool UseAsFinished
        {
            get => Item.UseAsFinished;
            set => SetProperty(Item.UseAsFinished, value, () => Item.UseAsFinished = value);
        }

        public bool HideScoreFromList
        {
            get => Item.HideScoreFromList;
            set => SetProperty(Item.HideScoreFromList, value, () => Item.HideScoreFromList = value);
        }

        public Microsoft.Maui.Graphics.Color Color
        {
            get => Item.Color.ToMAUIColor();
            set => SetProperty(Item.Color, value.ToFrameworkColor(), () => Item.Color = value.ToFrameworkColor());
        }

        public StatusUsage StatusUsage
        {
            get => Item.StatusUsage;
            set
            {
                SetProperty(Item.StatusUsage, value, () => Item.StatusUsage = value);
                OnPropertyChanged(nameof(ShowMarkAsFinishedOption));
            }
        }

        public IEnumerable<StatusUsage> StatusUsageValues
        {
            get => Enum.GetValues<StatusUsage>().AsEnumerable();
        }

        public bool ShowMarkAsFinishedOption
        {
            get => StatusUsage != StatusUsage.UnfinishableGamesOnly;
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewStatusViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            Title = "New Status";
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
                Item = (StatusGame)RatableTracker.Util.Util.FindObjectInList(SharedDataService.Module.StatusExtension.GetStatusList(), itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
