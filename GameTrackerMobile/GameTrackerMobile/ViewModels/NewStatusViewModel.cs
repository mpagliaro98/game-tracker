using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewStatusViewModel : BaseViewModel<CompletionStatus>
    {
        private CompletionStatus item;

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public CompletionStatus Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                Title = "Edit Status";
                Name = item.Name;
                UseAsFinished = item.UseAsFinished;
                ExcludeFromStats = item.ExcludeFromStats;
                Color = item.Color.ToXamarinColor();
            }
        }

        private string name = "";
        private bool useAsFinished;
        private bool excludeFromStats;
        private Color color;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public bool UseAsFinished
        {
            get => useAsFinished;
            set => SetProperty(ref useAsFinished, value);
        }

        public bool ExcludeFromStats
        {
            get => excludeFromStats;
            set => SetProperty(ref excludeFromStats, value);
        }

        public Color Color
        {
            get => color;
            set => SetProperty(ref color, value);
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
            return !String.IsNullOrWhiteSpace(name);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            CompletionStatus newItem = new CompletionStatus()
            {
                Name = Name,
                UseAsFinished = UseAsFinished,
                ExcludeFromStats = ExcludeFromStats,
                Color = Color.ToFrameworkColor()
            };

            try
            {
                ModuleService.GetActiveModule().ValidateStatus(newItem);
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
