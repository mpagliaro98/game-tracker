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
    public class CompilationDetailViewModel : BaseViewModel
    {
        private GameCompilation _item = new GameCompilation(SharedDataService.Settings, SharedDataService.Module);

        public Command EditCommand { get; }
        public Command<GameObject> ItemTapped { get; }

        public GameCompilation Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
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
            get => Item.UniqueID.ToString();
            set
            {
                var key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public StatusGame CompletionStatus
        {
            get => (StatusGame)Item.StatusExtension.Status ?? new StatusGame(SharedDataService.Module, SharedDataService.Settings);
        }

        public bool HasCompletionStatus
        {
            get => Item.StatusExtension.Status != null;
        }

        public bool StatusMarkedAsFinished
        {
            get => HasCompletionStatus ? CompletionStatus.UseAsFinished : false;
        }

        public GameTracker.Platform Platform
        {
            get => Item.Platform ?? new GameTracker.Platform(SharedDataService.Module, SharedDataService.Settings);
        }

        public bool HasPlatform
        {
            get => Item.Platform != null;
        }

        public GameTracker.Platform PlatformPlayedOn
        {
            get => Item.PlatformPlayedOn ?? new GameTracker.Platform(SharedDataService.Module, SharedDataService.Settings);
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
                if (platform != null) rankPlatform = SharedDataService.Module.GetRankOfScoreByPlatform(FinalScore, platform, SharedDataService.Settings);

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
                return Item.GamesInCompilation().OrderBy(game => game.Name.CleanForSorting()).ToList();
            }
        }

        public CompilationDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            ItemTapped = new Command<GameObject>(OnItemSelected);
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = (GameCompilation)SharedDataService.Module.GetModelObjectList(SharedDataService.Settings).First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(EditCompilationPage)}?{nameof(EditCompilationViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnItemSelected(GameObject item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(GameDetailPage)}?{nameof(GameDetailViewModel.ItemId)}={item.UniqueID}");
        }
    }
}
