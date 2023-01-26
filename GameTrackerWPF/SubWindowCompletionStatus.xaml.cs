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
    /// Interaction logic for SubWindowCompletionStatus.xaml
    /// </summary>
    public partial class SubWindowCompletionStatus : Window
    {
        private GameModule rm;
        private StatusGame orig;

        public SubWindowCompletionStatus(GameModule rm, SubWindowMode mode, StatusGame orig)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            if (this.orig == null) this.orig = new StatusGame(rm.StatusExtension);
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
                    CheckboxUseAsFinished.IsChecked = orig.UseAsFinished;
                    CheckboxExcludeFromStats.IsChecked = orig.ExcludeFromStats;
                    ColorPickerColor.SelectedColor = orig.Color.ToMediaColor();
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
            orig.UseAsFinished = CheckboxUseAsFinished.IsChecked.Value;
            orig.ExcludeFromStats = CheckboxExcludeFromStats.IsChecked.Value;
            orig.Color = ColorPickerColor.SelectedColor.ToDrawingColor();
            try
            {
                orig.Save(rm);
            }
            catch (ValidationException e)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = e.Message;
                return;
            }
            Close();
        }
    }
}
