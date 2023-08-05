using RatableTracker.Exceptions;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RatableTracker.ListManipulation.Filtering
{
    public enum FilterOperator : byte
    {
        And = 1,
        Or = 2
    }

    [Serializable]
    public class FilterEngine
    {
        public List<FilterSegment> Filters { get; set; } = new List<FilterSegment>();
        public FilterOperator Operator { get; set; } = FilterOperator.And;

        public IList<T> ApplyFilters<T>(IList<T> list)
        {
            var filterList = new List<Func<T, bool>>();
            foreach (FilterSegment segment in Filters)
            {
                filterList.Add(segment.GetFilterExpression<T>());
            }

            Func<T, bool> mainFunction = (obj) => true;
            if (filterList.Count > 0)
            {
                mainFunction = Operator switch
                {
                    FilterOperator.And => filterList.Aggregate((f1, f2) => (obj) => f1(obj) && f2(obj)),
                    FilterOperator.Or => filterList.Aggregate((f1, f2) => (obj) => f1(obj) || f2(obj)),
                    _ => throw new ListManipulationException("Invalid filter operator: " + Operator.ToString()),
                };
            }

            IList<T> results = list.Where(mainFunction).ToList();
            return results;
        }

        public void SetNonSerializableFields(TrackerModule module, Settings settings)
        {
            foreach (var filter in Filters)
            {
                filter.SetNonSerializableFields(module, settings);
            }
        }

        public static IList<IFilterOption> GetFilterOptionList<T>(TrackerModule module, Settings settings, ICollection<Type> exclude = null)
        {
            // use reflection to find and instantiate all objects with FilterOptionAttribute and InAutoList=true with the type of T or an ancestor
            exclude ??= new List<Type>();
            Type currentType = typeof(T);
            var listTypes = new List<Type>();
            var listTypesManual = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    var attributes = t.GetCustomAttributes(typeof(FilterOptionAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                    {
                        foreach (var attr in attributes.Cast<FilterOptionAttribute>())
                        {
                            if (attr.InAutoList)
                            {
                                if (exclude.Contains(t))
                                    continue;
                                if (currentType.Equals(attr.ExpectedType) || currentType.IsSubclassOf(attr.ExpectedType) || currentType.GetInterfaces().Contains(attr.ExpectedType))
                                {
                                    if (attr.InstantiateManually)
                                        listTypesManual.Add(t);
                                    else
                                        listTypes.Add(t);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            var listOptions = new List<IFilterOption>();
            foreach (var t in listTypes)
            {
                IFilterOption option = (IFilterOption)Activator.CreateInstance(t);
                option.Module = module;
                option.Settings = settings;
                listOptions.Add(option);
            }
            foreach (var t in listTypesManual)
            {
                IFilterOption option = (IFilterOption)Activator.CreateInstance(t);
                option.Module = module;
                option.Settings = settings;
                listOptions.AddRange(option.InstantiateManually());
            }
            return listOptions.OrderBy(op => op.Name).ToList();
        }
    }
}
