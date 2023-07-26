using GameTracker;
using RatableTracker.ObjAddOns;
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
    /// Interaction logic for ListBoxItemGameSmall.xaml
    /// </summary>
    public partial class ListBoxItemGameSmall : UserControl, IListBoxItemGame
    {
        private GameObject rg;
        public GameObject Game
        {
            get { return rg; }
        }

        public ListBoxItemGameSmall(GameModule rm, GameObject rg)
        {
            InitializeComponent();
            this.rg = rg;

            var platform = rg.Platform;
            var completionStatus = rg.StatusExtension.Status;

            LabelName.Content = rg.Name;
            LabelPlatform.Content = platform != null ? platform.Name : "";
            LabelStatus.Content = completionStatus != null ? completionStatus.Name : "";
            if (completionStatus != null) LabelStatus.Background = new SolidColorBrush(completionStatus.Color.ToMediaColor());
            if (!rg.CategoryExtension.IgnoreCategories) BuildCategories(rm, rg);
            LabelFinalScore.Content = rg.ShowScore ? rg.ScoreDisplay.ToString(UtilWPF.SCORE_FORMAT) : "";
            ScoreRange sr = rg.ScoreRangeDisplay;
            if (sr != null) LabelFinalScore.Background = new SolidColorBrush(sr.Color.ToMediaColor());
        }

        private void BuildCategories(GameModule rm, GameObject rg)
        {
            int i = 0;
            foreach (RatingCategory cat in rm.CategoryExtension.GetRatingCategoryList())
            {
                GridCategories.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                double score = rg.CategoryExtension.ScoreOfCategoryDisplay(cat);
                Label label = new Label();
                label.Content = rg.ShowScore ? score.ToString(UtilWPF.SCORE_FORMAT) : "";
                Grid.SetColumn(label, i);
                GridCategories.Children.Add(label);
                i++;
            }
        }
    }
}
