using GameTracker;
using GameTracker.Filtering;
using GameTrackerMAUI.Services;
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

        public FilterViewModel(IServiceProvider provider) : base(provider)
        {
            AddItemCommand = new Command(OnAddItem, CanAddSegments);
            SearchCommand = new Command(OnSearch);
            ClearCommand = new Command(OnClear);
            RemoveSegmentCommand = new Command<int>(OnRemoveSegment);
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
            var engine = new FilterEngine();
            engine.Operator = OperatorAnd ? FilterOperator.And : FilterOperator.Or;
            foreach (var segment in FilterSegments)
            {
                var newSegment = new FilterSegment()
                {
                    Module = Module,
                    Settings = Settings,
                    FilterOption = (FilterOptionBase)segment.FilterOption,
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
            if (_filterType == FilterType.Game)
                SavedState.FilterGames = engine;
            else
                SavedState.FilterPlatforms = engine;
            SavedState.Save(PathController);

            if (_filterType == FilterType.Game)
                await Shell.Current.GoToAsync($"..?{nameof(GamesViewModel.FromFilterPage)}={true}");
            else
                await Shell.Current.GoToAsync($"..?{nameof(PlatformsViewModel.FromFilterPage)}={true}");
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

        private bool CanAddSegments()
        {
            return FilterSegments.Count < 100;
        }

        private void InitUI()
        {
            if (_filterType == FilterType.Game)
                _options = FilterEngine.GetFilterOptionList<GameObject>(Module, Settings);
            else
                _options = FilterEngine.GetFilterOptionList<GameTracker.Platform>(Module, Settings);
            _textTypes = Enum.GetValues<FilterTextType>().OrderBy(e => e.ToDisplayString()).ToList();
            _datePresets = Enum.GetValues<FilterDatePreset>().OrderBy(e => e.ToDisplayString()).ToList();
            _dateTypes = Enum.GetValues<FilterDateType>().OrderBy(e => e.ToDisplayString()).ToList();
            _numericTypes = Enum.GetValues<FilterNumericType>().OrderBy(e => e.ToDisplayString()).ToList();

            // load current filters
            var engine = _filterType == FilterType.Game ? SavedState.FilterGames : SavedState.FilterPlatforms;
            OperatorAnd = (engine.Operator == FilterOperator.And);
            foreach (var filter in engine.Filters)
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
                        segment.NumberValue1 = double.Parse(valuesNum[1]);
                        segment.NumberValue2 = double.Parse(valuesNum[2]);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
