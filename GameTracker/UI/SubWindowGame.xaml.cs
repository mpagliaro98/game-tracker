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
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Exceptions;

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for SubWindowGame.xaml
    /// </summary>
    public partial class SubWindowGame : Window
    {
        private RatingModuleGame rm;
        private RatableGame orig;
        
        public SubWindowGame(RatingModuleGame rm, SubWindowMode mode, RatableGame orig = null)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            FillComboboxStatuses(ComboBoxStatus);
            FillComboboxPlatforms(ComboBoxPlatform);
            FillComboboxPlatforms(ComboBoxPlatformPlayedOn);
            FillComboboxGames(ComboboxOriginalGame);
            ComboBoxStatus.SelectedIndex = 0;
            ComboBoxPlatform.SelectedIndex = 0;
            ComboBoxPlatformPlayedOn.SelectedIndex = 0;
            ComboboxOriginalGame.SelectedIndex = 0;
            bool defaultIgnore = new RatableGame().IgnoreCategories;
            CreateRatingCategories(rm.RatingCategories,
                orig == null ? new List<RatingCategoryValue>() :
                    orig.IsRemaster && orig.UseOriginalGameScore ? rm.FindListedObject(orig.RefOriginalGame).CategoryValues : orig.CategoryValues,
                rm.Settings,
                orig == null ? !defaultIgnore : !orig.IgnoreCategories);
            UpdateScoreEditButton(orig == null ? !defaultIgnore : !orig.IgnoreCategories);
            switch (mode)
            {
                case SubWindowMode.MODE_ADD:
                    ButtonSave.Visibility = Visibility.Visible;
                    ButtonUpdate.Visibility = Visibility.Collapsed;
                    break;
                case SubWindowMode.MODE_EDIT:
                    ButtonSave.Visibility = Visibility.Collapsed;
                    ButtonUpdate.Visibility = Visibility.Visible;
                    TextboxName.Text = orig.Name;
                    if (orig.RefStatus.HasReference()) ComboBoxStatus.SelectedItem = rm.FindStatus(orig.RefStatus);
                    if (orig.RefPlatform.HasReference()) ComboBoxPlatform.SelectedItem = rm.FindPlatform(orig.RefPlatform);
                    if (orig.RefPlatformPlayedOn.HasReference()) ComboBoxPlatformPlayedOn.SelectedItem = rm.FindPlatform(orig.RefPlatformPlayedOn);
                    if (orig.RefOriginalGame.HasReference()) ComboboxOriginalGame.SelectedItem = rm.FindListedObject(orig.RefOriginalGame);
                    TextboxCompletionCriteria.Text = orig.CompletionCriteria;
                    TextboxCompletionComment.Text = orig.CompletionComment;
                    TextboxTimeSpent.Text = orig.TimeSpent;
                    if (!orig.AcquiredOn.Equals(DateTime.MinValue)) DatePickerAcquired.SelectedDate = orig.AcquiredOn;
                    if (!orig.StartedOn.Equals(DateTime.MinValue)) DatePickerStarted.SelectedDate = orig.StartedOn;
                    if (!orig.FinishedOn.Equals(DateTime.MinValue)) DatePickerFinished.SelectedDate = orig.FinishedOn;
                    TextBoxComments.Text = orig.Comment;
                    TextBoxFinalScore.Text = rm.GetScoreOfObject(orig).ToString("0.##");
                    CheckboxRemaster.IsChecked = orig.IsRemaster;
                    CheckboxUseOriginalGameScore.IsChecked = orig.UseOriginalGameScore;
                    break;
                default:
                    throw new Exception("Unhandled mode");
            }
            UpdateRemasterFields();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();
        }

        private void SaveResult()
        {
            if (!ValidateInputs(out string name, out CompletionStatus status, out Platform platform,
                out Platform platformPlayedOn, out string completionCriteria, out string completionComment,
                out string timeSpent, out DateTime acquiredOn, out DateTime startedOn, out DateTime finishedOn,
                out string comment, out bool isRemaster, out RatableGame originalGame, out bool useOriginalGameScore)) return;
            if (!ValidateScores(out IEnumerable<RatingCategoryValue> vals, out double finalScore,
                out bool ignoreCategories)) return;
            var game = new RatableGame()
            {
                Name = name,
                CompletionCriteria = completionCriteria,
                CompletionComment = completionComment,
                TimeSpent = timeSpent,
                AcquiredOn = acquiredOn,
                StartedOn = startedOn,
                FinishedOn = finishedOn,
                Comment = comment,
                IgnoreCategories = ignoreCategories,
                FinalScoreManual = finalScore,
                CategoryValues = vals,
                IsRemaster = isRemaster,
                UseOriginalGameScore = useOriginalGameScore
            };
            if (status != null)
                game.SetStatus(status);
            else
                game.RemoveStatus();
            if (platform != null)
                game.SetPlatform(platform);
            else
                game.RemovePlatform();
            if (platformPlayedOn != null)
                game.SetPlatformPlayedOn(platformPlayedOn);
            else
                game.RemovePlatformPlayedOn();
            if (originalGame != null)
                game.SetOriginalGame(originalGame);
            else
                game.RemoveOriginalGame();
            try
            {
                if (orig == null)
                    rm.AddListedObject(game);
                else
                    rm.UpdateListedObject(game, orig);
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
            out Platform platformPlayedOn, out string completionCriteria, out string completionComment,
            out string timeSpent, out DateTime acquiredOn, out DateTime startedOn, out DateTime finishedOn,
            out string comment, out bool isRemaster, out RatableGame originalGame, out bool useOriginalGameScore)
        {
            name = TextboxName.Text;
            status = ComboBoxStatus.SelectedIndex == 0 ? null : (CompletionStatus)ComboBoxStatus.SelectedItem;
            platform = ComboBoxPlatform.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatform.SelectedItem;
            platformPlayedOn = ComboBoxPlatformPlayedOn.SelectedIndex == 0 ? null : (Platform)ComboBoxPlatformPlayedOn.SelectedItem;
            completionCriteria = TextboxCompletionCriteria.Text;
            completionComment = TextboxCompletionComment.Text;
            timeSpent = TextboxTimeSpent.Text;
            acquiredOn = DatePickerAcquired.SelectedDate.HasValue ? DatePickerAcquired.SelectedDate.Value : DateTime.MinValue;
            startedOn = DatePickerStarted.SelectedDate.HasValue ? DatePickerStarted.SelectedDate.Value : DateTime.MinValue;
            finishedOn = DatePickerFinished.SelectedDate.HasValue ? DatePickerFinished.SelectedDate.Value : DateTime.MinValue;
            comment = TextBoxComments.Text;
            isRemaster = CheckboxRemaster.IsChecked.Value;
            originalGame = isRemaster && ComboboxOriginalGame.SelectedIndex > 0 ? (RatableGame)ComboboxOriginalGame.SelectedItem : null;
            useOriginalGameScore = isRemaster && originalGame != null ? CheckboxUseOriginalGameScore.IsChecked.Value : false;
            if (name == "")
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = "A name is required";
                return false;
            }
            return true;
        }

        private bool ValidateScores(out IEnumerable<RatingCategoryValue> vals, out double finalScore,
            out bool ignoreCategories)
        {
            if (CheckboxRemaster.IsChecked.Value && ComboboxOriginalGame.SelectedIndex > 0 && CheckboxUseOriginalGameScore.IsChecked.Value)
            {
                vals = orig == null ? new List<RatingCategoryValue>() : orig.CategoryValues;
                finalScore = orig == null ? rm.Settings.MinScore : orig.FinalScoreManual;
                ignoreCategories = orig == null ? new RatableGame().IgnoreCategories : orig.IgnoreCategories;
                return true;
            }
            vals = GetCategoryValueInputs(rm.RatingCategories);
            ignoreCategories = TextBoxFinalScore.IsEnabled;
            if (!double.TryParse(TextBoxFinalScore.Text, out finalScore))
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = "The value of score must be a number";
                return false;
            }
            try
            {
                rm.ValidateScore(finalScore);
                rm.ValidateCategoryScores(vals);
            }
            catch (ValidationException e)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = e.Message;
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
            foreach (CompletionStatus cs in rm.Statuses)
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
            foreach (Platform platform in rm.Platforms)
            {
                cb.Items.Add(platform);
            }
        }

        private void FillComboboxGames(ComboBox cb)
        {
            cb.Items.Clear();
            var item = new ComboBoxItem();
            item.Content = "--Select the game this is a remaster of--";
            cb.Items.Add(item);
            foreach (RatableGame game in rm.ListedObjects.OrderBy(ro => ro.Name))
            {
                cb.Items.Add(game);
            }
        }

        private void CreateRatingCategories(IEnumerable<RatingCategory> ratingCategories,
            IEnumerable<RatingCategoryValue> pointValues, SettingsScore settings, bool isEnabled)
        {
            GridRatingCategories.Children.Clear();
            GridRatingCategories.ColumnDefinitions.Clear();
            int i = 0;
            foreach (RatingCategory rc in ratingCategories)
            {
                GridRatingCategories.ColumnDefinitions.Add(new ColumnDefinition());
                DockPanel dock = new DockPanel
                {
                    Background = new SolidColorBrush(new Color { A = 0xFF, R = 0xF1, G = 0xF1, B = 0xF1 }),
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

                var matches = pointValues.Where(rcv => rcv.RefRatingCategory.IsReferencedObject(rc));
                RatingCategoryValue match = matches.FirstOrDefault();
                TextBox text = new TextBox
                {
                    Name = "TextBoxValue",
                    Margin = new Thickness { Bottom = 5, Left = 5, Right = 5 },
                    BorderThickness = new Thickness { Top = 0, Left = 0, Bottom = 0, Right = 0 },
                    Background = new SolidColorBrush(new Color { A = 0xFF, R = 0xF9, G = 0xF9, B = 0xF9}),
                    FontSize = 32,
                    IsEnabled = isEnabled,
                    Text = match == null ? settings.MinScore.ToString("0.##") : match.PointValue.ToString()
                };
                text.TextChanged += TextBoxScore_TextChanged;
                dock.Children.Add(label);
                dock.Children.Add(text);
                Grid.SetColumn(dock, i);
                GridRatingCategories.Children.Add(dock);
                i++;
            }
        }

        private double CalculateFinalScoreFromText()
        {
            IEnumerable<RatingCategoryValue> vals = GetCategoryValueInputs(rm.RatingCategories);
            return rm.GetScoreOfCategoryValues(vals);
        }

        private IEnumerable<RatingCategoryValue> GetCategoryValueInputs(IEnumerable<RatingCategory> ratingCategories)
        {
            List<RatingCategoryValue> result = new List<RatingCategoryValue>();
            ratingCategories = ratingCategories.ToList();
            for (int i = 0; i < ratingCategories.Count(); i++)
            {
                RatingCategory rc = ratingCategories.ElementAt(i);
                DockPanel dock = (DockPanel)GridRatingCategories.Children.Cast<UIElement>().First(d => Grid.GetColumn(d) == i);
                TextBox text = dock.FindChild<TextBox>("TextBoxValue");
                bool valid = double.TryParse(text.Text, out double value);
                if (!valid) value = rm.Settings.MinScore;
                RatingCategoryValue rcv = new RatingCategoryValue
                {
                    PointValue = value
                };
                rcv.SetRatingCategory(rc);
                result.Add(rcv);
            }
            return result;
        }

        private void TextBoxScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFinalScoreTextBoxAuto();
        }

        private void UpdateScoreEditButton(bool finalScoreEnabled)
        {
            TextBoxFinalScore.IsEnabled = !finalScoreEnabled;
            for (int i = 0; i < rm.RatingCategories.Count(); i++)
            {
                DockPanel dock = (DockPanel)GridRatingCategories.Children.Cast<UIElement>().First(d => Grid.GetColumn(d) == i);
                TextBox text = dock.FindChild<TextBox>("TextBoxValue");
                text.IsEnabled = finalScoreEnabled;
            }
            Image image = new Image();
            image.Source = (ImageSource)Resources[finalScoreEnabled ? "ButtonEdit" : "ButtonLock"];
            ButtonEditScore.Content = image;
            ButtonEditScore.ToolTip = finalScoreEnabled ? "Edit the final score manually" : "Use categories to automatically calculate the final score";
            if (finalScoreEnabled)
                UpdateFinalScoreTextBoxAuto();
            else
                UpdateFinalScoreTextBox();
        }

        private void ButtonEditScore_Click(object sender, RoutedEventArgs e)
        {
            UpdateScoreEditButton(TextBoxFinalScore.IsEnabled);
        }

        private void UpdateFinalScoreTextBoxAuto()
        {
            double score = CalculateFinalScoreFromText();
            TextBoxFinalScore.Text = score.ToString("0.##");
            FinalScoreUpdate(score);
        }

        private void UpdateFinalScoreColor(double score)
        {
            System.Drawing.Color color = rm.GetRangeColorFromValue(score);
            if (color.Equals(new System.Drawing.Color()))
            {
                TextBoxFinalScore.Background = new SolidColorBrush(new Color { A = 0xFF, R = 0xF9, G = 0xF9, B = 0xF9 });
            }
            else
            {
                TextBoxFinalScore.Background = new SolidColorBrush(color.ToMediaColor());
            }
        }

        private void TextBoxFinalScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFinalScoreTextBox();
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

        private void CheckboxRemaster_Checked(object sender, RoutedEventArgs e)
        {
            UpdateRemasterFields();
        }

        private void ComboboxOriginalGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRemasterFields();
        }

        private void CheckboxUseOriginalGameScore_Checked(object sender, RoutedEventArgs e)
        {
            UpdateRemasterFields();
        }

        private void UpdateRemasterFields()
        {
            ComboboxOriginalGame.Visibility = CheckboxRemaster.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            CheckboxUseOriginalGameScore.Visibility = CheckboxRemaster.IsChecked.Value && ComboboxOriginalGame.SelectedIndex > 0 ? Visibility.Visible : Visibility.Hidden;
            bool useOriginalGame = CheckboxRemaster.IsChecked.Value && ComboboxOriginalGame.SelectedIndex > 0 && CheckboxUseOriginalGameScore.IsChecked.Value;
            GridRatingCategories.IsEnabled = !useOriginalGame;
            GridFinalScore.IsEnabled = !useOriginalGame;

            bool defaultIgnore = new RatableGame().IgnoreCategories;
            RatableGame originalGame = ComboboxOriginalGame.SelectedIndex > 0 ? (RatableGame)ComboboxOriginalGame.SelectedItem : null;
            CreateRatingCategories(rm.RatingCategories,
                useOriginalGame ? originalGame.CategoryValues : orig == null ? new List<RatingCategoryValue>() : orig.CategoryValues,
                rm.Settings, orig == null ? !defaultIgnore : !orig.IgnoreCategories);
            UpdateScoreEditButton(orig == null ? !defaultIgnore : !orig.IgnoreCategories);
        }
    }
}
