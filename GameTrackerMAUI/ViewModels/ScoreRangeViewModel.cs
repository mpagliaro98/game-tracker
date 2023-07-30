using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ScoreRanges;
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
    public class ScoreRangeViewModel : BaseViewModelList<ScoreRange>
    {
        public override int ListLimit => Module.LimitRanges;

        public ScoreRangeViewModel(IServiceProvider provider) : base(provider)
        {
            Title = "Score Ranges";
        }

        protected override IList<ScoreRange> GetObjectList()
        {
            return Module.GetScoreRangeList();
        }

        protected override async Task GoToNewItemAsync()
        {
            await Shell.Current.GoToAsync(nameof(NewScoreRangePage));
        }

        protected override async Task GoToSelectedItemAsync(ScoreRange item)
        {
            await Shell.Current.GoToAsync($"{nameof(ScoreRangeDetailPage)}?{nameof(ScoreRangeDetailViewModel.ItemId)}={item.UniqueID}");
        }
    }
}
