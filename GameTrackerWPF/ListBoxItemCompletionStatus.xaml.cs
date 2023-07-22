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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameTracker;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for ListBoxItemCompletionStatus.xaml
    /// </summary>
    public partial class ListBoxItemCompletionStatus : UserControl
    {
        private StatusGame cs;
        public StatusGame CompletionStatus
        {
            get { return cs; }
        }

        public ListBoxItemCompletionStatus(StatusGame cs)
        {
            InitializeComponent();
            this.cs = cs;
            LabelName.Content = cs.Name;
            CheckboxUseAsFinished.IsChecked = cs.UseAsFinished;
            CheckboxHideScore.IsChecked = cs.HideScoreFromList;
            RectangeColor.Fill = new SolidColorBrush(cs.Color.ToMediaColor());
        }
    }
}
