using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
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
    public class StatusViewModel : BaseViewModelList<StatusGame>
    {
        public override int ListLimit => Module.StatusExtension.LimitStatuses;

        public StatusViewModel(IServiceProvider provider) : base(provider)
        {
            Title = "Statuses";
        }

        protected override IList<StatusGame> GetObjectList()
        {
            return Module.StatusExtension.GetStatusList().OfType<StatusGame>().ToList();
        }

        protected override async Task GoToNewItemAsync()
        {
            await Shell.Current.GoToAsync(nameof(NewStatusPage));
        }

        protected override async Task GoToSelectedItemAsync(StatusGame item)
        {
            await Shell.Current.GoToAsync($"{nameof(StatusDetailPage)}?{nameof(StatusDetailViewModel.ItemId)}={item.UniqueID}");
        }
    }
}
