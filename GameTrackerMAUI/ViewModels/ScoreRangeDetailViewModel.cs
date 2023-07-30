using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ScoreRangeDetailViewModel : BaseViewModelDetail<ScoreRange>
    {
        public ScoreRangeDetailViewModel(IServiceProvider provider) : base(provider) { }

        protected override ScoreRange CreateNewObject()
        {
            return new ScoreRange(Module, Settings);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            
        }

        protected override IList<ScoreRange> GetObjectList()
        {
            return Module.GetScoreRangeList();
        }

        protected override async Task GoToEditPageAsync()
        {
            await Shell.Current.GoToAsync($"../{nameof(NewScoreRangePage)}?{nameof(NewScoreRangeViewModel.ItemId)}={Item.UniqueID}");
        }
    }
}
