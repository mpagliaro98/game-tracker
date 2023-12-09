using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameTracker;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowGame.xaml
    /// </summary>
    public partial class SubWindowGame : MetroWindow
    {
        private GameModule rm;
        private GameObject orig;
        private SettingsGame settings;
        private string compNameOriginal;
        private string compName;
        private ILoadSaveHandler<ILoadSaveMethodGame> loadSave;
        private bool showScoreSuggestions = true;

        public ObservableCollection<string> CompNames { get; set; } = new ObservableCollection<string>();

        public event EventHandler Saved;

        public SubWindowGame(GameModule rm, SettingsGame settings, ILoadSaveHandler<ILoadSaveMethodGame> loadSave, SubWindowMode mode, GameObject orig)
        {
            InitializeComponent();
            this.rm = rm;
            this.orig = orig;
            this.loadSave = loadSave;
            this.settings = settings;
            this.compNameOriginal = orig.IsPartOfCompilation ? orig.Compilation.Name : "";
            this.compName = this.compNameOriginal;
            CompNames.Clear();
            foreach (var name in rm.GetModelObjectList(settings).OfType<GameObject>().Where(g => g.IsCompilation).Select(g => g.Name).ToList())
            {
                CompNames.Add(name);
            }

            // initialize UI containers
            CreateRatingCategories();
            FillComboboxStatuses(ComboBoxStatus);
            FillComboboxPlatforms(ComboBoxPlatform);
            FillComboboxPlatforms(ComboBoxPlatformPlayedOn, defaultItem: "Same as Platform");
            FillComboboxGames(ComboboxOriginalGame);
            ButtonSave.ToolTip = mode == SubWindowMode.MODE_ADD ? "Create" : "Update";
            TabList.SelectedIndex = string.IsNullOrEmpty(orig.Name) ? 1 : 0;

            // set max length
            TextboxName.MaxLength = GameObject.MaxLengthName;
            TextboxCompletionCriteria.MaxLength = GameObject.MaxLengthCompletionCriteria;
            TextboxCompletionComment.MaxLength = GameObject.MaxLengthCompletionComment;
            TextboxTimeSpent.MaxLength = GameObject.MaxLengthTimeSpent;
            TextBoxGameComments.MaxLength = GameObject.MaxLengthGameComment;
            TextBoxComments.MaxLength = GameObject.MaxLengthComment;

            // set fields in the UI
            TextboxName.Text = orig.Name;
            if (orig.StatusExtension.Status != null) ComboBoxStatus.SelectedItem = orig.StatusExtension.Status;
            if (orig.Platform != null) ComboBoxPlatform.SelectedItem = orig.Platform;
            if (orig.PlatformPlayedOn != null) ComboBoxPlatformPlayedOn.SelectedItem = orig.PlatformPlayedOn;
            if (orig.OriginalGame != null) ComboboxOriginalGame.SelectedItem = orig.OriginalGame;
            CheckboxUnfinishable.IsChecked = orig.IsUnfinishable;
            CheckboxNotOwned.IsChecked = orig.IsNotOwned;
            CheckboxNotOwned.Visibility = settings.TreatAllGamesAsOwned ? Visibility.Hidden : Visibility.Visible;
            CheckboxRemaster.IsChecked = orig.IsRemaster;
            CheckboxUseOriginalGameScore.IsChecked = orig.UseOriginalGameScore;
            TextboxCompletionCriteria.Text = orig.CompletionCriteria;
            TextboxCompletionComment.Text = orig.CompletionComment;
            TextboxTimeSpent.Text = orig.TimeSpent;
            if (!orig.ReleaseDate.Equals(DateTime.MinValue)) DatePickerRelease.SelectedDate = orig.ReleaseDate;
            if (!orig.AcquiredOn.Equals(DateTime.MinValue)) DatePickerAcquired.SelectedDate = orig.AcquiredOn;
            if (!orig.StartedOn.Equals(DateTime.MinValue)) DatePickerStarted.SelectedDate = orig.StartedOn;
            if (!orig.FinishedOn.Equals(DateTime.MinValue)) DatePickerFinished.SelectedDate = orig.FinishedOn;
            TextBoxComments.Text = orig.Comment;
            TextBoxGameComments.Text = orig.GameComment;
            TextBoxFinalScore.Text = orig.ScoreMinIfCyclical.ToString(UtilWPF.SCORE_FORMAT);
            CheckboxCompilation.IsChecked = orig.IsPartOfCompilation;
            TextboxComp.Text = this.compNameOriginal;

            SummaryName.Text = string.IsNullOrEmpty(orig.Name) ? "Save this game to see summary info" : orig.Name;
            SummaryPlatform.Content = orig.PlatformEffective != null ? "On " + orig.PlatformEffective.Name : "";
            SummaryStatus.Content = orig.StatusExtension.Status != null ? orig.StatusExtension.Status.Name : "";
            SummaryThoughts.Text = orig.Comment;
            SummaryRelease.Content = orig.ReleaseDate.ToShortDateString();
            SummaryReleaseContainer.Visibility = orig.ReleaseDate > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
            SummaryAcquired.Content = orig.AcquiredOn.ToShortDateString();
            SummaryAcquiredContainer.Visibility = orig.AcquiredOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
            SummaryStarted.Content = orig.StartedOn.ToShortDateString();
            SummaryStartedContainer.Visibility = orig.StartedOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
            SummaryStartedLabel.Content = orig.IsUnfinishable ? "Played On:" : "Started On:";
            SummaryFinished.Content = orig.FinishedOn.ToShortDateString();
            SummaryFinishedContainer.Visibility = orig.FinishedOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
            SummaryCompilation.Content = orig.IsPartOfCompilation ? "Part of compilation: " + orig.Compilation.Name : "";
            SummaryCompilationContainer.Visibility = orig.IsPartOfCompilation ? Visibility.Visible : Visibility.Collapsed;
            ButtonCompilationLink.Visibility = orig.IsPartOfCompilation ? Visibility.Visible : Visibility.Hidden;

            // set event handlers
            TextboxName.TextChanged += TextboxName_TextChanged;
            ComboBoxPlatform.SelectionChanged += ComboBoxPlatform_SelectionChanged;
            CheckboxRemaster.Checked += CheckboxRemaster_Checked;
            CheckboxRemaster.Unchecked += CheckboxRemaster_Checked;
            CheckboxUseOriginalGameScore.Checked += CheckboxUseOriginalGameScore_Checked;
            CheckboxUseOriginalGameScore.Unchecked += CheckboxUseOriginalGameScore_Checked;
            ComboboxOriginalGame.SelectionChanged += ComboboxOriginalGame_SelectionChanged;
            ComboBoxStatus.SelectionChanged += ComboBoxStatus_SelectionChanged;
            CheckboxUnfinishable.Checked += CheckboxUnfinishable_Checked;
            CheckboxUnfinishable.Unchecked += CheckboxUnfinishable_Checked;
            CheckboxNotOwned.Checked += CheckboxNotOwned_Checked;
            CheckboxNotOwned.Unchecked += CheckboxNotOwned_Checked;
            ComboBoxPlatformPlayedOn.SelectionChanged += ComboBoxPlatformPlayedOn_SelectionChanged;
            CheckboxCompilation.Checked += CheckboxCompilation_Checked;
            CheckboxCompilation.Unchecked += CheckboxCompilation_Checked;
            TextboxComp.TextChanged += TextboxCompilation_TextChanged;
            TextBoxFinalScore.TextChanged += TextBoxFinalScore_TextChanged;
            TextboxCompletionCriteria.TextChanged += TextboxCompletionCriteria_TextChanged;
            TextboxCompletionComment.TextChanged += TextboxCompletionComment_TextChanged;
            TextboxTimeSpent.TextChanged += TextboxTimeSpent_TextChanged;
            DatePickerRelease.SelectedDateChanged += DatePickerRelease_SelectedDateChanged;
            DatePickerAcquired.SelectedDateChanged += DatePickerAcquired_SelectedDateChanged;
            DatePickerStarted.SelectedDateChanged += DatePickerStarted_SelectedDateChanged;
            DatePickerFinished.SelectedDateChanged += DatePickerFinished_SelectedDateChanged;
            TextBoxComments.TextChanged += TextBoxComments_TextChanged;
            TextBoxGameComments.TextChanged += TextBoxGameComments_TextChanged;

            // refresh UI logic
            UpdateScores();
            UpdateRemasterFields();
            UpdateDateFieldVisibility();
            UpdateCompilationFields();
            UpdatePlatformColor();
            UpdateStatusColor();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckboxCompilation.IsChecked.Value && this.compName.Length <= 0)
                    throw new ValidationException("Compilation must be given a name");
                using var conn = loadSave.NewConnection();
                if (!CheckboxCompilation.IsChecked.Value)
                {
                    orig.Compilation = null;
                }
                orig.Save(rm, settings, conn);
                if (CheckboxCompilation.IsChecked.Value)
                {
                    var matches = rm.GetModelObjectList(settings).OfType<GameCompilation>().Where(c => c.Name.ToLower().Equals(compName.ToLower())).ToList();
                    GameCompilation comp;
                    if (matches.Count > 0)
                    {
                        // existing compilation - prompt if user would like to overwrite compilation fields with game ones
                        comp = new GameCompilation(matches[0]);
                        var compStatus = comp.StatusExtension.Status;
                        var compPlat = comp.Platform;
                        var compPlatPlayed = comp.PlatformPlayedOn;
                        var gameStatus = orig.StatusExtension.Status;
                        var gamePlat = orig.Platform;
                        var gamePlatPlayed = orig.PlatformPlayedOn;
                        string message = "";
                        if (compStatus == null ? gameStatus != null : !compStatus.Equals(gameStatus))
                            message += (message.Length > 0 ? "\n" : "") + "Status: " + (gameStatus == null ? "None" : gameStatus.Name);
                        if (compPlat == null ? gamePlat != null : !compPlat.Equals(gamePlat))
                            message += (message.Length > 0 ? "\n" : "") + "Platform: " + (gamePlat == null ? "None" : gamePlat.Name);
                        if (compPlatPlayed == null ? gamePlatPlayed != null : !compPlatPlayed.Equals(gamePlatPlayed))
                            message += (message.Length > 0 ? "\n" : "") + "Platform Played On: " + (gamePlatPlayed == null ? "None" : gamePlatPlayed.Name);

                        if (message.Length > 0)
                        {
                            MessageBoxResult mbr = Xceed.Wpf.Toolkit.MessageBox.Show($"Would you also like to apply these changes to {comp.Name}?\n{message}", "Game Changes", MessageBoxButton.YesNo);
                            if (mbr == MessageBoxResult.Yes)
                            {
                                comp.Platform = orig.Platform;
                                comp.PlatformPlayedOn = orig.PlatformPlayedOn;
                                comp.StatusExtension.Status = orig.StatusExtension.Status;
                            }
                        }
                    }
                    else
                    {
                        // new compilation
                        comp = new GameCompilation(settings, rm)
                        {
                            Name = compName,
                            Platform = orig.Platform,
                            PlatformPlayedOn = orig.PlatformPlayedOn
                        };
                        comp.StatusExtension.Status = orig.StatusExtension.Status;
                    }
                    orig.Compilation = comp;
                    comp.Save(rm, settings, conn);
                    orig.Save(rm, settings, conn);
                }
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            Saved?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void FillComboboxStatuses(ComboBox cb)
        {
            Status previousSelection = cb.SelectedIndex > 0 ? (Status)cb.SelectedItem : null;
            cb.Items.Clear();
            var item = new ComboBoxItem();
            item.Content = "N/A";
            cb.Items.Add(item);
            foreach (StatusGame cs in rm.StatusExtension.GetStatusList().OfType<StatusGame>().OrderBy(s => s.Name))
            {
                if ((orig.IsUnfinishable && cs.StatusUsage == StatusUsage.UnfinishableGamesOnly) ||
                    (!orig.IsUnfinishable && cs.StatusUsage == StatusUsage.FinishableGamesOnly) ||
                    cs.StatusUsage == StatusUsage.AllGames)
                {
                    cb.Items.Add(cs);
                }
            }
            if (previousSelection != null)
                cb.SelectedItem = previousSelection;
            else
                cb.SelectedIndex = 0;
            if (cb.SelectedItem == null)
                cb.SelectedIndex = 0;
        }

        private void FillComboboxPlatforms(ComboBox cb, string defaultItem = "N/A")
        {
            cb.Items.Clear();
            var item = new ComboBoxItem();
            item.Content = defaultItem;
            cb.Items.Add(item);
            foreach (Platform platform in rm.GetPlatformList(settings).OrderBy(p => p.Name))
            {
                cb.Items.Add(platform);
            }
            cb.SelectedIndex = 0;
        }

        private void FillComboboxGames(ComboBox cb)
        {
            cb.Items.Clear();
            cb.Items.Add(new { NameAndPlatform = "--Select the game this is a remaster of--" });
            foreach (GameObject game in rm.GetModelObjectList(settings).OfType<GameObject>().OrderBy(ro => ro.Name))
            {
                if (game.Equals(orig) || (game.IsPartOfCompilation && game.Equals(game.Compilation))) continue;
                cb.Items.Add(game);
            }
            cb.SelectedIndex = 0;
        }

        private void CreateRatingCategories()
        {
            GridRatingCategories.Children.Clear();
            GridRatingCategories.ColumnDefinitions.Clear();
            int i = 0;
            foreach (CategoryValue categoryValue in orig.CategoryExtension.CategoryValueListMinIfCyclical)
            {
                RatingCategory rc = categoryValue.RatingCategory;
                GridRatingCategories.ColumnDefinitions.Add(new ColumnDefinition());
                DockPanel dock = new DockPanel
                {
                    Background = new SolidColorBrush(new System.Windows.Media.Color { A = 0xFF, R = 0xF1, G = 0xF1, B = 0xF1 }),
                    Margin = new Thickness { Bottom = 5, Left = 5, Top = 5, Right = 5 }
                };
                TextBlock tb = new TextBlock
                {
                    Text = rc.Comment,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 400
                };
                Label label = new Label
                {
                    Content = rc.Name,
                    ToolTip = tb
                };
                DockPanel.SetDock(label, Dock.Top);

                TextBox text = new TextBox
                {
                    Name = "TextBoxValue",
                    Margin = new Thickness { Bottom = 5, Left = 5, Right = 5 },
                    BorderThickness = new Thickness { Top = 0, Left = 0, Bottom = 0, Right = 0 },
                    Background = new SolidColorBrush(new System.Windows.Media.Color { A = 0xFF, R = 0xF9, G = 0xF9, B = 0xF9}),
                    FontSize = 32,
                    IsEnabled = orig.CategoryExtension.AreCategoryValuesEditable,
                    Text = categoryValue.PointValue.ToString()
                };
                text.TextChanged += TextBoxScore_TextChanged;
                text.GotFocus += TextBoxScore_GotFocus;
                text.LostFocus += TextBoxScore_LostFocus;
                text.Tag = rc;
                dock.Children.Add(label);
                dock.Children.Add(text);
                Grid.SetColumn(dock, i);
                GridRatingCategories.Children.Add(dock);
                i++;
            }
        }

        private void ButtonCompilationLink_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Window window = new SubWindowCompilation(rm, settings, SubWindowMode.MODE_VIEW, orig.Compilation);
            window.ShowDialog();
            Show();
        }

        #region "Handlers"
        private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Name = TextboxName.Text.Trim();
            SummaryName.Text = orig.Name;
        }

        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.StatusExtension.Status = ComboBoxStatus.SelectedIndex > 0 ? (Status)ComboBoxStatus.SelectedItem : null;
            SummaryStatus.Content = orig.StatusExtension.Status != null ? orig.StatusExtension.Status.Name : "";
            UpdateDateFieldVisibility();
            UpdateStatusColor();
            UpdateScoreSummary();
        }

        private void CheckboxUnfinishable_Checked(object sender, RoutedEventArgs e)
        {
            orig.IsUnfinishable = CheckboxUnfinishable.IsChecked.Value;
            UpdateDateFieldVisibility();
            FillComboboxStatuses(ComboBoxStatus);
            SummaryStatus.Content = orig.StatusExtension.Status != null ? orig.StatusExtension.Status.Name : "";
            SummaryStartedLabel.Content = orig.IsUnfinishable ? "Played On:" : "Started On:";
            UpdateStatusColor();
        }

        private void CheckboxNotOwned_Checked(object sender, RoutedEventArgs e)
        {
            orig.IsNotOwned = CheckboxNotOwned.IsChecked.Value;
        }

        private void ComboBoxPlatform_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.Platform = ComboBoxPlatform.SelectedIndex > 0 ? (Platform)ComboBoxPlatform.SelectedItem : null;
            SummaryPlatform.Content = orig.PlatformEffective != null ? "On " + orig.PlatformEffective.Name : "";
            UpdateStats();
            UpdatePlatformColor();
        }

        private void ComboBoxPlatformPlayedOn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.PlatformPlayedOn = ComboBoxPlatformPlayedOn.SelectedIndex > 0 ? (Platform)ComboBoxPlatformPlayedOn.SelectedItem : null;
            SummaryPlatform.Content = orig.PlatformEffective != null ? "On " + orig.PlatformEffective.Name : "";
            UpdateStats();
            UpdatePlatformColor();
        }

        private void CheckboxRemaster_Checked(object sender, RoutedEventArgs e)
        {
            orig.IsRemaster = CheckboxRemaster.IsChecked.Value;
            UpdateRemasterFields();
            UpdateScores();
        }

        private void ComboboxOriginalGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.OriginalGame = ComboboxOriginalGame.SelectedIndex > 0 ? (GameObject)ComboboxOriginalGame.SelectedItem : null;
            UpdateRemasterFields();
            UpdateScores();
        }

        private void CheckboxUseOriginalGameScore_Checked(object sender, RoutedEventArgs e)
        {
            orig.UseOriginalGameScore = CheckboxUseOriginalGameScore.IsChecked.Value;
            UpdateRemasterFields();
            UpdateScores();
        }

        private void CheckboxCompilation_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCompilationFields();
        }

        private void TextboxCompilation_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.compName = TextboxComp.Text.Trim();
        }

        private async void TextBoxScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (orig.IsUsingOriginalGameScore || !orig.CategoryExtension.AreCategoryValuesEditable) return;

            TextBox textbox = (TextBox)sender;
            bool result = double.TryParse(textbox.Text, out double score);
            RatingCategory rc = textbox.Tag as RatingCategory;

            if (TextChangeNearPeriod(textbox, e.Changes)) return;

            orig.CategoryExtension.CategoryValuesManual.First(cv => cv.RatingCategory.Equals(rc)).PointValue = result ? score : settings.MinScore;
            UpdateScores();
            await RefreshScoreSuggestions(textbox, rc);
        }

        private void ButtonEditScore_Click(object sender, RoutedEventArgs e)
        {
            orig.ManualScore = orig.ScoreMinIfCyclical;
            orig.CategoryExtension.IgnoreCategories = !orig.CategoryExtension.IgnoreCategories;
            UpdateScores();
        }

        private async void TextBoxFinalScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (orig.IsUsingOriginalGameScore || !orig.CategoryExtension.IgnoreCategories) return;

            var textbox = (TextBox)sender;
            bool result = double.TryParse(textbox.Text, out double score);

            if (TextChangeNearPeriod(textbox, e.Changes)) return;

            orig.ManualScore = result ? score : settings.MinScore;
            UpdateScores();
            await RefreshScoreSuggestions(TextBoxFinalScore);
        }

        private void TextboxCompletionCriteria_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.CompletionCriteria = TextboxCompletionCriteria.Text.Trim();
        }

        private void TextboxCompletionComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.CompletionComment = TextboxCompletionComment.Text.Trim();
        }

        private void TextboxTimeSpent_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.TimeSpent = TextboxTimeSpent.Text.Trim();
        }

        private void DatePickerRelease_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.ReleaseDate = DatePickerRelease.SelectedDate.HasValue ? DatePickerRelease.SelectedDate.Value : DateTime.MinValue;
            SummaryRelease.Content = orig.ReleaseDate.ToShortDateString();
            SummaryReleaseContainer.Visibility = orig.ReleaseDate > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DatePickerAcquired_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.AcquiredOn = DatePickerAcquired.SelectedDate.HasValue ? DatePickerAcquired.SelectedDate.Value : DateTime.MinValue;
            SummaryAcquired.Content = orig.AcquiredOn.ToShortDateString();
            SummaryAcquiredContainer.Visibility = orig.AcquiredOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DatePickerStarted_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.StartedOn = DatePickerStarted.SelectedDate.HasValue ? DatePickerStarted.SelectedDate.Value : DateTime.MinValue;
            SummaryStarted.Content = orig.StartedOn.ToShortDateString();
            SummaryStartedContainer.Visibility = orig.StartedOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DatePickerFinished_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.FinishedOn = DatePickerFinished.SelectedDate.HasValue ? DatePickerFinished.SelectedDate.Value : DateTime.MinValue;
            SummaryFinished.Content = orig.FinishedOn.ToShortDateString();
            SummaryFinishedContainer.Visibility = orig.FinishedOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TextBoxComments_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Comment = TextBoxComments.Text.Trim();
            SummaryThoughts.Text = orig.Comment;
        }

        private void TextBoxGameComments_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.GameComment = TextBoxGameComments.Text.Trim();
        }

        private async void TextBoxScore_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            RatingCategory rc = (RatingCategory)tb.Tag;
            if (showScoreSuggestions)
                await RefreshScoreSuggestions(tb, rc);
            else
                showScoreSuggestions = true;
        }

        private void TextBoxScore_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            ClearPreviousScoreSuggestions(tb);
            if (tb.Text.Length == 0) tb.Text = settings.MinScore.ToString();
        }

        private async void TextBoxFinalScore_GotFocus(object sender, RoutedEventArgs e)
        {
            await RefreshScoreSuggestions(TextBoxFinalScore);
        }

        private void TextBoxFinalScore_LostFocus(object sender, RoutedEventArgs e)
        {
            ClearPreviousScoreSuggestions(TextBoxFinalScore);
            if (TextBoxFinalScore.Text.Length == 0) TextBoxFinalScore.Text = settings.MinScore.ToString();
        }
        #endregion

        private void UpdateScores()
        {
            for (int i = 0; i < rm.CategoryExtension.TotalNumRatingCategories(); i++)
            {
                DockPanel dock = (DockPanel)GridRatingCategories.Children.Cast<UIElement>().First(d => Grid.GetColumn(d) == i);
                TextBox text = dock.FindChild<TextBox>("TextBoxValue");
                text.IsEnabled = orig.CategoryExtension.AreCategoryValuesEditable;
                RatingCategory rc = (RatingCategory)text.Tag;
                text.Text = orig.CategoryExtension.ScoreOfCategory(rc).ToString(UtilWPF.SCORE_FORMAT);
            }

            if (ButtonEditScore.Template.FindName("IconEditScore", ButtonEditScore) is PackIconMaterial icon)
                icon.Kind = !orig.CategoryExtension.IgnoreCategories ? PackIconMaterialKind.Pencil : PackIconMaterialKind.Lock;
            ButtonEditScore.ToolTip = !orig.CategoryExtension.IgnoreCategories ? "Edit the final score manually" : "Use categories to automatically calculate the final score";
            TextBoxFinalScore.IsEnabled = !orig.IsUsingOriginalGameScore && orig.CategoryExtension.IgnoreCategories;

            TextBoxFinalScore.Text = orig.ScoreMinIfCyclical.ToString(UtilWPF.SCORE_FORMAT);
            ScoreRange sr = orig.ScoreRange;
            RatableTracker.Util.Color color = sr == null ? new RatableTracker.Util.Color() : sr.Color;
            if (color.Equals(new RatableTracker.Util.Color()))
            {
                TextBoxFinalScore.Background = new SolidColorBrush(new System.Windows.Media.Color { A = 0xFF, R = 0xF9, G = 0xF9, B = 0xF9 });
            }
            else
            {
                TextBoxFinalScore.Background = new SolidColorBrush(color.ToMediaColor());
            }
            UpdateStats();
            UpdateScoreSummary();
        }

        private void UpdateScoreSummary()
        {
            SummaryScoreContainer.Visibility = orig.ShowScore ? Visibility.Visible : Visibility.Collapsed;
            SummaryScoreContainer.Background = new SolidColorBrush(orig.ShowScore && orig.ScoreRangeDisplay != null ? orig.ScoreRangeDisplay.Color.ToMediaColor() : Colors.White);
            SummaryScore.Content = orig.ScoreMinIfCyclical.ToString(UtilWPF.SCORE_FORMAT);
        }

        private void UpdateRemasterFields()
        {
            ComboboxOriginalGame.Visibility = orig.IsRemaster ? Visibility.Visible : Visibility.Collapsed;
            CheckboxUseOriginalGameScore.Visibility = orig.IsRemaster && orig.HasOriginalGame ? Visibility.Visible : Visibility.Hidden;
            CheckboxUseOriginalGameScore.Content = "Use score of " + (orig.IsRemaster && orig.HasOriginalGame ? orig.OriginalGame.NameAndPlatform : "original game");
            GridRatingCategories.IsEnabled = !orig.IsUsingOriginalGameScore;
            TextBoxFinalScore.IsEnabled = !orig.IsUsingOriginalGameScore && orig.CategoryExtension.IgnoreCategories;
            ButtonEditScore.IsEnabled = !orig.IsUsingOriginalGameScore;
            SummaryRemaster.Content = orig.IsRemaster ? (orig.HasOriginalGame ? "Remaster/re-release of " + orig.OriginalGame.NameAndPlatform : "Is a remaster/re-release") : "";
            SummaryRemaster.Visibility = orig.IsRemaster ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateDateFieldVisibility()
        {
            var hideFinishedOn = orig.IsUnfinishable || !orig.IsFinished;
            StackPanelFinishedOn.Visibility = hideFinishedOn ? Visibility.Hidden : Visibility.Visible;
            Grid.SetColumnSpan(StackPanelStartedOn, hideFinishedOn ? 2 : 1);
            LabelStartedOn.Content = orig.IsUnfinishable ? "Played On" : "Started On";
            if (hideFinishedOn) DatePickerFinished.SelectedDate = DateTime.MinValue;
        }

        private void UpdateStats()
        {
            string text = "";
            if (orig.PlatformEffective != null)
            {
                var platform = orig.PlatformEffective;
                text += "#" + rm.GetRankOfScoreByPlatform(orig.ScoreMinIfCyclical, platform, settings).ToString() + " on " + platform.Name + "\n";
            }
            text += "#" + rm.GetRankOfScore(orig.ScoreMinIfCyclical, settings).ToString() + " overall";
            TextBlockStats.Text = text;
        }

        private void UpdatePlatformColor()
        {
            Resources["PlatformColorStyle"] = new LinearGradientBrush(
                new GradientStopCollection(
                    new List<GradientStop>() {
                        new(orig.PlatformEffective != null ? orig.PlatformEffective.Color.ToMediaColor() : Colors.White, 1),
                        new(Colors.White, 0.1)
                    }
                ),
                new System.Windows.Point(0, 0.5),
                new System.Windows.Point(1, 0.5)
            );
        }

        private void UpdateStatusColor()
        {
            Resources["StatusColorStyle"] = new LinearGradientBrush(
                new GradientStopCollection(
                    new List<GradientStop>() {
                        new(orig.StatusExtension.Status != null ? orig.StatusExtension.Status.Color.ToMediaColor() : Colors.White, 1),
                        new(Colors.White, 0.1)
                    }
                ),
                new System.Windows.Point(0, 0.5),
                new System.Windows.Point(1, 0.5)
            );
        }

        private void UpdateCompilationFields()
        {
            TextboxComp.Visibility = CheckboxCompilation.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
        }

        private bool TextChangeNearPeriod(TextBox textbox, ICollection<TextChange> changes)
        {
            // don't continue past this point if the character just entered was a period, or you delete a character right before a period
            if (changes.Count > 0)
            {
                var change = changes.ElementAt(0);
                if (change.AddedLength > 0)
                {
                    var addition = textbox.Text.Substring(change.Offset, change.AddedLength);
                    if ((addition.Contains('.') || addition.Contains('0')) && textbox.Text.Count((c) => c == '.') == 1) return true;
                }
                else if (change.RemovedLength > 0 && change.Offset > 0)
                {
                    var charBeforeRemoval = textbox.Text[change.Offset - 1];
                    if (charBeforeRemoval == '.' || charBeforeRemoval == '0') return true;
                }
                else if (change.RemovedLength > 0 && change.Offset == 0 && textbox.Text.Length == 0) return true;
            }
            return false;
        }

        private async Task RefreshScoreSuggestions(TextBox textBox, RatingCategory category = null)
        {
            ClearPreviousScoreSuggestions(textBox);
            bool result = double.TryParse(textBox.Text, out double score);
            if (!result)
            {
                textBox.ToolTip = null;
                return;
            }

            var tooltip = new ToolTip();
            var tb = new TextBlock();

            var suggestions = await Task.Run(() => orig.SimilarScoreSuggestions(score, category));
            foreach (var line in suggestions)
            {
                bool newLine = tb.Inlines.Count > 0;
                tb.Inlines.Add(new Run((newLine ? "\n" : "") + line.Item1) { FontWeight = orig.Equals(line.Item2) ? FontWeights.Bold : FontWeights.Normal });
            }
            tooltip.Content = tb;
            if (textBox.ToolTip != null) ((ToolTip)textBox.ToolTip).IsOpen = false;
            textBox.ToolTip = tooltip;
            tooltip.PlacementTarget = textBox;
            tooltip.IsOpen = true;
        }

        private void ClearPreviousScoreSuggestions(TextBox textBox)
        {
            if (textBox.ToolTip != null) ((ToolTip)textBox.ToolTip).IsOpen = false;
        }

        private void TabList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showScoreSuggestions = TabList.SelectedIndex != 3;
        }
    }
}
