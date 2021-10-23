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

        private const string SORT_PLATFORM_NAME = "PlatformsSortName";
        private const string SORT_PLATFORM_NUMGAMES = "PlatformsSortNumGames";
        private const string SORT_PLATFORM_AVERAGE = "PlatformsSortAverage";
        private const string SORT_PLATFORM_HIGHEST = "PlatformsSortHighest";
        private const string SORT_PLATFORM_LOWEST = "PlatformsSortLowest";
        private const string SORT_PLATFORM_PERCENT = "PlatformsSortPercentFinished";
        private const string SORT_PLATFORM_RELEASE = "PlatformsSortRelease";
        private const string SORT_PLATFORM_ACQUIRED = "PlatformsSortAcquired";

        private const string SORT_GAME_NAME = "GamesSortName";
        private const string SORT_GAME_STATUS = "GamesSortStatus";
        private const string SORT_GAME_PLATFORM = "GamesSortPlatform";
        private const string SORT_GAME_PLAYEDON = "GamesSortPlayedOn";
        private const string SORT_GAME_SCORE = "GamesSortScore";
        private const string SORT_GAME_HASCOMMENT = "GamesSortHasComment";

        private IEnumerable<RatableGame> gamesView;
        private IEnumerable<Platform> platformsView;

        private RatingModuleGame rm;

        public MainWindow()
        {
            PathController.PathControllerInstance = new PathControllerWindows();
            GlobalSettings.Autosave = true;
            rm = new RatingModuleGame();
            rm.Init();
            InitializeComponent();
            PlatformsButtonSortMode.Tag = SortMode.ASCENDING;
            GamesButtonSortMode.Tag = SortMode.ASCENDING;
            gamesView = rm.ListedObjects;
            platformsView = rm.Platforms;
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
                    UpdatePlatformsUI();
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

        private void ToggleSortModeButton(Button button)
        {
            SortMode mode = (SortMode)button.Tag;
            Image image;
            switch (mode)
            {
                case SortMode.ASCENDING:
                    image = new Image();
                    image.Source = (ImageSource)Resources["ButtonDown"];
                    button.Content = image;
                    button.ToolTip = "Descending";
                    button.Tag = SortMode.DESCENDING;
                    break;
                case SortMode.DESCENDING:
                    image = new Image();
                    image.Source = (ImageSource)Resources["ButtonUp"];
                    button.Content = image;
                    button.ToolTip = "Ascending";
                    button.Tag = SortMode.ASCENDING;
                    break;
                default:
                    throw new Exception("Unhandled mode");
            }
        }

        private SortMode GetSortModeFromButton(Button button)
        {
            return (SortMode)button.Tag;
        }
        #endregion

        #region Games Tab
        private void UpdateGamesUI()
        {
            GamesListbox.ClearItems();
            foreach (RatableGame rg in gamesView)
            {
                ListBoxItemGameSmall item = new ListBoxItemGameSmall(rm, rg);
                item.MouseDoubleClick += GameEdit;
                GamesListbox.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(GameEdit, GameDelete);
            }
            BuildCategoriesHeader(rm.RatingCategories);

            var vis = rm.ListedObjects.Count() >= rm.LimitListedObjects ? Visibility.Hidden : Visibility.Visible;
            GamesButtonNew.Visibility = vis;
        }

        private void BuildCategoriesHeader(IEnumerable<RatingCategory> cats)
        {
            GridCategories.Children.Clear();
            GridCategories.ColumnDefinitions.Clear();
            int i = 0;
            foreach (RatingCategory cat in cats)
            {
                GridCategories.ColumnDefinitions.Add(new ColumnDefinition());
                Label label = new Label();
                label.Content = cat.Name;
                TextBlock tb = new TextBlock();
                tb.Text = cat.Comment;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.MaxWidth = 400;
                label.ToolTip = tb;
                Grid.SetColumn(label, i);
                GridCategories.Children.Add(label);
                i++;
            }
        }

        private void GamesButtonNew_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowGame(SubWindowMode.MODE_ADD);
        }

        private void GameEdit(object sender, RoutedEventArgs e)
        {
            ListBoxItemGameSmall lbi;
            if (sender is MenuItem)
            {
                lbi = GetControlFromMenuItem<ListBoxItemGameSmall>((MenuItem)sender);
            }
            else
            {
                lbi = (ListBoxItemGameSmall)sender;
            }
            OpenSubWindowGame(SubWindowMode.MODE_EDIT, lbi.Game);
        }

        private void GameDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemGameSmall lbi = GetControlFromMenuItem<ListBoxItemGameSmall>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this game?", "Delete Game Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            RatableGame game = lbi.Game;
            rm.DeleteListedObject(game);
            UpdateGamesUI();
        }

        private void OpenSubWindowGame(SubWindowMode mode, RatableGame orig = null)
        {
            var window = new SubWindowGame(rm, mode, orig);
            window.Closed += GameWindow_Closed;
            window.ShowDialog();
        }

        private void GameWindow_Closed(object sender, EventArgs e)
        {
            UpdateGamesUI();
        }

        private void GamesButtonSort_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = GamesButtonSort.ContextMenu;
            contextMenu.PlacementTarget = GamesButtonSort;
            contextMenu.IsOpen = true;
        }

        private void GamesSort_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            foreach (MenuItem sortItem in GamesButtonSort.ContextMenu.Items)
            {
                if (!sortItem.Equals(item))
                {
                    sortItem.IsChecked = false;
                }
            }
            GamesSort(item.Name);
        }

        private void GamesSort_Unchecked(object sender, RoutedEventArgs e)
        {
            gamesView = rm.ListedObjects;
            UpdateGamesUI();
        }

        private void GamesSort(string sortField)
        {
            SortMode mode = GetSortModeFromButton(GamesButtonSortMode);
            switch (sortField)
            {
                case SORT_GAME_NAME:
                    gamesView = rm.SortListedObjects(game => game.Name, mode);
                    break;
                case SORT_GAME_STATUS:
                    gamesView = rm.SortListedObjects(game => game.RefStatus.HasReference() ? rm.FindStatus(game.RefStatus).Name : "", mode);
                    break;
                case SORT_GAME_PLATFORM:
                    gamesView = rm.SortListedObjects(game => game.RefPlatform.HasReference() ? rm.FindPlatform(game.RefPlatform).Name : "", mode);
                    break;
                case SORT_GAME_PLAYEDON:
                    gamesView = rm.SortListedObjects(game => game.RefPlatformPlayedOn.HasReference() ? rm.FindPlatform(game.RefPlatformPlayedOn).Name : "", mode);
                    break;
                case SORT_GAME_SCORE:
                    gamesView = rm.SortListedObjects(game => rm.GetScoreOfObject(game), mode);
                    break;
                case SORT_GAME_HASCOMMENT:
                    gamesView = rm.SortListedObjects(game => game.Comment.Length > 0, mode);
                    break;
                default:
                    throw new Exception("Unhandled sort expression");
            }
            UpdateGamesUI();
        }

        private void GamesSortRefresh()
        {
            foreach (MenuItem sortItem in GamesButtonSort.ContextMenu.Items)
            {
                if (sortItem.IsChecked)
                {
                    GamesSort(sortItem.Name);
                }
            }
        }

        private void GamesButtonSortMode_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ToggleSortModeButton(button);
            GamesSortRefresh();
        }
        #endregion

        #region Platforms Tab
        private void UpdatePlatformsUI()
        {
            PlatformsListbox.ClearItems();
            foreach (Platform platform in platformsView)
            {
                ListBoxItemPlatform item = new ListBoxItemPlatform(rm, platform);
                item.MouseDoubleClick += PlatformEdit;
                PlatformsListbox.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(PlatformEdit, PlatformDelete);
            }

            var vis = rm.Platforms.Count() >= rm.LimitPlatforms ? Visibility.Hidden : Visibility.Visible;
            PlatformsButtonNew.Visibility = vis;
        }

        private void PlatformsButtonNew_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowPlatform(SubWindowMode.MODE_ADD);
        }

        private void PlatformEdit(object sender, RoutedEventArgs e)
        {
            ListBoxItemPlatform lbi;
            if (sender is MenuItem)
            {
                lbi = GetControlFromMenuItem<ListBoxItemPlatform>((MenuItem)sender);
            }
            else
            {
                lbi = (ListBoxItemPlatform)sender;
            }
            OpenSubWindowPlatform(SubWindowMode.MODE_EDIT, lbi.Platform);
        }

        private void PlatformDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemPlatform lbi = GetControlFromMenuItem<ListBoxItemPlatform>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this platform and all data associated with it?", "Delete Platform Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            Platform platform = lbi.Platform;
            rm.DeletePlatform(platform);
            UpdatePlatformsUI();
        }

        private void OpenSubWindowPlatform(SubWindowMode mode, Platform orig = null)
        {
            var window = new SubWindowPlatform(rm, mode, orig);
            window.Closed += PlatformWindow_Closed;
            window.ShowDialog();
        }

        private void PlatformWindow_Closed(object sender, EventArgs e)
        {
            UpdatePlatformsUI();
        }

        private void PlatformsButtonSort_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = PlatformsButtonSort.ContextMenu;
            contextMenu.PlacementTarget = PlatformsButtonSort;
            contextMenu.IsOpen = true;
        }

        private void PlatformsSort_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            foreach (MenuItem sortItem in PlatformsButtonSort.ContextMenu.Items)
            {
                if (!sortItem.Equals(item))
                {
                    sortItem.IsChecked = false;
                }
            }
            PlatformSort(item.Name);
        }

        private void PlatformsSort_Unchecked(object sender, RoutedEventArgs e)
        {
            platformsView = rm.Platforms;
            UpdatePlatformsUI();
        }

        private void PlatformSort(string sortField)
        {
            SortMode mode = GetSortModeFromButton(PlatformsButtonSortMode);
            switch (sortField)
            {
                case SORT_PLATFORM_NAME:
                    platformsView = rm.SortPlatforms(platform => platform.Name, mode);
                    break;
                case SORT_PLATFORM_NUMGAMES:
                    platformsView = rm.SortPlatforms(platform => rm.GetNumGamesByPlatform(platform), mode);
                    break;
                case SORT_PLATFORM_AVERAGE:
                    platformsView = rm.SortPlatforms(platform => rm.GetAverageScoreOfGamesByPlatform(platform), mode);
                    break;
                case SORT_PLATFORM_HIGHEST:
                    platformsView = rm.SortPlatforms(platform => rm.GetHighestScoreFromGamesByPlatform(platform), mode);
                    break;
                case SORT_PLATFORM_LOWEST:
                    platformsView = rm.SortPlatforms(platform => rm.GetLowestScoreFromGamesByPlatform(platform), mode);
                    break;
                case SORT_PLATFORM_PERCENT:
                    platformsView = rm.SortPlatforms(platform => rm.GetPercentageGamesFinishedByPlatform(platform), mode);
                    break;
                case SORT_PLATFORM_RELEASE:
                    platformsView = rm.SortPlatforms(platform => platform.ReleaseYear, mode);
                    break;
                case SORT_PLATFORM_ACQUIRED:
                    platformsView = rm.SortPlatforms(platform => platform.AcquiredYear, mode);
                    break;
                default:
                    throw new Exception("Unhandled sort expression");
            }
            UpdatePlatformsUI();
        }

        private void PlatformSortRefresh()
        {
            foreach (MenuItem sortItem in PlatformsButtonSort.ContextMenu.Items)
            {
                if (sortItem.IsChecked)
                {
                    PlatformSort(sortItem.Name);
                }
            }
        }

        private void PlatformsButtonSortMode_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ToggleSortModeButton(button);
            PlatformSortRefresh();
        }
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

            rm.SetMinScoreAndUpdate(minScore);
            rm.SetMaxScoreAndUpdate(maxScore);
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

            var vis = rm.RatingCategories.Count() >= rm.LimitRatingCategories ? Visibility.Hidden : Visibility.Visible;
            SettingsButtonNewRatingCategory.Visibility = vis;
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

            RatingCategoryWeighted rc = lbi.RatingCategory;
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
            foreach (CompletionStatus cs in rm.Statuses)
            {
                ListBoxItemCompletionStatus item = new ListBoxItemCompletionStatus(cs);
                item.MouseDoubleClick += CompletionStatusEdit;
                SettingsListboxCompletionStatuses.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(CompletionStatusEdit, CompletionStatusDelete);
            }

            var vis = rm.Statuses.Count() >= rm.LimitStatuses ? Visibility.Hidden : Visibility.Visible;
            SettingsButtonNewCompletionStatus.Visibility = vis;
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
            rm.DeleteStatus(cs);
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
            foreach (ScoreRange sr in rm.Ranges)
            {
                ListBoxItemScoreRange item = new ListBoxItemScoreRange(rm, sr);
                item.MouseDoubleClick += ScoreRangeEdit;
                SettingsListboxScoreRanges.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(ScoreRangeEdit, ScoreRangeDelete);
            }

            var vis = rm.Ranges.Count() >= rm.LimitRanges ? Visibility.Hidden : Visibility.Visible;
            SettingsButtonNewScoreRange.Visibility = vis;
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
            rm.DeleteRange(sr);
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
