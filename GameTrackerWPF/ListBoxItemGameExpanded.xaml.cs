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
using RatableTracker.Framework;
using RatableTracker.Framework.Global;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for ListBoxItemGameExpanded.xaml
    /// </summary>
    public partial class ListBoxItemGameExpanded : UserControl, IListBoxItemGame
    {
        private const string DECIMAL_FORMAT = "0.##";

        private RatableGame rg;
        public RatableGame Game
        {
            get { return rg; }
        }

        public ListBoxItemGameExpanded(RatingModuleGame rm, RatableGame rg)
        {
            InitializeComponent();
            this.rg = rg;

            var platform = rm.FindPlatform(rg.RefPlatform);
            var playedOn = rm.FindPlatform(rg.RefPlatformPlayedOn);
            var completionStatus = rm.FindStatus(rg.RefStatus);

            TextBlockName.Text = rg.Name;
            TextBlockPlatform.Text = platform != null ? platform.Name : "";
            TextBlockPlayedOn.Text = playedOn != null ? "Played on: " + playedOn.Name : "";
            if (playedOn == null)
                Grid.SetRowSpan(TextBlockPlatform, 2);
            TextBlockStatus.Text = completionStatus != null ? completionStatus.Name : "";
            if (completionStatus != null)
                TextBlockStatus.Background = new SolidColorBrush(completionStatus.Color.ToMediaColor());
            BuildCategories(rm, rg);
            TextBlockFinalScore.Text = rm.GetScoreOfObject(rg).ToString(DECIMAL_FORMAT);
            TextBlockFinalScore.Background = new SolidColorBrush(rm.GetRangeColorFromObject(rg).ToMediaColor());
            TextBlockComment.Text = rg.Comment;
        }

        private void BuildCategories(RatingModuleGame rm, RatableGame rg)
        {
            for (int i = 0; i < rm.RatingCategories.Count(); i+=2)
            {
                var col = new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                GridCategories.ColumnDefinitions.Add(col);
            }
            var row = new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            GridCategories.RowDefinitions.Add(row);
            if (rm.RatingCategories.Count() > 1)
            {
                row = new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };
                GridCategories.RowDefinitions.Add(row);
            }

            int slot = 0;
            foreach (RatingCategoryWeighted cat in rm.RatingCategories)
            {
                double score = rm.GetScoreOfCategory(rg, cat);
                StackPanel panel = new StackPanel
                {
                    Background = new SolidColorBrush(new System.Windows.Media.Color() { A = 0xFF, R = 0xF4, G = 0xF4, B = 0xF4 }),
                    Margin = new Thickness(2)
                };
                TextBlock tb = new TextBlock()
                {
                    Text = cat.Name,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    FontSize = 8,
                    TextAlignment = TextAlignment.Center
                };
                Label label = new Label
                {
                    Content = score.ToString(DECIMAL_FORMAT),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 16
                };
                panel.Children.Add(tb);
                panel.Children.Add(label);
                Grid.SetColumn(panel, slot / 2);
                Grid.SetRow(panel, slot % 2);
                GridCategories.Children.Add(panel);
                slot++;
            }
        }
    }
}
