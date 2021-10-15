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
using GameTracker.Model;

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for SubWindowCompletionStatus.xaml
    /// </summary>
    public partial class SubWindowCompletionStatus : Window
    {
        public SubWindowCompletionStatus(RatingModuleGame rm, SubWindowMode mode, CompletionStatus orig = null)
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private bool ValidateInputs(out string name, out bool useAsFinished, out bool excludeFromStats)
        {
            name = "";
            useAsFinished = false;
            excludeFromStats = false;
            return true;
        }
    }
}
