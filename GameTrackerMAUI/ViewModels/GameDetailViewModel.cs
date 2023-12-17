using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ObjAddOns;
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
    public class GameDetailViewModel : BaseViewModelDetail<GameObject>
    {
        public Command CompilationCommand { get; }
        public Command OriginalGameCommand { get; }

        public StatusGame CompletionStatus
        {
            get => (StatusGame)Item.StatusExtension.Status ?? new StatusGame(Module, Settings);
        }

        public bool HasCompletionStatus
        {
            get => Item.StatusExtension.Status != null;
        }

        public GameTracker.Platform Platform
        {
            get => Item.Platform ?? new GameTracker.Platform(Module, Settings);
        }

        public bool HasPlatform
        {
            get => Item.Platform != null;
        }

        public GameTracker.Platform PlatformPlayedOn
        {
            get => Item.PlatformPlayedOn ?? new GameTracker.Platform(Module, Settings);
        }

        public bool HasPlatformPlayedOn
        {
            get => Item.PlatformPlayedOn != null;
        }

        public bool ShowStaticNotOwnedText
        {
            get => Item.IsNotOwned;
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
                if (Item.OriginalGame != null)
                    return Item.OriginalGame.NameAndPlatform;
                else
                    return "";
            }
        }

        public bool HasOriginalGame
        {
            get => Item.OriginalGame != null;
        }

        public GameCompilation Compilation
        {
            get => Item.Compilation ?? new GameCompilation(Settings, Module);
        }

        public bool HasCompilation
        {
            get => Item.Compilation != null;
        }

        public IEnumerable<CategoryValueContainer> CategoryValues
        {
            get
            {
                List<CategoryValueContainer> vals = new();
                foreach (var cv in Item.CategoryExtension.CategoryValuesDisplay)
                {
                    var container = new CategoryValueContainer(Item, cv.RatingCategory)
                    {
                        CategoryValue = cv.PointValue
                    };
                    vals.Add(container);
                }
                return vals;
            }
        }

        public double FinalScore
        {
            get => Item.ScoreDisplay;
        }

        public Microsoft.Maui.Graphics.Color FinalScoreColor
        {
            get => Item.ScoreRangeDisplay == null ? new Microsoft.Maui.Graphics.Color(255, 255, 255, 0) : Item.ScoreRangeDisplay.Color.ToMAUIColor();
        }

        public string Stats
        {
            get
            {
                int rankOverall = Item.Rank;
                int rankPlatform = -1;
                GameTracker.Platform platform = HasPlatform ? Platform : null;
                if (platform != null) rankPlatform = Module.GetRankOfScoreByPlatform(FinalScore, platform, Settings);

                string text = "";
                if (rankPlatform > 0) text += "#" + rankPlatform.ToString() + " on " + platform.Name + "\n";
                text += "#" + rankOverall.ToString() + " overall";
                return text;
            }
        }

        public bool ShowCategoryValues
        {
            get => !Item.CategoryExtension.IgnoreCategories;
        }

        public DateTime FinishedOn
        {
            get => Item.IsUnfinishable ? DateTime.MinValue : Item.FinishedOn;
        }

        public string StartedOnName
        {
            get => Item.IsUnfinishable ? "Played On:" : "Started On:";
        }

        public double ScoreInterval => (Settings.MaxScore - Settings.MinScore) / 10;

        public GameDetailViewModel(IServiceProvider provider) : base(provider)
        {
            CompilationCommand = new Command(OnCompilation);
            OriginalGameCommand = new Command(OnOriginalGame);
        }

        protected override GameObject CreateNewObject()
        {
            return new GameObject(Settings, Module);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            OnPropertyChanged(nameof(CompletionStatus));
            OnPropertyChanged(nameof(HasCompletionStatus));
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
            OnPropertyChanged(nameof(FinishedOn));
            OnPropertyChanged(nameof(StartedOnName));
            OnPropertyChanged(nameof(ShowStaticNotOwnedText));
        }

        protected override IList<GameObject> GetObjectList()
        {
            return Module.GetModelObjectList(Settings).OfType<GameObject>().ToList();
        }

        protected override async Task GoToEditPageAsync()
        {
            await Shell.Current.GoToAsync($"../{nameof(NewGamePage)}?{nameof(NewGameViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnCompilation()
        {
            await Shell.Current.GoToAsync($"../{nameof(CompilationDetailPage)}?{nameof(CompilationDetailViewModel.ItemId)}={Item.Compilation.UniqueID}");
        }

        async void OnOriginalGame()
        {
            await Shell.Current.GoToAsync($"../{nameof(GameDetailPage)}?{nameof(ItemId)}={Item.OriginalGame.UniqueID}");
        }
    }
}
