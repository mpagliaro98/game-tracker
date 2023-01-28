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
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            this.settings = settings;
            if (this.orig == null) this.orig = new Platform(rm, settings);
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
                    TextboxYear.Text = orig.ReleaseYear > 0 ? orig.ReleaseYear.ToString() : "";
                    TextboxAcquiredYear.Text = orig.AcquiredYear > 0 ? orig.AcquiredYear.ToString() : "";
                    ColorPickerColor.SelectedColor = orig.Color.ToMediaColor();
                    TextboxAbbreviation.Text = orig.Abbreviation;
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
            orig.ReleaseYear = TextboxYear.Text == "" ? 0 : TextboxYear.Value.HasValue ? TextboxYear.Value.Value : 0;
            orig.AcquiredYear = TextboxAcquiredYear.Text == "" ? 0 : TextboxAcquiredYear.Value.HasValue ? TextboxAcquiredYear.Value.Value : 0;
            orig.Color = ColorPickerColor.SelectedColor.ToDrawingColor();
            orig.Abbreviation = TextboxAbbreviation.Text;
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
