using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GameTracker.Model;
using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class GameDetailViewModel : BaseViewModel<RatableGame>
    {
        private RatableGame item = new RatableGame();

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command CompilationCommand { get; }
        public Command OriginalGameCommand { get; }

        public RatableGame Item
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
                OnPropertyChanged(nameof(IsRemaster));
                OnPropertyChanged(nameof(ShowStaticRemasterText));
                OnPropertyChanged(nameof(OriginalGameName));
                OnPropertyChanged(nameof(HasOriginalGame));
                OnPropertyChanged(nameof(Compilation));
                OnPropertyChanged(nameof(HasCompilation));
                OnPropertyChanged(nameof(CategoryValues));
                OnPropertyChanged(nameof(FinalScore));
                OnPropertyChanged(nameof(FinalScoreColor));
                OnPropertyChanged(nameof(Stats));
                OnPropertyChanged(nameof(ShowCategoryValues));
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

        public bool IsRemaster
        {
            get => Item.IsRemaster;
        }

        public bool ShowStaticRemasterText
        {
            get => IsRemaster && !HasOriginalGame;
        }

        public string OriginalGameName
        {
            get
            {
                if (Item.RefOriginalGame.HasReference())
                {
                    var game = ModuleService.GetActiveModule().FindListedObject(Item.RefOriginalGame);
                    Platform platform = null;
                    if (game.RefPlatform.HasReference())
                        platform = ModuleService.GetActiveModule().FindPlatform(game.RefPlatform);
                    return game.Name + (platform != null ? " (" + (platform.Abbreviation != "" ? platform.Abbreviation : platform.Name) + ")" : "");
                }
                else
                {
                    return "";
                }
            }
        }

        public bool HasOriginalGame
        {
            get => Item.RefOriginalGame.HasReference();
        }

        public GameCompilation Compilation
        {
            get => Item.RefCompilation.HasReference() ? ModuleService.GetActiveModule().FindGameCompilation(Item.RefCompilation) : new GameCompilation();
        }

        public bool HasCompilation
        {
            get => Item.RefCompilation.HasReference();
        }

        public IEnumerable<CategoryValueContainer> CategoryValues
        {
            get
            {
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
            get => ModuleService.GetActiveModule().GetScoreOfObject(Item);
        }

        public Xamarin.Forms.Color FinalScoreColor
        {
            get => ModuleService.GetActiveModule().GetRangeColorFromObject(Item).ToXamarinColor();
        }

        public string Stats
        {
            get
            {
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

        public GameDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
            CompilationCommand = new Command(OnCompilation);
            OriginalGameCommand = new Command(OnOriginalGame);
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
            await Shell.Current.GoToAsync($"{nameof(NewGamePage)}?{nameof(NewGameViewModel.ItemId)}={new ObjectReference(item)}");
        }

        async void OnDelete()
        {
            var ret = await Util.ShowPopupAsync("Attention", "Are you sure you would like to delete this game?", PopupViewModel.EnumInputType.YesNo);
            if (ret.Item1.ToString().ToUpper() == "YES")
            {
                await DataStore.DeleteItemAsync(Item);
                await Shell.Current.GoToAsync("..");
            }
        }

        async void OnCompilation()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(CompilationDetailPage)}?{nameof(CompilationDetailViewModel.ItemId)}={Item.RefCompilation}");
        }

        async void OnOriginalGame()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(GameDetailPage)}?{nameof(ItemId)}={Item.RefOriginalGame}");
        }
    }
}
