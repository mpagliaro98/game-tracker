using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ObjAddOns;
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
    public class NewCategoryViewModel : BaseViewModel
    {
        private RatingCategoryWeighted _item = new RatingCategoryWeighted(SharedDataService.Module, SharedDataService.Settings);

        public string ItemId
        {
            get => _item.UniqueID.ToString();
            set
            {
                UniqueID key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public RatingCategoryWeighted Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                Title = "Edit Category";
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Weight));
                OnPropertyChanged(nameof(Comment));
            }
        }

        public string Name
        {
            get => Item.Name;
            set => SetProperty(Item.Name, value, () => Item.Name = value);
        }

        public double Weight
        {
            get => Item.Weight;
            set => SetProperty(Item.Weight, value, () => Item.Weight = value);
        }

        public string Comment
        {
            get => Item.Comment;
            set => SetProperty(Item.Comment, value, () => Item.Comment = value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewCategoryViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            Title = "New Category";
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
                Item = (RatingCategoryWeighted)RatableTracker.Util.Util.FindObjectInList(SharedDataService.Module.CategoryExtension.GetRatingCategoryList(), itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
