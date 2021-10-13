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
            rm = new RatingModuleGame(new LoadSaveEngineGameJson());
            rm.Init();
            InitializeComponent();
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
            rm.SaveSettings();
            rm.SaveRatableObjects();
            UpdateSettingsUI();
            SettingsLabelSuccess.Visibility = Visibility.Visible;
        }

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

        private void UpdateRatingCategoryUI()
        {
            SettingsListboxRatingCategories.ClearItems();
            foreach (RatingCategory rc in rm.RatingCategories)
            {
                ListBoxItemRatingCategory item = new ListBoxItemRatingCategory(rc);
                item.MouseDoubleClick += RatingCategoryEditWindow_Open;
                SettingsListboxRatingCategories.AddItem(item);

                ContextMenu cm = new ContextMenu();
                item.ContextMenu = cm;
                MenuItem mie = new MenuItem();
                mie.Header = "Edit";
                mie.Click += RatingCategoryEditWindow_Open;
                cm.Items.Add(mie);
                MenuItem mid = new MenuItem();
                mid.Header = "Delete";
                cm.Items.Add(mid);
            }
        }

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
                    break;
                case TAB_PLATFORMS:
                    break;
                case TAB_SETTINGS:
                    ResetSettingsLabels();
                    UpdateSettingsUI();
                    UpdateRatingCategoryUI();
                    break;
                default:
                    throw new Exception("Unhandled tab");
            }
            e.Handled = true;
        }

        private void SettingsButtonNewRatingCategory_Click(object sender, RoutedEventArgs e)
        {
            OpenSubWindowRatingCategory(SubWindowMode.MODE_ADD);
        }

        private void RatingCategoryEditWindow_Open(object sender, RoutedEventArgs e)
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

        private void OpenSubWindowRatingCategory(SubWindowMode mode, RatingCategory orig = null)
        {
            var window = new SubWindowRatingCategory(rm, mode, orig);
            window.Closed += RatingCategoryWindow_Closed;
            window.ShowDialog();
        }

        private void RatingCategoryWindow_Closed(object sender, EventArgs e)
        {
            UpdateRatingCategoryUI();
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
            return null;
        }
    }
}
