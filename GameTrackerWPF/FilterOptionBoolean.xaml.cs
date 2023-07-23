using RatableTracker.ListManipulation.Filtering;
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

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for FilterOptionBoolean.xaml
    /// </summary>
    public partial class FilterOptionBoolean : UserControl, IFilterOptionControl
    {
        public object FilterValues => null;

        public FilterOptionBoolean(IFilterOptionBoolean option)
        {
            InitializeComponent();
            LabelBoolean.Content = option.DisplayText;
        }
    }
}
