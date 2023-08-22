using GameTracker;
using RatableTracker.Interfaces;
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
    /// Interaction logic for FilterRow.xaml
    /// </summary>
    public partial class FilterRow : UserControl
    {
        public FilterOptionBase FilterOption => (FilterOptionBase)ComboBoxOption.SelectedItem;
        public object FilterValues => FilterOptionControl.FilterValues;
        public bool Negate => CheckBoxNegate.IsChecked.Value;
        public int Index { get; set; }

        private IFilterOptionControl FilterOptionControl { get; set; }

        public event EventHandler<FilterRowRemoveEventArgs> Remove;

        public FilterRow(GameModule module, SettingsGame settings, IList<IFilterOption> filterOptions, int index, FilterOptionBase initialValue) : this(null, module, settings, filterOptions, index, initialValue) { }

        public FilterRow(FilterSegment segment, GameModule module, SettingsGame settings, IList<IFilterOption> filterOptions, int index, FilterOptionBase initialValue)
        {
            InitializeComponent();
            Index = index;
            FillComboBox(module, settings, segment == null ? initialValue : segment.FilterOption, filterOptions);
            RefreshFilterOptionControl(segment?.FilterValues);

            if (segment != null)
                CheckBoxNegate.IsChecked = segment.Negate;

            ComboBoxOption.SelectionChanged += ComboBoxOption_SelectionChanged;
        }

        private void FillComboBox(GameModule module, SettingsGame settings, FilterOptionBase initialOption, IList<IFilterOption> filterOptions)
        {
            ComboBoxOption.Items.Clear();
            foreach (var item in filterOptions)
            {
                ComboBoxOption.Items.Add(item);
            }
            if (initialOption != null)
                ComboBoxOption.SelectedItem = initialOption;
            else
                ComboBoxOption.SelectedIndex = 0;
        }

        private void ComboBoxOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshFilterOptionControl(null);
        }

        private void RefreshFilterOptionControl(object initialValues)
        {
            var option = (IFilterOption)ComboBoxOption.SelectedItem;
            if (option == null)
            {
                option = (IFilterOption)ComboBoxOption.Items[0];
                ComboBoxOption.SelectedIndex = 0;
                initialValues = null;
            }
            if (GridMain.Children.Count >= 4) GridMain.Children.RemoveAt(3);
            UIElement control = option.FilterType switch
            {
                FilterType.Text => new FilterOptionText((IFilterOptionText)option, initialValues),
                FilterType.Boolean => new FilterOptionBoolean((IFilterOptionBoolean)option),
                FilterType.List => new FilterOptionList((IFilterOptionList)option, initialValues),
                FilterType.Numeric => new FilterOptionNumeric((IFilterOptionNumeric)option, initialValues),
                FilterType.Date => new FilterOptionDate((IFilterOptionDate)option, initialValues),
                _ => throw new NotImplementedException("Unsupported filter type: " + option.FilterType.ToString()),
            };
            Grid.SetColumn(control, 1);
            GridMain.Children.Add(control);
            FilterOptionControl = (IFilterOptionControl)control;
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            Remove?.Invoke(this, new FilterRowRemoveEventArgs() { Index = Index });
        }
    }

    public sealed class FilterRowRemoveEventArgs
    {
        public int Index { get; set; }
    }
}
