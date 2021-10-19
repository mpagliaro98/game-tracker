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

        public ListBoxItemPlatform(Platform platform)
        {
            InitializeComponent();
            this.platform = platform;
            RectangeColor.Fill = new SolidColorBrush(platform.Color.ToMediaColor());
            LabelName.Content = platform.Name;
            LabelNumOwned.Content = platform.NumGamesOwned.ToString();
            LabelAverage.Content = platform.AverageScoreOfGames.ToString("0.#####");
            LabelHighest.Content = platform.HighestScoreFromGames.ToString("0.##");
            LabelLowest.Content = platform.LowestScoreFromGames.ToString("0.##");
            LabelFinishPercent.Content = platform.PercentageGamesFinished.ToString("0.##") + "%";
            LabelFinishRatio.Content = platform.NumGamesFinished.ToString() + platform.NumGamesFinishable.ToString() + " games";
        }
    }
}
