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
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowRatingCategory.xaml
    /// </summary>
    public partial class SubWindowRatingCategory : Window
    {
        private RatingModuleGame rm;
        private RatingCategoryWeighted orig;

        public SubWindowRatingCategory(RatingModuleGame rm, SubWindowMode mode, RatingCategoryWeighted orig = null)
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
            SaveResult();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();
        }

        private void SaveResult()
        {
            if (!ValidateInputs(out string name, out string comment, out double weight)) return;
            var cat = new RatingCategoryWeighted()
            {
                Name = name,
                Comment = comment
            };
            cat.SetWeight(weight);
            try
            {
                if (orig == null)
                    rm.AddRatingCategory(cat);
                else
                    rm.UpdateRatingCategory(cat, orig);
            }
            catch (ValidationException e)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = e.Message;
                return;
            }
            Close();
        }

        private bool ValidateInputs(out string name, out string comment, out double weight)
        {
            name = TextboxName.Text;
            comment = TextboxComment.Text;
            if (!double.TryParse(TextboxWeight.Text, out weight))
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = "The value for weight must be a number";
                return false;
            }
            return true;
        }
    }
}
