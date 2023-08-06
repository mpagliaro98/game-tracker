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
using GameTracker;
using Microsoft.Win32;
using RatableTracker.Util;
using System.Windows.Media.Animation;
using RatableTracker.LoadSave;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using RatableTracker.Exceptions;
using System.Diagnostics;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.ListManipulation.Filtering;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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

        private GameModule rm;
        private SettingsGame settings;
        private ILoadSaveHandler<ILoadSaveMethodGame> loadSave;
        private GameTrackerFactory factory;
        private IPathController pathController;

        private Task loadData = null;
        private Stopwatch swLoad = new Stopwatch();

        private static bool LOAD_ASYNC = true;

        private SavedState savedState;

        public MainWindow()
        {

            pathController = new PathControllerWindows();
            IFileHandler fileHandlerSaves;
            if (FileHandlerAWSS3.KeyFileExists(pathController))
                fileHandlerSaves = new FileHandlerAWSS3(pathController);
            else
                fileHandlerSaves = new FileHandlerLocalAppData(pathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);
            factory = new GameTrackerFactory();
            loadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(fileHandlerSaves, factory, App.Logger));
            rm = new GameModule(loadSave, App.Logger);
            try
            {
                settings = (SettingsGame)Settings.Load(loadSave);
            }
            catch (NoDataFoundException)
            {
                // first load
                settings = new SettingsGame();
            }

            savedState = SavedState.LoadSavedState(pathController, rm, settings, App.Logger);
            savedState.Loaded = false;

            swLoad.Start();
            if (LOAD_ASYNC)
                loadData = rm.LoadDataAsync(settings);

            InitializeComponent();

            // set sort mode buttons to their opposite, then toggle them to their real value to set the correct graphic
            PlatformsButtonSortMode.Tag = savedState.SortPlatforms.SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            ToggleSortModeButton(PlatformsButtonSortMode);
            GamesButtonSortMode.Tag = savedState.SortGames.SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            ToggleSortModeButton(GamesButtonSortMode);

            CheckboxShowCompilations.IsChecked = savedState.ShowCompilations;
            SetButtonInUse(GamesButtonSearch, savedState.FilterGames.Filters.Count > 0);
            SetButtonInUse(PlatformsButtonSearch, savedState.FilterPlatforms.Filters.Count > 0);
            SetButtonInUse(GamesButtonSort, savedState.SortGames.SortOption != null);
            SetButtonInUse(PlatformsButtonSort, savedState.SortPlatforms.SortOption != null);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllData();
        }

        private async Task LoadAllData()
        {
            EnableNewButtons(false);
            mainWindow.Title = "Game Tracker (Loading...)";

            if (!swLoad.IsRunning) swLoad.Restart();
            if (LOAD_ASYNC)
            {
                if (loadData == null)
                    await rm.LoadDataAsync(settings);
                else
                    await loadData;
            }
            else
                await Task.Run(() => rm.LoadData(settings));
            swLoad.Stop();
            App.Logger.Log("Initial data load finished in " + swLoad.ElapsedMilliseconds.ToString() + "ms");
            loadData = null;

            savedState.Loaded = true;
            UpdateCurrentTab();
            EnableNewButtons(true);
            mainWindow.Title = "Game Tracker";
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
            if (!savedState.Loaded) return;
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
                case SortMode.Ascending:
                    image = new Image();
                    image.Source = (ImageSource)Resources["ButtonDown"];
                    button.Content = image;
                    button.ToolTip = "Descending";
                    button.Tag = SortMode.Descending;
                    break;
                case SortMode.Descending:
                    image = new Image();
                    image.Source = (ImageSource)Resources["ButtonUp"];
                    button.Content = image;
                    button.ToolTip = "Ascending";
                    button.Tag = SortMode.Ascending;
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

        private void SetButtonInUse(Button button, bool inUse)
        {
            button.Background = new SolidColorBrush(inUse ? Colors.LightBlue : System.Windows.Media.Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD));
        }
        #endregion

        #region Games Tab
        private void UpdateGamesUI()
        {
            GamesListbox.ClearItems();
            GamesListBoxWrap.Items.Clear();
            switch (savedState.DisplayMode)
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
            int count = 0;
            foreach (GameObject rg in rm.GetModelObjectList<GameObject>(savedState.FilterGames, savedState.SortGames, settings))
            {
                if ((savedState.ShowCompilations && !rg.IsCompilation && rg.IsPartOfCompilation) || (!savedState.ShowCompilations && rg.IsCompilation))
                    continue;

                UserControl item = savedState.DisplayMode switch
                {
                    GameDisplayMode.DISPLAY_SMALL => new ListBoxItemGameSmall(rm, rg),
                    GameDisplayMode.DISPLAY_EXPANDED => new ListBoxItemGameExpanded(rm, rg),
                    GameDisplayMode.DISPLAY_BOXES => new ListBoxItemGameBox(rm, rg),
                    _ => throw new NotImplementedException(),
                };
                item.MouseDoubleClick += GameEdit;
                if (savedState.DisplayMode == GameDisplayMode.DISPLAY_BOXES)
                    GamesListBoxWrap.Items.Add(item);
                else
                    GamesListbox.AddItem(item);

                if (rg.IsCompilation)
                    item.ContextMenu = EditDeleteContextMenu(GameEdit, null);
                else
                    item.ContextMenu = EditDeleteContextMenu(GameEdit, GameDelete);
                count++;
            }
            BuildCategoriesHeader(rm.CategoryExtension.GetRatingCategoryList());
            BuildGamesSortOptions();

            var vis = rm.TotalNumModelObjects() >= rm.LimitModelObjects ? Visibility.Hidden : Visibility.Visible;
            GamesButtonNew.Visibility = vis;
            SavedState.SaveSavedState(pathController, savedState);
            LabelGamesCount1.Content = count.ToString();
            LabelGamesCount2.Content = count.ToString();
        }

        private void BuildCategoriesHeader(IList<RatingCategory> cats)
        {
            GridCategories.Children.Clear();
            GridCategories.ColumnDefinitions.Clear();
            int i = 0;
            foreach (RatingCategory cat in cats)
            {
                GridCategories.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
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

        private void BuildGamesSortOptions()
        {
            if (!savedState.Loaded || GamesButtonSort.ContextMenu.Items.Count > 0) return;
            GamesButtonSort.ContextMenu.Items.Clear();
            IList<ISortOption> sortOptions = SortEngine.GetSortOptionList<GameObject>(rm, settings, new List<Type>() { typeof(SortOptionModelRank) });
            foreach (var option in sortOptions)
            {
                var item = new MenuItem
                {
                    IsCheckable = true,
                    IsChecked = option.Equals(savedState.SortGames.SortOption),
                    DisplayMemberPath = "Name",
                    Header = option.Name
                };
                item.Items.Add(option);
                item.Checked += GamesSort_Checked;
                item.Unchecked += GamesSort_Unchecked;
                GamesButtonSort.ContextMenu.Items.Add(item);
            }
        }

        private void GamesButtonNew_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowGame(SubWindowMode.MODE_ADD, new GameObject(settings, rm));
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
            OpenSubWindowGame(SubWindowMode.MODE_EDIT, lbi.Game.IsCompilation ? new GameCompilation(lbi.Game as GameCompilation) : new GameObject(lbi.Game));
        }

        private void GameDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemGameSmall lbi = GetControlFromMenuItem<ListBoxItemGameSmall>((MenuItem)sender);

            MessageBoxResult mbr = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you would like to delete this game?", "Delete Game Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            try
            {
                lbi.Game.Delete(rm, settings);
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            UpdateGamesUI();
        }

        private void OpenSubWindowGame(SubWindowMode mode, GameObject orig)
        {
            Window window;
            if (orig.IsCompilation)
            {
                window = new SubWindowCompilation(rm, settings, mode, orig as GameCompilation);
                ((SubWindowCompilation)window).Saved += GameWindow_Closed;
            }
            else
            {
                window = new SubWindowGame(rm, settings, loadSave, mode, orig);
                ((SubWindowGame)window).Saved += GameWindow_Closed;
            }
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
            GamesSort((ISortOption)item.Items[0]);
            SetButtonInUse(GamesButtonSort, savedState.SortGames.SortOption != null);
        }

        private void GamesSort_Unchecked(object sender, RoutedEventArgs e)
        {
            savedState.SortGames.SortOption = null;
            UpdateGamesUI();
            SetButtonInUse(GamesButtonSort, savedState.SortGames.SortOption != null);
        }

        private void GamesSort(ISortOption sortOption)
        {
            savedState.SortGames.SortMode = GetSortModeFromButton(GamesButtonSortMode);
            savedState.SortGames.SortOption = (SortOptionBase)sortOption;
            UpdateGamesUI();
        }

        private void GamesSortRefresh()
        {
            foreach (MenuItem sortItem in GamesButtonSort.ContextMenu.Items)
            {
                if (sortItem.IsChecked)
                {
                    GamesSort((ISortOption)sortItem.Items[0]);
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
            if (savedState.DisplayMode != mode)
            {
                savedState.DisplayMode = mode;
                UpdateGamesUI();
            }
        }

        private void CheckboxShowCompilations_Checked(object sender, RoutedEventArgs e)
        {
            savedState.ShowCompilations = CheckboxShowCompilations.IsChecked.Value;
            UpdateGamesUI();
        }

        private void GamesButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterWindow window = new FilterWindow(savedState.FilterGames, rm, settings, FilterMode.Game, savedState, pathController);
            window.Search += GameFilterWindow_Search;
            window.ShowDialog();
        }

        private void GameFilterWindow_Search(object sender, FilterWindowSearchEventArgs e)
        {
            savedState.FilterGames = e.FilterEngine;
            UpdateGamesUI();
            SetButtonInUse(GamesButtonSearch, savedState.FilterGames.Filters.Count > 0);
        }
        #endregion

        #region Platforms Tab
        private void UpdatePlatformsUI()
        {
            PlatformsListbox.ClearItems();
            int count = 0;
            foreach (Platform platform in rm.GetPlatformList(savedState.FilterPlatforms, savedState.SortPlatforms, settings))
            {
                ListBoxItemPlatform item = new ListBoxItemPlatform(rm, settings, platform);
                item.MouseDoubleClick += PlatformEdit;
                PlatformsListbox.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(PlatformEdit, PlatformDelete);
                count++;
            }

            BuildPlatformsSortOptions();

            var vis = rm.TotalNumPlatforms() >= rm.LimitPlatforms ? Visibility.Hidden : Visibility.Visible;
            PlatformsButtonNew.Visibility = vis;
            SavedState.SaveSavedState(pathController, savedState);
            LabelPlatformsCount.Content = count.ToString();
        }

        private void BuildPlatformsSortOptions()
        {
            if (!savedState.Loaded || PlatformsButtonSort.ContextMenu.Items.Count > 0) return;
            PlatformsButtonSort.ContextMenu.Items.Clear();
            IList<ISortOption> sortOptions = SortEngine.GetSortOptionList<Platform>(rm, settings);
            foreach (var option in sortOptions)
            {
                var item = new MenuItem
                {
                    IsCheckable = true,
                    IsChecked = option.Equals(savedState.SortPlatforms.SortOption),
                    DisplayMemberPath = "Name",
                    Header = option.Name
                };
                item.Items.Add(option);
                item.Checked += PlatformsSort_Checked;
                item.Unchecked += PlatformsSort_Unchecked;
                PlatformsButtonSort.ContextMenu.Items.Add(item);
            }
        }

        private void PlatformsButtonNew_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowPlatform(SubWindowMode.MODE_ADD, new Platform(rm, settings));
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
            OpenSubWindowPlatform(SubWindowMode.MODE_EDIT, new Platform(lbi.Platform));
        }

        private void PlatformDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemPlatform lbi = GetControlFromMenuItem<ListBoxItemPlatform>((MenuItem)sender);

            MessageBoxResult mbr = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you would like to delete this platform and all data associated with it?", "Delete Platform Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            try
            {
                lbi.Platform.Delete(rm, settings);
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            UpdatePlatformsUI();
        }

        private void OpenSubWindowPlatform(SubWindowMode mode, Platform orig = null)
        {
            var window = new SubWindowPlatform(rm, settings, mode, orig);
            window.Saved += PlatformWindow_Closed;
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
            PlatformSort((ISortOption)item.Items[0]);
            SetButtonInUse(PlatformsButtonSort, savedState.SortPlatforms.SortOption != null);
        }

        private void PlatformsSort_Unchecked(object sender, RoutedEventArgs e)
        {
            savedState.SortPlatforms.SortOption = null;
            UpdatePlatformsUI();
            SetButtonInUse(PlatformsButtonSort, savedState.SortPlatforms.SortOption != null);
        }

        private void PlatformSort(ISortOption sortOption)
        {
            savedState.SortPlatforms.SortMode = GetSortModeFromButton(PlatformsButtonSortMode);
            savedState.SortPlatforms.SortOption = (SortOptionBase)sortOption;
            UpdatePlatformsUI();
        }

        private void PlatformSortRefresh()
        {
            foreach (MenuItem sortItem in PlatformsButtonSort.ContextMenu.Items)
            {
                if (sortItem.IsChecked)
                {
                    PlatformSort((ISortOption)sortItem.Items[0]);
                }
            }
        }

        private void PlatformsButtonSortMode_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ToggleSortModeButton(button);
            PlatformSortRefresh();
        }

        private void PlatformsButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterWindow window = new FilterWindow(savedState.FilterPlatforms, rm, settings, FilterMode.Platform, savedState, pathController);
            window.Search += PlatformFilterWindow_Search;
            window.ShowDialog();
        }

        private void PlatformFilterWindow_Search(object sender, FilterWindowSearchEventArgs e)
        {
            savedState.FilterPlatforms = e.FilterEngine;
            UpdatePlatformsUI();
            SetButtonInUse(PlatformsButtonSearch, savedState.FilterPlatforms.Filters.Count > 0);
        }
        #endregion

        #region Settings Tab
        #region General Settings
        private void UpdateSettingsUI()
        {
            SettingsTextboxMin.Text = settings.MinScore.ToString();
            SettingsTextboxMax.Text = settings.MaxScore.ToString();
            CheckboxShowScoreNullStatus.IsChecked = settings.ShowScoreWhenNullStatus;
            CheckboxTreatGamesAsOwned.IsChecked = settings.TreatAllGamesAsOwned;
            CheckboxUnownedFinishCount.IsChecked = settings.IncludeUnownedGamesInFinishCount;
            SettingsAWSButton.Content = FileHandlerAWSS3.KeyFileExists(pathController) ? "Switch back to local save files" : "Switch to remote save files with AWS";
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

            if (minScore != settings.MinScore || maxScore != settings.MaxScore)
            {
                MessageBoxResult mbr = Xceed.Wpf.Toolkit.MessageBox.Show("Changing the score ranges will scale all your existing scores to fit within the new range. Would you like to do this?", "Change Score Range Confirmation", MessageBoxButton.YesNo);
                if (mbr != MessageBoxResult.Yes) return;
            }

            settings.MinScore = minScore;
            settings.MaxScore = maxScore;
            settings.ShowScoreWhenNullStatus = CheckboxShowScoreNullStatus.IsChecked.Value;
            settings.TreatAllGamesAsOwned = CheckboxTreatGamesAsOwned.IsChecked.Value;
            settings.IncludeUnownedGamesInFinishCount = CheckboxUnownedFinishCount.IsChecked.Value;
            try
            {
                settings.Save(rm, settings);
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }

            UpdateSettingsUI();
            UpdateScoreRangeUI();
            SettingsLabelSuccess.Visibility = Visibility.Visible;
        }

        private async void SettingsAWSButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.IsEnabled = false;

            if (FileHandlerAWSS3.KeyFileExists(pathController))
            {
                // Remove key file
                var result = Xceed.Wpf.Toolkit.MessageBox.Show("Game Tracker will switch to using save files directly on this device.\n\nWould you also like to download your data from AWS and replace any local data with your AWS data?", "Overwrite local?", MessageBoxButton.YesNo);
                
                IFileHandler newFileHandler = new FileHandlerLocalAppData(pathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);
                ILoadSaveHandler<ILoadSaveMethodGame> newLoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(newFileHandler, factory, App.Logger));
                GameModule newModule = new GameModule(newLoadSave, App.Logger);
                if (result == MessageBoxResult.Yes)
                {
                    mainWindow.Title = "Game Tracker (Transferring Save Data...)";
                    App.Logger.Log("Starting transfer from AWS to local");
                    rm.TransferToNewModule(newModule, settings);
                }
                try
                {
                    settings = (SettingsGame)Settings.Load(newLoadSave);
                }
                catch (NoDataFoundException)
                {
                    // first load
                    settings = new SettingsGame();
                }
                loadSave = newLoadSave;
                rm = newModule;
                FileHandlerAWSS3.DeleteKeyFile(pathController);

                await LoadAllData();
            }
            else
            {
                // Add a key file
                OpenFileDialog fileDialog = new OpenFileDialog();
                if (fileDialog.ShowDialog() == true)
                {
                    var result = Xceed.Wpf.Toolkit.MessageBox.Show("Game Tracker will switch to using remote save files with AWS.\n\nWould you also like to upload your data to AWS and replace any existing remote data?", "Overwrite AWS?", MessageBoxButton.YesNo);
                    try
                    {
                        IFileHandler newFileHandler = new FileHandlerAWSS3(fileDialog.FileName, pathController);
                        ILoadSaveHandler<ILoadSaveMethodGame> newLoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(newFileHandler, factory, App.Logger));
                        GameModule newModule = new GameModule(newLoadSave, App.Logger);
                        if (result == MessageBoxResult.Yes)
                        {
                            mainWindow.Title = "Game Tracker (Transferring Save Data...)";
                            App.Logger.Log("Starting transfer from local to AWS");
                            rm.TransferToNewModule(newModule, settings);
                        }
                        try
                        {
                            settings = (SettingsGame)Settings.Load(newLoadSave);
                        }
                        catch (NoDataFoundException)
                        {
                            // first load
                            settings = new SettingsGame();
                        }
                        loadSave = newLoadSave;
                        rm = newModule;

                        await LoadAllData();
                    }
                    catch (Exception ex)
                    {
                        ex.DisplayUIExceptionMessage();
                    }
                }
            }
            mainWindow.IsEnabled = true;
        }
        #endregion

        #region Rating Categories
        private void UpdateRatingCategoryUI()
        {
            SettingsListboxRatingCategories.ClearItems();
            foreach (RatingCategoryWeighted rc in rm.CategoryExtension.GetRatingCategoryList().OfType<RatingCategoryWeighted>())
            {
                ListBoxItemRatingCategory item = new ListBoxItemRatingCategory(rc);
                item.MouseDoubleClick += RatingCategoryEdit;
                SettingsListboxRatingCategories.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(RatingCategoryEdit, RatingCategoryDelete);
            }

            var vis = rm.CategoryExtension.TotalNumRatingCategories() >= rm.CategoryExtension.LimitRatingCategories ? Visibility.Hidden : Visibility.Visible;
            SettingsButtonNewRatingCategory.Visibility = vis;
            GamesButtonSort.ContextMenu.Items.Clear();
        }

        private void SettingsButtonNewRatingCategory_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowRatingCategory(SubWindowMode.MODE_ADD, new RatingCategoryWeighted(rm, settings));
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
            OpenSubWindowRatingCategory(SubWindowMode.MODE_EDIT, new RatingCategoryWeighted(lbi.RatingCategory));
        }

        private void RatingCategoryDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemRatingCategory lbi = GetControlFromMenuItem<ListBoxItemRatingCategory>((MenuItem)sender);

            MessageBoxResult mbr = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you would like to delete this rating category and all data associated with it?", "Delete Rating Category Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            try
            {
                var count = savedState.FilterGames.Filters.RemoveAll(s => s.FilterOption is FilterOptionModelCategory cat && cat.Category.Equals(lbi.RatingCategory));
                bool remove = false;
                if (savedState.SortGames.SortOption is SortOptionModelCategory cat && cat.Category.Equals(lbi.RatingCategory))
                {
                    savedState.SortGames.SortOption = null;
                    remove = true;
                }
                if (count > 0 || remove)
                {
                    SavedState.SaveSavedState(pathController, savedState);
                    SetButtonInUse(GamesButtonSearch, savedState.FilterGames.Filters.Count > 0);
                    SetButtonInUse(GamesButtonSort, false);
                }

                lbi.RatingCategory.Delete(rm, settings);
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            UpdateRatingCategoryUI();
        }

        private void OpenSubWindowRatingCategory(SubWindowMode mode, RatingCategoryWeighted orig = null)
        {
            var window = new SubWindowRatingCategory(rm, settings, mode, orig);
            window.Saved += RatingCategoryWindow_Closed;
            window.ShowDialog();
        }

        private void RatingCategoryWindow_Closed(object sender, EventArgs e)
        {
            UpdateRatingCategoryUI();
        }
        #endregion

        #region Statuses
        private void UpdateCompletionStatusUI()
        {
            SettingsListboxCompletionStatuses.ClearItems();
            foreach (StatusGame cs in rm.StatusExtension.GetStatusList().OfType<StatusGame>())
            {
                ListBoxItemCompletionStatus item = new ListBoxItemCompletionStatus(cs);
                item.MouseDoubleClick += CompletionStatusEdit;
                SettingsListboxCompletionStatuses.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(CompletionStatusEdit, CompletionStatusDelete);
            }

            var vis = rm.StatusExtension.TotalNumStatuses() >= rm.StatusExtension.LimitStatuses ? Visibility.Hidden : Visibility.Visible;
            SettingsButtonNewCompletionStatus.Visibility = vis;
        }

        private void SettingsButtonNewCompletionStatus_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowCompletionStatus(SubWindowMode.MODE_ADD, new StatusGame(rm, settings));
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
            OpenSubWindowCompletionStatus(SubWindowMode.MODE_EDIT, new StatusGame(lbi.CompletionStatus));
        }

        private void CompletionStatusDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemCompletionStatus lbi = GetControlFromMenuItem<ListBoxItemCompletionStatus>((MenuItem)sender);

            MessageBoxResult mbr = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you would like to delete this status and all data associated with it?", "Delete Status Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            try
            {
                lbi.CompletionStatus.Delete(rm, settings);
            }
            catch (Exception ex) 
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            UpdateCompletionStatusUI();
        }

        private void OpenSubWindowCompletionStatus(SubWindowMode mode, StatusGame orig = null)
        {
            var window = new SubWindowCompletionStatus(rm, settings, mode, orig);
            window.Saved += CompletionStatusWindow_Closed;
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
            foreach (ScoreRange sr in rm.GetScoreRangeList())
            {
                ListBoxItemScoreRange item = new ListBoxItemScoreRange(rm, sr);
                item.MouseDoubleClick += ScoreRangeEdit;
                SettingsListboxScoreRanges.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(ScoreRangeEdit, ScoreRangeDelete);
            }

            var vis = rm.TotalNumScoreRanges() >= rm.LimitRanges ? Visibility.Hidden : Visibility.Visible;
            SettingsButtonNewScoreRange.Visibility = vis;
        }

        private void SettingsButtonNewScoreRange_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowScoreRange(SubWindowMode.MODE_ADD, new ScoreRange(rm, settings));
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
            OpenSubWindowScoreRange(SubWindowMode.MODE_EDIT, new ScoreRange(lbi.ScoreRange));
        }

        private void ScoreRangeDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemScoreRange lbi = GetControlFromMenuItem<ListBoxItemScoreRange>((MenuItem)sender);

            MessageBoxResult mbr = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you would like to delete this score range?", "Delete Score Range Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            try
            {
                lbi.ScoreRange.Delete(rm, settings);
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            UpdateScoreRangeUI();
        }

        private void OpenSubWindowScoreRange(SubWindowMode mode, ScoreRange orig)
        {
            var window = new SubWindowScoreRange(rm, settings, mode, orig);
            window.Saved += ScoreRangeWindow_Closed;
            window.ShowDialog();
        }

        private void ScoreRangeWindow_Closed(object sender, EventArgs e)
        {
            UpdateScoreRangeUI();
        }
        #endregion

        #endregion

        #region Menu Bar
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuLogFiles_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", pathController.Combine(pathController.ApplicationDirectory(), LoggerThreaded.LOG_DIRECTORY));
        }

        private void MenuGitHub_Click(object sender, RoutedEventArgs e)
        {
            UtilWPF.GoToURL("https://github.com/mpagliaro98/game-tracker");
        }

        private void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            UtilWPF.GoToURL("https://github.com/mpagliaro98/game-tracker/releases");
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            string message = "Game Tracker: " + UtilWPF.GetVersionNumber().ToString() +
#if DEBUG
                " DEBUG VERSION" +
