﻿using GameTracker;
using GameTracker.Sorting;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Exceptions;
using RatableTracker.ListManipulation.Sorting;
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
        private string _originalName = "";

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
            get => SharedDataService.Module.GetPlatformList(new SortEngine() { SortOption = new SortOptionPlatformName() }, SharedDataService.Settings);
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
                if (Name.Length > 0 && !Name.Equals(_originalName))
                {
                    var matches = SharedDataService.Module.GetModelObjectList(SharedDataService.Settings).OfType<GameCompilation>().Where(c => c.Name.ToLower().Equals(Name.ToLower())).ToList();
                    if (matches.Count > 0)
                        throw new ValidationException("A compilation with that name already exists");
                }
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
                Item = (GameCompilation)SharedDataService.Module.GetModelObjectList(SharedDataService.Settings).First((obj) => obj.UniqueID.Equals(itemId));
                _originalName = Item.Name;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
