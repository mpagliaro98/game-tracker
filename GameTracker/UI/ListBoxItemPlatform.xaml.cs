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
using RatableTracker.Framework.Global;

namespace GameTracker.UI
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

        public ListBoxItemPlatform(RatingModuleGame rm, Platform platform)
        {
            InitializeComponent();
            this.platform = platform;
            RectangeColor.Fill = new SolidColorBrush(platform.Color.ToMediaColor());
            LabelName.Content = platform.Name;
            LabelNumOwned.Content = rm.GetNumGamesByPlatform(platform).ToString();
            LabelAverage.Content = rm.GetAverageScoreOfGamesByPlatform(platform).ToString("0.#####");
            LabelHighest.Content = rm.GetHighestScoreFromGamesByPlatform(platform).ToString("0.##");
            LabelLowest.Content = rm.GetLowestScoreFromGamesByPlatform(platform).ToString("0.##");
            LabelFinishPercent.Content = rm.GetPercentageGamesFinishedByPlatform(platform).ToString("0.##") + "%";
            LabelFinishRatio.Content = rm.GetNumGamesFinishedByPlatform(platform).ToString() + rm.GetNumGamesFinishableByPlatform(platform).ToString() + " games";
        }
    }
}
