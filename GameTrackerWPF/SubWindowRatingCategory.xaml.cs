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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using GameTracker;
using RatableTracker.Exceptions;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowRatingCategory.xaml
    /// </summary>
    public partial class SubWindowRatingCategory : Window
    {
        private GameModule rm;
        private RatingCategoryWeighted orig;
        private SettingsGame settings;

        public SubWindowRatingCategory(GameModule rm, SettingsGame settings, SubWindowMode mode, RatingCategoryWeighted orig)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            this.settings = settings;
            if (this.orig == null) this.orig = new RatingCategoryWeighted(rm, settings);
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
            orig.Name = TextboxName.Text;
            orig.Comment = TextboxComment.Text;
            if (!double.TryParse(TextboxWeight.Text, out double weight))
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = "The value for weight must be a number";
                return;
            }
            orig.SetWeight(weight);
            try
            {
                orig.Save(rm, settings);
            }
            catch (ValidationException e)
            {
                e.DisplayUIExceptionMessage();
                return;
            }
            Close();
        }
    }
}
