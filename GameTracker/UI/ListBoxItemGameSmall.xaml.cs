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

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for ListBoxItemGameSmall.xaml
    /// </summary>
    public partial class ListBoxItemGameSmall : UserControl
    {
        private const string DECIMAL_FORMAT = "0.##";

        private RatableGame rg;
        public RatableGame Game
        {
            get { return rg; }
        }

        public ListBoxItemGameSmall(RatingModuleGame rm, RatableGame rg)
        {
            InitializeComponent();
            this.rg = rg;

            var platform = rm.FindPlatform(rg.RefPlatform);
            var completionStatus = rm.FindCompletionStatus(rg.RefCompletionStatus);

            LabelName.Content = rg.Name;
            LabelPlatform.Content = platform != null ? platform.Name : "";
            LabelStatus.Content = completionStatus != null ? completionStatus.Name : "";
            if (completionStatus != null) LabelStatus.Background = new SolidColorBrush(completionStatus.Color.ToMediaColor());
            BuildCategories(rm.RatingCategories, rg.CategoryValues);
            LabelFinalScore.Content = rm.CalculateFinalScore(rg).ToString(DECIMAL_FORMAT);
            LabelFinalScore.Background = new SolidColorBrush(rm.ApplyScoreRange(rm.CalculateFinalScore(rg)).Color.ToMediaColor());
        }

        private void BuildCategories(IEnumerable<RatingCategory> cats, IEnumerable<RatingCategoryValue> vals)
        {
            int i = 0;
            foreach (RatingCategory cat in cats)
            {
                GridCategories.ColumnDefinitions.Add(new ColumnDefinition());
                var matches = vals.Where(val => val.RefRatingCategory.IsReferencedObject(cat));
                if (matches.Count() > 0)
                {
                    RatingCategoryValue pointValue = matches.First();
                    Label label = new Label();
                    label.Content = pointValue.PointValue.ToString(DECIMAL_FORMAT);
                    Grid.SetColumn(label, i);
                    GridCategories.Children.Add(label);
                }
                i++;
            }
        }
    }
}
