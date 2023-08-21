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
    public class NewStatusViewModel : BaseViewModelEdit<StatusGame>
    {
        public bool UseAsFinished
        {
            get => Item.UseAsFinished;
            set => SetProperty(Item.UseAsFinished, value, () => Item.UseAsFinished = value);
        }

        public bool HideScoreFromList
        {
            get => Item.HideScoreFromList;
            set => SetProperty(Item.HideScoreFromList, value, () => Item.HideScoreFromList = value);
        }

        public Microsoft.Maui.Graphics.Color Color
        {
            get => Item.Color.ToMAUIColor();
            set => SetProperty(Item.Color, value.ToFrameworkColor(), () => Item.Color = value.ToFrameworkColor());
        }

        public StatusUsage StatusUsage
        {
            get => Item.StatusUsage;
            set
            {
                SetProperty(Item.StatusUsage, value, () => Item.StatusUsage = value);
                OnPropertyChanged(nameof(ShowMarkAsFinishedOption));
            }
        }

        public IEnumerable<StatusUsage> StatusUsageValues
        {
            get => Enum.GetValues<StatusUsage>().AsEnumerable();
        }

        public bool ShowMarkAsFinishedOption
        {
            get => StatusUsage != StatusUsage.UnfinishableGamesOnly;
        }

        public int MaxLengthName => StatusGame.MaxLengthName;

        public NewStatusViewModel(IServiceProvider provider) : base(provider)
        {
            Title = "New Status";
        }

        protected override StatusGame CreateNewObject()
        {
            return new StatusGame(Module, Settings);
        }

        protected override StatusGame CreateCopyObject(StatusGame item)
        {
            return new StatusGame(item);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            Title = "Edit Status";
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(UseAsFinished));
            OnPropertyChanged(nameof(HideScoreFromList));
            OnPropertyChanged(nameof(Color));
            OnPropertyChanged(nameof(StatusUsage));
            OnPropertyChanged(nameof(ShowMarkAsFinishedOption));
        }

        protected override IList<StatusGame> GetObjectList()
        {
            return Module.StatusExtension.GetStatusList().OfType<StatusGame>().ToList();
        }
    }
}
