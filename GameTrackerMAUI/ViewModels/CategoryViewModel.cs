using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public class CategoryViewModel : BaseViewModelList<RatingCategoryWeighted>
    {
        public override int ListLimit => Module.CategoryExtension.LimitRatingCategories;

        public CategoryViewModel(IServiceProvider provider) : base(provider)
        {
            Title = "Rating Categories";
        }

        protected override IList<RatingCategoryWeighted> GetObjectList()
        {
            return Module.CategoryExtension.GetRatingCategoryList().OfType<RatingCategoryWeighted>().ToList();
        }

        protected override async Task GoToNewItemAsync()
        {
            await Shell.Current.GoToAsync(nameof(NewCategoryPage));
        }

        protected override async Task GoToSelectedItemAsync(RatingCategoryWeighted item)
        {
            await Shell.Current.GoToAsync($"{nameof(CategoryDetailPage)}?{nameof(CategoryDetailViewModel.ItemId)}={item.UniqueID}");
        }
    }
}
