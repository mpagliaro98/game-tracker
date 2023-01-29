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

        private const string SORT_CM_PREFIX = "sort";

        private GameModule rm;
        private SettingsGame settings;
        private ILoadSaveHandler<ILoadSaveMethodGame> loadSave;
        private readonly Task loadData = null;
        private Stopwatch swLoad = new Stopwatch();

        private static bool LOAD_ASYNC = false;

        private class SavedState
        {
            public FilterGames filterGames = new FilterGames();
            public SortGames sortGames = new SortGames();
            public FilterPlatforms filterPlatforms = new FilterPlatforms();
            public SortPlatforms sortPlatforms = new SortPlatforms();
            public bool loaded = false;
            public GameDisplayMode displayMode = GameDisplayMode.DISPLAY_SMALL;
        }

        private SavedState savedState = new SavedState();

        public MainWindow()
        {
            savedState.loaded = false;

            // TODO use aws file handler if key file exists
            IPathController pathController = new PathControllerWindows();
            IFileHandler fileHandlerSaves = new FileHandlerLocalAppData(pathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);
            GameTrackerFactory factory = new GameTrackerFactory();
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
            swLoad.Start();
            if (LOAD_ASYNC)
                loadData = rm.LoadDataAsync(settings);

            savedState.filterGames.Module = rm;
            savedState.filterGames.Settings = settings;
            savedState.sortGames.Module = rm;
            savedState.sortGames.Settings = settings;
            savedState.filterPlatforms.Module = rm;
            savedState.filterPlatforms.Settings = settings;
            savedState.sortPlatforms.Module = rm;
            savedState.sortPlatforms.Settings = settings;

            InitializeComponent();
            PlatformsButtonSortMode.Tag = savedState.sortPlatforms.SortMode;
            GamesButtonSortMode.Tag = savedState.sortGames.SortMode;
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

            savedState.loaded = true;
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
        #endregion

        #region Games Tab
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
            foreach (GameObject rg in rm.GetModelObjectList(savedState.filterGames, savedState.sortGames))
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
            BuildCategoriesHeader(rm.CategoryExtension.GetRatingCategoryList());
            BuildGamesSortOptions(rm.CategoryExtension.GetRatingCategoryList());

            var vis = rm.TotalNumModelObjects() >= rm.LimitModelObjects ? Visibility.Hidden : Visibility.Visible;
            GamesButtonNew.Visibility = vis;
        }

        private void BuildCategoriesHeader(IList<RatingCategory> cats)
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

        private void BuildGamesSortOptions(IList<RatingCategory> cats)
        {
            if (!savedState.loaded || GamesButtonSort.ContextMenu.Items.Count > 0) return;
            GamesButtonSort.ContextMenu.Items.Clear();
            MenuItem item;
            foreach (Tuple<string, string> sortOption in new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(SortGames.SORT_Name.ToString(), "Name"),
                new Tuple<string, string>(SortGames.SORT_Status.ToString(), "Completion Status"),
                new Tuple<string, string>(SortGames.SORT_Platform.ToString(), "Platform"),
                new Tuple<string, string>(SortGames.SORT_PlatformPlayedOn.ToString(), "Platform Played On"),
                new Tuple<string, string>(SortGames.SORT_Score.ToString(), "Final Score"),
                new Tuple<string, string>(SortGames.SORT_HasComment.ToString(), "Has Comment"),
                new Tuple<string, string>(SortGames.SORT_ReleaseDate.ToString(), "Release Date"),
                new Tuple<string, string>(SortGames.SORT_AcquiredOn.ToString(), "Acquired On"),
                new Tuple<string, string>(SortGames.SORT_StartedOn.ToString(), "Started On"),
                new Tuple<string, string>(SortGames.SORT_FinishedOn.ToString(), "Finished On")
            })
            {
                item = new MenuItem
                {
                    Name = SORT_CM_PREFIX + sortOption.Item1,
                    Header = sortOption.Item2,
                    IsCheckable = true
                };
                item.Checked += GamesSort_Checked;
                item.Unchecked += GamesSort_Unchecked;
                GamesButtonSort.ContextMenu.Items.Add(item);
            }
            for (int i = 0; i < cats.Count(); i++)
            {
                var cat = cats.ElementAt(i);
                item = new MenuItem
                {
                    Header = cat.Name,
                    Name = SORT_CM_PREFIX + (SortGames.SORT_CategoryStart + i).ToString(),
                    IsCheckable = true
                };
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
            OpenSubWindowGame(SubWindowMode.MODE_EDIT, new GameObject(lbi.Game));
        }

        private void GameDelete(object sender, RoutedEventArgs e)
        {
            ListBoxItemGameSmall lbi = GetControlFromMenuItem<ListBoxItemGameSmall>((MenuItem)sender);

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this game?", "Delete Game Confirmation", MessageBoxButton.YesNo);
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
            if (orig != null && orig.IsCompilation)
                window = new SubWindowCompilation(rm, settings, mode, orig as GameCompilation);
            else
                window = new SubWindowGame(rm, settings, loadSave, mode, orig);
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
            savedState.sortGames.SortMethod = SortGames.SORT_None;
            UpdateGamesUI();
        }

        private void GamesSort(string sortField)
        {
            savedState.sortGames.SortMode = GetSortModeFromButton(GamesButtonSortMode);
            savedState.sortGames.SortMethod = Convert.ToInt32(sortField.Substring(SORT_CM_PREFIX.Length));
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
            savedState.filterGames.ShowCompilations = CheckboxShowCompilations.IsChecked.Value;
            UpdateGamesUI();
        }
        #endregion

        #region Platforms Tab
        private void UpdatePlatformsUI()
        {
            PlatformsListbox.ClearItems();
            foreach (Platform platform in rm.GetPlatformList(savedState.filterPlatforms, savedState.sortPlatforms))
            {
                ListBoxItemPlatform item = new ListBoxItemPlatform(rm, settings, platform);
                item.MouseDoubleClick += PlatformEdit;
                PlatformsListbox.AddItem(item);

                item.ContextMenu = EditDeleteContextMenu(PlatformEdit, PlatformDelete);
            }

            BuildPlatformsSortOptions();

            var vis = rm.TotalNumPlatforms() >= rm.LimitPlatforms ? Visibility.Hidden : Visibility.Visible;
            PlatformsButtonNew.Visibility = vis;
        }

        private void BuildPlatformsSortOptions()
        {
            if (!savedState.loaded || PlatformsButtonSort.ContextMenu.Items.Count > 0) return;
            PlatformsButtonSort.ContextMenu.Items.Clear();
            MenuItem item;
            foreach (Tuple<string, string> sortOption in new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(SortPlatforms.SORT_Name.ToString(), "Name"),
                new Tuple<string, string>(SortPlatforms.SORT_NumGames.ToString(), "# Games"),
                new Tuple<string, string>(SortPlatforms.SORT_Average.ToString(), "Average Score"),
                new Tuple<string, string>(SortPlatforms.SORT_Highest.ToString(), "Highest Score"),
                new Tuple<string, string>(SortPlatforms.SORT_Lowest.ToString(), "Lowest Score"),
                new Tuple<string, string>(SortPlatforms.SORT_PercentFinished.ToString(), "% Finished"),
                new Tuple<string, string>(SortPlatforms.SORT_Release.ToString(), "Release Year"),
                new Tuple<string, string>(SortPlatforms.SORT_Acquired.ToString(), "Acquired Year")
            })
            {
                item = new MenuItem
                {
                    Name = SORT_CM_PREFIX + sortOption.Item1,
                    Header = sortOption.Item2,
                    IsCheckable = true
                };
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

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this platform and all data associated with it?", "Delete Platform Confirmation", MessageBoxButton.YesNo);
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
            savedState.sortPlatforms.SortMethod = SortPlatforms.SORT_None;
            UpdatePlatformsUI();
        }

        private void PlatformSort(string sortField)
        {
            savedState.sortPlatforms.SortMode = GetSortModeFromButton(PlatformsButtonSortMode);
            savedState.sortPlatforms.SortMethod = Convert.ToInt32(sortField.Substring(SORT_CM_PREFIX.Length));
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
            SettingsTextboxMin.Text = settings.MinScore.ToString();
            SettingsTextboxMax.Text = settings.MaxScore.ToString();
            // TODO
            //SettingsAWSButton.Content = ContentLoadSaveAWSS3.KeyFileExists() ? "Switch back to local save files" : "Switch to remote save files with AWS";
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
                MessageBoxResult mbr = MessageBox.Show("Changing the score ranges will scale all your existing scores to fit within the new range. Would you like to do this?", "Change Score Range Confirmation", MessageBoxButton.YesNo);
                if (mbr != MessageBoxResult.Yes) return;
                settings.MinScore = minScore;
                settings.MaxScore = maxScore;
                try
                {
                    settings.Save(rm, settings);
                }
                catch (Exception ex)
                {
                    ex.DisplayUIExceptionMessage();
                    return;
                }
            }

            UpdateSettingsUI();
            SettingsLabelSuccess.Visibility = Visibility.Visible;
        }

        private async void SettingsAWSButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.IsEnabled = false;

            // TODO
            if (mainWindow.IsEnabled) //(ContentLoadSaveAWSS3.KeyFileExists())
            {
                // Remove key file
                var result = MessageBox.Show("Transfer AWS save files to local? This will overwrite anything currently on this device.", "Overwrite local?", MessageBoxButton.YesNo);
                // TODO transfer save files, make new module object
                //IContentLoadSave<string, string> cls = new ContentLoadSaveLocal();
                //if (result == MessageBoxResult.Yes)
                //{
                //    IContentLoadSave<string, string> from = new ContentLoadSaveAWSS3();
                //    await rm.TransferSaveFilesAsync(from, cls);
                //}
                //ContentLoadSaveAWSS3.DeleteKeyFile();
                //LoadSaveEngineGameJson<ValueContainer> engine = new LoadSaveEngineGameJson<ValueContainer>
                //{
                //    ContentLoadSaveInstance = cls
                //};
                //rm = new RatingModuleGame(engine);
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
                        // TODO transfer save files, make new module object
                        //ContentLoadSaveAWSS3.CreateKeyFile(fileDialog.FileName);
                        //IContentLoadSave<string, string> cls = new ContentLoadSaveAWSS3();
                        //if (result == MessageBoxResult.Yes)
                        //{
                        //    IContentLoadSave<string, string> from = new ContentLoadSaveLocal();
                        //    await rm.TransferSaveFilesAsync(from, cls);
                        //}
                        //LoadSaveEngineGameJson<ValueContainer> engine = new LoadSaveEngineGameJson<ValueContainer>
                        //{
                        //    ContentLoadSaveInstance = cls
                        //};
                        //rm = new RatingModuleGame(engine);
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
            savedState.sortGames.SortMethod = SortGames.SORT_None;
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

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this rating category and all data associated with it?", "Delete Rating Category Confirmation", MessageBoxButton.YesNo);
            if (mbr != MessageBoxResult.Yes) return;

            try
            {
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

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this completion status and all data associated with it?", "Delete Completion Status Confirmation", MessageBoxButton.YesNo);
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

            MessageBoxResult mbr = MessageBox.Show("Are you sure you would like to delete this score range?", "Delete Score Range Confirmation", MessageBoxButton.YesNo);
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
