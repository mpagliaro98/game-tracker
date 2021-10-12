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
using System.Windows.Shapes;
using GameRater.Model;

namespace GameRater.UI
{
    /// <summary>
    /// Interaction logic for SubWindowRatingCategoryNew.xaml
    /// </summary>
    public partial class SubWindowRatingCategoryNew : Window
    {
        private RatingModuleGame rm;

        public SubWindowRatingCategoryNew(RatingModuleGame rm)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TextboxName.Text;
            string comment = TextboxComment.Text;
            if (!double.TryParse(TextboxWeight.Text, out double weight) || name == "")
            {
                LabelError.Visibility = Visibility.Visible;
                return;
            }
            var cat = new RatingCategoryWeighted(name, comment, weight);
            rm.AddRatingCategory(cat);
            rm.SaveRatingCategories();
            Close();
        }
    }
}
