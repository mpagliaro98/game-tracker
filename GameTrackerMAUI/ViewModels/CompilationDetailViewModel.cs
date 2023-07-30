using GameTracker;
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
    public class CompilationDetailViewModel : BaseViewModelDetail<GameObject>
    {
        public Command<GameObject> ItemTapped { get; }

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

        public IEnumerable<CategoryValueContainer> CategoryValues
        {
            get
            {
                List<CategoryValueContainer> vals = new();
                foreach (var cv in Item.CategoryExtension.CategoryValuesDisplay)
                {
                    var container = new CategoryValueContainer();
                    container.CategoryName = cv.RatingCategory.Name;
                    container.CategoryValue = cv.PointValue;
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
            get => !Item.CategoryExtension.IgnoreCategories && !Item.UseOriginalGameScore;
        }

        public IEnumerable<GameObject> GamesInCompilation
        {
            get
            {
                if (Item.Name == "")
                    return new List<GameObject>();
                return ((GameCompilation)Item).GamesInCompilation().OrderBy(game => game.Name.CleanForSorting()).ToList();
            }
        }

        public bool ShowStaticNotOwnedText
        {
            get => Item.IsNotOwned;
        }

        public DateTime FinishedOn
        {
            get => Item.IsUnfinishable ? DateTime.MinValue : Item.FinishedOn;
        }

        public string StartedOnName
        {
            get => Item.IsUnfinishable ? "Played On:" : "Started On:";
        }

        public CompilationDetailViewModel(IServiceProvider provider) : base(provider)
        {
            ItemTapped = new Command<GameObject>(OnItemSelected);
        }

        protected override GameObject CreateNewObject()
        {
            return new GameCompilation(Settings, Module);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            OnPropertyChanged(nameof(CompletionStatus));
            OnPropertyChanged(nameof(HasCompletionStatus));
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
            await Shell.Current.GoToAsync($"../{nameof(EditCompilationPage)}?{nameof(EditCompilationViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnItemSelected(GameObject item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"../{nameof(GameDetailPage)}?{nameof(GameDetailViewModel.ItemId)}={item.UniqueID}");
        }
    }
}
