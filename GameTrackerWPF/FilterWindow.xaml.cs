using GameTracker;
using GameTracker.Filtering;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using RatableTracker.Interfaces;
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
    public partial class FilterWindow : MetroWindow
    {
        public event EventHandler<FilterWindowSearchEventArgs> Search;

        private GameModule module;
        private SettingsGame settings;
        private FilterMode filterMode;
        private IList<IFilterOption> filterOptions;
        private SavedState savedState;
        private IPathController pathController;

        public FilterWindow(FilterEngine filterEngine, GameModule module, SettingsGame settings, FilterMode filterType, SavedState savedState, IPathController pathController)
        {
            InitializeComponent();
            this.module = module;
            this.settings = settings;
            this.savedState = savedState;
            this.pathController = pathController;
            filterMode = filterType;
            filterOptions = GetFilterOptionList(module, settings, filterType);
            if (filterEngine.Operator == FilterOperator.And) RadioAnd.IsChecked = true;
            if (filterEngine.Operator == FilterOperator.Or) RadioOr.IsChecked = true;
            BindFilterList(SetDefaultFilterRow(filterEngine.Filters, filterType));
        }

        private IList<IFilterOption> GetFilterOptionList(GameModule module, SettingsGame settings, FilterMode filterType)
        {
            return filterType switch
            {
                FilterMode.Game => FilterEngine.GetFilterOptionList<GameObject>(module, settings, new List<Type>() { typeof(FilterOptionModelRank), typeof(FilterOptionModelComment) }),
                FilterMode.Platform => FilterEngine.GetFilterOptionList<Platform>(module, settings),
                _ => throw new NotImplementedException()
            };
        }

        private void BindFilterList(IList<FilterSegment> filters)
        {
            ListBoxFilters.Items.Clear();
            foreach (var filter in filters)
            {
                var newRow = new FilterRow(filter, module, settings, filterOptions, ListBoxFilters.Items.Count, null);
                newRow.Remove += FilterRow_Remove;
                ListBoxFilters.Items.Add(newRow);
            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFilters.Items.Count >= 100) return;
            var newRow = new FilterRow(module, settings, filterOptions, ListBoxFilters.Items.Count, filterMode == FilterMode.Game ? new FilterOptionModelName() : new FilterOptionPlatformName());
            newRow.Remove += FilterRow_Remove;
            ListBoxFilters.Items.Add(newRow);
            if (ListBoxFilters.Items.Count >= 100) ButtonNew.IsEnabled = false;
        }

        private void FilterRow_Remove(object sender, FilterRowRemoveEventArgs e)
        {
            ListBoxFilters.Items.RemoveAt(e.Index);
            for (int i = 0; i < ListBoxFilters.Items.Count; i++)
            {
                ((FilterRow)ListBoxFilters.Items[i]).Index = i;
            }
            ButtonNew.IsEnabled = true;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            var filterEngine = UIValuesToFilterEngine();

            Search?.Invoke(this, new FilterWindowSearchEventArgs() { FilterEngine = filterEngine });
            Close();
        }

        private FilterEngine UIValuesToFilterEngine()
        {
            FilterEngine filterEngine = new()
            {
                Operator = RadioAnd.IsChecked.Value ? FilterOperator.And : FilterOperator.Or
            };
            foreach (FilterRow row in ListBoxFilters.Items)
            {
                var filter = new FilterSegment()
                {
                    FilterOption = row.FilterOption.Copy(),
                    FilterValues = row.FilterValues,
                    Negate = row.Negate,
                    Module = module,
                    Settings = settings
                };
                filterEngine.Filters.Add(filter);
            }
            return filterEngine;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Search?.Invoke(this, new FilterWindowSearchEventArgs() { FilterEngine = new FilterEngine() });
            Close();
        }

        private List<FilterSegment> SetDefaultFilterRow(List<FilterSegment> filters, FilterMode filterType)
        {
            if (filters.Count <= 0)
            {
                return filterType switch
                {
                    FilterMode.Game => new List<FilterSegment>() { new FilterSegment() { FilterOption = new FilterOptionModelName() { Module = module, Settings = settings }, FilterValues = new List<string>() { FilterTextType.Contains.ToString(), "" }, Module = module, Settings = settings } },
                    FilterMode.Platform => new List<FilterSegment>() { new FilterSegment() { FilterOption = new FilterOptionPlatformName() { Module = module, Settings = settings }, FilterValues = new List<string>() { FilterTextType.Contains.ToString(), "" }, Module = module, Settings = settings } },
                    _ => throw new NotImplementedException()
                };
            }
            else
                return filters;
        }

        private async void ButtonSaveSearch_Click(object sender, RoutedEventArgs e)
        {
            if ((filterMode == FilterMode.Game ? savedState.GameSavedSearches : savedState.PlatformSavedSearches).Count >= 30)
            {
                await this.ShowMessageAsync("Limit Reached", "You can keep up to 30 saved searches at a time.");
                return;
            }

            InputDialog inputDialog = new("Enter a name for this saved search:", "New Saved Search");
            if (inputDialog.ShowDialog() == true && inputDialog.Answer.Trim().Length > 0)
            {
                var engine = UIValuesToFilterEngine();
                engine.SearchName = inputDialog.Answer.Trim();
                if (filterMode == FilterMode.Game)
                    savedState.GameSavedSearches.Add(engine);
                else
                    savedState.PlatformSavedSearches.Add(engine);
                SavedState.SaveSavedState(pathController, savedState);
            }
        }

        private void ButtonLoadSearch_Click(object sender, RoutedEventArgs e)
        {
            ButtonLoadSearch.ContextMenu.Items.Clear();
            int idx = 0;
            foreach (var engine in filterMode == FilterMode.Game ? savedState.GameSavedSearches : savedState.PlatformSavedSearches)
            {
                var item = new MenuItem
                {
                    DisplayMemberPath = "Item2",
                    Header = engine.SearchName
                };
                item.Items.Add(Tuple.Create(engine, "Load", idx));
                item.Items.Add(Tuple.Create<FilterEngine, string, int>(null, "Delete", idx));
                item.Click += Item_Click;
                ButtonLoadSearch.ContextMenu.Items.Add(item);
                idx++;
            }

            var contextMenu = ButtonLoadSearch.ContextMenu;
            contextMenu.PlacementTarget = ButtonLoadSearch;
            contextMenu.IsOpen = true;
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            var src = (Tuple<FilterEngine, string, int>)((MenuItem)e.OriginalSource).Header;
            if (src.Item1 == null)
            {
                if (filterMode == FilterMode.Game)
                    savedState.GameSavedSearches.RemoveAt(src.Item3);
                else
                    savedState.PlatformSavedSearches.RemoveAt(src.Item3);
                SavedState.SaveSavedState(pathController, savedState);
            }
            else
            {
                FilterEngine engine = src.Item1;
                BindFilterList(engine.Filters);
                if (engine.Operator == FilterOperator.And) RadioAnd.IsChecked = true;
                if (engine.Operator == FilterOperator.Or) RadioOr.IsChecked = true;
                item.IsChecked = false;
            }
        }
    }

    public sealed class FilterWindowSearchEventArgs : EventArgs
    {
        public FilterEngine FilterEngine { get; set; }
    }
}
