using GameTrackerMAUI.Services;
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
    public class CategoryDetailViewModel : BaseViewModelDetail<RatingCategoryWeighted>
    {
        public CategoryDetailViewModel(IServiceProvider provider) : base(provider) { }

        protected override RatingCategoryWeighted CreateNewObject()
        {
            return new RatingCategoryWeighted(Module, Settings);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            
        }

        protected override IList<RatingCategoryWeighted> GetObjectList()
        {
            return Module.CategoryExtension.GetRatingCategoryList().OfType<RatingCategoryWeighted>().ToList();
        }

        protected override async Task GoToEditPageAsync()
        {
            await Shell.Current.GoToAsync($"../{nameof(NewCategoryPage)}?{nameof(NewCategoryViewModel.ItemId)}={Item.UniqueID}");
        }

        protected override void PreDelete()
        {
            var count = SavedState.FilterGames.Filters.RemoveAll(s => s.FilterOption is FilterOptionModelCategory cat && cat.Category.Equals(Item));
            bool remove = false;
            if (SavedState.SortGames.SortOption is SortOptionModelCategory cat && cat.Category.Equals(Item))
            {
                SavedState.SortGames.SortOption = null;
                remove = true;
            }
            if (count > 0 || remove) SavedState.Save(PathController);
        }
    }
}
