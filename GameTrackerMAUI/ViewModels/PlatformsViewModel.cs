using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GameTracker;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.ListManipulation.Filtering;

namespace GameTrackerMAUI.ViewModels
{
    public class PlatformsViewModel : BaseViewModelListSortSearch<GameTracker.Platform>, IQueryAttributable
    {
        protected override FilterEngine FilterObject => SavedState.FilterPlatforms;
        protected override SortEngine SortObject => SavedState.SortPlatforms;
        protected override FilterType FilterType => FilterType.Platform;
        public override int ListLimit => Module.LimitPlatforms;

        public PlatformsViewModel(IServiceProvider provider) : base(provider) { }

        protected override IList<GameTracker.Platform> GetObjectList()
        {
            return Module.GetPlatformList(FilterObject, SortObject, Settings);
        }

        protected override async Task GoToNewItemAsync()
        {
            await Shell.Current.GoToAsync(nameof(NewPlatformPage));
        }

        protected override async Task GoToSelectedItemAsync(GameTracker.Platform item)
        {
            await Shell.Current.GoToAsync($"{nameof(PlatformDetailPage)}?{nameof(PlatformDetailViewModel.ItemId)}={item.UniqueID}");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey(nameof(FromFilterPage)))
            {
                FromFilterPage = Convert.ToBoolean(query[nameof(FromFilterPage)]);
                query.Remove(nameof(FromFilterPage));
            }
        }
    }
}
