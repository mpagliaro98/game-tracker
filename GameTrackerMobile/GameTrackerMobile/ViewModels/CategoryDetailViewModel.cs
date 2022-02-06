using GameTrackerMobile.Views;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class CategoryDetailViewModel : BaseViewModel<RatingCategoryWeighted>
    {
        private RatingCategoryWeighted item = new RatingCategoryWeighted();

        public Command EditCommand { get; }

        public RatingCategoryWeighted Item
        {
            get => item;
            set => SetProperty(ref item, value);
        }

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public CategoryDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
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

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewCategoryPage)}?{nameof(NewCategoryViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
