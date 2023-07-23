using GameTracker;
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
    public enum FilterMode
    {
        Game,
        Platform
    }

    /// <summary>
    /// Interaction logic for FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window
    {
        public event EventHandler<FilterWindowSearchEventArgs> Search;

        private FilterEngine filterEngine;
        private GameModule module;
        private SettingsGame settings;
        private IList<IFilterOption> filterOptions;

        public FilterWindow(FilterEngine filterEngine, GameModule module, SettingsGame settings, FilterMode filterType)
        {
            InitializeComponent();
            this.filterEngine = filterEngine;
            this.module = module;
            this.settings = settings;
            filterOptions = GetFilterOptionList(module, settings, filterType);
            if (filterEngine.Operator == FilterOperator.And) RadioAnd.IsChecked = true;
            if (filterEngine.Operator == FilterOperator.Or) RadioOr.IsChecked = true;
            BindFilterList(filterEngine.Filters);
        }

        private IList<IFilterOption> GetFilterOptionList(GameModule module, SettingsGame settings, FilterMode filterType)
        {
            return filterType switch
            {
                FilterMode.Game => FilterEngine.GetFilterOptionList<GameObject>(module, settings),
                FilterMode.Platform => FilterEngine.GetFilterOptionList<Platform>(module, settings),
                _ => throw new NotImplementedException()
            };
        }

        private void BindFilterList(IList<FilterSegment> filters)
        {
            ListBoxFilters.Items.Clear();
            foreach (var filter in filters)
            {
                var newRow = new FilterRow(filter, module, settings, filterOptions, ListBoxFilters.Items.Count);
                newRow.Remove += FilterRow_Remove;
                ListBoxFilters.Items.Add(newRow);
            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFilters.Items.Count >= 100) return;
            var newRow = new FilterRow(module, settings, filterOptions, ListBoxFilters.Items.Count);
            newRow.Remove += FilterRow_Remove;
            ListBoxFilters.Items.Add(newRow);
            if (ListBoxFilters.Items.Count >= 100) ButtonNew.IsEnabled = false;
        }

        private void FilterRow_Remove(object sender, FilterRowRemoveEventArgs e)
        {
            ListBoxFilters.Items.RemoveAt(e.Index);
            ButtonNew.IsEnabled = true;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterEngine filterEngine = new()
            {
                Operator = RadioAnd.IsChecked.Value ? FilterOperator.And : FilterOperator.Or
            };
            foreach (FilterRow row in ListBoxFilters.Items)
            {
                var filter = new FilterSegment()
                {
                    FilterOption = row.FilterOption,
                    FilterValues = row.FilterValues,
                    Negate = row.Negate
                };
                filterEngine.Filters.Add(filter);
            }

            Search?.Invoke(this, new FilterWindowSearchEventArgs() { FilterEngine = filterEngine });
            Close();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Search?.Invoke(this, new FilterWindowSearchEventArgs() { FilterEngine = new FilterEngine() });
            Close();
        }
    }

    public sealed class FilterWindowSearchEventArgs : EventArgs
    {
        public FilterEngine FilterEngine { get; set; }
    }
}
