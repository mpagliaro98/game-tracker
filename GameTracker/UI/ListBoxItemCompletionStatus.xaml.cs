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
using RatableTracker.Framework;

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for ListBoxItemCompletionStatus.xaml
    /// </summary>
    public partial class ListBoxItemCompletionStatus : UserControl
    {
        private CompletionStatus cs;
        public CompletionStatus CompletionStatus
        {
            get { return cs; }
        }

        public ListBoxItemCompletionStatus(CompletionStatus cs)
        {
            InitializeComponent();
            this.cs = cs;
            LabelName.Content = cs.Name;
            CheckboxUseAsFinished.IsChecked = cs.UseAsFinished;
            CheckboxExcludeFromStats.IsChecked = cs.ExcludeFromStats;
        }
    }
}
