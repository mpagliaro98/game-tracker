using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GameTrackerMAUI
{
    public class FilterContainer : INotifyPropertyChanged
    {
        private IFilterOption _filterOption = null;
        private bool _negate = false;
        private int _index = 0;
        private FilterTextType? _filterTextType = null;
        private string _textValue = "";
        private FilterListOption<UniqueID, string> _listValue = null;
        private FilterDatePreset? _filterDatePreset = null;
        private FilterDateType? _filterDateType = null;
        private DateTime? _date1 = DateTime.Today;
        private DateTime? _date2 = DateTime.Today;
        private FilterNumericType? _filterNumericType = null;
        private double? _value1 = null;
        private double? _value2 = null;

        public IFilterOption FilterOption
        {
            get { return _filterOption; }
            set
            {
                SetProperty(ref _filterOption, value);
                OnPropertyChanged(nameof(BooleanDisplayText));
                OnPropertyChanged(nameof(ListValues));
                if (ListValues.Count > 0) ListSelectedValue = ListValues[0];
                OnPropertyChanged(nameof(NumberFormat));
            }
        }

        public List<string> FilterValuesText => new() { FilterTextType.ToString(), TextValue };
        public string FilterValuesList => ListSelectedValue == null ? ListValues[0].Key.ToString() : ListSelectedValue.Key.ToString();
        public List<string> FilterValuesDate => new() { FilterDateType.ToString(), FilterDatePreset.ToString(), DateValue1.ToString(), DateValue2.ToString() };
        public List<string> FilterValuesNumeric => new() { FilterNumericType.ToString(), CleanInput(NumberValue1 ?? 0).ToString(), CleanInput(NumberValue2 ?? 0).ToString() };

        public bool Negate
        {
            get { return _negate; }
            set { SetProperty(ref _negate, value); }
        }

        public int Index
        {
            get { return _index; }
            internal set { SetProperty(ref _index, value); }
        }

        public string BooleanDisplayText
        {
            get
            {
                if (_filterOption is not IFilterOptionBoolean option) return "";
                return option.DisplayText;
            }
        }

        public List<FilterListOption<UniqueID, string>> ListValues
        {
            get
            {
                if (_filterOption is not IFilterOptionList option) return new();
                return option.ListValues.Select(val => new FilterListOption<UniqueID, string>(val.Key, val.Value)).ToList();
            }
        }

        public FilterNumberFormat NumberFormat
        {
            get
            {
                if (_filterOption is not IFilterOptionNumeric option) return FilterNumberFormat.Decimal;
                return option.NumberFormat;
            }
        }

        public FilterTextType? FilterTextType
        {
            get { return _filterTextType; }
            set { SetProperty(ref _filterTextType, value); }
        }

        public string TextValue
        {
            get { return _textValue; }
            set { SetProperty(ref _textValue, value); }
        }

        public FilterListOption<UniqueID, string> ListSelectedValue
        {
            get { return _listValue; }
            set { SetProperty(ref _listValue, value); }
        }

        public FilterDatePreset? FilterDatePreset
        {
            get { return _filterDatePreset; }
            set { SetProperty(ref _filterDatePreset, value); }
        }

        public FilterDateType? FilterDateType
        {
            get { return _filterDateType; }
            set { SetProperty(ref _filterDateType, value); }
        }

        public DateTime? DateValue1
        {
            get { return _date1; }
            set { SetProperty(ref _date1, value); }
        }

        public DateTime? DateValue2
        {
            get { return _date2; }
            set { SetProperty(ref _date2, value); }
        }

        public FilterNumericType? FilterNumericType
        {
            get { return _filterNumericType; }
            set { SetProperty(ref _filterNumericType, value); }
        }

        public double? NumberValue1
        {
            get { return _value1; }
            set { SetProperty(ref _value1, value); }
        }

        public double? NumberValue2
        {
            get { return _value2; }
            set { SetProperty(ref _value2, value); }
        }

        public FilterContainer(int index)
        {
            _index = index;
        }

        private double CleanInput(double input)
        {
            return ((IFilterOptionNumeric)FilterOption).NumberFormat switch
            {
                FilterNumberFormat.Decimal => input,
                FilterNumberFormat.Integer => Math.Floor(input),
                FilterNumberFormat.Percentage => (double)input / 100,
                _ => throw new NotImplementedException("Invalid number format: " + ((IFilterOptionNumeric)FilterOption).NumberFormat.ToDisplayString())
            };
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class FilterListOption<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public FilterListOption(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"[{Key}, {Value}]";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not FilterListOption<TKey, TValue> other) return false;
            return Key.Equals(other.Key) && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() & Value.GetHashCode();
        }
    }
}
