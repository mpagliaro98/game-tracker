using GameTracker;
using MahApps.Metro.Controls;
using RatableTracker.Exceptions;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowCompilation.xaml
    /// </summary>
    public partial class SubWindowCompilation : MetroWindow
    {
        private GameModule rm;
        private SettingsGame settings;
        private GameCompilation orig;
        private string originalName;

        public event EventHandler Saved;

        public SubWindowCompilation(GameModule rm, SettingsGame settings, SubWindowMode mode, GameCompilation orig)
        {
            InitializeComponent();
            this.rm = rm;
            this.settings = settings;
            this.orig = orig;
            this.originalName = orig.Name;

            // initialize UI containers
            CreateRatingCategories();
            FillComboboxStatuses(ComboBoxStatus);
            FillComboboxPlatforms(ComboBoxPlatform);
            FillComboboxPlatforms(ComboBoxPlatformPlayedOn, defaultItem: "Same as Platform");
            ComboBoxStatus.SelectedIndex = 0;
            ComboBoxPlatform.SelectedIndex = 0;
            ComboBoxPlatformPlayedOn.SelectedIndex = 0;
            CreateGamesInCompilation();
            ButtonUpdate.Visibility = (mode == SubWindowMode.MODE_VIEW ? Visibility.Collapsed : Visibility.Visible);
            TextboxName.IsReadOnly = (mode == SubWindowMode.MODE_VIEW);
            ComboBoxStatus.IsEnabled = (mode != SubWindowMode.MODE_VIEW);
            ComboBoxPlatform.IsEnabled = (mode != SubWindowMode.MODE_VIEW);
            ComboBoxPlatformPlayedOn.IsEnabled = (mode != SubWindowMode.MODE_VIEW);
            TextBoxGameComments.IsEnabled = (mode != SubWindowMode.MODE_VIEW);

            // set max length
            TextboxName.MaxLength = GameCompilation.MaxLengthName;
            TextBoxGameComments.MaxLength = GameCompilation.MaxLengthGameComment;

            // set fields in the UI
            TextboxName.Text = orig.Name;
            if (orig.StatusExtension.Status != null) ComboBoxStatus.SelectedItem = orig.StatusExtension.Status;
            if (orig.Platform != null) ComboBoxPlatform.SelectedItem = orig.Platform;
            if (orig.PlatformPlayedOn != null) ComboBoxPlatformPlayedOn.SelectedItem = orig.PlatformPlayedOn;
            TextBoxFinalScore.Text = orig.Score.ToString(UtilWPF.SCORE_FORMAT);
            TextBoxGameComments.Text = orig.GameComment;
            CheckboxUnfinishable.IsChecked = orig.IsUnfinishable;
            CheckboxNotOwned.IsChecked = orig.IsNotOwned;

            SummaryName.Text = string.IsNullOrEmpty(orig.Name) ? "Save this game to see summary info" : orig.Name;
            SummaryPlatform.Content = orig.PlatformEffective != null ? "On " + orig.PlatformEffective.Name : "";
            SummaryStatus.Content = orig.StatusExtension.Status != null ? orig.StatusExtension.Status.Name : "";
            SummaryRelease.Content = orig.ReleaseDate.ToShortDateString();
            SummaryReleaseContainer.Visibility = orig.ReleaseDate > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
            SummaryAcquired.Content = orig.AcquiredOn.ToShortDateString();
            SummaryAcquiredContainer.Visibility = orig.AcquiredOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
            SummaryStarted.Content = orig.StartedOn.ToShortDateString();
            SummaryStartedContainer.Visibility = orig.StartedOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;
            SummaryStartedLabel.Content = orig.IsUnfinishable ? "Played On:" : "Started On:";
            SummaryFinished.Content = orig.FinishedOn.ToShortDateString();
            SummaryFinishedContainer.Visibility = orig.FinishedOn > DateTime.MinValue ? Visibility.Visible : Visibility.Collapsed;

            // set event handlers
            TextboxName.TextChanged += TextboxName_TextChanged;
            ComboBoxStatus.SelectionChanged += ComboBoxStatus_SelectionChanged;
            ComboBoxPlatform.SelectionChanged += ComboBoxPlatform_SelectionChanged;
            ComboBoxPlatformPlayedOn.SelectionChanged += ComboBoxPlatformPlayedOn_SelectionChanged;
            TextBoxGameComments.TextChanged += TextBoxGameComments_TextChanged;

            // refresh UI logic
            UpdateStats();
            UpdateScoreSummary();
            UpdatePlatformColor();
            UpdateStatusColor();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (orig.Name.Length > 0 && !orig.Name.Equals(originalName))
                {
                    var matches = rm.GetModelObjectList(settings).OfType<GameCompilation>().Where(c => c.Name.ToLower().Equals(orig.Name.ToLower())).ToList();
                    if (matches.Count > 0)
                        throw new ValidationException("A compilation with that name already exists");
                }
                orig.Save(rm, settings);
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
        }

        private void CreateRatingCategories()
        {
            GridRatingCategories.Children.Clear();
            GridRatingCategories.ColumnDefinitions.Clear();
            int i = 0;
            foreach (CategoryValue categoryValue in orig.CategoryExtension.CategoryValueList)
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
                    Background = new SolidColorBrush(new System.Windows.Media.Color { A = 0xFF, R = 0xF9, G = 0xF9, B = 0xF9 }),
                    FontSize = 32,
                    IsReadOnly = true,
                    Text = categoryValue.PointValue.ToString()
                };
                text.Tag = rc;
                dock.Children.Add(label);
                dock.Children.Add(text);
                Grid.SetColumn(dock, i);
                GridRatingCategories.Children.Add(dock);
                i++;
            }
        }

        private void CreateGamesInCompilation()
        {
            var games = orig.GamesInCompilation();
            GamesListBoxWrap.Items.Clear();
            foreach (var game in games)
            {
                IListBoxItemGame item = new ListBoxItemGameBox(rm, game);
                GamesListBoxWrap.Items.Add(item);
            }
        }

        private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Name = TextboxName.Text.Trim();
            SummaryName.Text = orig.Name;
        }

        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.StatusExtension.Status = ComboBoxStatus.SelectedIndex > 0 ? (Status)ComboBoxStatus.SelectedItem : null;
            SummaryStatus.Content = orig.StatusExtension.Status != null ? orig.StatusExtension.Status.Name : "";
            UpdateStatusColor();
            UpdateScoreSummary();
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
            UpdatePlatformColor();
        }

        private void TextBoxGameComments_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.GameComment = TextBoxGameComments.Text.Trim();
        }

        private void UpdateStats()
        {
            string text = "";
            if (orig.Platform != null)
            {
                var platform = orig.Platform;
                text += "#" + rm.GetRankOfScoreByPlatform(orig.Score, platform, settings).ToString() + " on " + platform.Name + "\n";
            }
            text += "#" + rm.GetRankOfScore(orig.Score, settings).ToString() + " overall";
            TextBlockStats.Text = text;
        }

        private void UpdateScoreSummary()
        {
            SummaryScoreContainer.Visibility = orig.ShowScore ? Visibility.Visible : Visibility.Collapsed;
            SummaryScoreContainer.Background = new SolidColorBrush(orig.ShowScore && orig.ScoreRangeDisplay != null ? orig.ScoreRangeDisplay.Color.ToMediaColor() : Colors.White);
            SummaryScore.Content = orig.ScoreMinIfCyclical.ToString(UtilWPF.SCORE_FORMAT);
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
    }
}
