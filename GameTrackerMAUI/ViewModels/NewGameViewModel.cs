﻿using CommunityToolkit.Maui.Views;
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
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewGameViewModel : BaseViewModel
    {
        private GameObject _item = new GameObject(SharedDataService.Settings, SharedDataService.Module);
        private GameCompilation _comp = new GameCompilation(SharedDataService.Settings, SharedDataService.Module);
        private string _compNameOriginal = "";
        private string _compName = "";

        public string ItemId
        {
            get => _item.UniqueID.ToString();
            set
            {
                UniqueID key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public GameObject Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
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
                OnPropertyChanged(nameof(IsRemaster));
                IsPartOfCompilation = _item.IsPartOfCompilation;
                OnPropertyChanged(nameof(CompName));
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
        }

        private bool isPartOfCompilation;
        private bool showScoreFlag;
        private BindingList<CategoryValueContainer> vals = new BindingList<CategoryValueContainer>();
        private bool showFinishedOn;

        public string Name
        {
            get => Item.Name;
            set => SetProperty(Item.Name, value, () => Item.Name = value);
        }

        public Status Status
        {
            get => Item.StatusExtension.Status;
            set => SetProperty(Item.StatusExtension.Status, value, () => Item.StatusExtension.Status = value);
        }

        public IEnumerable<Status> CompletionStatuses
        {
            get => SharedDataService.Module.StatusExtension.GetStatusList()
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
            set => SetProperty(_compName, value, () => _compName = value);
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
                    return SharedDataService.Module.CategoryExtension.GetTotalScoreFromCategoryValues(GetValuesFromUI());
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
            get => SharedDataService.Settings.TreatAllGamesAsOwned;
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
                var range = RatableTracker.Util.Util.GetScoreRange(FinalScore, SharedDataService.Module);
                return range == null ? new Microsoft.Maui.Graphics.Color(255, 255, 255, 0) : range.Color.ToMAUIColor();
            }
        }

        public IEnumerable<GameTracker.Platform> Platforms
        {
            get => SharedDataService.Module.GetPlatformList(new SortEngine() { SortOption = new SortOptionPlatformName() }, SharedDataService.Settings);
        }

        public IEnumerable<GameObject> Games
        {
            get
            {
                var lst = SharedDataService.Module.GetModelObjectList(new SortEngine() { SortOption = new SortOptionModelName() }, SharedDataService.Settings).OfType<GameObject>().ToList();
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
            return !string.IsNullOrWhiteSpace(Name) && (!IsPartOfCompilation || (IsPartOfCompilation && !string.IsNullOrWhiteSpace(CompName)));
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

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            try
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
                using var conn = SharedDataService.LoadSave.NewConnection();
                if (!IsPartOfCompilation)
                {
                    Item.Compilation = null;
                }
                Item.Save(SharedDataService.Module, SharedDataService.Settings, conn);
                if (IsPartOfCompilation)
                {
                    var matches = SharedDataService.Module.GetModelObjectList(SharedDataService.Settings).OfType<GameCompilation>().Where(c => c.Name.ToLower().Equals(CompName.ToLower())).ToList();
                    GameCompilation comp;
                    if (matches.Count > 0)
                    {
                        comp = matches[0];
                        if (((comp.Platform == null && Platform != null) || (comp.Platform != null && Platform == null) || (comp.Platform != null && !comp.Platform.Equals(Platform))) ||
                            ((comp.PlatformPlayedOn == null && PlatformPlayedOn != null) || (comp.PlatformPlayedOn != null && PlatformPlayedOn == null) || (comp.PlatformPlayedOn != null && !comp.PlatformPlayedOn.Equals(PlatformPlayedOn))) ||
                            ((comp.StatusExtension.Status == null && Status != null) || (comp.StatusExtension.Status != null && Status == null) || (comp.StatusExtension.Status != null && !comp.StatusExtension.Status.Equals(Status))))
                        {
                            var popup = new PopupMain("Game Changes", $"The status or platform fields of this game are different from the compilation's ({CompName}) status/platform fields. Would you like to propagate those changes to the compilation?", PopupMain.EnumInputType.YesNo)
                            {
                                Size = new Size(300, 250)
                            };
                            var result = (Tuple<PopupMain.EnumOutputType, string>)await UtilMAUI.ShowPopupAsync(popup);
                            if (result != null && result.Item1 == PopupMain.EnumOutputType.Yes)
                            {
                                comp.Platform = Platform;
                                comp.PlatformPlayedOn = PlatformPlayedOn;
                                comp.StatusExtension.Status = Status;
                            }
                        }
                    }
                    else
                    {
                        comp = new GameCompilation(SharedDataService.Settings, SharedDataService.Module)
                        {
                            Name = CompName,
                            Platform = Platform,
                            PlatformPlayedOn = PlatformPlayedOn
                        };
                        comp.StatusExtension.Status = Status;
                    }
                    Item.Compilation = comp;
                    comp.Save(SharedDataService.Module, SharedDataService.Settings, conn);
                    Item.Save(SharedDataService.Module, SharedDataService.Settings, conn);
                }
            }
            catch (Exception ex)
            {
                await UtilMAUI.ShowPopupMainAsync("Unable to Save", ex.Message, PopupMain.EnumInputType.Ok);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                var game = RatableTracker.Util.Util.FindObjectInList(SharedDataService.Module.GetModelObjectList(SharedDataService.Settings), itemId) as GameObject;
                Item = new GameObject(game);
                _compNameOriginal = game.IsPartOfCompilation ? game.Compilation.Name : "";
                CompName = _compNameOriginal;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        private BindingList<CategoryValueContainer> ToValueContainerList(IEnumerable<CategoryValue> oldVals)
        {
            BindingList<CategoryValueContainer> newVals = new BindingList<CategoryValueContainer>();
            foreach (CategoryValue item in oldVals)
            {
                newVals.Add(new CategoryValueContainer() { CategoryName = item.RatingCategory.Name, CategoryValue = item.PointValue });
            }
            newVals.ListChanged += CategoryValues_ListChanged;
            return newVals;
        }

        private BindingList<CategoryValue> GetValuesFromUI()
        {
            var values = new BindingList<CategoryValue>();
            var module = SharedDataService.Module;
            for (int i = 0; i < module.CategoryExtension.GetRatingCategoryList().Count; i++)
            {
                var cat = module.CategoryExtension.GetRatingCategoryList().ElementAt(i);
                var container = CategoryValues.Count <= i ? null : CategoryValues.ElementAt(i);
                var newVal = new CategoryValue(module.CategoryExtension, SharedDataService.Settings, cat) { PointValue = container == null ? SharedDataService.Settings.MinScore : Math.Round(container.CategoryValue, 1) };
                values.Add(newVal);
            }
            values.ListChanged += CategoryValues_ListChanged;
            return values;
        }

        private BindingList<CategoryValueContainer> InitCategoryValues()
        {
            var vals = new BindingList<CategoryValueContainer>();
            var module = SharedDataService.Module;
            foreach (var cat in module.CategoryExtension.GetRatingCategoryList())
            {
                var container = new CategoryValueContainer();
                container.CategoryName = cat.Name;
                container.CategoryValue = Item == null ? SharedDataService.Settings.MinScore : Item.CategoryExtension.ScoreOfCategory(cat);
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
