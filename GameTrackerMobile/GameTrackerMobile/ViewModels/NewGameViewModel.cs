using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using GameTracker.Model;
using GameTrackerMobile.Services;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using Xamarin.Forms;
using System.Linq;
using System.ComponentModel;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewGameViewModel : BaseViewModel<RatableGame>
    {
        private RatableGame item;

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public RatableGame Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                Title = "Edit Game";
                Name = item.Name;
                CompletionStatus = ModuleService.GetActiveModule().FindStatus(item.RefStatus);
                Platform = ModuleService.GetActiveModule().FindPlatform(item.RefPlatform);
                PlatformPlayedOn = ModuleService.GetActiveModule().FindPlatform(item.RefPlatformPlayedOn);
                CompletionCriteria = item.CompletionCriteria;
                CompletionComment = item.CompletionComment;
                TimeSpent = item.TimeSpent;
                ReleaseDate = item.ReleaseDate;
                AcquiredOn = item.AcquiredOn;
                StartedOn = item.StartedOn;
                FinishedOn = item.FinishedOn;
                Comment = item.Comment;
                IsRemaster = item.IsRemaster;
                IsPartOfCompilation = item.IsPartOfCompilation;
                CompName = item.RefCompilation.HasReference() ? ModuleService.GetActiveModule().FindGameCompilation(item.RefCompilation).Name : "";
                ManualFinalScore = item.IgnoreCategories;
                CategoryValues = InitCategoryValues();
                FinalScore = ModuleService.GetActiveModule().GetScoreOfObject(item);
                OnPropertyChanged(nameof(Games));
                OriginalGame = ModuleService.GetActiveModule().FindListedObject(item.RefOriginalGame);
                UseOriginalGameScore = item.UseOriginalGameScore;
            }
        }

        private string name = "";
        private CompletionStatus status;
        private Platform platform;
        private Platform platformPlayedOn;
        private string completionCriteria = "";
        private string completionComment = "";
        private string timeSpent = "";
        private DateTime releaseDate;
        private DateTime acquiredOn;
        private DateTime startedOn;
        private DateTime finishedOn;
        private string comment = "";
        private bool isRemaster;
        private bool isPartOfCompilation;
        private RatableGame originalGame;
        private bool showScoreFlag;
        private bool useOriginalNameScore;
        private string compName = "";
        private BindingList<CategoryValueContainer> vals = new BindingList<CategoryValueContainer>();
        private double finalScore;
        private bool manualFinalScore;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public CompletionStatus CompletionStatus
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public IEnumerable<CompletionStatus> CompletionStatuses
        {
            get => ModuleService.GetActiveModule().Statuses.OrderBy(s => s.Name).ToList();
        }

        public Platform Platform
        {
            get => platformPlayedOn;
            set => SetProperty(ref platformPlayedOn, value);
        }

        public Platform PlatformPlayedOn
        {
            get => platform;
            set => SetProperty(ref platform, value);
        }

        public string CompletionCriteria
        {
            get => completionCriteria;
            set => SetProperty(ref completionCriteria, value);
        }

        public string CompletionComment
        {
            get => completionComment;
            set => SetProperty(ref completionComment, value);
        }

        public string TimeSpent
        {
            get => timeSpent;
            set => SetProperty(ref timeSpent, value);
        }

        public DateTime ReleaseDate
        {
            get => releaseDate;
            set => SetProperty(ref releaseDate, value);
        }

        public DateTime AcquiredOn
        {
            get => acquiredOn;
            set => SetProperty(ref acquiredOn, value);
        }

        public DateTime StartedOn
        {
            get => startedOn;
            set => SetProperty(ref startedOn, value);
        }

        public DateTime FinishedOn
        {
            get => finishedOn;
            set => SetProperty(ref finishedOn, value);
        }

        public string Comment
        {
            get => comment;
            set => SetProperty(ref comment, value);
        }

        public bool IsRemaster
        {
            get => isRemaster;
            set
            {
                SetProperty(ref isRemaster, value);
                if (!value)
                    OriginalGame = null;
            }
        }

        public bool IsPartOfCompilation
        {
            get => isPartOfCompilation;
            set => SetProperty(ref isPartOfCompilation, value);
        }

        public RatableGame OriginalGame
        {
            get => originalGame;
            set
            {
                SetProperty(ref originalGame, value);
                ShowScoreFlag = (value != null);
                if (value == null)
                    UseOriginalGameScore = false;
                else if (value != null && UseOriginalGameScore)
                {
                    CategoryValues = ToValueContainerList(OriginalGame.CategoryValues);
                    OnPropertyChanged(nameof(FinalScore));
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
            get => useOriginalNameScore;
            set
            {
                SetProperty(ref useOriginalNameScore, value);
                if (value)
                    CategoryValues = ToValueContainerList(OriginalGame.CategoryValues);
                else
                    CategoryValues = InitCategoryValues();
                OnPropertyChanged(nameof(FinalScore));
            }
        }

        public string CompName
        {
            get => compName;
            set => SetProperty(ref compName, value);
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
                    return finalScore;
                else
                    return ModuleService.GetActiveModule().GetScoreOfCategoryValues(GetValuesFromUI());
            }
            set => SetProperty(ref finalScore, value);
        }

        public bool ManualFinalScore
        {
            get => manualFinalScore;
            set
            {
                SetProperty(ref manualFinalScore, value);
                OnPropertyChanged(nameof(FinalScore));
            }
        }

        public IEnumerable<Platform> Platforms
        {
            get => ModuleService.GetActiveModule().Platforms.OrderBy(p => p.Name).ToList();
        }

        public IEnumerable<RatableGame> Games
        {
            get
            {
                var lst = ModuleService.GetActiveModule().ListedObjects.OrderBy(game => game.Name.ToLower().StartsWith("the ") ? game.Name.Substring(4) : game.Name).ToList();
                lst.Remove(Item);
                return lst;
            }
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command ClearStatusCommand { get; }
        public Command ClearPlatformCommand { get; }
        public Command ClearPlatformPlayedOnCommand { get; }
        public Command ClearOriginalGameCommand { get; }
        public Command ClearReleaseDateCommand { get; }
        public Command ClearAcquiredOnCommand { get; }
        public Command ClearStartedOnCommand { get; }
        public Command ClearFinishedOnCommand { get; }

        public NewGameViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ClearStatusCommand = new Command(OnClearStatus);
            ClearPlatformCommand = new Command(OnClearPlatform);
            ClearPlatformPlayedOnCommand = new Command(OnClearPlatformPlayedOn);
            ClearOriginalGameCommand = new Command(OnClearOriginalGame);
            ClearReleaseDateCommand = new Command(OnClearReleaseDate);
            ClearAcquiredOnCommand = new Command(OnClearAcquiredOn);
            ClearStartedOnCommand = new Command(OnClearStartedOn);
            ClearFinishedOnCommand = new Command(OnClearFinishedOn);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();

            CategoryValues = InitCategoryValues();
            Title = "New Game";
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name) && (!isPartOfCompilation || (isPartOfCompilation && !string.IsNullOrWhiteSpace(compName)));
        }

        private void OnClearStatus()
        {
            CompletionStatus = null;
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

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            RatableGame newItem = new RatableGame()
            {
                Name = Name,
                CompletionCriteria = CompletionCriteria,
                CompletionComment = CompletionComment,
                TimeSpent = TimeSpent,
                ReleaseDate = ReleaseDate,
                AcquiredOn = AcquiredOn,
                StartedOn = StartedOn,
                FinishedOn = FinishedOn,
                Comment = Comment,
                IsRemaster = IsRemaster,
                UseOriginalGameScore = UseOriginalGameScore,
                IgnoreCategories = ManualFinalScore,
                CategoryValues = UseOriginalGameScore ? (Item == null ? GetValuesFromUI() : Item.CategoryValues) : GetValuesFromUI(),
                FinalScoreManual = FinalScore
            };
            if (CompletionStatus != null)
                newItem.SetStatus(CompletionStatus);
            if (Platform != null)
                newItem.SetPlatform(Platform);
            if (PlatformPlayedOn != null)
                newItem.SetPlatformPlayedOn(PlatformPlayedOn);
            if (OriginalGame != null)
                newItem.SetOriginalGame(OriginalGame);
            bool newCompilation = false;
            GameCompilation comp = null;
            if (IsPartOfCompilation)
            {
                
                var matches = ModuleService.GetActiveModule().GameCompilations.Where(c => c.Name == CompName);
                if (matches.Count() >= 1)
                {
                    comp = matches.First();
                }
                else
                {
                    comp = new GameCompilation()
                    {
                        Name = compName
                    };
                    newCompilation = true;
                }
                newItem.SetCompilation(comp);
            }

            try
            {
                if (newCompilation)
                    ModuleService.GetActiveModule().ValidateGameCompilation(comp);
                ModuleService.GetActiveModule().ValidateListedObject(newItem);
            }
            catch (ValidationException e)
            {
                await Util.ShowPopupAsync("Error", e.Message, PopupViewModel.EnumInputType.Ok);
                return;
            }

            if (Item == null)
                await DataStore.AddItemAsync(newItem);
            else
            {
                newItem.OverwriteReferenceKey(Item);
                await DataStore.UpdateItemAsync(newItem);
            }
            if (newCompilation)
                await DependencyService.Get<IDataStore<GameCompilation>>().AddItemAsync(comp);

            await Shell.Current.GoToAsync("..");
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

        private BindingList<CategoryValueContainer> ToValueContainerList(IEnumerable<RatingCategoryValue> oldVals)
        {
            BindingList<CategoryValueContainer> newVals = new BindingList<CategoryValueContainer>();
            var module = ModuleService.GetActiveModule();
            foreach (RatingCategoryValue item in oldVals)
            {
                newVals.Add(new CategoryValueContainer() { CategoryName = module.FindRatingCategory(item.RefRatingCategory).Name, CategoryValue = item.PointValue });
            }
            newVals.ListChanged += CategoryValues_ListChanged;
            return newVals;
        }

        private BindingList<RatingCategoryValue> GetValuesFromUI()
        {
            var values = new BindingList<RatingCategoryValue>();
            var module = ModuleService.GetActiveModule();
            for (int i = 0; i < module.RatingCategories.Count(); i++)
            {
                var cat = module.RatingCategories.ElementAt(i);
                var container = CategoryValues.Count() <= i ? null : CategoryValues.ElementAt(i);
                var newVal = new RatingCategoryValue() { PointValue = container == null ? module.Settings.MinScore : Math.Round(container.CategoryValue, 1) };
                newVal.SetRatingCategory(cat);
                values.Add(newVal);
            }
            values.ListChanged += CategoryValues_ListChanged;
            return values;
        }

        private BindingList<CategoryValueContainer> InitCategoryValues()
        {
            var vals = new BindingList<CategoryValueContainer>();
            var module = ModuleService.GetActiveModule();
            foreach (var cat in module.RatingCategories)
            {
                var container = new CategoryValueContainer();
                container.CategoryName = cat.Name;
                container.CategoryValue = Item == null ? module.Settings.MinScore : module.GetScoreOfCategory(Item, cat);
                vals.Add(container);
            }
            vals.ListChanged += CategoryValues_ListChanged;
            return vals;
        }

        void CategoryValues_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FinalScore));
        }
    }
}
