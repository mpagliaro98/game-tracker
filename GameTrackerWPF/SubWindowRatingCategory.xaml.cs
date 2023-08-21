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

        public event EventHandler Saved;

        public SubWindowRatingCategory(GameModule rm, SettingsGame settings, SubWindowMode mode, RatingCategoryWeighted orig)
        {
            InitializeComponent();
            this.rm = rm;
            this.orig = orig;
            this.settings = settings;

            // initialize UI containers
            ButtonSave.Content = mode == SubWindowMode.MODE_ADD ? "Create" : "Update";

            // set max length
            TextboxName.MaxLength = RatingCategoryWeighted.MaxLengthName;
            TextboxComment.MaxLength = RatingCategoryWeighted.MaxLengthComment;

            // set fields in the UI
            TextboxName.Text = orig.Name;
            TextboxComment.Text = orig.Comment;
            TextboxWeight.Value = orig.Weight;

            // set event handlers
            TextboxName.TextChanged += TextboxName_TextChanged;
            TextboxComment.TextChanged += TextboxComment_TextChanged;
            TextboxWeight.ValueChanged += TextboxWeight_ValueChanged;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                orig.Save(rm, settings);
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            Saved?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Name = TextboxName.Text.Trim();
        }

        private void TextboxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Comment = TextboxComment.Text.Trim();
        }

        private void TextboxWeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            orig.Weight = TextboxWeight.Value ?? 1;
        }
    }
}
