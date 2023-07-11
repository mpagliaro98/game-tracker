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
            Tuple<PopupMain.EnumOutputType, string> ret = (Tuple<PopupMain.EnumOutputType, string>)await ShowPopupAsync(new PopupMain("Attention", "Are you sure you would like to delete this rating category?", PopupMain.EnumInputType.YesNo));
            if (ret.Item1 == PopupMain.EnumOutputType.Yes)
            {
                Item.Delete(SharedDataService.Module, SharedDataService.Settings);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
