using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class PlatformDetailViewModel : BaseViewModelDetail<GameTracker.Platform>
    {
        public int NumGames
        {
            get => Module.GetNumGamesByPlatform(Item, Settings);
        }

        public double AverageScore
        {
            get => Module.GetAverageScoreOfGamesByPlatform(Item, Settings);
        }

        public double HighestScore
        {
            get => Module.GetHighestScoreFromGamesByPlatform(Item, Settings);
        }

        public double LowestScore
        {
            get => Module.GetLowestScoreFromGamesByPlatform(Item, Settings);
        }

        public double PercentageFinished
        {
            get => Module.GetProportionGamesFinishedByPlatform(Item, Settings);
        }

        public string RatioFinished
        {
            get => Module.GetNumGamesFinishedByPlatform(Item, Settings).ToString() + "/" +
                Module.GetNumGamesFinishableByPlatform(Item, Settings).ToString() + " games";
        }

        public string TopGames
        {
            get
            {
                var top = Module.GetTopGamesByPlatform(Item, Settings, 5);
                return string.Join("\n", top.ForEach(game => game.Name));
            }
        }

        public string BottomGames
        {
            get
            {
                var top = Module.GetBottomGamesByPlatform(Item, Settings, 3);
                return string.Join("\n", top.ForEach(game => game.Name));
            }
        }

        public string Abbreviation
        {
            get => Item.Abbreviation == "" ? "N/A" : Item.Abbreviation;
        }

        public string ReleaseYear
        {
            get => Item.ReleaseYear == 0 ? "N/A" : Item.ReleaseYear.ToString();
        }

        public string AcquiredYear
        {
            get => Item.AcquiredYear == 0 ? "N/A" : Item.AcquiredYear.ToString();
        }

        public PlatformDetailViewModel(IServiceProvider provider) : base(provider) { }

        protected override GameTracker.Platform CreateNewObject()
        {
            return new GameTracker.Platform(Module, Settings);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            OnPropertyChanged(nameof(NumGames));
            OnPropertyChanged(nameof(AverageScore));
            OnPropertyChanged(nameof(HighestScore));
            OnPropertyChanged(nameof(LowestScore));
            OnPropertyChanged(nameof(PercentageFinished));
            OnPropertyChanged(nameof(RatioFinished));
            OnPropertyChanged(nameof(TopGames));
            OnPropertyChanged(nameof(BottomGames));
            OnPropertyChanged(nameof(ReleaseYear));
            OnPropertyChanged(nameof(AcquiredYear));
            OnPropertyChanged(nameof(Abbreviation));
        }

        protected override IList<GameTracker.Platform> GetObjectList()
        {
            return Module.GetPlatformList(Settings);
        }

        protected override async Task GoToEditPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(NewPlatformPage)}?{nameof(NewPlatformViewModel.ItemId)}={Item.UniqueID}");
        }
    }
}
