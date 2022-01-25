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
using RatableTracker.Framework;
using RatableTracker.Framework.Global;
using GameTracker.Model;
using RatableTracker.Framework.Exceptions;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowCompletionStatus.xaml
    /// </summary>
    public partial class SubWindowCompletionStatus : Window
    {
        private RatingModuleGame rm;
        private CompletionStatus orig;

        public SubWindowCompletionStatus(RatingModuleGame rm, SubWindowMode mode, CompletionStatus orig = null)
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
            if (!ValidateInputs(out string name, out bool useAsFinished,
                out bool excludeFromStats, out RatableTracker.Framework.Color color)) return;
            var status = new CompletionStatus()
            {
                Name = name,
                UseAsFinished = useAsFinished,
                ExcludeFromStats = excludeFromStats,
                Color = color
            };
            try
            {
                if (orig == null)
                    rm.AddStatus(status);
                else
                    rm.UpdateStatus(status, orig);
            }
            catch (ValidationException e)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = e.Message;
                return;
            }
            Close();
        }

        private bool ValidateInputs(out string name, out bool useAsFinished, out bool excludeFromStats,
            out RatableTracker.Framework.Color color)
        {
            name = TextboxName.Text;
            useAsFinished = CheckboxUseAsFinished.IsChecked.Value;
            excludeFromStats = CheckboxExcludeFromStats.IsChecked.Value;
            color = ColorPickerColor.SelectedColor.ToDrawingColor();
            return true;
        }
    }
}
