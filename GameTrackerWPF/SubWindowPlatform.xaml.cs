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
using GameTracker;
using RatableTracker.Exceptions;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowPlatform.xaml
    /// </summary>
    public partial class SubWindowPlatform : Window
    {
        private GameModule rm;
        private Platform orig;
        private SettingsGame settings;

        public SubWindowPlatform(GameModule rm, SettingsGame settings, SubWindowMode mode, Platform orig)
        {
            InitializeComponent();
            this.rm = rm;
            this.orig = orig;
            this.settings = settings;

            // initialize UI containers
            ButtonSave.Content = mode == SubWindowMode.MODE_ADD ? "Create" : "Update";

            // set fields in the UI
            TextboxName.Text = orig.Name;
            TextboxYear.Text = orig.ReleaseYear > 0 ? orig.ReleaseYear.ToString() : "";
            TextboxAcquiredYear.Text = orig.AcquiredYear > 0 ? orig.AcquiredYear.ToString() : "";
            ColorPickerColor.SelectedColor = orig.Color.ToMediaColor();
            TextboxAbbreviation.Text = orig.Abbreviation;

            // set event handlers
            TextboxName.TextChanged += TextboxName_TextChanged;
            TextboxYear.ValueChanged += TextboxYear_ValueChanged;
            TextboxAcquiredYear.ValueChanged += TextboxAcquiredYear_ValueChanged;
            ColorPickerColor.SelectedColorChanged += ColorPickerColor_SelectedColorChanged;
            TextboxAbbreviation.TextChanged += TextboxAbbreviation_TextChanged;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                orig.Save(rm, settings);
            }
            catch (ValidationException ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            Close();
        }

        private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Name = TextboxName.Text.Trim();
        }

        private void TextboxYear_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            orig.ReleaseYear = TextboxYear.Value ?? 0;
        }

        private void TextboxAcquiredYear_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            orig.AcquiredYear = TextboxAcquiredYear.Value ?? 0;
        }

        private void ColorPickerColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            orig.Color = ColorPickerColor.SelectedColor.ToDrawingColor();
        }

        private void TextboxAbbreviation_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Abbreviation = TextboxAbbreviation.Text.Trim();
        }
    }
}
