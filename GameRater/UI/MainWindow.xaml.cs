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
using GameRater.Model;

namespace GameRater.UI
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
            rm = new RatingModuleGame();
            rm.Init();
            InitializeComponent();
        }

        private void SettingsGridButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ResetSettingsLabels();
            string minScoreInput = SettingsTextboxMin.Text;
            string maxScoreInput = SettingsTextboxMax.Text;
            double minScore, maxScore;
            if (!(double.TryParse(minScoreInput, out minScore) && double.TryParse(maxScoreInput, out maxScore)))
            {
                SettingsLabelError.Visibility = Visibility.Visible;
                return;
            }
            rm.Settings.MinScore = minScore;
            rm.Settings.MaxScore = maxScore;
            rm.SaveSettings();
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
                ListBoxItemRatingCategory item = new ListBoxItemRatingCategory();
                item.SetContent(rc);
                SettingsListboxRatingCategories.AddItem(item);
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
    }
}
