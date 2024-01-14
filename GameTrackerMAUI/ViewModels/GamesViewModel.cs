using CommunityToolkit.Maui.Alerts;
using GameTracker;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
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
    public class GamesViewModel : BaseViewModelListSortSearch<GameObject>, IQueryAttributable
    {
        public Command SaveStateAndLoad { get; }

        public bool ShowCompilations { get; set; } = false;
        public bool ShowDLC { get; set; } = false;

        private string _compilationsImageName = "compilations";
        public string CompilationsImageName
        {
            get => _compilationsImageName;
            set => SetProperty(ref _compilationsImageName, value);
        }

        protected override FilterEngine FilterObject => SavedState.FilterGames;
        protected override SortEngine SortObject => SavedState.SortGames;
        protected override FilterType FilterType => FilterType.Game;
        public override int ListLimit => Module.LimitModelObjects;

        public GamesViewModel(IServiceProvider provider) : base(provider)
        {
            SaveStateAndLoad = new Command(async () => await OnSaveStateAndLoad());
            ShowCompilations = SavedState.ShowCompilations;
            ShowDLC = SavedState.ShowDLC;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            SetCompilationsButton();
        }

        public Task OnSaveStateAndLoad()
        {
            SavedState.ShowCompilations = ShowCompilations;
            SavedState.ShowDLC = ShowDLC;
            SavedState.Save(PathController);
            IsBusy = true;
            SetCompilationsButton();
            return ExecuteLoadItemsCommand();
        }

        protected override bool SkipItemOnLoadList(GameObject item)
        {
            return (!SavedState.ShowDLC && item.IsDLC) || (SavedState.ShowCompilations && !item.IsCompilation && item.IsPartOfCompilation) || (!SavedState.ShowCompilations && item.IsCompilation);
        }

        protected override IList<GameObject> GetObjectList()
        {
            return Module.GetModelObjectList<GameObject>(FilterObject, SortObject, Settings).OfType<GameObject>().ToList();
        }

        protected override async Task GoToNewItemAsync()
        {
            await Shell.Current.GoToAsync(nameof(NewGamePage));
        }

        protected override async Task GoToSelectedItemAsync(GameObject item)
        {
            if (item.IsCompilation)
                await Shell.Current.GoToAsync($"{nameof(CompilationDetailPage)}?{nameof(CompilationDetailViewModel.ItemId)}={item.UniqueID}");
            else
                await Shell.Current.GoToAsync($"{nameof(GameDetailPage)}?{nameof(GameDetailViewModel.ItemId)}={item.UniqueID}");
        }

        private void SetCompilationsButton()
        {
            CompilationsImageName = SavedState.ShowCompilations ? "compilations_active" : "compilations";
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
