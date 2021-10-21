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
using RatableTracker.Framework.ObjectHierarchy;

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
            ComboBoxStatus.SelectedIndex = 0;
            ComboBoxPlatform.SelectedIndex = 0;
            ComboBoxPlatformPlayedOn.SelectedIndex = 0;
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
                    TextboxCompletionCriteria.Text = orig.CompletionCriteria;
                    TextboxCompletionComment.Text = orig.CompletionComment;
                    TextboxTimeSpent.Text = orig.TimeSpent;
                    if (!orig.AcquiredOn.Equals(DateTime.MinValue)) DatePickerAcquired.SelectedDate = orig.AcquiredOn;
                    if (!orig.StartedOn.Equals(DateTime.MinValue)) DatePickerStarted.SelectedDate = orig.StartedOn;
                    if (!orig.FinishedOn.Equals(DateTime.MinValue)) DatePickerFinished.SelectedDate = orig.FinishedOn;
                    TextBoxComments.Text = orig.Comment;
                    break;
                default:
                    throw new Exception("Unhandled mode");
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out CompletionStatus status, out Platform platform,
                out Platform platformPlayedOn, out string completionCriteria, out string completionComment,
                out string timeSpent, out DateTime acquiredOn, out DateTime startedOn, out DateTime finishedOn,
                out string comment)) return;
            var game = new RatableGame()
            {
                Name = name,
                CompletionCriteria = completionCriteria,
                CompletionComment = completionComment,
                TimeSpent = timeSpent,
                AcquiredOn = acquiredOn,
                StartedOn = startedOn,
                FinishedOn = finishedOn,
                Comment = comment
            };
            if (status != null)
                orig.SetStatus(status);
            else
                orig.RemoveStatus();
            if (platform != null)
                orig.SetPlatform(platform);
            else
                orig.RemovePlatform();
            if (platformPlayedOn != null)
                orig.SetPlatformPlayedOn(platformPlayedOn);
            else
                orig.RemovePlatformPlayedOn();
            rm.AddListedObject(game);
            Close();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out CompletionStatus status, out Platform platform,
                out Platform platformPlayedOn, out string completionCriteria, out string completionComment,
                out string timeSpent, out DateTime acquiredOn, out DateTime startedOn, out DateTime finishedOn,
                out string comment)) return;
            orig.Name = name;
            if (status != null)
                orig.SetStatus(status);
            else
                orig.RemoveStatus();
            if (platform != null)
                orig.SetPlatform(platform);
            else
                orig.RemovePlatform();
            if (platformPlayedOn != null)
                orig.SetPlatformPlayedOn(platformPlayedOn);
            else
                orig.RemovePlatformPlayedOn();
            orig.CompletionCriteria = completionCriteria;
            orig.CompletionComment = completionComment;
            orig.TimeSpent = timeSpent;
            orig.AcquiredOn = acquiredOn;
            orig.StartedOn = startedOn;
            orig.FinishedOn = finishedOn;
            orig.Comment = comment;
            rm.SaveListedObjects();
            Close();
        }

        private bool ValidateInputs(out string name, out CompletionStatus status, out Platform platform,
            out Platform platformPlayedOn, out string completionCriteria, out string completionComment,
            out string timeSpent, out DateTime acquiredOn, out DateTime startedOn, out DateTime finishedOn,
            out string comment)
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
            if (name == "")
            {
                LabelError.Visibility = Visibility.Visible;
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
    }
}
