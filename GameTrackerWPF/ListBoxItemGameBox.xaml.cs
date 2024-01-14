using GameTracker;
using RatableTracker.ScoreRanges;
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
    /// Interaction logic for ListBoxItemGameBox.xaml
    /// </summary>
    public partial class ListBoxItemGameBox : UserControl, IListBoxItemGame
    {
        private GameObject rg;
        public GameObject Game
        {
            get { return rg; }
        }

        public ListBoxItemGameBox(GameModule rm, GameObject rg, bool shortDLCName = false)
        {
            InitializeComponent();
            this.rg = rg;

            var platform = rg.Platform;
            var completionStatus = rg.StatusExtension.Status;

            GridMain.Opacity = rg.IsNotOwned ? 0.7 : 1.0;
            GridMain.ToolTip = shortDLCName ? rg.DisplayName : rg.Name;
            DLCText.Visibility = rg.IsDLC ? Visibility.Visible : Visibility.Collapsed;
            DLCIndicator.Visibility = rg.IsDLC ? Visibility.Visible : Visibility.Collapsed;
            TextBlockName.Text = shortDLCName ? rg.DisplayName : rg.Name;
            TextBlockName.FontStyle = rg.IsNotOwned ? FontStyles.Italic : FontStyles.Normal;
            TextBlockPlatform.Text = platform != null ? platform.Name : "";
            TextBlockStatus.Text = completionStatus != null ? completionStatus.Name : "";
            if (completionStatus != null)
                TextBlockStatus.Background = new SolidColorBrush(completionStatus.Color.ToMediaColor());
            TextBlockFinalScore.Text = rg.ShowScore ? rg.ScoreDisplay.ToString(UtilWPF.SCORE_FORMAT) : "";
            if (rg.ShowScore)
            {
                ScoreRange sr = rg.ScoreRangeDisplay;
                if (sr != null) TextBlockFinalScore.Background = new SolidColorBrush(sr.Color.ToMediaColor());
            }
        }
    }
}
