using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FilterOptionAttribute : Attribute
    {
        public Type ExpectedType { get; init; }
        public bool InAutoList { get; set; } = true;
        public bool InstantiateManually { get; set; } = false;

        public FilterOptionAttribute(Type expectedType)
        {
            ExpectedType = expectedType;
        }
    }
}
