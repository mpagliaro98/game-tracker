using GameTracker;
using GameTracker.Filtering;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public enum FilterType : int
    {
        Game = 1,
        Platform = 2
    }

    [QueryProperty(nameof(FilterTypeParam), nameof(FilterTypeParam))]
    public class FilterViewModel : BaseViewModel
    {
        private FilterType _filterType = FilterType.Game;
        public string FilterTypeParam
        {
            get { return _filterType.ToString(); }
            set 
            { 
                _filterType = Enum.Parse<FilterType>(value);
                InitUI();
            }
        }

        private bool _operatorAnd = true;
        public bool OperatorAnd
        {
            get { return _operatorAnd; }
            set { SetProperty(ref _operatorAnd, value); }
        }

        private ObservableCollection<FilterContainer> _segments = new();
        public ObservableCollection<FilterContainer> FilterSegments
        {
            get => _segments;
            set => SetProperty(ref _segments, value);
        }

        private IEnumerable<IFilterOption> _options;
        public IEnumerable<IFilterOption> FilterOptions
        {
            get { return _options; }
        }

        private IEnumerable<FilterTextType> _textTypes;
        public IEnumerable<FilterTextType> FilterTextTypes
        {
            get { return _textTypes; }
        }

        private IEnumerable<FilterDatePreset> _datePresets;
        public IEnumerable<FilterDatePreset> FilterDatePresets
        {
            get { return _datePresets; }
        }

        private IEnumerable<FilterDateType> _dateTypes;
        public IEnumerable<FilterDateType> FilterDateTypes
        {
            get { return _dateTypes; }
        }

        private IEnumerable<FilterNumericType> _numericTypes;
        public IEnumerable<FilterNumericType> FilterNumericTypes
        {
            get { return _numericTypes; }
        }

        public Command AddItemCommand { get; }
        public Command SearchCommand { get; }
        public Command ClearCommand { get; }
        public Command RemoveSegmentCommand { get; }
        public Command SaveSearchCommand { get; }
        public Command LoadSavedSearchCommand { get; }

        public FilterViewModel(IServiceProvider provider) : base(provider)
        {
            AddItemCommand = new Command(OnAddItem, CanAddSegments);
            SearchCommand = new Command(OnSearch);
            ClearCommand = new Command(OnClear);
            RemoveSegmentCommand = new Command<int>(OnRemoveSegment);
            SaveSearchCommand = new Command(OnSaveSearch);
            LoadSavedSearchCommand = new Command(OnLoadSavedSearch);
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();
        }

        private void OnAddItem()
        {
            var segment = new FilterContainer(FilterSegments.Count);
            FilterSegments.Add(segment);

            // set default values
            if (_filterType == FilterType.Game)
                segment.FilterOption = new FilterOptionModelName();
            else
                segment.FilterOption = new FilterOptionPlatformName();
            segment.FilterTextType = FilterTextType.Contains;
            if (segment.ListValues.Count > 0) segment.ListSelectedValue = segment.ListValues[0];
            segment.FilterDatePreset = FilterDatePreset.None;
            segment.FilterDateType = FilterDateType.Between;
            segment.FilterNumericType = FilterNumericType.GreaterThan;

            // triggers validation check for add button
            OnPropertyChanged(nameof(FilterSegments));
        }

        private async void OnSearch()
        {
            var engine = UIValuesToFilterEngine();
            if (_filterType == FilterType.Game)
                SavedState.FilterGames = engine;
            else
                SavedState.FilterPlatforms = engine;
            if (SavedState.FilterGames.Filters.Exists(s => s.FilterOption.Equals(new FilterOptionGameCompilations()) && !s.Negate))
                SavedState.ShowCompilations = true;
            SavedState.Save(PathController);

            if (_filterType == FilterType.Game)
                await Shell.Current.GoToAsync($"..?{nameof(GamesViewModel.FromFilterPage)}={true}");
            else
                await Shell.Current.GoToAsync($"..?{nameof(PlatformsViewModel.FromFilterPage)}={true}");
        }

        private FilterEngine UIValuesToFilterEngine()
        {
            var engine = new FilterEngine();
            engine.Operator = OperatorAnd ? FilterOperator.And : FilterOperator.Or;
            foreach (var segment in FilterSegments)
            {
                var newSegment = new FilterSegment()
                {
                    Module = Module,
                    Settings = Settings,
                    FilterOption = ((FilterOptionBase)segment.FilterOption).Copy(),
                    Negate = segment.Negate,
                    FilterValues = segment.FilterOption.FilterType switch
                    {
                        RatableTracker.ListManipulation.Filtering.FilterType.Text => segment.FilterValuesText,
                        RatableTracker.ListManipulation.Filtering.FilterType.Boolean => null,
                        RatableTracker.ListManipulation.Filtering.FilterType.Date => segment.FilterValuesDate,
                        RatableTracker.ListManipulation.Filtering.FilterType.List => segment.FilterValuesList,
                        RatableTracker.ListManipulation.Filtering.FilterType.Numeric => segment.FilterValuesNumeric,
                        _ => throw new NotImplementedException()
                    }
                };
                engine.Filters.Add(newSegment);
            }
            return engine;
        }

        private async void OnClear()
        {
            if (_filterType == FilterType.Game)
                SavedState.FilterGames = new FilterEngine();
            else
                SavedState.FilterPlatforms = new FilterEngine();
            SavedState.Save(PathController);

            await Shell.Current.GoToAsync("..");
        }

        private void OnRemoveSegment(int index)
        {
            FilterSegments.RemoveAt(index);
            for (int i = 0; i < FilterSegments.Count; i++)
            {
                FilterSegments[i].Index = i;
            }
            OnPropertyChanged(nameof(FilterSegments)); // triggers validation check for add button
        }

        private async void OnSaveSearch()
        {
            if ((_filterType == FilterType.Game ? SavedState.GameSavedSearches : SavedState.PlatformSavedSearches).Count >= 30)
            {
                await AlertService.DisplayAlertAsync("Limit Reached", "You can keep up to 30 saved searches at a time.");
                return;
            }

            string name = await AlertService.DisplayInputAsync("Save Search", "Enter a name for this saved search", "New Saved Search");
            if (name != null && name.Trim().Length > 0)
            {
                var engine = UIValuesToFilterEngine();
                engine.SearchName = name.Trim();
                if (_filterType == FilterType.Game)
                    SavedState.GameSavedSearches.Add(engine);
                else
                    SavedState.PlatformSavedSearches.Add(engine);
                SavedState.Save(PathController);
                await ToastService.ShowToastAsync("Successfully saved your search criteria");
            }
        }

        private async void OnLoadSavedSearch()
        {
            List<PopupListOption> options = (_filterType == FilterType.Game ? SavedState.GameSavedSearches : SavedState.PlatformSavedSearches).Select(e => new PopupListOption(e, e.SearchName)).ToList();
            var ret = await UtilMAUI.ShowPopupListAsync("Saved Searches", options, null, OnSavedSearchDelete);
            if (ret != null && ret.Item1 == PopupList.EnumOutputType.Selection)
            {
                if (ret.Item2 is null)
                    return;
                else
                    BindFilters((FilterEngine)ret.Item2);
            }
        }

        private async void OnSavedSearchDelete(PopupListOption item, int index)
        {
            if (await AlertService.DisplayConfirmationAsync("Delete", "Are you sure you would like to delete this saved search?"))
            {
                if (_filterType == FilterType.Game)
                    SavedState.GameSavedSearches.RemoveAt(index);
                else
                    SavedState.PlatformSavedSearches.RemoveAt(index);
                SavedState.Save(PathController);
            }
        }

        private bool CanAddSegments()
        {
            return FilterSegments.Count < 100;
        }

        private void InitUI()
        {
            if (_filterType == FilterType.Game)
                _options = FilterEngine.GetFilterOptionList<GameObject>(Module, Settings, new List<Type>() { typeof(FilterOptionModelRank), typeof(FilterOptionModelComment) });
            else
                _options = FilterEngine.GetFilterOptionList<GameTracker.Platform>(Module, Settings);
            _textTypes = Enum.GetValues<FilterTextType>().OrderBy(e => e.ToDisplayString()).ToList();
            _datePresets = Enum.GetValues<FilterDatePreset>().OrderBy(e => e.ToDisplayString()).ToList();
            _dateTypes = Enum.GetValues<FilterDateType>().OrderBy(e => e.ToDisplayString()).ToList();
            _numericTypes = Enum.GetValues<FilterNumericType>().OrderBy(e => e.ToDisplayString()).ToList();

            // load current filters
            var engine = _filterType == FilterType.Game ? SavedState.FilterGames : SavedState.FilterPlatforms;
            BindFilters(engine);
        }

        private void BindFilters(FilterEngine engine)
        {
            FilterSegments.Clear();
            OperatorAnd = (engine.Operator == FilterOperator.And);
            foreach (var filter in SetDefaultFilterRow(engine.Filters, _filterType))
            {
                var segment = new FilterContainer(FilterSegments.Count);
                FilterSegments.Add(segment);

                segment.FilterOption = filter.FilterOption;
                segment.Negate = filter.Negate;
                switch (filter.FilterOption.FilterType)
                {
                    case RatableTracker.ListManipulation.Filtering.FilterType.Text:
                        List<string> values = (List<string>)filter.FilterValues;
                        segment.FilterTextType = Enum.Parse<FilterTextType>(values[0]);
                        segment.TextValue = values[1];
                        break;
                    case RatableTracker.ListManipulation.Filtering.FilterType.List:
                        string value = (string)filter.FilterValues;
                        var uniqueID = UniqueID.Parse(value);
                        var selectedValue = segment.ListValues.Find(v => v.Key.Equals(uniqueID));
                        segment.ListSelectedValue = selectedValue;
                        break;
                    case RatableTracker.ListManipulation.Filtering.FilterType.Date:
                        List<string> valuesDate = (List<string>)filter.FilterValues;
                        segment.FilterDateType = Enum.Parse<FilterDateType>(valuesDate[0]);
                        segment.FilterDatePreset = Enum.Parse<FilterDatePreset>(valuesDate[1]);
                        segment.DateValue1 = valuesDate[2].Length > 0 ? DateTime.Parse(valuesDate[2]) : DateTime.Today;
                        segment.DateValue2 = valuesDate[3].Length > 0 ? DateTime.Parse(valuesDate[3]) : DateTime.Today;
                        break;
                    case RatableTracker.ListManipulation.Filtering.FilterType.Boolean:
                        break;
                    case RatableTracker.ListManipulation.Filtering.FilterType.Numeric:
                        List<string> valuesNum = (List<string>)filter.FilterValues;
                        segment.FilterNumericType = Enum.Parse<FilterNumericType>(valuesNum[0]);
                        segment.NumberValue1 = ((IFilterOptionNumeric)filter.FilterOption).NumberFormat == FilterNumberFormat.Percentage ? double.Parse(valuesNum[1]) * 100 : double.Parse(valuesNum[1]);
                        segment.NumberValue2 = ((IFilterOptionNumeric)filter.FilterOption).NumberFormat == FilterNumberFormat.Percentage ? double.Parse(valuesNum[2]) * 100 : double.Parse(valuesNum[2]);
                        break;
                    default:
                        break;
                }
            }
        }

        private List<FilterSegment> SetDefaultFilterRow(List<FilterSegment> filters, FilterType filterType)
        {
            if (filters.Count <= 0)
            {
                return filterType switch
                {
                    FilterType.Game => new List<FilterSegment>() { new FilterSegment() { FilterOption = new FilterOptionModelName() { Module = Module, Settings = Settings }, FilterValues = new List<string>() { FilterTextType.Contains.ToString(), "" }, Module = Module, Settings = Settings } },
                    FilterType.Platform => new List<FilterSegment>() { new FilterSegment() { FilterOption = new FilterOptionPlatformName() { Module = Module, Settings = Settings }, FilterValues = new List<string>() { FilterTextType.Contains.ToString(), "" }, Module = Module, Settings = Settings } },
                    _ => throw new NotImplementedException()
                };
            }
            else
                return filters;
        }
    }
}
