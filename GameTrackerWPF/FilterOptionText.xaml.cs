using GameTracker;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
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
    /// Interaction logic for FilterOptionText.xaml
    /// </summary>
    public partial class FilterOptionText : UserControl, IFilterOptionControl
    {
        public object FilterValues => new List<string>() { ((FilterTextComboBoxItem)ComboBoxTextType.SelectedItem).Value.ToString(), TextBoxValue.Text.Trim() };

        public FilterOptionText(IFilterOptionText option, object initialValue)
        {
            InitializeComponent();
            FillComboBox();
            ComboBoxTextType.SelectionChanged += ComboBoxTextType_SelectionChanged;
            if (initialValue != null) SetInitialValues((List<string>)initialValue);
        }

        private void FillComboBox()
        {
            ComboBoxTextType.Items.Clear();
            foreach (FilterTextType textType in Enum.GetValues<FilterTextType>().OrderBy(o => o.ToString()).ToList())
            {
                ComboBoxTextType.Items.Add(new FilterTextComboBoxItem(textType));
            }
            ComboBoxTextType.SelectedValue = FilterTextType.Contains;
        }

        private void SetInitialValues(List<string> values)
        {
            ComboBoxTextType.SelectedValue = (FilterTextType)Enum.Parse(typeof(FilterTextType), values[0]);
            TextBoxValue.Text = values[1];
        }

        private void ComboBoxTextType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxValue.Visibility = (FilterTextType)ComboBoxTextType.SelectedValue == FilterTextType.IsEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        public sealed class FilterTextComboBoxItem
        {
            public string DisplayText => Value.ToDisplayString();
            public FilterTextType Value { get; init; }

            public FilterTextComboBoxItem(FilterTextType value)
            {
                Value = value;
            }
        }
    }
}