#endif
                "\nAuthor: Michael Pagliaro" +
                "\nGitHub: github.com/mpagliaro98" +
                "\nThis open-source software is covered under the MIT license, see the license in the GitHub repository for more information.";
            Xceed.Wpf.Toolkit.MessageBox.Show(message, "About", MessageBoxButton.OK);
        }

        private void MenuBackupExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new()
                {
                    DefaultExt = ".bac",
                    Title = "Export Save Backup",
                    FileName = "backup_" + DateTime.UtcNow.ToString("MM-dd-yyyy_HH-mm-ss"),
                    Filter = "Save Backups|*.bac"
                };
                if (dialog.ShowDialog() == true)
                {
                    IFileHandler fileHandler = new FileHandlerLocal("", pathController);
                    byte[] contents;
                    using (var conn = loadSave.NewConnection())
                        contents = conn.ExportSaveBackup();
                    string encodedString = Convert.ToBase64String(contents);
                    byte[] fileData = RatableTracker.Util.Util.TextEncoding.GetBytes(encodedString);
                    fileHandler.SaveFile(dialog.FileName, fileData, App.Logger);
                    Xceed.Wpf.Toolkit.MessageBox.Show("Saved a save backup to " + dialog.FileName, "Save Backup", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                App.Logger.Log("Error exporting save backup: " + ex.GetType().Name + " - " + ex.Message);
                App.Logger.Log(ex.StackTrace);
                Xceed.Wpf.Toolkit.MessageBox.Show("An error occurred when trying to export the save backup. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MenuBackupImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new()
                {
                    DefaultExt = ".bac",
                    Title = "Import Save Backup",
                    Filter = "Save Backups|*.bac"
                };
                if (dialog.ShowDialog() == true)
                {
                    IFileHandler fileHandler = new FileHandlerLocal("", pathController);
                    byte[] fileData = fileHandler.LoadFile(dialog.FileName, App.Logger);
                    string base64Data = RatableTracker.Util.Util.TextEncoding.GetString(fileData);
                    byte[] contents = Convert.FromBase64String(base64Data);
                    using (var conn = loadSave.NewConnection())
                        conn.ImportSaveBackup(contents);
                    Xceed.Wpf.Toolkit.MessageBox.Show("Successfully imported the save backup from " + dialog.FileName, "Import Backup", MessageBoxButton.OK);
                    await LoadAllData();
                }
            }
            catch (Exception ex)
            {
                App.Logger.Log("Error importing save backup: " + ex.GetType().Name + " - " + ex.Message);
                App.Logger.Log(ex.StackTrace);
                Xceed.Wpf.Toolkit.MessageBox.Show("An error occurred when trying to import the save backup. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
