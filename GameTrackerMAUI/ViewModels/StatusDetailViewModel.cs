using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
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
    public class StatusDetailViewModel : BaseViewModelDetail<StatusGame>
    {
        public string StatusUsageName
        {
            get => Item.StatusUsage.StatusUsageToString();
        }

        public bool ShowMarkAsFinishedOption
        {
            get => Item.StatusUsage != StatusUsage.UnfinishableGamesOnly;
        }

        public StatusDetailViewModel(IServiceProvider provider) : base(provider) { }

        protected override StatusGame CreateNewObject()
        {
            return new StatusGame(Module, Settings);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            OnPropertyChanged(nameof(StatusUsageName));
            OnPropertyChanged(nameof(ShowMarkAsFinishedOption));
        }

        protected override IList<StatusGame> GetObjectList()
        {
            return Module.StatusExtension.GetStatusList().OfType<StatusGame>().ToList();
        }

        protected override async Task GoToEditPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(NewStatusPage)}?{nameof(NewStatusViewModel.ItemId)}={Item.UniqueID}");
        }
    }
}
