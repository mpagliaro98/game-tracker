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
    /// Interaction logic for FilterOptionDate.xaml
    /// </summary>
    public partial class FilterOptionDate : UserControl, IFilterOptionControl
    {
        public object FilterValues => new List<string>() { ((FilterDateType)ComboBoxType.SelectedValue).ToString(), ((FilterDatePreset)ComboBoxPreset.SelectedValue).ToString(), (DatePicker1.SelectedDate ?? DateTime.MinValue).ToString(), (DatePicker2.SelectedDate ?? DateTime.MinValue).ToString() };

        public FilterOptionDate(IFilterOptionDate option, object initialValue)
        {
            InitializeComponent();
            FillComboBoxPreset();
            FillComboBoxType();
            ComboBoxPreset.SelectionChanged += ComboBoxPreset_SelectionChanged;
            ComboBoxType.SelectionChanged += ComboBoxType_SelectionChanged;
            RefreshVisibility();
            if (initialValue != null) SetInitialValues((List<string>)initialValue);
        }

        private void FillComboBoxPreset()
        {
            ComboBoxPreset.Items.Clear();
            foreach (FilterDatePreset datePreset in Enum.GetValues<FilterDatePreset>().OrderBy(o => o.ToDisplayString()).ToList())
            {
                ComboBoxPreset.Items.Add(new FilterDatePresetComboBoxItem(datePreset));
            }
            ComboBoxPreset.SelectedValue = FilterDatePreset.None;
        }

        private void FillComboBoxType()
        {
            ComboBoxType.Items.Clear();
            foreach (FilterDateType dateType in Enum.GetValues<FilterDateType>().OrderBy(o => o.ToDisplayString()).ToList())
            {
                ComboBoxType.Items.Add(new FilterDateTypeComboBoxItem(dateType));
            }
            ComboBoxType.SelectedValue = FilterDateType.Between;
        }

        private void SetInitialValues(List<string> values)
        {
            ComboBoxType.SelectedValue = (FilterDateType)Enum.Parse(typeof(FilterDateType), values[0]);
            ComboBoxPreset.SelectedValue = (FilterDatePreset)Enum.Parse(typeof(FilterDatePreset), values[1]);
            DatePicker1.SelectedDate = DateTime.Parse(values[2]);
            DatePicker2.SelectedDate = DateTime.Parse(values[3]);
        }

        private void ComboBoxPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshVisibility();
        }

        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            ComboBoxType.Visibility = (FilterDatePreset)ComboBoxPreset.SelectedValue == FilterDatePreset.None ? Visibility.Visible : Visibility.Collapsed;
            DatePicker1.Visibility = (FilterDatePreset)ComboBoxPreset.SelectedValue == FilterDatePreset.None ? Visibility.Visible : Visibility.Collapsed;
            DatePicker2.Visibility = (FilterDatePreset)ComboBoxPreset.SelectedValue == FilterDatePreset.None && (FilterDateType)ComboBoxType.SelectedValue == FilterDateType.Between ? Visibility.Visible : Visibility.Collapsed;
        }

        public sealed class FilterDatePresetComboBoxItem
        {
            public string DisplayText => Value.ToDisplayString();
            public FilterDatePreset Value { get; init; }

            public FilterDatePresetComboBoxItem(FilterDatePreset value)
            {
                Value = value;
            }
        }

        public sealed class FilterDateTypeComboBoxItem
        {
            public string DisplayText => Value.ToDisplayString();
            public FilterDateType Value { get; init; }

            public FilterDateTypeComboBoxItem(FilterDateType value)
            {
                Value = value;
            }
        }
    }
}