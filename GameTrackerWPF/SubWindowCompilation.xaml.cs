using GameTracker;
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
    public partial class SubWindowCompilation : Window
    {
        private GameModule rm;
        private SettingsGame settings;
        private GameCompilation orig;

        public SubWindowCompilation(GameModule rm, SettingsGame settings, SubWindowMode mode, GameCompilation orig)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.settings = settings;
            this.orig = orig;
            FillComboboxStatuses(ComboBoxStatus);
            FillComboboxPlatforms(ComboBoxPlatform);
            FillComboboxPlatforms(ComboBoxPlatformPlayedOn);
            ComboBoxStatus.SelectedIndex = 0;
            ComboBoxPlatform.SelectedIndex = 0;
            ComboBoxPlatformPlayedOn.SelectedIndex = 0;
            CreateRatingCategories(rm, orig);
            CreateGamesInCompilation(rm, orig);

            TextboxName.Text = orig.Name;
            if (orig.StatusExtension.Status != null) ComboBoxStatus.SelectedItem = orig.StatusExtension.Status;
            if (orig.Platform != null) ComboBoxPlatform.SelectedItem = orig.Platform;
            if (orig.PlatformPlayedOn != null) ComboBoxPlatformPlayedOn.SelectedItem = orig.PlatformPlayedOn;
            TextBoxFinalScore.Text = orig.Score.ToString("0.##");
            UpdateFinalScoreTextBox();

            if (mode == SubWindowMode.MODE_VIEW)
            {
                TextboxName.IsReadOnly = true;
                ComboBoxStatus.IsEnabled = false;
                ComboBoxPlatform.IsEnabled = false;
                ComboBoxPlatformPlayedOn.IsEnabled = false;
                ButtonUpdate.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();
        }

        private void SaveResult()
        {
            orig.Name = TextboxName.Text;
            orig.StatusExtension.Status = ComboBoxStatus.SelectedIndex == 0 ? null : (StatusGame)ComboBoxStatus.SelectedItem;
            orig.Platform = ComboBoxPlatform.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatform.SelectedItem;
            orig.PlatformPlayedOn = ComboBoxPlatformPlayedOn.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatformPlayedOn.SelectedItem;
            try
            {
                orig.Save(rm, settings);
            }
            catch (ValidationException e)
            {
                e.DisplayUIExceptionMessage();
                return;
            }
            Close();
        }

        private void FillComboboxStatuses(ComboBox cb)
        {
            cb.Items.Clear();
            var item = new ComboBoxItem();
            item.Content = "N/A";
            cb.Items.Add(item);
            foreach (StatusGame cs in rm.StatusExtension.GetStatusList().OrderBy(s => s.Name))
            {
                cb.Items.Add(cs);
            }
        }

        private void FillComboboxPlatforms(ComboBox cb)
        {
            cb.Items.Clear();
            var item = new ComboBoxItem();
            item.Content = "N/A";
            cb.Items.Add(item);
            foreach (Platform platform in rm.GetPlatformList().OrderBy(p => p.Name))
            {
                cb.Items.Add(platform);
            }
        }

        private void CreateRatingCategories(GameModule rm, GameCompilation gc)
        {
            GridRatingCategories.Children.Clear();
            GridRatingCategories.ColumnDefinitions.Clear();
            int i = 0;
            foreach (RatingCategoryWeighted rc in rm.CategoryExtension.GetRatingCategoryList())
            {
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
                    Text = gc.CategoryExtension.ScoreOfCategory(rc).ToString("0.##")
                };
                dock.Children.Add(label);
                dock.Children.Add(text);
                Grid.SetColumn(dock, i);
                GridRatingCategories.Children.Add(dock);
                i++;
            }
        }

        private void UpdateFinalScoreColor(double score)
        {
            ScoreRange sr = score.GetScoreRange(rm);
            RatableTracker.Util.Color color = sr == null ? new RatableTracker.Util.Color() : sr.Color;
            if (color.Equals(new RatableTracker.Util.Color()))
            {
                TextBoxFinalScore.Background = new SolidColorBrush(new System.Windows.Media.Color { A = 0xFF, R = 0xF9, G = 0xF9, B = 0xF9 });
            }
            else
            {
                TextBoxFinalScore.Background = new SolidColorBrush(color.ToMediaColor());
            }
        }

        private void UpdateFinalScoreTextBox()
        {
            bool result = double.TryParse(TextBoxFinalScore.Text, out double score);
            if (result) FinalScoreUpdate(score);
        }

        private void FinalScoreUpdate(double score)
        {
            UpdateFinalScoreColor(score);
            UpdateStats(score);
        }

        private void UpdateStats(double score)
        {
            // TODO
            int rankOverall = orig.Rank;
            int rankPlatform = -1;
            Platform platform = ComboBoxPlatform.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatform.SelectedItem;
            if (platform != null) rankPlatform = 1; // rm.GetRankOfScoreByPlatform(score, platform, orig);

            string text = "";
            if (rankPlatform > 0) text += "#" + rankPlatform.ToString() + " on " + platform.Name + "\n";
            text += "#" + rankOverall.ToString() + " overall";
            TextBlockStats.Text = text;
        }

        private void ComboBoxPlatform_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFinalScoreTextBox();
        }

        private void CreateGamesInCompilation(GameModule rm, GameCompilation gc)
        {
            var games = gc.GamesInCompilation();
            GamesListBoxWrap.Items.Clear();
            foreach (var game in games)
            {
                IListBoxItemGame item = new ListBoxItemGameBox(rm, game);
                GamesListBoxWrap.Items.Add(item);
            }
        }
    }
}
