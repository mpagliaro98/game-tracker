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
using GameTracker.Model;

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for SubWindowRatingCategory.xaml
    /// </summary>
    public partial class SubWindowRatingCategory : Window
    {
        private RatingModuleGame rm;
        private RatingCategory orig;

        public SubWindowRatingCategory(RatingModuleGame rm, SubWindowMode mode, RatingCategory orig = null)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            switch (mode)
            {
                case SubWindowMode.MODE_ADD:
                    ButtonSave.Visibility = Visibility.Visible;
                    ButtonUpdate.Visibility = Visibility.Collapsed;
                    break;
                case SubWindowMode.MODE_EDIT:
                    ButtonSave.Visibility = Visibility.Collapsed;
                    ButtonUpdate.Visibility = Visibility.Visible;
                    TextboxName.Text = orig.Name;
                    TextboxComment.Text = orig.Comment;
                    TextboxWeight.Text = orig.Weight.ToString();
                    break;
                default:
                    throw new Exception("Unhandled mode");
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out string comment, out double weight)) return;
            var cat = new RatingCategoryWeighted(name, comment, weight);
            rm.AddRatingCategory(cat);
            Close();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out string comment, out double weight)) return;
            var cat = new RatingCategoryWeighted(name, comment, weight);
            rm.UpdateRatingCategory(cat, orig);
            Close();
        }

        private bool ValidateInputs(out string name, out string comment, out double weight)
        {
            name = TextboxName.Text;
            comment = TextboxComment.Text;
            if (!double.TryParse(TextboxWeight.Text, out weight) || name == "")
            {
                LabelError.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }
    }
}
