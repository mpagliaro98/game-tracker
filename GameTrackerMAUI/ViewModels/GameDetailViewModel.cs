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
    public class GameDetailViewModel : BaseViewModel
    {
        private GameObject _item = new GameObject(SharedDataService.Settings, SharedDataService.Module);

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command CompilationCommand { get; }
        public Command OriginalGameCommand { get; }

        public GameObject Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
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
                    return Item.OriginalGame.Name + (Item.OriginalGame.Platform != null ? " (" + (Item.OriginalGame.Platform.Abbreviation != "" ? Item.OriginalGame.Platform.Abbreviation : Item.OriginalGame.Platform.Name) + ")" : "");
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
            get => Item.Compilation ?? new GameCompilation(SharedDataService.Settings, SharedDataService.Module);
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

        public GameDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
            CompilationCommand = new Command(OnCompilation);
            OriginalGameCommand = new Command(OnOriginalGame);
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = (GameObject)SharedDataService.Module.GetModelObjectList(SharedDataService.Settings).First((obj) => obj.UniqueID.Equals(itemId));
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(NewGamePage)}?{nameof(NewGameViewModel.ItemId)}={Item.UniqueID}");
        }

        async void OnDelete()
        {
            var ret = await UtilMAUI.ShowPopupMainAsync("Attention", "Are you sure you would like to delete this game?", PopupMain.EnumInputType.YesNo);
            if (ret.Item1 == PopupMain.EnumOutputType.Yes)
            {
                Item.Delete(SharedDataService.Module, SharedDataService.Settings);
                await Shell.Current.GoToAsync("..");
            }
        }

        async void OnCompilation()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(CompilationDetailPage)}?{nameof(CompilationDetailViewModel.ItemId)}={Item.Compilation.UniqueID}");
        }

        async void OnOriginalGame()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(GameDetailPage)}?{nameof(ItemId)}={Item.OriginalGame.UniqueID}");
        }
    }
}
