using CommunityToolkit.Maui.Views;
using GameTracker;
using GameTracker.Sorting;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Exceptions;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewGameViewModel : BaseViewModelEdit<GameObject>
    {
        private GameCompilation _comp;
        private string _compNameOriginal = "";
        private string _compName = "";

        private bool isPartOfCompilation;
        private bool showScoreFlag;
        private BindingList<CategoryValueContainer> vals = new BindingList<CategoryValueContainer>();
        private bool showFinishedOn;

        public Status Status
        {
            get => Item.StatusExtension.Status;
            set => SetProperty(Item.StatusExtension.Status, value, () => Item.StatusExtension.Status = value);
        }

        public IEnumerable<Status> CompletionStatuses
        {
            get => Module.StatusExtension.GetStatusList()
                .OfType<StatusGame>()
                .Where(s => (IsUnfinishable && s.StatusUsage == StatusUsage.UnfinishableGamesOnly) ||
                            (!IsUnfinishable && s.StatusUsage == StatusUsage.FinishableGamesOnly) ||
                            s.StatusUsage == StatusUsage.AllGames)
                .OrderBy(s => s.Name)
                .ToList();
        }

        public GameTracker.Platform Platform
        {
            get => Item.Platform;
            set => SetProperty(Item.Platform, value, () => Item.Platform = value);
        }

        public GameTracker.Platform PlatformPlayedOn
        {
            get => Item.PlatformPlayedOn;
            set => SetProperty(Item.PlatformPlayedOn, value, () => Item.PlatformPlayedOn = value);
        }

        public string CompletionCriteria
        {
            get => Item.CompletionCriteria;
            set => SetProperty(Item.CompletionCriteria, value, () => Item.CompletionCriteria = value);
        }

        public string CompletionComment
        {
            get => Item.CompletionComment;
            set => SetProperty(Item.CompletionComment, value, () => Item.CompletionComment = value);
        }

        public string TimeSpent
        {
            get => Item.TimeSpent;
            set => SetProperty(Item.TimeSpent, value, () => Item.TimeSpent = value);
        }

        public DateTime ReleaseDate
        {
            get => Item.ReleaseDate;
            set => SetProperty(Item.ReleaseDate, value, () => Item.ReleaseDate = value);
        }

        public DateTime AcquiredOn
        {
            get => Item.AcquiredOn;
            set => SetProperty(Item.AcquiredOn, value, () => Item.AcquiredOn = value);
        }

        public DateTime StartedOn
        {
            get => Item.StartedOn;
            set => SetProperty(Item.StartedOn, value, () => Item.StartedOn = value);
        }

        public DateTime FinishedOn
        {
            get => Item.FinishedOn;
            set => SetProperty(Item.FinishedOn, value, () => Item.FinishedOn = value);
        }

        public string GameComment
        {
            get => Item.GameComment;
            set => SetProperty(Item.GameComment, value, () => Item.GameComment = value);
        }

        public string Comment
        {
            get => Item.Comment;
            set => SetProperty(Item.Comment, value, () => Item.Comment = value);
        }

        public bool IsRemaster
        {
            get => Item.IsRemaster;
            set
            {
                SetProperty(Item.IsRemaster, value, () => Item.IsRemaster = value);
                if (!value)
                    OriginalGame = null;
            }
        }

        public bool IsPartOfCompilation
        {
            get => isPartOfCompilation;
            set => SetProperty(ref isPartOfCompilation, value);
        }

        public GameObject OriginalGame
        {
            get => Item.OriginalGame;
            set
            {
                SetProperty(Item.OriginalGame, value, () => Item.OriginalGame = value);
                ShowScoreFlag = (value != null);
                if (value == null)
                    UseOriginalGameScore = false;
                else if (value != null && UseOriginalGameScore)
                {
                    CategoryValues = ToValueContainerList(OriginalGame.CategoryExtension.CategoryValuesDisplay);
                    OnPropertyChanged(nameof(FinalScore));
                    OnPropertyChanged(nameof(IsUsingOriginalGameScore));
                }
            }
        }

        public bool ShowScoreFlag
        {
            get => showScoreFlag;
            set => SetProperty(ref showScoreFlag, value);
        }

        public bool UseOriginalGameScore
        {
            get => Item.UseOriginalGameScore;
            set
            {
                SetProperty(Item.UseOriginalGameScore, value, () => Item.UseOriginalGameScore = value);
                if (value)
                    CategoryValues = ToValueContainerList(OriginalGame.CategoryExtension.CategoryValuesDisplay);
                else
                    CategoryValues = InitCategoryValues();
                OnPropertyChanged(nameof(FinalScore));
                OnPropertyChanged(nameof(IsUsingOriginalGameScore));
            }
        }

        public string CompName
        {
            get => _compName;
            set
            {
                SetProperty(ref _compName, value);
                OnPropertyChanged(nameof(Compilations));
            }
        }

        public BindingList<CategoryValueContainer> CategoryValues
        {
            get => vals;
            set => SetProperty(ref vals, value);
        }

        public double FinalScore
        {
            get
            {
                if (ManualFinalScore)
                    return Item.ManualScore;
                else
                    return Module.CategoryExtension.GetTotalScoreFromCategoryValues(GetValuesFromUI());
            }
            set => SetProperty(Item.ManualScore, value, () => Item.ManualScore = value);
        }

        public bool ManualFinalScore
        {
            get => Item.CategoryExtension.IgnoreCategories;
            set
            {
                SetProperty(Item.CategoryExtension.IgnoreCategories, value, () => Item.CategoryExtension.IgnoreCategories = value);
                OnPropertyChanged(nameof(FinalScore));
            }
        }

        public bool IsUnfinishable
        {
            get => Item.IsUnfinishable;
            set
            {
                SetProperty(Item.IsUnfinishable, value, () => Item.IsUnfinishable = value);
                ShowFinishedOn = !value;
                OnPropertyChanged(nameof(StartedOnName));
                var previousStatus = Status;
                OnPropertyChanged(nameof(CompletionStatuses));
                Status = CompletionStatuses.Contains(previousStatus) ? previousStatus : null;
            }
        }

        public bool IsNotOwned
        {
            get => Item.IsNotOwned;
            set => SetProperty(Item.IsNotOwned, value, () => Item.IsNotOwned = value);
        }

        public bool TreatAllGamesAsOwned
        {
            get => Settings.TreatAllGamesAsOwned;
        }

        public bool ShowFinishedOn
        {
            get => showFinishedOn;
            set => SetProperty(ref showFinishedOn, value);
        }

        public string StartedOnName
        {
            get => Item.IsUnfinishable ? "Played On:" : "Started On:";
        }

        public bool IsUsingOriginalGameScore
        {
            get => IsRemaster && OriginalGame != null && UseOriginalGameScore;
        }

        public Microsoft.Maui.Graphics.Color FinalScoreColor
        {
            get
            {
                var range = RatableTracker.Util.Util.GetScoreRange(FinalScore, Module);
                return range == null ? new Microsoft.Maui.Graphics.Color(255, 255, 255, 0) : range.Color.ToMAUIColor();
            }
        }

        public IEnumerable<GameTracker.Platform> Platforms
        {
            get => Module.GetPlatformList(new SortEngine() { SortOption = new SortOptionPlatformName() }, Settings);
        }

        public IEnumerable<GameObject> Games
        {
            get
            {
                var lst = Module.GetModelObjectList(new SortEngine() { SortOption = new SortOptionModelName() }, Settings).OfType<GameObject>().ToList();
                lst.Remove(Item);
                return lst;
            }
        }

        public IEnumerable<GameCompilation> Compilations
        {
            get => Module.GetModelObjectList(Settings).OfType<GameCompilation>().Where(g => g.Name.ToLower().Contains(CompName.ToLower())).ToList();
        }

        private GameCompilation _selectedComp = null;
        public GameCompilation SelectedComp
        {
            get => _selectedComp;
            set => SetProperty(ref _selectedComp, value);
        }

        public Command ClearStatusCommand { get; }
        public Command ClearPlatformCommand { get; }
        public Command ClearPlatformPlayedOnCommand { get; }
        public Command ClearOriginalGameCommand { get; }
        public Command ClearReleaseDateCommand { get; }
        public Command ClearAcquiredOnCommand { get; }
        public Command ClearStartedOnCommand { get; }
        public Command ClearFinishedOnCommand { get; }

        public NewGameViewModel(IServiceProvider provider) : base(provider)
        {
            ClearStatusCommand = new Command(OnClearStatus);
            ClearPlatformCommand = new Command(OnClearPlatform);
            ClearPlatformPlayedOnCommand = new Command(OnClearPlatformPlayedOn);
            ClearOriginalGameCommand = new Command(OnClearOriginalGame);
            ClearReleaseDateCommand = new Command(OnClearReleaseDate);
            ClearAcquiredOnCommand = new Command(OnClearAcquiredOn);
            ClearStartedOnCommand = new Command(OnClearStartedOn);
            ClearFinishedOnCommand = new Command(OnClearFinishedOn);

            _comp = new GameCompilation(Settings, Module);

            CategoryValues = InitCategoryValues();
            Title = "New Game";
        }

        protected override GameObject CreateNewObject()
        {
            return new GameObject(Settings, Module);
        }

        protected override GameObject CreateCopyObject(GameObject item)
        {
            return new GameObject(item);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            Title = "Edit Game";
            CategoryValues = InitCategoryValues();
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Platform));
            OnPropertyChanged(nameof(PlatformPlayedOn));
            OnPropertyChanged(nameof(CompletionCriteria));
            OnPropertyChanged(nameof(CompletionComment));
            OnPropertyChanged(nameof(TimeSpent));
            OnPropertyChanged(nameof(ReleaseDate));
            OnPropertyChanged(nameof(AcquiredOn));
            OnPropertyChanged(nameof(StartedOn));
            OnPropertyChanged(nameof(FinishedOn));
            OnPropertyChanged(nameof(Comment));
            OnPropertyChanged(nameof(GameComment));
            OnPropertyChanged(nameof(IsRemaster));
            IsPartOfCompilation = Item.IsPartOfCompilation;
            OnPropertyChanged(nameof(CompName));
            OnPropertyChanged(nameof(Compilations));
            OnPropertyChanged(nameof(ManualFinalScore));
            OnPropertyChanged(nameof(CategoryValues));
            OnPropertyChanged(nameof(FinalScore));
            OnPropertyChanged(nameof(FinalScoreColor));
            OnPropertyChanged(nameof(Games));
            OnPropertyChanged(nameof(OriginalGame));
            OnPropertyChanged(nameof(UseOriginalGameScore));
            OnPropertyChanged(nameof(IsUnfinishable));
            OnPropertyChanged(nameof(CompletionStatuses));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(IsNotOwned));
        }

        protected override IList<GameObject> GetObjectList()
        {
            return Module.GetModelObjectList(Settings).OfType<GameObject>().ToList();
        }

        protected override void PostLoad(GameObject item)
        {
            _compNameOriginal = item.IsPartOfCompilation ? item.Compilation.Name : "";
            CompName = _compNameOriginal;
            // fix for AutoCompleteEntry bug - otherwise setting text would always result in empty text
            if (CompName.Length > 0) SelectedComp = Compilations.Where(g => g.Name.Equals(CompName)).FirstOrDefault();
        }

        protected override bool ValidateSave()
        {
            return base.ValidateSave() && (!IsPartOfCompilation || (IsPartOfCompilation && !string.IsNullOrWhiteSpace(CompName)));
        }

        protected override void PreSave()
        {
            base.PreSave();
            CompName = CompName.Trim();
            CompletionCriteria = CompletionCriteria.Trim();
            CompletionComment = CompletionComment.Trim();
            TimeSpent = TimeSpent.Trim();
            Comment = Comment.Trim();
            GameComment = GameComment.Trim();
        }

        protected override async Task SaveObject()
        {
            var vals = UseOriginalGameScore ? (Item == null ? GetValuesFromUI() : Item.CategoryExtension.CategoryValuesManual) : GetValuesFromUI();
            foreach (CategoryValue cv in Item.CategoryExtension.CategoryValuesManual)
            {
                var valUI = vals.First((tofind) => tofind.RatingCategory.Equals(cv.RatingCategory));
                if (valUI != null)
                    cv.PointValue = valUI.PointValue;
            }

            if (IsPartOfCompilation && CompName.Length <= 0)
                throw new ValidationException("Compilation must be given a name");
            using var conn = LoadSave.NewConnection();
            if (!IsPartOfCompilation)
            {
                Item.Compilation = null;
            }
            Item.Save(Module, Settings, conn);
            if (IsPartOfCompilation)
            {
                var matches = Module.GetModelObjectList(Settings).OfType<GameCompilation>().Where(c => c.Name.ToLower().Equals(CompName.ToLower())).ToList();
                GameCompilation comp;
                if (matches.Count > 0)
                {
                    // existing compilation - prompt if user would like to overwrite compilation fields with game ones
                    comp = matches[0];
                    var compStatus = comp.StatusExtension.Status;
                    var compPlat = comp.Platform;
                    var compPlatPlayed = comp.PlatformPlayedOn;
                    var gameStatus = Status;
                    var gamePlat = Platform;
                    var gamePlatPlayed = PlatformPlayedOn;
                    string message = "";
                    if (compStatus == null ? gameStatus != null : !compStatus.Equals(gameStatus))
                        message += (message.Length > 0 ? "\n" : "") + "Status: " + (gameStatus == null ? "None" : gameStatus.Name);
                    if (compPlat == null ? gamePlat != null : !compPlat.Equals(gamePlat))
                        message += (message.Length > 0 ? "\n" : "") + "Platform: " + (gamePlat == null ? "None" : gamePlat.Name);
                    if (compPlatPlayed == null ? gamePlatPlayed != null : !compPlatPlayed.Equals(gamePlatPlayed))
                        message += (message.Length > 0 ? "\n" : "") + "Platform Played On: " + (gamePlatPlayed == null ? "None" : gamePlatPlayed.Name);

                    if (message.Length > 0)
                    {
                        if (await AlertService.DisplayConfirmationAsync("Game Changes", $"Would you also like to apply these changes to {comp.Name}?\n{message}"))
                        {
                            comp.Platform = Platform;
                            comp.PlatformPlayedOn = PlatformPlayedOn;
                            comp.StatusExtension.Status = Status;
                        }
                    }
                }
                else
                {
                    comp = new GameCompilation(Settings, Module)
                    {
                        Name = CompName,
                        Platform = Platform,
                        PlatformPlayedOn = PlatformPlayedOn
                    };
                    comp.StatusExtension.Status = Status;
                }
                Item.Compilation = comp;
                comp.Save(Module, Settings, conn);
                Item.Save(Module, Settings, conn);
            }
        }

        private void OnClearStatus()
        {
            Status = null;
        }

        private void OnClearPlatform()
        {
            Platform = null;
        }

        private void OnClearPlatformPlayedOn()
        {
            PlatformPlayedOn = null;
        }

        private void OnClearOriginalGame()
        {
            OriginalGame = null;
        }

        private void OnClearReleaseDate()
        {
            ReleaseDate = DateTime.MinValue;
        }

        private void OnClearAcquiredOn()
        {
            AcquiredOn = DateTime.MinValue;
        }

        private void OnClearStartedOn()
        {
            StartedOn = DateTime.MinValue;
        }

        private void OnClearFinishedOn()
        {
            FinishedOn = DateTime.MinValue;
        }

        private BindingList<CategoryValueContainer> ToValueContainerList(IEnumerable<CategoryValue> oldVals)
        {
            BindingList<CategoryValueContainer> newVals = new();
            foreach (CategoryValue item in oldVals)
            {
                newVals.Add(new CategoryValueContainer(Item, item.RatingCategory) { CategoryValue = item.PointValue });
            }
            newVals.ListChanged += CategoryValues_ListChanged;
            return newVals;
        }

        private BindingList<CategoryValue> GetValuesFromUI()
        {
            var values = new BindingList<CategoryValue>();
            for (int i = 0; i < Module.CategoryExtension.GetRatingCategoryList().Count; i++)
            {
                var cat = Module.CategoryExtension.GetRatingCategoryList().ElementAt(i);
                var container = CategoryValues.Count <= i ? null : CategoryValues.ElementAt(i);
                var newVal = new CategoryValue(Module.CategoryExtension, Settings, cat) { PointValue = container == null ? Settings.MinScore : Math.Round(container.CategoryValue, 1) };
                values.Add(newVal);
            }
            values.ListChanged += CategoryValues_ListChanged;
            return values;
        }

        private BindingList<CategoryValueContainer> InitCategoryValues()
        {
            var vals = new BindingList<CategoryValueContainer>();
            foreach (var cat in Module.CategoryExtension.GetRatingCategoryList())
            {
                var container = new CategoryValueContainer(Item, cat)
                {
                    CategoryValue = Item == null ? Settings.MinScore : Item.CategoryExtension.ScoreOfCategory(cat)
                };
                vals.Add(container);
            }
            vals.ListChanged += CategoryValues_ListChanged;
            return vals;
        }

        void CategoryValues_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FinalScore));
            OnPropertyChanged(nameof(FinalScoreColor));
        }
    }
}
