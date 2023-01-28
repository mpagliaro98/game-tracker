using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameTracker.Model;
using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using Rg.Plugins.Popup.Services;
using System.Linq;
using RatableTracker.Framework.Global;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class PlatformDetailViewModel : BaseViewModel<Platform>
    {
        private Platform item = new Platform();

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public Platform Item
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
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public int NumGames
        {
            get => ModuleService.GetActiveModule().GetNumGamesByPlatform(Item);
        }

        public double AverageScore
        {
            get => ModuleService.GetActiveModule().GetAverageScoreOfGamesByPlatform(Item);
        }

        public double HighestScore
        {
            get => ModuleService.GetActiveModule().GetHighestScoreFromGamesByPlatform(Item);
        }

        public double LowestScore
        {
            get => ModuleService.GetActiveModule().GetLowestScoreFromGamesByPlatform(Item);
        }

        public double PercentageFinished
        {
            get => ModuleService.GetActiveModule().GetPercentageGamesFinishedByPlatform(Item);
        }

        public string RatioFinished
        {
            get => ModuleService.GetActiveModule().GetNumGamesFinishedByPlatform(Item).ToString() + "/" +
                ModuleService.GetActiveModule().GetNumGamesFinishableByPlatform(Item).ToString() + " games";
        }

        public string TopGames
        {
            get
            {
                var top = ModuleService.GetActiveModule().GetTopGamesByPlatform(Item, 5);
                return string.Join("\n", top.ForEach(game => game.Name));
            }
        }

        public string BottomGames
        {
            get
            {
                var top = ModuleService.GetActiveModule().GetBottomGamesByPlatform(Item, 3);
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

        public async void LoadItemId(ObjectReference itemId)
        {
            try
            {
                Item = await DataStore.GetItemAsync(itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewPlatformPage)}?{nameof(NewPlatformViewModel.ItemId)}={new ObjectReference(item)}");
        }

        async void OnDelete()
        {
            var ret = await Util.ShowPopupAsync("Attention", "Are you sure you would like to delete this platform?", PopupViewModel.EnumInputType.YesNo);
            if (ret.Item1.ToString().ToUpper() == "YES")
            {
                await DataStore.DeleteItemAsync(Item);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
