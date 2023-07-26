using RatableTracker.Exceptions;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.ListManipulation.Filtering
{
    public enum FilterDateType : byte
    {
        Before = 1,
        After = 2,
        Between = 3
    }

    public enum FilterDatePreset : byte
    {
        [EnumDisplay(Name = "Custom Date Range")] None = 0,
        [EnumDisplay(Name = "Past Day")] PastDay = 1,
        [EnumDisplay(Name = "Past Week")] PastWeek = 2,
        [EnumDisplay(Name = "Past Month")] PastMonth = 3,
        [EnumDisplay(Name = "Past Year")] PastYear = 4,
        [EnumDisplay(Name = "Today")] Today = 5,
        [EnumDisplay(Name = "This Week")] ThisWeek = 6,
        [EnumDisplay(Name = "This Month")] ThisMonth = 7,
        [EnumDisplay(Name = "This Year")] ThisYear = 8,
        [EnumDisplay(Name = "Next Week")] NextWeek = 9,
        [EnumDisplay(Name = "Next Month")] NextMonth = 10,
        [EnumDisplay(Name = "Next Year")] NextYear = 11
    }

    public abstract class FilterOptionDateBase<T> : FilterOptionBase, IFilterOptionDate, IFilterOptionAction<T>
    {
        public override FilterType FilterType => FilterType.Date;

        protected FilterDateType DateType { get; private set; }
        protected FilterDatePreset DatePreset { get; private set; }
        protected DateTime FilterDate1 { get; private set; }
        protected DateTime FilterDate2 { get; private set; }

        public FilterOptionDateBase() : base() { }

        protected internal override void ValidateFilterValues(object filterValues)
        {
            // validate filter values, throw exception if invalid, then fill protected properties with filter values
            if (filterValues is not List<string> listValues || listValues.Count != 4)
                throw new ListManipulationException("Invalid filter value, date filters expect four values", filterValues);

            if (!Enum.TryParse(typeof(FilterDateType), listValues[0], out object dateType))
                throw new ListManipulationException("Invalid date type", listValues[0]);
            if (!Enum.TryParse(typeof(FilterDatePreset), listValues[1], out object datePreset))
                throw new ListManipulationException("Invalid date preset", listValues[1]);
            if (!DateTime.TryParse(listValues[2], out DateTime dateValue1))
                throw new ListManipulationException("Invalid date 1", listValues[2]);
            if (!DateTime.TryParse(listValues[3], out DateTime dateValue2))
                throw new ListManipulationException("Invalid date 2", listValues[3]);

            DateType = (FilterDateType)dateType;
            DatePreset = (FilterDatePreset)datePreset;
            FilterDate1 = dateValue1;
            FilterDate2 = dateValue2;
        }

        public Func<T, bool> GenerateFilterExpression()
        {
            if (DatePreset != FilterDatePreset.None)
            {
                return DatePreset switch
                {
                    FilterDatePreset.PastDay => (obj) => GetComparisonValue(obj) >= DateTime.Now.AddDays(-1) && GetComparisonValue(obj) < DateTime.Now,
                    FilterDatePreset.PastWeek => (obj) => GetComparisonValue(obj) >= DateTime.Now.AddDays(-7) && GetComparisonValue(obj) < DateTime.Now,
                    FilterDatePreset.PastMonth => (obj) => GetComparisonValue(obj) >= DateTime.Now.AddMonths(-1) && GetComparisonValue(obj) < DateTime.Now,
                    FilterDatePreset.PastYear => (obj) => GetComparisonValue(obj) >= DateTime.Now.AddYears(-1) && GetComparisonValue(obj) < DateTime.Now,
                    FilterDatePreset.Today => (obj) => GetComparisonValue(obj).Date.Equals(DateTime.Today),
                    FilterDatePreset.ThisWeek => (obj) => GetComparisonValue(obj) >= DateTime.Today.StartOfWeek() && GetComparisonValue(obj) < DateTime.Today.EndOfWeek(),
                    FilterDatePreset.ThisMonth => (obj) => GetComparisonValue(obj) >= DateTime.Today.StartOfMonth() && GetComparisonValue(obj) < DateTime.Today.EndOfMonth(),
                    FilterDatePreset.ThisYear => (obj) => GetComparisonValue(obj) >= DateTime.Today.StartOfYear() && GetComparisonValue(obj) < DateTime.Today.EndOfYear(),
                    FilterDatePreset.NextWeek => (obj) => GetComparisonValue(obj) >= DateTime.Now && GetComparisonValue(obj) < DateTime.Now.AddDays(7),
                    FilterDatePreset.NextMonth => (obj) => GetComparisonValue(obj) >= DateTime.Now && GetComparisonValue(obj) < DateTime.Now.AddMonths(1),
                    FilterDatePreset.NextYear => (obj) => GetComparisonValue(obj) >= DateTime.Now && GetComparisonValue(obj) < DateTime.Now.AddYears(1),
                    _ => throw new ListManipulationException("Invalid date preset: " + DatePreset.ToDisplayString(), DatePreset)
                };
            }
            return DateType switch
            {
                FilterDateType.Before => (obj) => GetComparisonValue(obj) < FilterDate1,
                FilterDateType.After => (obj) => GetComparisonValue(obj) > FilterDate1,
                FilterDateType.Between => (obj) => GetComparisonValue(obj) >= FilterDate1 && GetComparisonValue(obj) < FilterDate2,
                _ => throw new ListManipulationException("Invalid filter date type: " + DateType.ToDisplayString(), DateType),
            };
        }

        protected abstract DateTime GetComparisonValue(T obj);
    }
}
