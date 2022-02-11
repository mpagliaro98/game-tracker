﻿using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewCategoryViewModel : BaseViewModel<RatingCategoryWeighted>
    {
        private RatingCategoryWeighted item;

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public RatingCategoryWeighted Item
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

        public NewCategoryViewModel()
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
            RatingCategoryWeighted newItem = new RatingCategoryWeighted()
            {
                Name = Name
            };

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