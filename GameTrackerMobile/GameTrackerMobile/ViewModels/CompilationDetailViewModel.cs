using GameTracker.Model;
using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class CompilationDetailViewModel : BaseViewModel<GameCompilation>
    {
        private GameCompilation item = new GameCompilation();

        public Command EditCommand { get; }
        public Command<RatableGame> ItemTapped { get; }

        public GameCompilation Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                OnPropertyChanged(nameof(CompletionStatus));
                OnPropertyChanged(nameof(HasCompletionStatus));
                OnPropertyChanged(nameof(StatusMarkedAsFinished));
                OnPropertyChanged(nameof(Platform));
                OnPropertyChanged(nameof(HasPlatform));
                OnPropertyChanged(nameof(PlatformPlayedOn));
                OnPropertyChanged(nameof(HasPlatformPlayedOn));
                OnPropertyChanged(nameof(CategoryValues));
                OnPropertyChanged(nameof(FinalScore));
                OnPropertyChanged(nameof(FinalScoreColor));
                OnPropertyChanged(nameof(Stats));
                OnPropertyChanged(nameof(ShowCategoryValues));
                OnPropertyChanged(nameof(GamesInCompilation));
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

        public CompletionStatus CompletionStatus
        {
            get => Item.RefStatus.HasReference() ? ModuleService.GetActiveModule().FindStatus(Item.RefStatus) : new CompletionStatus();
        }

        public bool HasCompletionStatus
        {
            get => Item.RefStatus.HasReference();
        }

        public bool StatusMarkedAsFinished
        {
            get => HasCompletionStatus ? CompletionStatus.UseAsFinished : false;
        }

        public Platform Platform
        {
            get => Item.RefPlatform.HasReference() ? ModuleService.GetActiveModule().FindPlatform(Item.RefPlatform) : new Platform();
        }

        public bool HasPlatform
        {
            get => Item.RefPlatform.HasReference();
        }

        public Platform PlatformPlayedOn
        {
            get => Item.RefPlatformPlayedOn.HasReference() ? ModuleService.GetActiveModule().FindPlatform(Item.RefPlatformPlayedOn) : new Platform();
        }

        public bool HasPlatformPlayedOn
        {
            get => Item.RefPlatformPlayedOn.HasReference();
        }

        public IEnumerable<CategoryValueContainer> CategoryValues
        {
            get
            {
                if (Item.Name == "")
                    return new List<CategoryValueContainer>();
                List<CategoryValueContainer> vals = new List<CategoryValueContainer>();
                var module = ModuleService.GetActiveModule();
                foreach (var cat in module.RatingCategories)
                {
                    var container = new CategoryValueContainer();
                    container.CategoryName = cat.Name;
                    container.CategoryValue = module.GetScoreOfCategory(Item, cat);
                    vals.Add(container);
                }
                return vals;
            }
        }

        public double FinalScore
        {
            get
            {
                if (Item.Name == "")
                    return ModuleService.GetActiveModule().Settings.MinScore;
                return ModuleService.GetActiveModule().GetScoreOfObject(Item);
            }
        }

        public Xamarin.Forms.Color FinalScoreColor
        {
            get
            {
                if (Item.Name == "")
                    return new Xamarin.Forms.Color();
                return ModuleService.GetActiveModule().GetRangeColorFromObject(Item).ToXamarinColor();
            }
        }

        public string Stats
        {
            get
            {
                if (Item.Name == "")
                    return "";
                var module = ModuleService.GetActiveModule();
                int rankOverall = module.GetRankOfScore(FinalScore, Item);
                int rankPlatform = -1;
                Platform platform = HasPlatform ? Platform : null;
                if (platform != null) rankPlatform = module.GetRankOfScoreByPlatform(FinalScore, platform, Item);

                string text = "";
                if (rankPlatform > 0) text += "#" + rankPlatform.ToString() + " on " + platform.Name + "\n";
                text += "#" + rankOverall.ToString() + " overall";
                return text;
            }
        }

        public bool ShowCategoryValues
        {
            get => !Item.IgnoreCategories && !Item.UseOriginalGameScore;
        }

        public IEnumerable<RatableGame> GamesInCompilation
        {
            get
            {
                if (Item.Name == "")
                    return new List<RatableGame>();
                return ModuleService.GetActiveModule().FindGamesInCompilation(Item).OrderBy(game => game.Name.ToLower().StartsWith("the ") ? game.Name.Substring(4) : game.Name).ToList();
            }
        }

        public CompilationDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            ItemTapped = new Command<RatableGame>(OnItemSelected);
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
            await Shell.Current.GoToAsync($"{nameof(EditCompilationPage)}?{nameof(EditCompilationViewModel.ItemId)}={new ObjectReference(item)}");
        }

        async void OnItemSelected(RatableGame item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(GameDetailPage)}?{nameof(GameDetailViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
