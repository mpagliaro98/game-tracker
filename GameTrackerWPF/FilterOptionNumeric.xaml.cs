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
    /// Interaction logic for FilterOptionNumeric.xaml
    /// </summary>
    public partial class FilterOptionNumeric : UserControl, IFilterOptionControl
    {
        public object FilterValues => new List<string>() { ((FilterNumericType)ComboBoxType.SelectedValue).ToString(), CleanInput(EntryValue1.Value ?? 0).ToString(), CleanInput(EntryValue2.Value ?? 0).ToString() };

        private IFilterOptionNumeric option;

        public FilterOptionNumeric(IFilterOptionNumeric option, object initialValue)
        {
            InitializeComponent();
            this.option = option;
            FillComboBox();
            ComboBoxType.SelectionChanged += ComboBoxType_SelectionChanged;
            if (initialValue != null) SetInitialValues((List<string>)initialValue);
        }

        private void FillComboBox()
        {
            ComboBoxType.Items.Clear();
            foreach (FilterNumericType numericType in Enum.GetValues<FilterNumericType>().OrderBy(o => o.ToString()).ToList())
            {
                ComboBoxType.Items.Add(new FilterNumericComboBoxItem(numericType));
            }
            ComboBoxType.SelectedValue = FilterNumericType.GreaterThan;
            EntryValue2.Visibility = Visibility.Collapsed;
        }

        private void SetInitialValues(List<string> values)
        {
            ComboBoxType.SelectedValue = (FilterNumericType)Enum.Parse(typeof(FilterNumericType), values[0]);
            EntryValue1.Value = double.Parse(values[1]);
            EntryValue2.Value = double.Parse(values[2]);
        }

        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EntryValue2.Visibility = (FilterNumericType)ComboBoxType.SelectedValue == FilterNumericType.Between ? Visibility.Visible : Visibility.Collapsed;
        }

        private double CleanInput(double input)
        {
            return option.NumberFormat switch
            {
                FilterNumberFormat.Decimal => input,
                FilterNumberFormat.Integer => Math.Floor(input),
                FilterNumberFormat.Percentage => input / 100,
                _ => throw new NotImplementedException("Invalid number format: " + option.NumberFormat.ToDisplayString())
            };
        }

        public sealed class FilterNumericComboBoxItem
        {
            public string DisplayText => Value.ToDisplayString();
            public FilterNumericType Value { get; init; }

            public FilterNumericComboBoxItem(FilterNumericType value)
            {
                Value = value;
            }
        }
    }
}
