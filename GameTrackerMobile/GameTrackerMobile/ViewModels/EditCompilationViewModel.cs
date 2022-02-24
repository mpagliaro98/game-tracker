using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class EditCompilationViewModel : BaseViewModel<GameCompilation>
    {
        private GameCompilation item;

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public GameCompilation Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                Name = item.Name;
                CompletionStatus = ModuleService.GetActiveModule().FindStatus(item.RefStatus);
                Platform = ModuleService.GetActiveModule().FindPlatform(item.RefPlatform);
                PlatformPlayedOn = ModuleService.GetActiveModule().FindPlatform(item.RefPlatformPlayedOn);
            }
        }

        private string name = "";
        private CompletionStatus status;
        private Platform platform;
        private Platform platformPlayedOn;
        
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public CompletionStatus CompletionStatus
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public IEnumerable<CompletionStatus> CompletionStatuses
        {
            get => ModuleService.GetActiveModule().Statuses.OrderBy(s => s.Name).ToList();
        }

        public Platform Platform
        {
            get => platformPlayedOn;
            set => SetProperty(ref platformPlayedOn, value);
        }

        public Platform PlatformPlayedOn
        {
            get => platform;
            set => SetProperty(ref platform, value);
        }

        public IEnumerable<Platform> Platforms
        {
            get => ModuleService.GetActiveModule().Platforms.OrderBy(p => p.Name).ToList();
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
            return !String.IsNullOrWhiteSpace(name);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            GameCompilation newItem = new GameCompilation()
            {
                Name = Name
            };
            if (CompletionStatus != null)
                newItem.SetStatus(CompletionStatus);
            if (Platform != null)
                newItem.SetPlatform(Platform);
            if (PlatformPlayedOn != null)
                newItem.SetPlatformPlayedOn(PlatformPlayedOn);

            try
            {
                ModuleService.GetActiveModule().ValidateGameCompilation(newItem);
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
