﻿using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ListManipulation.Filtering;
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
    public class CategoryDetailViewModel : BaseViewModel
    {
        private RatingCategoryWeighted _item = new RatingCategoryWeighted(SharedDataService.Module, SharedDataService.Settings);

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public RatingCategoryWeighted Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        public string ItemId
        {
            get => Item.UniqueID.ToString();
            set
            {
                var key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public CategoryDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = (RatingCategoryWeighted)SharedDataService.Module.CategoryExtension.GetRatingCategoryList().First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewCategoryPage)}?{nameof(NewCategoryViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnDelete()
        {
            var ret = await UtilMAUI.ShowPopupMainAsync("Attention", "Are you sure you would like to delete this rating category?", PopupMain.EnumInputType.YesNo);
            if (ret != null && ret.Item1 == PopupMain.EnumOutputType.Yes)
            {
                var count = SharedDataService.SavedState.FilterGames.Filters.RemoveAll(s => s.FilterOption is FilterOptionModelCategory cat && cat.Category.Equals(Item));
                bool remove = false;
                if (SharedDataService.SavedState.SortGames.SortOption is SortOptionModelCategory cat && cat.Category.Equals(Item))
                {
                    SharedDataService.SavedState.SortGames.SortOption = null;
                    remove = true;
                }
                if (count > 0 || remove) SavedState.SaveSavedState(SharedDataService.PathController, SharedDataService.SavedState);

                Item.Delete(SharedDataService.Module, SharedDataService.Settings);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
