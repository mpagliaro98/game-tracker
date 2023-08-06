using RatableTracker.Exceptions;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    public enum FilterTextType : byte
    {
        [EnumDisplay(Name = "Starts With")] StartsWith = 1,
        [EnumDisplay(Name = "Ends With")] EndsWith = 2,
        Contains = 3,
        [EnumDisplay(Name = "Is Empty")] IsEmpty = 4
    }

    public abstract class FilterOptionTextBase<T> : FilterOptionBase, IFilterOptionText, IFilterOptionAction<T>
    {
        public override FilterType FilterType => FilterType.Text;

        protected FilterTextType TextType { get; private set; }
        protected string FilterText { get; private set; }

        public FilterOptionTextBase() : base() { }

        protected internal override void ValidateFilterValues(object filterValues)
        {
            // validate filter values, throw exception if invalid, then fill protected properties with filter values
            if (filterValues is not List<string> listValues || listValues.Count != 2)
                throw new ListManipulationException("Invalid filter value, text filters expect two values", filterValues);

            if (!Enum.TryParse(typeof(FilterTextType), listValues[0], out object textType))
                throw new ListManipulationException("Invalid text type", listValues[0]);

            TextType = (FilterTextType)textType;
            FilterText = listValues[1];
        }

        public Func<T, bool> GenerateFilterExpression()
        {
            return TextType switch
            {
                FilterTextType.StartsWith => (obj) => GetComparisonValue(obj).ToLower().StartsWith(FilterText.ToLower()),
                FilterTextType.EndsWith => (obj) => GetComparisonValue(obj).ToLower().EndsWith(FilterText.ToLower()),
                FilterTextType.Contains => (obj) => GetComparisonValue(obj).ToLower().Contains(FilterText.ToLower()),
                FilterTextType.IsEmpty => (obj) => GetComparisonValue(obj).Length <= 0,
                _ => throw new ListManipulationException("Invalid filter text type: " + TextType.ToString(), TextType),
            };
        }

        protected abstract string GetComparisonValue(T obj);
    }
}
