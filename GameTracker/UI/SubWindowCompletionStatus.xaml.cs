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

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for SubWindowCompletionStatus.xaml
    /// </summary>
    public partial class SubWindowCompletionStatus : Window
    {
        private RatingModuleGame rm;
        private CompletionStatusGame orig;

        public SubWindowCompletionStatus(RatingModuleGame rm, SubWindowMode mode, CompletionStatusGame orig = null)
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
            if (!ValidateInputs(out string name, out bool useAsFinished,
                out bool excludeFromStats, out System.Drawing.Color color)) return;
            var status = new CompletionStatusGame()
            {
                Name = name,
                UseAsFinished = useAsFinished,
                ExcludeFromStats = excludeFromStats,
                Color = color
            };
            rm.AddCompletionStatus(status);
            Close();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out bool useAsFinished,
                out bool excludeFromStats, out System.Drawing.Color color)) return;
            orig.Name = name;
            orig.UseAsFinished = useAsFinished;
            orig.ExcludeFromStats = excludeFromStats;
            orig.Color = color;
            rm.SaveCompletionStatuses();
            Close();
        }

        private bool ValidateInputs(out string name, out bool useAsFinished, out bool excludeFromStats,
            out System.Drawing.Color color)
        {
            name = TextboxName.Text;
            useAsFinished = CheckboxUseAsFinished.IsChecked.Value;
            excludeFromStats = CheckboxExcludeFromStats.IsChecked.Value;
            color = ColorPickerColor.SelectedColor.ToDrawingColor();
            if (name == "")
            {
                LabelError.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }
    }
}
