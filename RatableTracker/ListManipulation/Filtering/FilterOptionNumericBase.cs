using RatableTracker.Exceptions;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.ListManipulation.Filtering
{
    public enum FilterNumberFormat : byte
    {
        Decimal = 1,
        Integer = 2,
        Percentage = 3
    }

    public enum FilterNumericType : byte
    {
        [EnumDisplay(Name = "Greater Than")] GreaterThan = 1,
        [EnumDisplay(Name = "Greater Than / Equal To")] GreaterThanOrEqualTo = 2,
        [EnumDisplay(Name = "Less Than")] LessThan = 3,
        [EnumDisplay(Name = "Less Than / Equal To")] LessThanOrEqualTo = 4,
        [EnumDisplay(Name = "Equal To")] EqualTo = 5,
        Between = 6
    }

    public abstract class FilterOptionNumericBase<T> : FilterOptionBase, IFilterOptionNumeric, IFilterOptionAction<T>
    {
        public override FilterType FilterType => FilterType.Numeric;
        public virtual FilterNumberFormat NumberFormat => FilterNumberFormat.Decimal;

        protected FilterNumericType NumericType { get; private set; }
        protected double Value1 { get; private set; }
        protected double Value2 { get; private set; }

        public FilterOptionNumericBase() : base() { }

        protected internal override void ValidateFilterValues(object filterValues)
        {
            // validate filter values, throw exception if invalid, then fill protected properties with filter values
            if (filterValues is not List<string> listValues || listValues.Count != 3)
                throw new ListManipulationException("Invalid filter value, numeric filters expect three values", filterValues);
            if (!Enum.TryParse(typeof(FilterNumericType), listValues[0], out object numericType))
                throw new ListManipulationException("Invalid numeric type", numericType);
            NumericType = (FilterNumericType)numericType;
            if (!double.TryParse(listValues[1], out double value1))
                throw new ListManipulationException("Invalid filter value, value 1 is not a number", listValues[1]);
            double value2 = 0;
            if (NumericType == FilterNumericType.Between && !double.TryParse(listValues[2], out value2))
                throw new ListManipulationException("Invalid filter value, value 2 is not a number", listValues[2]);

            Value1 = value1;
            Value2 = value2;
        }

        public Func<T, bool> GenerateFilterExpression()
        {
            return NumericType switch
            {
                FilterNumericType.GreaterThan => (obj) => GetComparisonValue(obj) > Value1,
                FilterNumericType.GreaterThanOrEqualTo => (obj) => GetComparisonValue(obj) >= Value1,
                FilterNumericType.LessThan => (obj) => GetComparisonValue(obj) < Value1,
                FilterNumericType.LessThanOrEqualTo => (obj) => GetComparisonValue(obj) <= Value1,
                FilterNumericType.EqualTo => (obj) => GetComparisonValue(obj) == Value1,
                FilterNumericType.Between => (obj) => GetComparisonValue(obj) >= Value1 && GetComparisonValue(obj) < Value2,
                _ => throw new ListManipulationException("Invalid filter text type: " + NumericType.ToString(), NumericType),
            };
        }

        protected abstract double GetComparisonValue(T obj);
    }
}
