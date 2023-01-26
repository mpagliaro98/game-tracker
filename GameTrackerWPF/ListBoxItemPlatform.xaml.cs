using GameTracker;
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

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for ListBoxItemPlatform.xaml
    /// </summary>
    public partial class ListBoxItemPlatform : UserControl
    {
        private Platform platform;
        public Platform Platform
        {
            get { return platform; }
        }

        public ListBoxItemPlatform(GameModule rm, SettingsGame settings, Platform platform)
        {
            // TODO
            InitializeComponent();
            this.platform = platform;
            RectangeColor.Fill = new SolidColorBrush(platform.Color.ToMediaColor());
            LabelName.Content = platform.Name;
            LabelNumOwned.Content = "0"; // rm.GetNumGamesByPlatform(platform).ToString();
            LabelAverage.Content = "0"; // rm.GetAverageScoreOfGamesByPlatform(platform).ToString("0.#####");
            LabelHighest.Content = "0"; // rm.GetHighestScoreFromGamesByPlatform(platform).ToString("0.##");
            LabelLowest.Content = "0"; // rm.GetLowestScoreFromGamesByPlatform(platform).ToString("0.##");
            LabelFinishPercent.Content = rm.GetProportionGamesFinishedByPlatform(platform, settings).ToString("P2") + "%";
            LabelFinishRatio.Content = rm.GetNumGamesFinishedByPlatform(platform, settings).ToString() + "/" + rm.GetNumGamesFinishableByPlatform(platform, settings).ToString() + " games";
            SetStackPanelLabels(StackPanelTop, rm.GetGamesOnPlatform(platform, settings).Take(3).ToList()); // rm.GetTopGamesByPlatform(platform, 3));
            SetStackPanelLabels(StackPanelBottom, rm.GetGamesOnPlatform(platform, settings).Take(3).ToList()); // rm.GetBottomGamesByPlatform(platform, 3));
        }

        private void SetStackPanelLabels(StackPanel panel, IList<GameObject> list)
        {
            panel.Children.Clear();
            foreach (GameObject rg in list)
            {
                Label label = new Label();
                label.Content = rg.Name;
                panel.Children.Add(label);
            }
        }
    }
}
