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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameTracker.Model;
using RatableTracker.Framework.PathController;
using RatableTracker.Framework;
using RatableTracker.Framework.Global;

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string TAB_GAMES = "TabGames";
        private const string TAB_PLATFORMS = "TabPlatforms";
        private const string TAB_SETTINGS = "TabSettings";

        private RatingModuleGame rm;

        public MainWindow()
        {
            PathController.PathControllerInstance = new PathControllerWindows();
            GlobalSettings.Autosave = true;
            rm = new RatingModuleGame(new LoadSaveEngineGameJson());
            rm.Init();
            InitializeComponent();
        }

        #region General Functionality and Utilities
        private void TabsBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != TabsBase)
            {
                return;
            }
            TabItem tab = (TabItem)TabsBase.SelectedItem;
            switch (tab.Name)
            {
                case TAB_GAMES:
                    UpdateGamesUI();
                    break;
                case TAB_PLATFORMS:
                    break;
                case TAB_SETTINGS:
                    ResetSettingsLabels();
                    UpdateSettingsUI();
                    UpdateRatingCategoryUI();
                    UpdateCompletionStatusUI();
                    UpdateScoreRangeUI();
                    break;
                default:
                    throw new Exception("Unhandled tab");
            }
            e.Handled = true;
        }

        private T GetControlFromMenuItem<T>(MenuItem menuItem) where T : UIElement
        {
            while (menuItem.Parent is MenuItem)
            {
                menuItem = (MenuItem)menuItem.Parent;
            }
            if (menuItem.Parent is ContextMenu menu)
            {
                return menu.PlacementTarget as T;
            }
            throw new Exception("Could not find parent control for menu item");
        }

        private ContextMenu EditDeleteContextMenu(RoutedEventHandler editFunc, RoutedEventHandler deleteFunc)
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mie = new MenuItem();
            mie.Header = "Edit";
            if (editFunc != null) mie.Click += editFunc;
            cm.Items.Add(mie);
            MenuItem mid = new MenuItem();
            mid.Header = "Delete";
            if (deleteFunc != null) mid.Click += deleteFunc;
            cm.Items.Add(mid);
            return cm;
        }
        #endregion

        #region Games Tab
        private void UpdateGamesUI()
        {
            GamesListbox.ClearItems();
            foreach (RatableGame rg in rm.RatableObjects)
            {
                ListBoxItemGameSmall item = new ListBoxItemGameSmall(rg);
                GamesListbox.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(null, null);
            }
        }
        #endregion

        #region Platforms Tab
        #endregion

        #region Settings Tab
        #region General Settings
        private void UpdateSettingsUI()
        {
            SettingsTextboxMin.Text = rm.Settings.MinScore.ToString();
            SettingsTextboxMax.Text = rm.Settings.MaxScore.ToString();
        }

        private void ResetSettingsLabels()
        {
            SettingsLabelError.Visibility = Visibility.Collapsed;
            SettingsLabelSuccess.Visibility = Visibility.Collapsed;
        }

        private void SettingsGridButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ResetSettingsLabels();
            string minScoreInput = SettingsTextboxMin.Text;
            string maxScoreInput = SettingsTextboxMax.Text;
            if (!(double.TryParse(minScoreInput, out double minScore) && double.TryParse(maxScoreInput, out double maxScore)))
            {
                SettingsLabelError.Visibility = Visibility.Visible;
                return;
            }

            if (minScore != rm.Settings.MinScore || maxScore != rm.Settings.MaxScore)
            {
                MessageBoxResult mbr = MessageBox.Show("Changing the score ranges will scale all your existing scores to fit within the new range. Would you like to do this?", "Change Score Range Confirmation", MessageBoxButton.YesNo);
                if (mbr != MessageBoxResult.Yes) return;
            }

            rm.Settings.MinScore = minScore;
            rm.Settings.MaxScore = maxScore;
            UpdateSettingsUI();
            SettingsLabelSuccess.Visibility = Visibility.Visible;
        }
        #endregion

        #region Rating Categories
        private void UpdateRatingCategoryUI()
        {
            SettingsListboxRatingCategories.ClearItems();
            foreach (RatingCategoryWeighted rc in rm.RatingCategories)
            {
                ListBoxItemRatingCategory item = new ListBoxItemRatingCategory(rc);
                item.MouseDoubleClick += RatingCategoryEdit;
                SettingsListboxRatingCategories.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(RatingCategoryEdit, RatingCategoryDelete);
            }
        }

        private void SettingsButtonNewRatingCategory_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowRatingCategory(SubWindowMode.MODE_ADD);
        }

        private void RatingCategoryEdit(object sender, RoutedEventArgs e)
        {
            ListBoxItemRatingCategory lbi;
            if (sender is MenuItem)
            {
                lbi = GetControlFromMenuItem<ListBoxItemRatingCategory>((MenuItem)sender);
            }
            else
            {
                lbi = (ListBoxItemRatingCategory)sender;
            }
            OpenSubWindowRatingCategory(SubWindowMode.MODE_EDIT, lbi.RatingCategory);
        }

        private void RatingCategoryDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemRatingCategory lbi = GetControlFromMenuItem<ListBoxItemRatingCategory>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this rating category and all data associated with it?", "Delete Rating Category Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            RatingCategory rc = lbi.RatingCategory;
            rm.DeleteRatingCategory(rc);
            UpdateRatingCategoryUI();
        }

        private void OpenSubWindowRatingCategory(SubWindowMode mode, RatingCategoryWeighted orig = null)
        {
            var window = new SubWindowRatingCategory(rm, mode, orig);
            window.Closed += RatingCategoryWindow_Closed;
            window.ShowDialog();
        }

        private void RatingCategoryWindow_Closed(object sender, EventArgs e)
        {
            UpdateRatingCategoryUI();
        }
        #endregion

        #region Completion Statuses
        private void UpdateCompletionStatusUI()
        {
            SettingsListboxCompletionStatuses.ClearItems();
            foreach (CompletionStatus cs in rm.CompletionStatuses)
            {
                ListBoxItemCompletionStatus item = new ListBoxItemCompletionStatus(cs);
                item.MouseDoubleClick += CompletionStatusEdit;
                SettingsListboxCompletionStatuses.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(CompletionStatusEdit, CompletionStatusDelete);
            }
        }

        private void SettingsButtonNewCompletionStatus_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowCompletionStatus(SubWindowMode.MODE_ADD);
        }

        private void CompletionStatusEdit(object sender, RoutedEventArgs e)
        {
            ListBoxItemCompletionStatus lbi;
            if (sender is MenuItem)
            {
                lbi = GetControlFromMenuItem<ListBoxItemCompletionStatus>((MenuItem)sender);
            }
            else
            {
                lbi = (ListBoxItemCompletionStatus)sender;
            }
            OpenSubWindowCompletionStatus(SubWindowMode.MODE_EDIT, lbi.CompletionStatus);
        }

        private void CompletionStatusDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemCompletionStatus lbi = GetControlFromMenuItem<ListBoxItemCompletionStatus>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this completion status and all data associated with it?", "Delete Completion Status Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            CompletionStatus cs = lbi.CompletionStatus;
            rm.DeleteCompletionStatus(cs);
            UpdateCompletionStatusUI();
        }

        private void OpenSubWindowCompletionStatus(SubWindowMode mode, CompletionStatus orig = null)
        {
            var window = new SubWindowCompletionStatus(rm, mode, orig);
            window.Closed += CompletionStatusWindow_Closed;
            window.ShowDialog();
        }

        private void CompletionStatusWindow_Closed(object sender, EventArgs e)
        {
            UpdateCompletionStatusUI();
        }
        #endregion

        #region Score Ranges
        private void UpdateScoreRangeUI()
        {
            SettingsListboxScoreRanges.ClearItems();
            foreach (ScoreRange sr in rm.ScoreRanges)
            {
                ListBoxItemScoreRange item = new ListBoxItemScoreRange(sr);
                item.MouseDoubleClick += ScoreRangeEdit;
                SettingsListboxScoreRanges.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(ScoreRangeEdit, ScoreRangeDelete);
            }
        }

        private void SettingsButtonNewScoreRange_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowScoreRange(SubWindowMode.MODE_ADD);
        }

        private void ScoreRangeEdit(object sender, RoutedEventArgs e)
        {
            ListBoxItemScoreRange lbi;
            if (sender is MenuItem)
            {
                lbi = GetControlFromMenuItem<ListBoxItemScoreRange>((MenuItem)sender);
            }
            else
            {
                lbi = (ListBoxItemScoreRange)sender;
            }
            OpenSubWindowScoreRange(SubWindowMode.MODE_EDIT, lbi.ScoreRange);
        }

        private void ScoreRangeDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemScoreRange lbi = GetControlFromMenuItem<ListBoxItemScoreRange>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this score range?", "Delete Score Range Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            ScoreRange sr = lbi.ScoreRange;
            rm.DeleteScoreRange(sr);
            UpdateScoreRangeUI();
        }

        private void OpenSubWindowScoreRange(SubWindowMode mode, ScoreRange orig = null)
        {
            var window = new SubWindowScoreRange(rm, mode, orig);
            window.Closed += ScoreRangeWindow_Closed;
            window.ShowDialog();
        }

        private void ScoreRangeWindow_Closed(object sender, EventArgs e)
        {
            UpdateScoreRangeUI();
        }
        #endregion
        #endregion
    }
}
