using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
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
    public class PlatformDetailViewModel : BaseViewModel
    {
        private GameTracker.Platform item = new GameTracker.Platform(SharedDataService.Module, SharedDataService.Settings);

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public GameTracker.Platform Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
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

        public int NumGames
        {
            get => SharedDataService.Module.GetNumGamesByPlatform(Item, SharedDataService.Settings);
        }

        public double AverageScore
        {
            get => SharedDataService.Module.GetAverageScoreOfGamesByPlatform(Item, SharedDataService.Settings);
        }

        public double HighestScore
        {
            get => SharedDataService.Module.GetHighestScoreFromGamesByPlatform(Item, SharedDataService.Settings);
        }

        public double LowestScore
        {
            get => SharedDataService.Module.GetLowestScoreFromGamesByPlatform(Item, SharedDataService.Settings);
        }

        public double PercentageFinished
        {
            get => SharedDataService.Module.GetProportionGamesFinishedByPlatform(Item, SharedDataService.Settings);
        }

        public string RatioFinished
        {
            get => SharedDataService.Module.GetNumGamesFinishedByPlatform(Item, SharedDataService.Settings).ToString() + "/" +
                SharedDataService.Module.GetNumGamesFinishableByPlatform(Item, SharedDataService.Settings).ToString() + " games";
        }

        public string TopGames
        {
            get
            {
                var top = SharedDataService.Module.GetTopGamesByPlatform(Item, SharedDataService.Settings, 5);
                return string.Join("\n", top.ForEach(game => game.Name));
            }
        }

        public string BottomGames
        {
            get
            {
                var top = SharedDataService.Module.GetBottomGamesByPlatform(Item, SharedDataService.Settings, 3);
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

        public PlatformDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = SharedDataService.Module.GetPlatformList(SharedDataService.Settings).First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewPlatformPage)}?{nameof(NewPlatformViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnDelete()
        {
            var ret = await UtilMAUI.ShowPopupMainAsync("Attention", "Are you sure you would like to delete this platform?", PopupMain.EnumInputType.YesNo);
            if (ret.Item1 == PopupMain.EnumOutputType.Yes)
            {
                Item.Delete(SharedDataService.Module, SharedDataService.Settings);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
