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
using RatableTracker.Framework.IO;
using RatableTracker.Framework;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Interfaces;
using Microsoft.Win32;

namespace GameTrackerWPF
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

        private RatingModuleGame rm;

        private class SavedState
        {
            public Func<RatableGame, object> gamesSortFunc = null;
            public SortMode gamesSortMode = SortMode.ASCENDING;
            public Func<Platform, object> platformsSortFunc = null;
            public SortMode platformsSortMode = SortMode.ASCENDING;
            public bool gamesSortCatCreated = false;
            public GameDisplayMode displayMode = GameDisplayMode.DISPLAY_SMALL;
        }

        private SavedState savedState = new SavedState();

        public MainWindow()
        {
            PathController.PathControllerInstance = new PathControllerWindows();
            GlobalSettings.Autosave = false;
            IContentLoadSave<string, string> cls;
            if (ContentLoadSaveAWSS3.KeyFileExists())
                cls = new ContentLoadSaveAWSS3();
            else
                cls = new ContentLoadSaveLocal();
            LoadSaveEngineGameJson<ValueContainer> engine = new LoadSaveEngineGameJson<ValueContainer>
            {
                ContentLoadSaveInstance = cls
            };
            rm = new RatingModuleGame(engine);
            InitializeComponent();
            PlatformsButtonSortMode.Tag = savedState.platformsSortMode;
            GamesButtonSortMode.Tag = savedState.gamesSortMode;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllData();
        }

        private async Task LoadAllData()
        {
            EnableNewButtons(false);
            mainWindow.Title = "Game Tracker (Loading...)";
            await Task.Run(() => rm.InitAsync());
            UpdateCurrentTab();
            EnableNewButtons(true);
            mainWindow.Title = "Game Tracker";
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            //rm.SaveListedObjects();
            //rm.SavePlatforms();
            //rm.SaveRanges();
            //rm.SaveRatingCategories();
            //rm.SaveSettings();
            //rm.SaveStatuses();
        }

        #region General Functionality and Utilities
        private void TabsBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != TabsBase)
            {
                return;
            }
            UpdateCurrentTab();
            e.Handled = true;
        }

        private void UpdateCurrentTab()
        {
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

        private void EnableNewButtons(bool state)
        {
            GamesButtonNew.IsEnabled = state;
            PlatformsButtonNew.IsEnabled = state;
            SettingsButtonSave.IsEnabled = state;
            SettingsButtonNewCompletionStatus.IsEnabled = state;
            SettingsButtonNewRatingCategory.IsEnabled = state;
            SettingsButtonNewScoreRange.IsEnabled = state;
        }
        #endregion

        #region Games Tab
        private IEnumerable<RatableGame> GetGamesView()
        {
            IEnumerable<RatableGame> temp;
            if (CheckboxShowCompilations.IsChecked.Value)
                temp = rm.ListedObjects.Where(rg => !rg.IsPartOfCompilation).Concat(rm.GameCompilations);
            else
                temp = rm.ListedObjects;
            if (savedState.gamesSortFunc != null) temp = rm.SortListedObjects(savedState.gamesSortFunc, savedState.gamesSortMode);
            return temp;
        }

        private void UpdateGamesUI()
        {
            GamesListbox.ClearItems();
            GamesListBoxWrap.Items.Clear();
            switch (savedState.displayMode)
            {
                case GameDisplayMode.DISPLAY_SMALL:
                    GamesTop.Visibility = Visibility.Visible;
                    GamesTopExpanded.Visibility = Visibility.Collapsed;
                    GamesListbox.Visibility = Visibility.Visible;
                    GamesListBoxWrap.Visibility = Visibility.Collapsed;
                    break;
                case GameDisplayMode.DISPLAY_EXPANDED:
                    GamesTop.Visibility = Visibility.Collapsed;
                    GamesTopExpanded.Visibility = Visibility.Visible;
                    GamesListbox.Visibility = Visibility.Visible;
                    GamesListBoxWrap.Visibility = Visibility.Collapsed;
                    break;
                case GameDisplayMode.DISPLAY_BOXES:
                    GamesTop.Visibility = Visibility.Collapsed;
                    GamesTopExpanded.Visibility = Visibility.Collapsed;
                    GamesListbox.Visibility = Visibility.Collapsed;
                    GamesListBoxWrap.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new NotImplementedException();
            }
            foreach (RatableGame rg in GetGamesView())
            {
                UserControl item;
                switch (savedState.displayMode)
                {
                    case GameDisplayMode.DISPLAY_SMALL:
                        item = new ListBoxItemGameSmall(rm, rg);
                        break;
                    case GameDisplayMode.DISPLAY_EXPANDED:
                        item = new ListBoxItemGameExpanded(rm, rg);
                        break;
                    case GameDisplayMode.DISPLAY_BOXES:
                        item = new ListBoxItemGameBox(rm, rg);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                item.MouseDoubleClick += GameEdit;
                if (savedState.displayMode == GameDisplayMode.DISPLAY_BOXES)
                    GamesListBoxWrap.Items.Add(item);
                else
                    GamesListbox.AddItem(item);

                if (rg is GameCompilation)
                    item.ContextMenu = EditDeleteContextMenu(GameEdit, null);
                else
                    item.ContextMenu = EditDeleteContextMenu(GameEdit, GameDelete);
            }
            BuildCategoriesHeader(rm.RatingCategories);
            BuildGamesSortOptions(rm.RatingCategories);

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
                var label = new TextBlock();
                label.Text = cat.Name;
                label.TextWrapping = TextWrapping.Wrap;
                label.MaxHeight = 35;
                label.VerticalAlignment = VerticalAlignment.Center;
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

        private void BuildGamesSortOptions(IEnumerable<RatingCategory> cats)
        {
            if (savedState.gamesSortCatCreated) return;
            GamesButtonSort.ContextMenu.Items.Clear();
            MenuItem item;
            foreach (Tuple<string, string> sortOption in new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(SORT_GAME_NAME, "Name"),
                new Tuple<string, string>(SORT_GAME_STATUS, "Completion Status"),
                new Tuple<string, string>(SORT_GAME_PLATFORM, "Platform"),
                new Tuple<string, string>(SORT_GAME_PLAYEDON, "Platform Played On"),
                new Tuple<string, string>(SORT_GAME_SCORE, "Final Score"),
                new Tuple<string, string>(SORT_GAME_HASCOMMENT, "Has Comment"),
            })
            {
                item = new MenuItem
                {
                    Name = sortOption.Item1,
                    Header = sortOption.Item2,
                    IsCheckable = true
                };
                item.Checked += GamesSort_Checked;
                item.Unchecked += GamesSort_Unchecked;
                GamesButtonSort.ContextMenu.Items.Add(item);
            }
            foreach (RatingCategory cat in cats)
            {
                item = new MenuItem
                {
                    Header = cat.Name,
                    Name = "z" + cat.ReferenceKey.ToString(),
                    IsCheckable = true
                };
                item.Checked += GamesSort_Checked;
                item.Unchecked += GamesSort_Unchecked;
                GamesButtonSort.ContextMenu.Items.Add(item);
            }
            savedState.gamesSortCatCreated = true;
        }

        private void GamesButtonNew_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowGame(SubWindowMode.MODE_ADD);
        }

        private void GameEdit(object sender, RoutedEventArgs e)
        {
            IListBoxItemGame lbi = null;
            if (sender is MenuItem)
            {
                try
                {
                    lbi = GetControlFromMenuItem<ListBoxItemGameSmall>((MenuItem)sender);
                }
                catch (Exception) { }
                if (lbi == null) throw new Exception("Could not find list box item");
            }
            else
            {
                lbi = (IListBoxItemGame)sender;
            }
            OpenSubWindowGame(SubWindowMode.MODE_EDIT, lbi.Game);
        }

        private async void GameDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemGameSmall lbi = GetControlFromMenuItem<ListBoxItemGameSmall>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this game?", "Delete Game Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            RatableGame game = lbi.Game;
            rm.DeleteListedObject(game);
            UpdateGamesUI();
            await SaveListedObjectsAsync();
        }

        private void OpenSubWindowGame(SubWindowMode mode, RatableGame orig = null)
        {
            Window window;
            if (orig != null && orig is GameCompilation)
                window = new SubWindowCompilation(rm, mode, orig as GameCompilation);
            else
                window = new SubWindowGame(rm, mode, orig);
            window.Closed += GameWindow_Closed;
            window.ShowDialog();
        }

        private async void GameWindow_Closed(object sender, EventArgs e)
        {
            UpdateGamesUI();
            await SaveListedObjectsAsync();
            await SaveGameCompilationsAsync();
        }

        private async Task SaveListedObjectsAsync()
        {
            await rm.SaveListedObjectsAsync();
        }

        private async Task SaveGameCompilationsAsync()
        {
            await rm.SaveGameCompilationsAsync();
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
            savedState.gamesSortFunc = null;
            UpdateGamesUI();
        }

        private void GamesSort(string sortField)
        {
            savedState.gamesSortMode = GetSortModeFromButton(GamesButtonSortMode);
            switch (sortField)
            {
                case SORT_GAME_NAME:
                    savedState.gamesSortFunc = game => game.Name.ToLower().StartsWith("the ") ? game.Name.Substring(4) : game.Name;
                    break;
                case SORT_GAME_STATUS:
                    savedState.gamesSortFunc = game => game.RefStatus.HasReference() ? rm.FindStatus(game.RefStatus).Name : "";
                    break;
                case SORT_GAME_PLATFORM:
                    savedState.gamesSortFunc = game => game.RefPlatform.HasReference() ? rm.FindPlatform(game.RefPlatform).Name : "";
                    break;
                case SORT_GAME_PLAYEDON:
                    savedState.gamesSortFunc = game => game.RefPlatformPlayedOn.HasReference() ? rm.FindPlatform(game.RefPlatformPlayedOn).Name : "";
                    break;
                case SORT_GAME_SCORE:
                    savedState.gamesSortFunc = game => rm.GetScoreOfObject(game);
                    break;
                case SORT_GAME_HASCOMMENT:
                    savedState.gamesSortFunc = game => game.Comment.Length > 0;
                    break;
                default:
                    if (Guid.TryParse(sortField.Substring(1), out Guid key))
                    {
                        ObjectReference refCat = new ObjectReference(key);
                        savedState.gamesSortFunc = game => rm.GetScoreOfCategory(game, rm.FindRatingCategory(refCat));
                    }
                    else
                    {
                        throw new Exception("Unhandled sort expression");
                    }
                    break;
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

        private void GamesButtonList_Click(object sender, RoutedEventArgs e)
        {
            ChangeGameDisplayMode(GameDisplayMode.DISPLAY_SMALL);
        }

        private void GamesButtonExpanded_Click(object sender, RoutedEventArgs e)
        {
            ChangeGameDisplayMode(GameDisplayMode.DISPLAY_EXPANDED);
        }

        private void GamesButtonBoxes_Click(object sender, RoutedEventArgs e)
        {
            ChangeGameDisplayMode(GameDisplayMode.DISPLAY_BOXES);
        }

        private void ChangeGameDisplayMode(GameDisplayMode mode)
        {
            if (savedState.displayMode != mode)
            {
                savedState.displayMode = mode;
                UpdateGamesUI();
            }
        }

        private void CheckboxShowCompilations_Checked(object sender, RoutedEventArgs e)
        {
            UpdateGamesUI();
        }
        #endregion

        #region Platforms Tab
        private IEnumerable<Platform> GetPlatformsView()
        {
            IEnumerable<Platform> temp = rm.Platforms;
            if (savedState.platformsSortFunc != null) temp = rm.SortPlatforms(savedState.platformsSortFunc, savedState.platformsSortMode);
            return temp;
        }

        private void UpdatePlatformsUI()
        {
            PlatformsListbox.ClearItems();
            foreach (Platform platform in GetPlatformsView())
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

        private async void PlatformDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemPlatform lbi = GetControlFromMenuItem<ListBoxItemPlatform>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this platform and all data associated with it?", "Delete Platform Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            Platform platform = lbi.Platform;
            rm.DeletePlatform(platform);
            UpdatePlatformsUI();
            await SavePlatformsAsync();
        }

        private void OpenSubWindowPlatform(SubWindowMode mode, Platform orig = null)
        {
            var window = new SubWindowPlatform(rm, mode, orig);
            window.Closed += PlatformWindow_Closed;
            window.ShowDialog();
        }

        private async void PlatformWindow_Closed(object sender, EventArgs e)
        {
            UpdatePlatformsUI();
            await SavePlatformsAsync();
        }

        private async Task SavePlatformsAsync()
        {
            await rm.SavePlatformsAsync();
            await rm.SaveListedObjectsAsync();
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
            savedState.platformsSortFunc = null;
            UpdatePlatformsUI();
        }

        private void PlatformSort(string sortField)
        {
            SortMode mode = GetSortModeFromButton(PlatformsButtonSortMode);
            switch (sortField)
            {
                case SORT_PLATFORM_NAME:
                    savedState.platformsSortFunc = platform => platform.Name;
                    break;
                case SORT_PLATFORM_NUMGAMES:
                    savedState.platformsSortFunc = platform => rm.GetNumGamesByPlatform(platform);
                    break;
                case SORT_PLATFORM_AVERAGE:
                    savedState.platformsSortFunc = platform => rm.GetAverageScoreOfGamesByPlatform(platform);
                    break;
                case SORT_PLATFORM_HIGHEST:
                    savedState.platformsSortFunc = platform => rm.GetHighestScoreFromGamesByPlatform(platform);
                    break;
                case SORT_PLATFORM_LOWEST:
                    savedState.platformsSortFunc = platform => rm.GetLowestScoreFromGamesByPlatform(platform);
                    break;
                case SORT_PLATFORM_PERCENT:
                    savedState.platformsSortFunc = platform => rm.GetPercentageGamesFinishedByPlatform(platform);
                    break;
                case SORT_PLATFORM_RELEASE:
                    savedState.platformsSortFunc = platform => platform.ReleaseYear;
                    break;
                case SORT_PLATFORM_ACQUIRED:
                    savedState.platformsSortFunc = platform => platform.AcquiredYear;
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
            SettingsAWSButton.Content = ContentLoadSaveAWSS3.KeyFileExists() ? "Switch back to local save files" : "Switch to remote save files with AWS";
        }

        private void ResetSettingsLabels()
        {
            SettingsLabelError.Visibility = Visibility.Collapsed;
            SettingsLabelSuccess.Visibility = Visibility.Collapsed;
        }

        private async void SettingsGridButtonSave_Click(object sender, RoutedEventArgs e)
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
                rm.SetScoresAndUpdate(minScore, maxScore);
            }

            UpdateSettingsUI();
            SettingsLabelSuccess.Visibility = Visibility.Visible;
            await SaveSettingsAsync();
        }

        private async void SettingsAWSButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.IsEnabled = false;
            if (ContentLoadSaveAWSS3.KeyFileExists())
            {
                // Remove key file
                var result = MessageBox.Show("Transfer AWS save files to local? This will overwrite anything currently on this device.", "Overwrite local?", MessageBoxButton.YesNo);
                IContentLoadSave<string, string> cls = new ContentLoadSaveLocal();
                if (result == MessageBoxResult.Yes)
                {
                    IContentLoadSave<string, string> from = new ContentLoadSaveAWSS3();
                    await rm.TransferSaveFilesAsync(from, cls);
                }
                ContentLoadSaveAWSS3.DeleteKeyFile();
                LoadSaveEngineGameJson<ValueContainer> engine = new LoadSaveEngineGameJson<ValueContainer>
                {
                    ContentLoadSaveInstance = cls
                };
                rm = new RatingModuleGame(engine);
                await LoadAllData();
            }
            else
            {
                // Add a key file
                OpenFileDialog fileDialog = new OpenFileDialog();
                if (fileDialog.ShowDialog() == true)
                {
                    var result = MessageBox.Show("Transfer local save files to AWS? This will overwrite anything currently on your AWS account.", "Overwrite AWS?", MessageBoxButton.YesNo);
                    try
                    {
                        ContentLoadSaveAWSS3.CreateKeyFile(fileDialog.FileName);
                        IContentLoadSave<string, string> cls = new ContentLoadSaveAWSS3();
                        if (result == MessageBoxResult.Yes)
                        {
                            IContentLoadSave<string, string> from = new ContentLoadSaveLocal();
                            await rm.TransferSaveFilesAsync(from, cls);
                        }
                        LoadSaveEngineGameJson<ValueContainer> engine = new LoadSaveEngineGameJson<ValueContainer>
                        {
                            ContentLoadSaveInstance = cls
                        };
                        rm = new RatingModuleGame(engine);
                        await LoadAllData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Something went wrong.\n" + ex.Message, "Error");
                    }
                }
            }
            mainWindow.IsEnabled = true;
        }

        private async Task SaveSettingsAsync()
        {
            await rm.SaveSettingsAsync();
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
            savedState.gamesSortCatCreated = false;
            savedState.gamesSortFunc = null;
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

        private async void RatingCategoryDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemRatingCategory lbi = GetControlFromMenuItem<ListBoxItemRatingCategory>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this rating category and all data associated with it?", "Delete Rating Category Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            RatingCategoryWeighted rc = lbi.RatingCategory;
            rm.DeleteRatingCategory(rc);
            UpdateRatingCategoryUI();
            await SaveRatingCategoriesAsync();
        }

        private void OpenSubWindowRatingCategory(SubWindowMode mode, RatingCategoryWeighted orig = null)
        {
            var window = new SubWindowRatingCategory(rm, mode, orig);
            window.Closed += RatingCategoryWindow_Closed;
            window.ShowDialog();
        }

        private async void RatingCategoryWindow_Closed(object sender, EventArgs e)
        {
            UpdateRatingCategoryUI();
            await SaveRatingCategoriesAsync();
        }

        private async Task SaveRatingCategoriesAsync()
        {
            await rm.SaveRatingCategoriesAsync();
            await rm.SaveListedObjectsAsync();
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

        private async void CompletionStatusDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemCompletionStatus lbi = GetControlFromMenuItem<ListBoxItemCompletionStatus>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this completion status and all data associated with it?", "Delete Completion Status Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            CompletionStatus cs = lbi.CompletionStatus;
            rm.DeleteStatus(cs);
            UpdateCompletionStatusUI();
            await SaveCompletionStatusesAsync();
        }

        private void OpenSubWindowCompletionStatus(SubWindowMode mode, CompletionStatus orig = null)
        {
            var window = new SubWindowCompletionStatus(rm, mode, orig);
            window.Closed += CompletionStatusWindow_Closed;
            window.ShowDialog();
        }

        private async void CompletionStatusWindow_Closed(object sender, EventArgs e)
        {
            UpdateCompletionStatusUI();
            await SaveCompletionStatusesAsync();
        }

        private async Task SaveCompletionStatusesAsync()
        {
            await rm.SaveStatusesAsync();
            await rm.SaveListedObjectsAsync();
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

        private async void ScoreRangeDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemScoreRange lbi = GetControlFromMenuItem<ListBoxItemScoreRange>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this score range?", "Delete Score Range Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            ScoreRange sr = lbi.ScoreRange;
            rm.DeleteRange(sr);
            UpdateScoreRangeUI();
            await SaveScoreRangesAsync();
        }

        private void OpenSubWindowScoreRange(SubWindowMode mode, ScoreRange orig = null)
        {
            var window = new SubWindowScoreRange(rm, mode, orig);
            window.Closed += ScoreRangeWindow_Closed;
            window.ShowDialog();
        }

        private async void ScoreRangeWindow_Closed(object sender, EventArgs e)
        {
            UpdateScoreRangeUI();
            await SaveScoreRangesAsync();
        }

        private async Task SaveScoreRangesAsync()
        {
            await rm.SaveRangesAsync();
        }
        #endregion

        #endregion
    }
}
