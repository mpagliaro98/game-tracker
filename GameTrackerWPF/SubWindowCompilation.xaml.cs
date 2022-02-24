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
using GameTracker.Model;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Global;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowCompilation.xaml
    /// </summary>
    public partial class SubWindowCompilation : Window
    {
        private RatingModuleGame rm;
        private GameCompilation orig;

        public SubWindowCompilation(RatingModuleGame rm, SubWindowMode mode, GameCompilation orig)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
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
            if (orig.RefStatus.HasReference()) ComboBoxStatus.SelectedItem = rm.FindStatus(orig.RefStatus);
            if (orig.RefPlatform.HasReference()) ComboBoxPlatform.SelectedItem = rm.FindPlatform(orig.RefPlatform);
            if (orig.RefPlatformPlayedOn.HasReference()) ComboBoxPlatformPlayedOn.SelectedItem = rm.FindPlatform(orig.RefPlatformPlayedOn);
            TextBoxFinalScore.Text = rm.GetScoreOfObject(orig).ToString("0.##");
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
            if (!ValidateInputs(out string name, out CompletionStatus status, out Platform platform,
                out Platform platformPlayedOn)) return;
            var comp = new GameCompilation()
            {
                Name = name
            };
            if (status != null)
                comp.SetStatus(status);
            else
                comp.RemoveStatus();
            if (platform != null)
                comp.SetPlatform(platform);
            else
                comp.RemovePlatform();
            if (platformPlayedOn != null)
                comp.SetPlatformPlayedOn(platformPlayedOn);
            else
                comp.RemovePlatformPlayedOn();
            try
            {
                rm.UpdateGameCompilation(comp, orig);
            }
            catch (ValidationException e)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = e.Message;
                return;
            }
            Close();
        }

        private bool ValidateInputs(out string name, out CompletionStatus status, out Platform platform,
            out Platform platformPlayedOn)
        {
            name = TextboxName.Text;
            status = ComboBoxStatus.SelectedIndex == 0 ? null : (CompletionStatus)ComboBoxStatus.SelectedItem;
            platform = ComboBoxPlatform.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatform.SelectedItem;
            platformPlayedOn = ComboBoxPlatformPlayedOn.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatformPlayedOn.SelectedItem;
            if (name == "")
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = "A name is required";
                return false;
            }
            return true;
        }

        private void FillComboboxStatuses(ComboBox cb)
        {
            cb.Items.Clear();
            var item = new ComboBoxItem();
            item.Content = "N/A";
            cb.Items.Add(item);
            foreach (CompletionStatus cs in rm.Statuses.OrderBy(s => s.Name))
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
            foreach (Platform platform in rm.Platforms.OrderBy(p => p.Name))
            {
                cb.Items.Add(platform);
            }
        }

        private void CreateRatingCategories(RatingModuleGame rm, GameCompilation gc)
        {
            GridRatingCategories.Children.Clear();
            GridRatingCategories.ColumnDefinitions.Clear();
            int i = 0;
            foreach (RatingCategoryWeighted rc in rm.RatingCategories)
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
                    Text = rm.GetScoreOfCategory(gc, rc).ToString("0.##")
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
            RatableTracker.Framework.Color color = rm.GetRangeColorFromValue(score);
            if (color.Equals(new RatableTracker.Framework.Color()))
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
            int rankOverall = rm.GetRankOfScore(score, orig);
            int rankPlatform = -1;
            Platform platform = ComboBoxPlatform.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatform.SelectedItem;
            if (platform != null) rankPlatform = rm.GetRankOfScoreByPlatform(score, platform, orig);

            string text = "";
            if (rankPlatform > 0) text += "#" + rankPlatform.ToString() + " on " + platform.Name + "\n";
            text += "#" + rankOverall.ToString() + " overall";
            TextBlockStats.Text = text;
        }

        private void ComboBoxPlatform_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFinalScoreTextBox();
        }

        private void CreateGamesInCompilation(RatingModuleGame rm, GameCompilation gc)
        {
            var games = rm.FindGamesInCompilation(gc);
            GamesListBoxWrap.Items.Clear();
            foreach (var game in games)
            {
                IListBoxItemGame item = new ListBoxItemGameBox(rm, game);
                GamesListBoxWrap.Items.Add(item);
            }
        }
    }
}
